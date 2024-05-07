using System.Collections.Concurrent;
using DiContainer.Registrations;
using DiContainer.Utils;
namespace DiContainer.Resolving;

public sealed class Container : IResolver
{
    private readonly Registry _registry;
    private readonly ConcurrentDictionary<Registration, Lazy<object>> _singletons = new();
    private readonly Func<Registration, Lazy<object>> _createSingleton;
    private readonly CompositeDisposable _disposables = new();

    public Container(Registry registry)
    {
        _registry = registry;
        _createSingleton = registration =>
            new Lazy<object>(() => registration.GetInstance(this), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public T Resolve<T>() => (T)Resolve(typeof(T));

    public object Resolve(Type type)
    {
        if (!_registry.TryGetRegistration(type, out Registration? registration))
            throw new DiContainerException(type, $"Type {type} is not registered.");

        return registration.Lifetime switch
        {
            Lifetime.Transient => registration.GetInstance(this),
            Lifetime.Singleton => GetOrAddSingleton(registration),
            _ => throw new ArgumentOutOfRangeException(registration.Lifetime.ToString())
        };
    }

    private object GetOrAddSingleton(Registration registration)
    {
        var singleton = _singletons.GetOrAdd(registration, _createSingleton);
        if (singleton is { IsValueCreated: false, Value: IDisposable disposable })
            _disposables.Add(disposable);

        return singleton.Value;
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _registry.Dispose();
        _singletons.Clear();
    }
}
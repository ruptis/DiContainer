using System.Collections.Concurrent;
using DiContainer.Registrations;
using DiContainer.Resolving;
namespace DiContainer.Instantiation;

public class OpenGenericsInstantiator : IInstantiator
{
    private class TypeParametersEqualityComparer : IEqualityComparer<Type[]>
    {
        public bool Equals(Type[]? x, Type[]? y) => y != null && x != null && x.SequenceEqual(y);
        public int GetHashCode(Type[] obj) => obj.Aggregate(0, (hash, type) => hash ^ type.GetHashCode());
    }
    
    private readonly Type _implementationType;
    private readonly Lifetime _lifetime;
    private readonly IInstantiatorFactory _instantiatorFactory;

    private readonly ConcurrentDictionary<Type[], Registration> _constructedRegistrations = new(new TypeParametersEqualityComparer());
    private Func<Type[], Registration> _constructRegistration;

    public OpenGenericsInstantiator(Type implementationType, Lifetime lifetime, IInstantiatorFactory instantiatorFactory)
    {
        _implementationType = implementationType;
        _lifetime = lifetime;
        _instantiatorFactory = instantiatorFactory;
        _constructRegistration = ConstructRegistration;
    }

    public object CreateInstance(IResolver resolver)
    {
        throw new NotSupportedException("Open generics cannot be instantiated directly.");
    }
    
    public Registration GetConstructedRegistration(Type[] typeParameters)
    {
        _constructRegistration = ConstructRegistration;
        return _constructedRegistrations.GetOrAdd(typeParameters, _constructRegistration);
    }

    private Registration ConstructRegistration(Type[] typeParameters)
    {
        Type constructedType = _implementationType.MakeGenericType(typeParameters);
        RegistrationBuilder registrationBuilder = new(constructedType, _lifetime);
        return registrationBuilder.WithFactory(_instantiatorFactory).Build();
    }
}

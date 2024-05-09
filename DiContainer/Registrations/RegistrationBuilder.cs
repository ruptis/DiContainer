using DiContainer.Instantiation;
using DiContainer.Resolving;
namespace DiContainer.Registrations;

public class RegistrationBuilder(Type implementationType, Lifetime lifetime)
{
    protected internal readonly Type ImplementationType = implementationType;
    protected internal readonly Lifetime Lifetime = lifetime;
    protected internal readonly List<Type> InterfaceTypes = new();
    protected internal IInstantiatorFactory? InstantiatorFactory;

    public virtual Registration Build()
    {
        IInstantiator instantiator = InstantiatorFactory?.CreateInstantiator(this)
            ?? throw new InvalidOperationException("Instantiator factory is not set.");
        return new Registration(ImplementationType, InterfaceTypes, Lifetime, instantiator);
    }

    public RegistrationBuilder As<TInterface>() => As(typeof(TInterface));
    public RegistrationBuilder AsSelf() => As(ImplementationType);

    public RegistrationBuilder As(Type interfaceType)
    {
        AddInterfaceType(interfaceType);
        return this;
    }

    protected virtual void AddInterfaceType(Type interfaceType)
    {
        if (!interfaceType.IsAssignableFrom(ImplementationType))
            throw new InvalidOperationException($"Type {ImplementationType} does not implement interface {interfaceType}.");
        
        InterfaceTypes.Add(interfaceType);
    }

    public RegistrationBuilder WithFactory(IInstantiatorFactory instantiatorFactory)
    {
        InstantiatorFactory = instantiatorFactory;
        return this;
    }
}
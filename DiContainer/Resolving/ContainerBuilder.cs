using DiContainer.Instantiation;
using DiContainer.Registrations;
namespace DiContainer.Resolving;

public class ContainerBuilder
{
    private readonly InstantiatorFactory _instantiatorFactory = new();
    private readonly List<RegistrationBuilder> _registrationBuilders = [];

    public Container Build() => new(BuildRegistry());

    private Registry BuildRegistry()
    {
        var registryBuilder = new RegistryBuilder();

        Parallel.ForEach(_registrationBuilders, registrationBuilder =>
        {
            Registration registration = registrationBuilder.Build();
            registryBuilder.Register(registration);
        });

        return registryBuilder.Build();
    }

    internal T Register<T>(T registrationBuilder) where T : RegistrationBuilder
    {
        _registrationBuilders.Add(registrationBuilder.WithFactory(_instantiatorFactory));
        return registrationBuilder;
    }
}
public static class ContainerBuilderExtensions
{
    public static RegistrationBuilder Register(this ContainerBuilder builder, Type implementationType, Lifetime lifetime) =>
        builder.Register(implementationType is { IsGenericType: true, IsGenericTypeDefinition: true }
            ? new OpenGenericsRegistrationBuilder(implementationType, lifetime)
            : new RegistrationBuilder(implementationType, lifetime));

    public static RegistrationBuilder Register(this ContainerBuilder builder, Type implementationType, Type interfaceType, Lifetime lifetime) =>
        builder.Register(new RegistrationBuilder(implementationType, lifetime).As(interfaceType));

    public static RegistrationBuilder Register<TImplementation>(this ContainerBuilder builder, Lifetime lifetime = Lifetime.Singleton) =>
        builder.Register(typeof(TImplementation), lifetime);

    public static RegistrationBuilder Register<TImplementation, TInterface>(this ContainerBuilder builder, Lifetime lifetime = Lifetime.Singleton) =>
        builder.Register(typeof(TImplementation), typeof(TInterface), lifetime);
}

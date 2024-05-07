using DiContainer.Resolving;
namespace DiContainer.Registrations;

public class OpenGenericsRegistrationBuilder : RegistrationBuilder
{
    public OpenGenericsRegistrationBuilder(Type implementationType, Lifetime lifetime) : base(implementationType, lifetime)
    {
        if (!implementationType.IsGenericTypeDefinition || implementationType.IsConstructedGenericType)
            throw new ArgumentException("Type must be an open generic type definition.", nameof(implementationType));
    }

    protected override void AddInterfaceType(Type interfaceType)
    {
        if (interfaceType.IsConstructedGenericType)
            throw new ArgumentException("Interface type must be an open generic type definition.", nameof(interfaceType));

        base.AddInterfaceType(interfaceType);
    }
}

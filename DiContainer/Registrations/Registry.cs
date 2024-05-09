using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using DiContainer.Instantiation;

namespace DiContainer.Registrations;

public sealed class Registry(IReadOnlyDictionary<Type, Registration> registrations) : IDisposable
{
    private readonly FrozenDictionary<Type, Registration> _registrations = registrations.ToFrozenDictionary();

    public bool TryGetRegistration(Type type, [NotNullWhen(true)] out Registration? registration)
    {
        if (_registrations.TryGetValue(type, out registration))
            return true;

        if (type.IsConstructedGenericType)
        {
            Type openGenericType = type.GetGenericTypeDefinition();
            var typeParameters = type.GetGenericArguments();
            return TryGetOpenGenericRegistration(openGenericType, typeParameters, out registration);
        }

        return false;
    }

    public void Dispose()
    {
    }

    private bool TryGetOpenGenericRegistration(Type openGenericType, Type[] typeParameters,
        [NotNullWhen(true)] out Registration? registration)
    {
        if (_registrations.TryGetValue(openGenericType, out Registration? openGenericRegistration) &&
            openGenericRegistration.Instantiator is OpenGenericsInstantiator openGenericsInstantiator)
        {
            registration = openGenericsInstantiator.GetConstructedRegistration(typeParameters);
            return true;
        }

        registration = null;
        return false;
    }
}
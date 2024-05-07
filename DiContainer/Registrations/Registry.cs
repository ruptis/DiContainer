using System.Diagnostics.CodeAnalysis;
namespace DiContainer.Registrations;

public sealed class Registry : IDisposable
{
    private readonly Dictionary<Type, Registration> _registrations;

    public Registry(IEnumerable<Registration> registrations) => _registrations = registrations.ToDictionary(r => r.InterfaceType);

    public bool TryGetRegistration(Type type, [NotNullWhen(true)] out Registration? registration)
    {
        if (_registrations.TryGetValue(type, out registration))
            return true;

        if (type.IsConstructedGenericType)
        {
            Type openGenericType = type.GetGenericTypeDefinition();
            return _registrations.TryGetValue(openGenericType, out registration);
        }
        
        return false;
    }

    public void Dispose()
    {
        
    }
}
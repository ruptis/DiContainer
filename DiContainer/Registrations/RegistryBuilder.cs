using System.Collections.Concurrent;
namespace DiContainer.Registrations;

public sealed class RegistryBuilder
{
    private readonly ConcurrentBag<Registration> _registrations = [];

    public Registry Build()
    {
        var registrations = new Dictionary<Type, Registration>(_registrations.Count);
        foreach (Registration registration in _registrations)
        {
            foreach (Type interfaceType in registration.InterfaceTypes)
            {
                /*if (registrations.TryGetValue(interfaceType, out Registration? existingRegistration))
                {
                    
                }*/
                registrations[interfaceType] = registration;
            }
        }

        return new Registry(registrations);
    }

    public T Register<T>(T registration) where T : Registration
    {
        _registrations.Add(registration);
        return registration;
    }
}

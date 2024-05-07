using System.Collections.Concurrent;
namespace DiContainer.Registrations;

public sealed class RegistryBuilder
{
    private readonly ConcurrentBag<Registration> _registrations = [];

    public Registry Build() => new(_registrations);

    public T Register<T>(T registration) where T : Registration
    {
        _registrations.Add(registration);
        return registration;
    }
}

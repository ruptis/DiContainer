using System.Collections.Concurrent;
using DiContainer.Registrations;
using DiContainer.Resolving;
namespace DiContainer.Instantiation;

public class OpenGenericsInstantiator(Type implementationType, Lifetime lifetime, IInstantiatorFactory instantiatorFactory) : IInstantiator
{
    private class TypeParametersEqualityComparer : IEqualityComparer<Type[]>
    {
        public bool Equals(Type[]? x, Type[]? y) => y != null && x != null && x.SequenceEqual(y);
        public int GetHashCode(Type[] obj) => obj.Aggregate(0, (hash, type) => hash ^ type.GetHashCode());
    }

    private readonly ConcurrentDictionary<Type[], Registration> _constructedRegistrations = new(new TypeParametersEqualityComparer());

    public object CreateInstance(IResolver resolver)
    {
        var typeParameters = implementationType.GetGenericArguments();
        Registration registration = _constructedRegistrations.GetOrAdd(typeParameters, ConstructRegistration);
        return registration.Instantiator.CreateInstance(resolver);
    }
    
    private Registration ConstructRegistration(Type[] typeParameters)
    {
        Type constructedType = implementationType.MakeGenericType(typeParameters);
        RegistrationBuilder registrationBuilder = new(constructedType, lifetime);
        return registrationBuilder.WithFactory(instantiatorFactory).Build();
    }
}

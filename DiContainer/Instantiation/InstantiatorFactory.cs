using DiContainer.Registrations;
using DiContainer.Resolving;
using DiContainer.Utils;
namespace DiContainer.Instantiation;

public class InstantiatorFactory : IInstantiatorFactory
{
    private readonly TypeAnalyzer _typeAnalyzer = new();
    private readonly Dictionary<Type, IInstantiator> _instantiatorsCache = new();

    public IInstantiator CreateInstantiator(RegistrationBuilder registrationBuilder)
    {
        Type implementationType = registrationBuilder.ImplementationType;
        Lifetime lifetime = registrationBuilder.Lifetime;
        if (_instantiatorsCache.TryGetValue(implementationType, out IInstantiator? instantiator))
            return instantiator;

        instantiator = registrationBuilder switch
        {
            OpenGenericsRegistrationBuilder => CreateOpenGenericInstantiator(implementationType, lifetime),
            _ => CreateActivatorInstantiator(implementationType)
        };
        
        _instantiatorsCache[implementationType] = instantiator;
        return instantiator;
    }

    private OpenGenericsInstantiator CreateOpenGenericInstantiator(Type implementationType, Lifetime lifetime) =>
        new(implementationType, lifetime, this);

    private ActivatorInstantiator CreateActivatorInstantiator(Type implementationType) =>
        new(_typeAnalyzer.GetTypeInfo(implementationType));
}

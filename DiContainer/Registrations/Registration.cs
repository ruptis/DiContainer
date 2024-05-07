using System.Runtime.CompilerServices;
using DiContainer.Instantiation;
using DiContainer.Resolving;
namespace DiContainer.Registrations;

public record Registration(Type ImplementationType, Type InterfaceType, Lifetime Lifetime, IInstantiator Instantiator)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object GetInstance(IResolver resolver) => Instantiator.CreateInstance(resolver);
}

using DiContainer.Resolving;
namespace DiContainer.Instantiation;

public interface IInstantiator
{
    object CreateInstance(IResolver resolver);
}

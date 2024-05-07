using DiContainer.Instantiation.Info;
using DiContainer.Resolving;
using DiContainer.Utils;
namespace DiContainer.Instantiation;

public sealed class ActivatorInstantiator(InjectTypeInfo injectTypeInfo) : IInstantiator
{
    private readonly CappedArrayPool<object> _objectPool = CappedArrayPool<object>.Shared8Limit;

    public object CreateInstance(IResolver resolver)
    {
        var parameterInfos = injectTypeInfo.ConstructorInfo.Parameters;
        var parameters = _objectPool.Rent(parameterInfos.Length);
        try
        {
            for (var i = 0; i < parameterInfos.Length; i++)
                parameters[i] = resolver.Resolve(parameterInfos[i].ParameterType);
            return injectTypeInfo.ConstructorInfo.Constructor.Invoke(parameters);
        }
        catch (DiContainerException ex)
        {
            throw new DiContainerException(injectTypeInfo.Type, $"Failed to resolve dependencies for type {injectTypeInfo.Type}: {ex.Message}");
        }
        finally
        {
            _objectPool.Return(parameters);
        }
    }
}
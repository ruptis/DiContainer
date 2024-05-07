using System.Collections.Concurrent;
using DiContainer.Instantiation.Info;
namespace DiContainer.Utils;

public sealed class TypeAnalyzer
{
    private readonly ConcurrentDictionary<Type, InjectTypeInfo> _typeInfoCache = new();
    
    public InjectTypeInfo GetTypeInfo(Type type) => _typeInfoCache.GetOrAdd(type, CreateTypeInfo);
    
    private static InjectTypeInfo CreateTypeInfo(Type type)
    {
        var constructors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).ToArray();
        if (constructors.Length == 0)
            throw new InvalidOperationException($"Type {type} does not have any public constructors.");
        
        var parameters = constructors[0].GetParameters();
        return new InjectTypeInfo(type, new InjectConstructorInfo(constructors[0], parameters));
    }
}

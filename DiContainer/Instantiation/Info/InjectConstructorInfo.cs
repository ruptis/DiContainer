using System.Reflection;
namespace DiContainer.Instantiation.Info;

public record struct InjectConstructorInfo(ConstructorInfo Constructor, ParameterInfo[] Parameters);

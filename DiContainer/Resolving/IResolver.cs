namespace DiContainer.Resolving;

public interface IResolver : IDisposable
{
    object Resolve(Type type);
    TType Resolve<TType>();
}

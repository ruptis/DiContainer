using System.Collections.Concurrent;
namespace DiContainer.Utils;

public sealed class CompositeDisposable : IDisposable
{
    private readonly ConcurrentBag<IDisposable?> _disposables = [];

    public void Add(IDisposable? disposable)
    {
        _disposables.Add(disposable);
    }

    public void Dispose()
    {
        while (_disposables.TryTake(out IDisposable? disposable))
            disposable?.Dispose();
    }
}

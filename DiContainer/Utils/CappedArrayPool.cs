namespace DiContainer.Utils;

internal sealed class CappedArrayPool<T>
{
    private const int InitialBucketSize = 4;

    public static readonly CappedArrayPool<T> Shared8Limit = new(8);

    private readonly T[][][] _buckets;
    private readonly object _syncRoot = new();
    private readonly int[] _tails;

    public CappedArrayPool(int maxLength)
    {
        _buckets = new T[maxLength][][];
        _tails = new int[maxLength];
        for (var i = 0; i < maxLength; i++)
        {
            var arrayLength = i + 1;
            _buckets[i] = new T[InitialBucketSize][];
            for (var j = 0; j < InitialBucketSize; j++)
            {
                _buckets[i][j] = new T[arrayLength];
            }
            _tails[i] = _buckets[i].Length - 1;
        }
    }

    public T[] Rent(int length)
    {
        if (length <= 0)
            return [];

        if (length > _buckets.Length)
            return new T[length];

        var i = length - 1;

        lock (_syncRoot)
        {
            var bucket = _buckets[i];
            var tail = _tails[i];
            if (tail >= bucket.Length)
            {
                Array.Resize(ref bucket, bucket.Length * 2);
                _buckets[i] = bucket;
            }
            
            var result = bucket[tail] ?? new T[length];
            _tails[i] = tail + 1;
            return result;
        }
    }

    public void Return(T[] array)
    {
        if (array.Length <= 0 || array.Length > _buckets.Length)
            return;

        var i = array.Length - 1;
        lock (_syncRoot)
        {
            Array.Clear(array, 0, array.Length);
            if (_tails[i] > 0)
                _tails[i] -= 1;
        }
    }
}

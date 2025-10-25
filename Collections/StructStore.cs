using System.Collections.Generic;

namespace GodotLib;

public sealed class StructStore
{
    private readonly Dictionary<Type, object> _pools = new();

    public void Add<T>(T value) where T : struct
    {
        var pool = GetOrCreatePool<T>();
        pool.Add(value);
    }

    public ref T Get<T>(int index) where T : struct
    {
        var pool = (StructPool<T>)_pools[typeof(T)];
        return ref pool.Get(index);
    }

    public Span<T> GetAll<T>() where T : struct
    {
        var pool = (StructPool<T>)_pools[typeof(T)];
        return pool.AsSpan();
    }

    private StructPool<T> GetOrCreatePool<T>() where T : struct
    {
        if (!_pools.TryGetValue(typeof(T), out var obj))
        {
            var pool = new StructPool<T>();
            _pools[typeof(T)] = pool;
            return pool;
        }
        return (StructPool<T>)obj;
    }

    private sealed class StructPool<T> where T : struct
    {
        private T[] _items = new T[8];
        private int _count;

        public void Add(T item)
        {
            if (_count >= _items.Length)
                Array.Resize(ref _items, _items.Length * 2);
            _items[_count++] = item;
        }

        public ref T Get(int index)
        {
            return ref _items[index];
        }

        public Span<T> AsSpan() => _items.AsSpan(0, _count);
    }
}

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Util;

public static class CollectionExtensions
{
    [MethodImpl(AggressiveInlining)]
    public static void RemoveUnorderedAt<T>(this IList<T> list, int index)
    {
        var lastIndex = list.Count - 1;
        list[index] = list[lastIndex];
        list.RemoveAt(lastIndex);
    }
    
    [MethodImpl(AggressiveInlining)]
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        if (dictionary.TryGetValue(key, out var result))
            return result;
        
        dictionary.Add(key, defaultValue);
        return defaultValue;
    }
    
    [MethodImpl(AggressiveInlining)]
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueFactory)
    {
        if (dictionary.TryGetValue(key, out var result))
            return result;

        var defaultValue = defaultValueFactory();
        dictionary.Add(key, defaultValue);
        return defaultValue;
    }

    [MethodImpl(AggressiveInlining)]
    public static void Clear(this Array array)
    {
        Array.Clear(array);
    }
}
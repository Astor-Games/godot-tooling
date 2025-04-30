using System.Collections.Generic;

namespace GodotLib.Util;

public static class CollectionExtensions
{
    public static void RemoveUnorderedAt<T>(this List<T> list, int index)
    {
        var lastIndex = list.Count - 1;
        list[index] = list[lastIndex];
        list.RemoveAt(lastIndex);
    }
    
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        if (dictionary.TryGetValue(key, out var result))
            return result;
        
        dictionary.Add(key, defaultValue);
        return defaultValue;
    }
}
using System.Collections.Generic;

namespace GodotLib.Util;

public static class CollectionUtils
{
    public static void RemoveUnorderedAt<T>(this List<T> list, int index)
    {
        var lastIndex = list.Count - 1;
        list[index] = list[lastIndex];
        list.RemoveAt(lastIndex);
    }
}
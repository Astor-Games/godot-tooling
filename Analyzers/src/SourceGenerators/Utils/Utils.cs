using System;
using System.Collections.Generic;
using System.Linq;

namespace GodotLib.Analyzers.SourceGenerators.Utils;

public static class Utils
{
    public static string SnakeToPascalCase(this string snakeString)
    {
        return snakeString
            .Split(["_"], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
            .Aggregate(string.Empty, (s1, s2) => s1 + s2);
    }
    
    public static void Deconstruct<TKey, T1, T2>(this KeyValuePair<TKey, (T1, T2)> kvp, out TKey key, out T1 item1, out T2 item2)
    {
        key = kvp.Key;
        item1 = kvp.Value.Item1;
        item2 = kvp.Value.Item2;
    }
}
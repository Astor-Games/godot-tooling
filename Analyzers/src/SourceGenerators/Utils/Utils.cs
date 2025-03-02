using System;
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
}
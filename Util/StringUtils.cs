using System.Text.RegularExpressions;

namespace GodotLib.Util;

public static partial class StringUtils
{
    [GeneratedRegex("[a-z][A-Z]")]
    private static partial Regex PascalCaseRegex();

    [GeneratedRegex(@"<([^>]+)>")]
    private static partial Regex GenericTypeRegex();

    private static readonly string[] CapitalizedWords = { "GUID" };
    private static readonly string[] GenericPrepositions = { "on", "of", "to" };

    public static string HumanizeName(string name)
    {
        if (name.IsNullOrEmpty()) return string.Empty;

        // Add spaces between PascalCase words
        var result = PascalCaseRegex().Replace(name, m => $"{m.Value[0]} {m.Value[1]}");

        // Handle generics: replace <Type> with (Type) unless preceded by "of", "on", "to"
        result = GenericTypeRegex().Replace(result, match =>
        {
            var matchIndex = match.Index;
            if (matchIndex >= 3)
            {
                var precedingWord = result.Substring(Math.Max(0, matchIndex - 3), Math.Min(3, matchIndex)).Trim();
                if (GenericPrepositions.Contains(precedingWord, StringComparer.OrdinalIgnoreCase))
                {
                    return " " + match.Groups[1].Value;
                }
            }
            return $" ({match.Groups[1].Value})";
        });

        // Capitalize first letter
        if (result.Length > 0)
        {
            result = char.ToUpper(result[0]) + result[1..];
        }

        // Capitalize specific words
        foreach (var word in CapitalizedWords)
        {
            var lowerWord = word.ToLower();
            result = Regex.Replace(result, $@"\b{lowerWord}\b", word, RegexOptions.IgnoreCase);
        }

        return result;
    }
}
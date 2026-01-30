using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace GodotLib.Util;

public static partial class StringExtensions
{
    [GeneratedRegex("[a-z][A-Z]")]
    private static partial Regex PascalCaseRegex();
    
    extension([CanBeNull] string value)
    {
        [ContractAnnotation("value:null => true")]
        public bool IsNullOrEmpty()
        {
            return string.IsNullOrEmpty(value);
        }

        public string OrDefault(string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }

        public string FormatSentence()
        {
            if (value.IsNullOrEmpty()) return string.Empty;
            return PascalCaseRegex().Replace(value, m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");
        }
    }
}
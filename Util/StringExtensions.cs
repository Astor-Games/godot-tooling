using JetBrains.Annotations;

namespace GodotLib.Util;

public static class StringExtensions
{
    public static bool IsNullOrEmpty([CanBeNull] this string value)
    {
        return string.IsNullOrEmpty(value);
    }
    
    public static string OrDefault([CanBeNull] this string value, string defaultValue)
    {
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }
}
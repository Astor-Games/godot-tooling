using JetBrains.Annotations;

namespace GodotLib.Util;

public static class StringExtensions
{
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
    }
}
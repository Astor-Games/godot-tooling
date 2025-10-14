using System.Linq;

namespace GodotLib.Util;

public static class TypeUtils
{
    public static bool IsGenericOf(this Type type, Type openGenericType)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType;
    }
    
    public static string GetHumanReadableName(this Type type)
    {
        if (type.IsGenericType)
        {
            var baseName = type.Name;
            var tickIndex = baseName.IndexOf('`');
            if (tickIndex >= 0)
                baseName = baseName[0..tickIndex];

            var genericArgs = type.GetGenericArguments().Select(GetHumanReadableName);

            return $"{baseName}<{string.Join(", ", genericArgs)}>";
        }

        // Handle arrays
        if (type.IsArray)
        {
            return $"{GetHumanReadableName(type.GetElementType()!)}[{new string(',', type.GetArrayRank() - 1)}]";
        }

        // Built-in type aliases
        return type switch
        {
            { } t when t == typeof(int) => "int",
            { } t when t == typeof(string) => "string",
            { } t when t == typeof(bool) => "bool",
            { } t when t == typeof(object) => "object",
            { } t when t == typeof(void) => "void",
            { } t when t == typeof(double) => "double",
            { } t when t == typeof(float) => "float",
            { } t when t == typeof(decimal) => "decimal",
            { } t when t == typeof(byte) => "byte",
            { } t when t == typeof(char) => "char",
            { } t when t == typeof(long) => "long",
            { } t when t == typeof(short) => "short",
            { } t when t == typeof(uint) => "uint",
            { } t when t == typeof(ulong) => "ulong",
            { } t when t == typeof(ushort) => "ushort",
            { } t when t == typeof(sbyte) => "sbyte",
            _ => type.Name
        };
    }
}
using System.Linq;
using JetBrains.Annotations;

namespace GodotLib.Util;

public static class TypeUtils
{
    public static bool IsGenericOf(this Type type, Type openGenericType)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == openGenericType;
    }
    
    [CanBeNull]
    public static string GetHumanReadableName(this Type type)
    {
        if (type.IsGenericType)
        {
            var baseName = type.Name;
            var tickIndex = baseName.IndexOf('`');
            if (tickIndex >= 0)
                baseName = baseName[..tickIndex];

            var genericArgs = type.GetGenericArguments().Select(GetHumanReadableName);

            return $"{baseName}<{string.Join(", ", genericArgs)}>";
        }

        // Handle arrays
        if (type.IsArray)
        {
            return $"{type.GetElementType()!.GetHumanReadableName()}[{new string(',', type.GetArrayRank() - 1)}]";
        }

        // Built-in type aliases
        if (type == typeof(int)) return "int";
        if (type == typeof(string)) return "string";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(object)) return "object";
        if (type == typeof(void)) return "void";
        if (type == typeof(double)) return "double";
        if (type == typeof(float)) return "float";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(byte)) return "byte";
        if (type == typeof(char)) return "char";
        if (type == typeof(long)) return "long";
        if (type == typeof(short)) return "short";
        if (type == typeof(uint)) return "uint";
        if (type == typeof(ulong)) return "ulong";
        if (type == typeof(ushort)) return "ushort";
        if (type == typeof(sbyte)) return "sbyte";
        
        return type.Name;
    }
}
namespace GodotLib;

using System;
using System.Collections.Concurrent;
using MessagePack;
using MessagePack.Formatters;

public sealed class InlineListResolver : IFormatterResolver
{
    public static readonly InlineListResolver Instance = new();

    private InlineListResolver() { }

    private static readonly ConcurrentDictionary<Type, object> formatterCache = new();

    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        return (IMessagePackFormatter<T>)formatterCache.GetOrAdd(typeof(T), static type =>
        {
            if (!type.IsGenericType)
            {
                return null;
            }
            
            var elementType = type.GetGenericArguments()[0];

            Type formatterType = null;

            if (type.GetGenericTypeDefinition() == typeof(InlineList4<>))
                formatterType = typeof(InlineList4Formatter<>).MakeGenericType(elementType);
            
            else if (type.GetGenericTypeDefinition() == typeof(InlineList8<>))
                formatterType = typeof(InlineList8Formatter<>).MakeGenericType(elementType);
            
            else if (type.GetGenericTypeDefinition() == typeof(InlineList16<>))
                formatterType = typeof(InlineList16Formatter<>).MakeGenericType(elementType);
            
            else if (type.GetGenericTypeDefinition() == typeof(InlineList32<>)) 
                formatterType = typeof(InlineList32Formatter<>).MakeGenericType(elementType);

            return formatterType != null ? Activator.CreateInstance(formatterType) : null;
        });
    }
}

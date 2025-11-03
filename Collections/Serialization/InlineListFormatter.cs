namespace GodotLib;

using System;
using MessagePack;
using MessagePack.Formatters;

public sealed class InlineList4Formatter<T> : IMessagePackFormatter<InlineList4<T>> where T : struct
{
    private const int MaxLength = 4;
    
    public void Serialize(ref MessagePackWriter writer, InlineList4<T> value, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        // Write array header: [count, elements...]
        writer.WriteArrayHeader(value.Count + 1);
        writer.Write(value.Count);

        for (var i = 0; i < value.Count; i++)
        {
            elementFormatter.Serialize(ref writer, value[i], options);
        }
    }

    public InlineList4<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        var length = reader.ReadArrayHeader();
        if (length == 0)
            return default;

        var count = reader.ReadByte(); // first element is count
        if (count > MaxLength) throw new InvalidOperationException($"InlineList cannot have more than {MaxLength} elements");

        var list = new InlineList4<T>();
        for (var i = 0; i < count; i++)
        {
            var element = elementFormatter.Deserialize(ref reader, options);
            list.Add(element);
        }

        // Skip remaining elements if any (defensive)
        for (var i = count + 1; i < length; i++)
            reader.Skip();

        return list;
    }
}

public sealed class InlineList8Formatter<T> : IMessagePackFormatter<InlineList8<T>> where T : struct
{
    private const int MaxLength = 8;
    
    public void Serialize(ref MessagePackWriter writer, InlineList8<T> value, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        // Write array header: [count, elements...]
        writer.WriteArrayHeader(value.Count + 1);
        writer.Write(value.Count);

        for (var i = 0; i < value.Count; i++)
        {
            elementFormatter.Serialize(ref writer, value[i], options);
        }
    }

    public InlineList8<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        var length = reader.ReadArrayHeader();
        if (length == 0)
            return default;

        var count = reader.ReadByte(); // first element is count
        if (count > MaxLength) throw new InvalidOperationException($"InlineList cannot have more than {MaxLength} elements");

        var list = new InlineList8<T>();
        for (var i = 0; i < count; i++)
        {
            var element = elementFormatter.Deserialize(ref reader, options);
            list.Add(element);
        }

        // Skip remaining elements if any (defensive)
        for (var i = count + 1; i < length; i++)
            reader.Skip();

        return list;
    }
}

public sealed class InlineList16Formatter<T> : IMessagePackFormatter<InlineList16<T>> where T : struct
{
    private const int MaxLength = 16;
    
    public void Serialize(ref MessagePackWriter writer, InlineList16<T> value, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        // Write array header: [count, elements...]
        writer.WriteArrayHeader(value.Count + 1);
        writer.Write(value.Count);

        for (var i = 0; i < value.Count; i++)
        {
            elementFormatter.Serialize(ref writer, value[i], options);
        }
    }

    public InlineList16<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        var length = reader.ReadArrayHeader();
        if (length == 0)
            return default;

        var count = reader.ReadByte(); // first element is count
        if (count > MaxLength) throw new InvalidOperationException($"InlineList cannot have more than {MaxLength} elements");

        var list = new InlineList16<T>();
        for (var i = 0; i < count; i++)
        {
            var element = elementFormatter.Deserialize(ref reader, options);
            list.Add(element);
        }

        // Skip remaining elements if any (defensive)
        for (var i = count + 1; i < length; i++)
            reader.Skip();

        return list;
    }
}

public sealed class InlineList32Formatter<T> : IMessagePackFormatter<InlineList32<T>> where T : struct
{
    private const int MaxLength = 32;
    
    public void Serialize(ref MessagePackWriter writer, InlineList32<T> value, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        // Write array header: [count, elements...]
        writer.WriteArrayHeader(value.Count + 1);
        writer.Write(value.Count);

        for (var i = 0; i < value.Count; i++)
        {
            elementFormatter.Serialize(ref writer, value[i], options);
        }
    }

    public InlineList32<T> Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        var resolver = options.Resolver;
        var elementFormatter = resolver.GetFormatterWithVerify<T>();

        var length = reader.ReadArrayHeader();
        if (length == 0)
            return default;

        var count = reader.ReadByte(); // first element is count
        if (count > MaxLength) throw new InvalidOperationException($"InlineList cannot have more than {MaxLength} elements");

        var list = new InlineList32<T>();
        for (var i = 0; i < count; i++)
        {
            var element = elementFormatter.Deserialize(ref reader, options);
            list.Add(element);
        }

        // Skip remaining elements if any (defensive)
        for (var i = count + 1; i < length; i++)
            reader.Skip();

        return list;
    }
}

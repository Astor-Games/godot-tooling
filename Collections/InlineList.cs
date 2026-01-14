using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GodotLib.Debug;
using Turtles;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib;

// TODO template this to deduplicate code

public struct InlineList4<T>() where T : unmanaged
{
    private const int Length = 4;
    private InnerBuffer buffer = new();
    private byte count = 0;

    public int Count => count;
    public bool IsFull => count == Length;

    public void Insert(int index, T item)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[index] = item;
    }
    
    public unsafe ref T this[int index] => ref buffer[index];

    public void Add(T element)
    {
        if (count >= Length)
        {
            throw new InvalidOperationException(message: "List is full.");
        }
        
        buffer[count++] = element;
    }

    public void Clear()
    {
        count = 0;
    }
    
    public bool Remove(in T element)
    {
        var removeIdx = -1;
        for (var i = 0; i < Count; i++)
        {
            if (!Equals(buffer[i], element)) continue;
            removeIdx = i;
            break;
        }

        if (removeIdx == -1) return false;
        
        buffer[count] = buffer[removeIdx];
        count--;
        return true;
    }
    
    public void RemoveAt(int index)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[count] = buffer[index];
        count--;
    }
    
    [MethodImpl(AggressiveInlining)]
    public Span<T> ToSpan()
    {
        return MemoryMarshal.CreateSpan(ref buffer[0], count);
    }
    
    public Span<T>.Enumerator GetEnumerator()
    {
        return ToSpan().GetEnumerator();
    }
    
    public static implicit operator Span<T>(InlineList4<T> list) => list.ToSpan();
    
    [InlineArray(Length)]
    private struct InnerBuffer
    {
        private T element;
    }
}

public struct InlineList8<T>() where T : unmanaged
{
    private const int Length = 8;
    private InnerBuffer buffer = new();
    private byte count = 0;

    public int Count => count;
    public bool IsFull => count == Length;

    public void Insert(int index, T item)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[index] = item;
    }
    
    public unsafe ref T this[int index] => ref buffer[index];

    public void Add(T element)
    {
        if (count >= Length)
        {
            throw new InvalidOperationException(message: "List is full.");
        }
        
        buffer[count++] = element;
    }
    
    public void Fill(ReadOnlySpan<T> source)
    {
        var n = source.Length;
        Assertions.AssertTrue(n <= Length);
        source[..n].CopyTo(buffer);
        count = (byte)n;
    }

    public void Clear()
    {
        count = 0;
    }

    public bool Remove(in T element)
    {
        var removeIdx = -1;
        for (var i = 0; i < Count; i++)
        {
            if (!Equals(buffer[i], element)) continue;
            removeIdx = i;
            break;
        }

        if (removeIdx == -1) return false;
        
        buffer[count] = buffer[removeIdx];
        count--;
        return true;
    }
    
    public void RemoveAt(int index)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[count] = buffer[index];
        count--;
    }
    
    [MethodImpl(AggressiveInlining)]
    public Span<T> ToSpan()
    {
        return MemoryMarshal.CreateSpan(ref buffer[0], count);
    }
    
    public Span<T>.Enumerator GetEnumerator()
    {
        return ToSpan().GetEnumerator();
    }
    
    public static implicit operator Span<T>(InlineList8<T> list) => list.ToSpan();
    
    [InlineArray(Length)]
    private struct InnerBuffer
    {
        private T element;
    }
}

public struct InlineList16<T>() where T : unmanaged
{
    private const int Length = 16;
    private InnerBuffer buffer = new();
    private byte count = 0;

    public int Count => count;
    public bool IsFull => count == Length;

    public void Insert(int index, T item)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[index] = item;
    }
    
    public unsafe ref T this[int index] => ref buffer[index];

    public void Add(T element)
    {
        if (count >= Length)
        {
            throw new InvalidOperationException(message: "List is full.");
        }
        
        buffer[count++] = element;
    }
    
    public void Fill(ReadOnlySpan<T> source)
    {
        var n = source.Length;
        Assertions.AssertTrue(n <= Length);
        source[..n].CopyTo(buffer);
        count = (byte)n;
    }

    public void Clear()
    {
        count = 0;
    }
    
    public bool Remove(in T element)
    {
        var removeIdx = -1;
        for (var i = 0; i < Count; i++)
        {
            if (!Equals(buffer[i], element)) continue;
            removeIdx = i;
            break;
        }

        if (removeIdx == -1) return false;
        
        buffer[count] = buffer[removeIdx];
        count--;
        return true;
    }
    
    public void RemoveAt(int index)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[count] = buffer[index];
        count--;
    }
    
    [MethodImpl(AggressiveInlining)]
    public Span<T> ToSpan()
    {
        return MemoryMarshal.CreateSpan(ref buffer[0], count);
    }
    
    public Span<T>.Enumerator GetEnumerator()
    {
        return ToSpan().GetEnumerator();
    }
    
    public static implicit operator Span<T>(InlineList16<T> list) => list.ToSpan();
    
    [InlineArray(Length)]
    private struct InnerBuffer
    {
        private T element;
    }
}

public struct InlineList32<T>() where T : unmanaged
{
    private const int Length = 32;
    private InnerBuffer buffer = new();
    private byte count = 0;

    public int Count => count;
    public bool IsFull => count == Length;

    public void Insert(int index, T item)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[index] = item;
    }
    
    public unsafe ref T this[int index] => ref buffer[index];

    public void Add(T element)
    {
        if (count >= Length)
        {
            throw new InvalidOperationException(message: "List is full.");
        }
        
        buffer[count++] = element;
    }
    
    public void Fill(ReadOnlySpan<T> source)
    {
        var n = source.Length;
        Assertions.AssertTrue(n <= Length);
        source[..n].CopyTo(buffer);
        count = (byte)n;
    }

    public void Clear()
    {
        count = 0;
    }
    
    public bool Remove(in T element)
    {
        var removeIdx = -1;
        for (var i = 0; i < Count; i++)
        {
            if (!Equals(buffer[i], element)) continue;
            removeIdx = i;
            break;
        }

        if (removeIdx == -1) return false;
        
        buffer[count] = buffer[removeIdx];
        count--;
        return true;
    }
    
    public void RemoveAt(int index)
    {
        if (index > count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        buffer[count] = buffer[index];
        count--;
    }
    
    [MethodImpl(AggressiveInlining)]
    public Span<T> ToSpan()
    {
        return MemoryMarshal.CreateSpan(ref buffer[0], count);
    }
    
    public Span<T>.Enumerator GetEnumerator()
    {
        return ToSpan().GetEnumerator();
    }
    
    public static implicit operator Span<T>(InlineList32<T> list) => list.ToSpan();
    
    [InlineArray(Length)]
    private struct InnerBuffer
    {
        private T element;
    }
}
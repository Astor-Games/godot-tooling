using System.Buffers;
using System.Runtime.InteropServices;

namespace GodotLib.Util;

public static class MemoryUtils
{
    
    public static unsafe Span<T> ReadBytesAs<T>(this ReadOnlySequence<byte> seq) where T : unmanaged
    {
        var count = (int)seq.Length / sizeof(T);
        var dst = new T[count].AsSpan();
        
        // Single segment (fast path)
        if (seq.IsSingleSegment)
        {
            var src = MemoryMarshal.Cast<byte, T>(seq.First.Span);
            src.CopyTo(dst);
            return dst;
        }

        // Multi-segment (slow path)
        int offset = 0;
        foreach (var segment in seq)
        {
            var src = MemoryMarshal.Cast<byte, T>(segment.Span);
            src.CopyTo(dst[offset..]);
            offset += src.Length;
        }
        return dst;
    }
}
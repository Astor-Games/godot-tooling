namespace GodotLib;

public static class ArrayExtensions
{
    extension<T>(T[] src)
    {
        public void CopyEnsureSize(ref T[] dst)
        {
            var size = src.Length;
            if (dst == null)
            {
                dst = new T[size];
            } 
            else if (dst.Length < size)
            {
                Array.Resize(ref dst, size);
            }
            src.CopyTo(dst.AsSpan()); 
        }
    }
}
using Collections.Pooled;
using ZLinq;

namespace GodotLib.Util;

public static class GeometryUtils
{
    public static (Vector3 tangent, Vector3 bitangent) BuildOrthonormalBasis(Vector3 normal)
    {
        var xOverZ = MathF.Abs(normal.X) > MathF.Abs(normal.Z);
        var tangent = xOverZ ? new Vector3(-normal.Y, normal.X, 0f).Normalized() : new Vector3(0f, -normal.Z, normal.Y).Normalized();
        var bitangent = normal.Cross(tangent).Normalized();
        return (tangent, bitangent);
    }
    
    public static ReadOnlySpan<Vector2> CreateConvexHull(ReadOnlySpan<Vector2> points)
    {
        if (points.Length <= 3) return points;

        // Sort by X then Y
        points = points.AsValueEnumerable().OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();

        PooledList<Vector2> lower = new();
        foreach (var p in points)
        {
            while (lower.Count >= 2 &&
                   Cross(lower[^2], lower[^1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }

        PooledList<Vector2> upper = new();
        for (var i = points.Length - 1; i >= 0; i--)
        {
            var p = points[i];
            while (upper.Count >= 2 &&
                   Cross(upper[^2], upper[^1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }

        upper.RemoveAt(upper.Count - 1);
        lower.RemoveAt(lower.Count - 1);
        return lower.AsValueEnumerable().Concat(upper).ToArray();
    }

    static float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
    }
    
    public static float CalculateArea(ReadOnlySpan<Vector2> polygon)
    {
        if (polygon.Length < 3) return 0f;

        var area = 0f;
        var origin = polygon[0];

        for (var i = 1; i < polygon.Length - 1; i++)
            area += CalculateArea(origin, polygon[i], polygon[i + 1]);

        return area;
    }

    public static float CalculateArea(Vector2 a, Vector2 b, Vector2 c)
    {
        return MathF.Abs((a.X*(b.Y-c.Y) + b.X*(c.Y-a.Y) + c.X*(a.Y-b.Y)) * 0.5f);
    }
}
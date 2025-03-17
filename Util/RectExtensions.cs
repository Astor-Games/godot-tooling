namespace GodotLib.Util;
using static Mathf;

public static class RectExtensions
{
    public static float DistanceTo(this Rect2 rect, Vector2 point)
    {
        var v1 = rect.Position - point;
        var v2 = point - (rect.Position + rect.Size);
        var d = v1.Max(v2);

        return d.Max(Vector2.Zero).Length() + Min(0.0f, Max(d.X, d.Y));
    }
}
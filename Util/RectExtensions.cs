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

    public static Vector2 ClosestPointOnBorder(this Rect2 rect, Vector2 point)
    {
        var leftPoint = new Vector2(rect.Position.X, Clamp(point.Y, rect.Position.Y, rect.Position.Y + rect.Size.Y));
        var rightPoint = new Vector2(rect.Position.X + rect.Size.X, Clamp(point.Y, rect.Position.Y, rect.Position.Y + rect.Size.Y));
        var topPoint = new Vector2(Clamp(point.X, rect.Position.X, rect.Position.X + rect.Size.X), rect.Position.Y);
        var bottomPoint = new Vector2(Clamp(point.X, rect.Position.X, rect.Position.X + rect.Size.X), rect.Position.Y + rect.Size.Y);

        var leftDist = point.DistanceSquaredTo(leftPoint);
        var rightDist = point.DistanceSquaredTo(rightPoint);
        var topDist = point.DistanceSquaredTo(topPoint);
        var bottomDist = point.DistanceSquaredTo(bottomPoint);

        var minDist = Min(Min(leftDist, rightDist), Min(topDist, bottomDist));

        if (minDist == leftDist) return leftPoint;
        if (minDist == rightDist) return rightPoint;
        if (minDist == topDist) return topPoint;
        return bottomPoint;
    }


}
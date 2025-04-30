using static Godot.Mathf;

namespace GodotLib.Util;

public static class RandomUtils
{
    public static Vector2 RandomPointInCircle(float radius)
    {
        var r = radius * Sqrt(Randf());
        var theta = Randf() * Tau;
        return new Vector2(r * Cos(theta), r * Sin(theta));
    }
}
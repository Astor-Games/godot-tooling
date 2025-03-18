namespace GodotLib.Util;

public static class Vector3Utils
{
    public static Vector3 ExpDamped(this Vector3 velocity, Vector3 damping, float delta)
    {
        return new Vector3(
            velocity.X * Mathf.Exp(-damping.X * delta),
            velocity.Y * Mathf.Exp(-damping.Y * delta),
            velocity.Z * Mathf.Exp(-damping.Z * delta)
        );
    }
}
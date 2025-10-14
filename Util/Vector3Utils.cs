namespace GodotLib.Util;

public static class Vector3Utils
{
    public static Vector3 ExpDamped(this Vector3 velocity, Vector3 damping, double delta)
    {
        return new Vector3(
            velocity.X * (float)Mathf.Exp(-damping.X * delta),
            velocity.Y * (float)Mathf.Exp(-damping.Y * delta),
            velocity.Z * (float)Mathf.Exp(-damping.Z * delta)
        );
    }
}
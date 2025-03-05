using System;
using Godot;
using static Godot.Mathf;

namespace GodotLib.Util;

public static class MathUtils
{
    //https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
    public static float Damp(float source, float target, float smoothing, double dt)
    {
        return Lerp(source, target, GetDampWeight(smoothing, dt));
    }

    public static float DampAngle(float source, float target, float smoothing, double dt)
    {
        return LerpAngle(source, target, GetDampWeight(smoothing, dt));
    }

    public static Vector2 Damp(this Vector2 source, Vector2 target, float smoothing, double dt)
    {
        return source.Lerp(target, GetDampWeight(smoothing, dt));
    }

    public static Vector3 Damp(this Vector3 source, Vector3 target, float smoothing, double dt)
    {
        return source.Lerp(target, GetDampWeight(smoothing, dt));
    }
    
    public static Vector3 Sdamp(this Vector3 source, Vector3 target, float smoothing, double dt)
    {
        return source.Slerp(target, GetDampWeight(smoothing, dt));
    }

    private static float GetDampWeight(float smoothing, double dt)
    {
        return 1 - Pow(smoothing, (float)dt);
    }
}

public static class MathExtensions
{
    public static void Clamp(this ref float value, float min, float max) {
        value = Mathf.Clamp(value, min, max);
    }
}
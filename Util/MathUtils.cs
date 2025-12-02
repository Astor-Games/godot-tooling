using Godot;
using JetBrains.Annotations;

namespace GodotLib.Util;

public static class MathUtils
{
    public static bool IsZeroApprox(this float value)
    {
        return Mathf.IsZeroApprox(value);
    }
    
    public static bool IsEqualApprox(this float value, float other)
    {
        return Mathf.IsEqualApprox(value, other);
    }
    
    //https://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
    public static float Damp(float source, float target, float smoothing, double dt)
    {
        return Mathf.Lerp(source, target, GetDampWeight(smoothing, dt));
    }

    public static float DampAngle(float source, float target, float smoothing, double dt)
    {
        return Mathf.LerpAngle(source, target, GetDampWeight(smoothing, dt));
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
    
    public static Basis Sdamp(this Basis source, Basis target, float smoothing, double dt)
    {
        return source.Slerp(target, GetDampWeight(smoothing, dt));
    }

    private static float GetDampWeight(float smoothing, double dt)
    {
        return 1 - Mathf.Pow(smoothing, (float)dt);
    }

    public static Vector3 DegToRad(Vector3 degrees)
    {
        return degrees * 0.017453292f;
    }
    
    public static Vector2 DegToRad(Vector2 degrees)
    {
        return degrees * 0.017453292f;
    }
    
    public static Vector3 RadToDeg(Vector3 radians)
    {
        return radians * 57.29578f;
    }
    
    public static Vector2 RadToDeg(Vector2 radians)
    {
        return radians * 57.29578f;
    }

    public static float Clamped(this float value, float min, float max) 
    {
        return Mathf.Clamp(value, min, max);
    }
    
    public static int Clamped(this int value, int min, int max) 
    {
        return Mathf.Clamp(value, min, max);
    }

    public static float Remap(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        return outputMin + (value - inputMin) * (outputMax - outputMin) / (inputMax - inputMin);
    }
    
    public static Vector2 Remap(Vector2 value, Vector2 inputMin, Vector2 inputMax, Vector2 outputMin, Vector2 outputMax)
    {
        return new Vector2(
            Remap(value.X, inputMin.X, inputMax.X, outputMin.X, outputMax.X),
            Remap(value.Y, inputMin.Y, inputMax.Y, outputMin.Y, outputMax.Y)
        );
    }
    
    public static Vector3 Remap(Vector3 value, Vector3 inputMin, Vector3 inputMax, Vector3 outputMin, Vector3 outputMax)
    {
        return new Vector3(
            Remap(value.X, inputMin.X, inputMax.X, outputMin.X, outputMax.X),
            Remap(value.Y, inputMin.Y, inputMax.Y, outputMin.Y, outputMax.Y),
            Remap(value.Z, inputMin.Z, inputMax.Z, outputMin.Z, outputMax.Z)
        );
    }

    public static int NextPowerOfTwo(int x)
    {
        if (x <= 1)
        {
            return 1;
        }

        x--;
        x |= x >> 1;
        x |= x >> 2;
        x |= x >> 4;
        x |= x >> 8;
        x |= x >> 16;
        x++;
        return x;
    }
}
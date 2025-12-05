using Godot;
using JetBrains.Annotations;

namespace GodotLib.Util;

public static class MathUtils
{
    extension(float value)
    {
        public bool IsZeroApprox()
        {
            return Mathf.IsZeroApprox(value);
        }

        public bool IsEqualApprox(float other)
        {
            return Mathf.IsEqualApprox(value, other);
        }

        public float Clamped(float min, float max) 
        {
            return Mathf.Clamp(value, min, max);
        }
    }
    
    extension(int value)
    {
        public int Clamped(int min, int max) 
        {
            return Mathf.Clamp(value, min, max);
        }
    }
    
    extension(Vector2 value)
    {
        public Vector2 Damp(Vector2 target, float smoothing, double dt)
        {
            return value.Lerp(target, GetDampWeight(smoothing, dt));
        }
    }
    
    extension(Vector3 value)
    {
        public Vector3 Damp(Vector3 target, float smoothing, double dt)
        {
            return value.Lerp(target, GetDampWeight(smoothing, dt));
        }

        public Vector3 Sdamp(Vector3 target, float smoothing, double dt)
        {
            return value.Slerp(target, GetDampWeight(smoothing, dt));
        }

        public Vector3 SlerpSafe(Vector3 b, float t)
        {
            const float epsilon = Mathf.Epsilon;

            var lenA = value.Length();
            var lenB = b.Length();

            if (lenA < epsilon && lenB < epsilon)
                return Vector3.Zero; // no direction at all

            if (lenA < epsilon)
                return b.Normalized();

            if (lenB < epsilon)
                return value.Normalized();

            // Normalize for angle test
            value /= lenA;
            b /= lenB;

            var dot = value.Dot(b);

            // Near-colinear or opposite — slerp becomes unstable
            if (Mathf.Abs(dot) > 0.9995f)
            {
                // Fall back to normalized linear interpolation
                return (value + (b - value) * t).Normalized();
            }

            return value.Slerp(b, t);
        }
    }
    
    extension(Basis value)
    {
        public Basis Sdamp(Basis target, float smoothing, double dt)
        {
            return value.Slerp(target, GetDampWeight(smoothing, dt));
        }
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
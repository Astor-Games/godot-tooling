using System.Diagnostics.Contracts;


namespace GodotLib;

public enum Shape2DType : byte
{
    Circle,
    Rect
}


public struct Shape2D
{
     public Shape2DType Type;

    // Shared
     public Vector2 Center;

    // Circle
     public float Radius;

    // Rect
     public Vector2 HalfSize;
  
    public static Shape2D Circle(Vector2 center, float radius)
    {
        return new Shape2D
        {
            Type = Shape2DType.Circle,
            Center = center,
            Radius = radius
        };
    }

    public static Shape2D Rect(Vector2 center, Vector2 size)
    {
        return new Shape2D
        {
            Type = Shape2DType.Rect,
            Center = center,
            HalfSize = size * 0.5f
        };
    }

    
    [Pure]
    public Shape2D Shrink(float delta)
    {
        return Grow(-delta);
    }

    [Pure]
    public Shape2D Grow(float delta)
    {
        var copy = this;
        switch (Type)
        {
            case Shape2DType.Circle:
                copy.Radius = Mathf.Max(0f, Radius + delta);
                break;

            case Shape2DType.Rect:
                var grown = HalfSize + new Vector2(delta, delta);
                copy.HalfSize = grown.Max(Vector2.Zero);
                break;
        }
        return copy;
    }
    
    [Pure]
    public Vector2 Sample(RandomNumberGenerator rng)
    {
        switch (Type)
        {
            case Shape2DType.Circle:
            {
                // sqrt for uniform area distribution
                var r = Radius * MathF.Sqrt(rng.Randf());
                var a = rng.Randf() * MathF.Tau;

                return Center + new Vector2(
                    MathF.Cos(a) * r,
                    MathF.Sin(a) * r
                );
            }

            case Shape2DType.Rect:
            {
                return Center + new Vector2(
                    (rng.Randf() * 2f - 1f) * HalfSize.X,
                    (rng.Randf() * 2f - 1f) * HalfSize.Y
                );
            }

            default:
                return Center;
        }
    }
}

namespace GodotLib.Util;

using Godot;

public static class Transform3DExtensions
{
    public static Vector3 Forward(this Transform3D transform)
    {
        return -transform.Basis.Z;
    }
    
    public static Vector3 Back(this Transform3D transform)
    {
        return transform.Basis.Z;
    }
    
    public static Vector3 Right(this Transform3D transform)
    {
        return transform.Basis.X;
    }
    
    public static Vector3 Left(this Transform3D transform)
    {
        return -transform.Basis.X;
    }
    
    public static Vector3 Up(this Transform3D transform)
    {
        return transform.Basis.Y;
    }
    
    public static Vector3 Down(this Transform3D transform)
    {
        return -transform.Basis.Y;
    }
}

public static class BasisExtensions
{
    public static Vector3 Forward(this Basis basis)
    {
        return -basis.Z;
    }
    
    public static Vector3 Back(this Basis basis)
    {
        return basis.Z;
    }
    
    public static Vector3 Right(this Basis basis)
    {
        return basis.X;
    }
    
    public static Vector3 Left(this Basis basis)
    {
        return -basis.X;
    }
    
    public static Vector3 Up(this Basis basis)
    {
        return basis.Y;
    }
    
    public static Vector3 Down(this Basis basis)
    {
        return -basis.Y;
    }
}

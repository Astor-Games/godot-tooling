namespace GodotLib.Util;

using Godot;

public static class Transform3DExtensions
{
    extension(Transform3D transform)
    {
        public Vector3 Forward => -transform.Basis.Z;

        public Vector3 Back => transform.Basis.Z;

        public Vector3 Right => transform.Basis.X;

        public Vector3 Left => -transform.Basis.X;

        public Vector3 Up => transform.Basis.Y;

        public Vector3 Down => -transform.Basis.Y;
        

        public Transform3D Rotated(Quaternion quaternion)
        {
            return new Transform3D(new Basis(quaternion), new Vector3()) * transform;
        }

        public Transform3D RotatedLocal(Quaternion quaternion)
        {
            return new Transform3D(transform.Basis * new Basis(quaternion), transform.Origin);
        }
    }
}

public static class BasisExtensions
{
    extension(Basis basis)
    {
        public Vector3 Forward => -basis.Z;

        public Vector3 Back => basis.Z;

        public Vector3 Right => basis.X;

        public Vector3 Left => -basis.X;

        public Vector3 Up => basis.Y;

        public Vector3 Down => -basis.Y;
    }

    public static Basis FromTangent(Vector3 tangent)
    {
        var up = Vector3.Up;

        if (Mathf.Abs(tangent.Dot(up)) > 0.99f)
            up = Vector3.Right;

        var x = tangent;
        var z = x.Cross(up).Normalized();
        var y = z.Cross(x);

        return new Basis(x, y, z);
    }
}

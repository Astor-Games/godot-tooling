namespace GodotLib.Util;

public static class ShapeExtensions
{
    extension(CylinderShape3D cylinderShape)
    {
        public float BaseArea => Mathf.Pi * cylinderShape.Radius * cylinderShape.Radius;
    }
    
    extension(BoxShape3D boxShape)
    {
        public float BaseArea => boxShape.Size.X * boxShape.Size.Z;
    }
}
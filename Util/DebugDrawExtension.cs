namespace GodotLib.Util;

internal static class DebugDrawExtension
{
    extension(DebugDraw3D)
    {
        public static void DrawDirections(in Transform3D transform, float scale = 1f, float duration = 0f)
        {
            var length = 0.75f * scale;
            DebugDraw3D.DrawArrowRay(transform.Origin, transform.Up, length, Colors.LimeGreen, 0.1f, duration: duration);
            DebugDraw3D.DrawArrowRay(transform.Origin, transform.Right, length, Colors.Red,0.1f, duration: duration);
            DebugDraw3D.DrawArrowRay(transform.Origin, transform.Back, length, Colors.DodgerBlue,0.1f,  duration: duration);
        }

        public static void DrawShape(Shape3D shape, in Transform3D transform, Color? color = null, float duration = 0f)
        {
            Transform3D scaledTransform;
            switch (shape)
            {
                case BoxShape3D boxShape:
                    scaledTransform = transform.ScaledLocal(boxShape.Size);
                    DebugDraw3D.DrawBoxXf(scaledTransform, color, true, duration);
                    return;
                
                case SphereShape3D sphereShape:
                    scaledTransform = transform.ScaledLocal( Vector3.One * sphereShape.Radius);
                    DebugDraw3D.DrawSphereXf(scaledTransform, color, duration);
                    return;
                
                case CylinderShape3D cylinderShape:
                    scaledTransform = transform.ScaledLocal(new Vector3(cylinderShape.Radius, cylinderShape.Height, cylinderShape.Radius));
                    DebugDraw3D.DrawCylinder(scaledTransform, color, duration);
                    return;
                
                case CapsuleShape3D capsuleShape:
                    var radius = capsuleShape.Radius;

                    var centerHeight = capsuleShape.Height - 2 * radius;
                    var a = transform.Origin + (transform.Up * centerHeight / 2f);
                    var b = transform.Origin - (transform.Up * centerHeight / 2f);
                    
                    DebugDraw3D.DrawCylinderAb(a, b, capsuleShape.Radius, color, duration);
                    
                    DebugDraw3D.DrawSphere(a, radius, color, duration);
                    DebugDraw3D.DrawSphere(b, radius, color, duration);
                    return;
                
                default:
                    PushWarning($"Shape {shape.GetType().Name} not supported.");
                    return;
            }
        }
    }
}
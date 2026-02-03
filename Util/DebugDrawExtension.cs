namespace GodotLib.Util;

internal static class DebugDrawExtension
{
    extension(DebugDrawMT)
    {
        public static void DrawDirections(in Transform3D transform, float scale = 1f, float duration = 0f)
        {
            var length = 0.75f * scale;
            DebugDrawMT.DrawArrowRay(transform.Origin, transform.Up, length, Colors.LimeGreen, 0.1f, duration: duration);
            DebugDrawMT.DrawArrowRay(transform.Origin, transform.Right, length, Colors.Red,0.1f, duration: duration);
            DebugDrawMT.DrawArrowRay(transform.Origin, transform.Back, length, Colors.DodgerBlue,0.1f, duration: duration);
        }

        public static void DrawShape(Shape3D shape, in Transform3D transform, Color? color = null, float duration = 0f)
        {
            Transform3D scaledTransform;
            switch (shape)
            {
                case BoxShape3D boxShape:
                    scaledTransform = transform.ScaledLocal(boxShape.Size);
                    DebugDrawMT.DrawBoxXf(scaledTransform, color, true, duration);
                    return;
                
                case SphereShape3D sphereShape:
                    scaledTransform = transform.ScaledLocal( Vector3.One * sphereShape.Radius);
                    DebugDrawMT.DrawSphereXf(scaledTransform, color, duration);
                    return;
                
                case CylinderShape3D cylinderShape:
                    scaledTransform = transform.ScaledLocal(new Vector3(cylinderShape.Radius, cylinderShape.Height, cylinderShape.Radius));
                    DebugDrawMT.DrawCylinder(scaledTransform, color, duration);
                    return;
                
                case CapsuleShape3D capsuleShape:
                    var position = transform.Origin;
                    var rotation = transform.Basis.GetRotationQuaternion();
                    DebugDrawMT.DrawCapsule(position, rotation, capsuleShape.Radius, capsuleShape.Height, color, duration);
                   // DebugDrawMT.DrawSphere(position, capsuleShape.Radius, color, duration);
                    return;
                
                default:
                    PushWarning($"Shape {shape.GetType().Name} not supported.");
                    return;
            }
        }
    }
}
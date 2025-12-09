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
            DebugDraw3D.DrawArrowRay(transform.Origin, transform.Forward, length, Colors.DodgerBlue,0.1f,  duration: duration);
        }
    }
}
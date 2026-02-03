using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using real_t = float;

namespace GodotLib.Util;

public partial class DebugDrawMT : Node
{
    private static DebugDrawMT Instance
    {
        get
        {
            if (field != null) return field;

            field = new DebugDrawMT()
            {
                Name = "dd3d_defer",
                ProcessPriority = int.MaxValue - 10, // process slightly before the DD3D manager
                ProcessPhysicsPriority = int.MaxValue - 10,
            };
            
            EngineUtils.SceneTree.Root.AddChild(field);
            return field;
        }
    }
    private readonly ConcurrentBag<Action> processCalls = new();
    private readonly ConcurrentBag<Action> physicsProcessCalls = new();
    
     /// <summary>
    /// Draw a sphere
    /// </summary>
    /// <param name="position">Center of the sphere</param>
    /// <param name="radius">Sphere radius</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawSphere(Vector3 position, real_t radius = 0.5f, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawSphere(position, radius, color, duration));
    }

    /// <summary>
    /// Draw a sphere with a radius of 0.5
    /// </summary>
    /// <param name="transform">Sphere transform</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawSphereXf(Transform3D transform, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawSphereXf(transform, color, duration));
    }

    /// <summary>
    /// Draw a vertical capsule
    /// <para>
    /// A capsule will not be displayed if the height or radius is approximately equal to or less than zero.
    /// </para>
    ///
    /// ---
    /// <para>
    /// If you need to apply additional transformations, you can use DebugDraw3DScopeConfig.set_transform.
    /// </para>
    ///
    /// </summary>
    /// <param name="position">Capsule position</param>
    /// <param name="rotation">Capsule rotation</param>
    /// <param name="radius">Capsule radius</param>
    /// <param name="height">Capsule height including caps. Based on this value, the actual radius of the capsule will be calculated.</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawCapsule(Vector3 position, Quaternion rotation, real_t radius, real_t height, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawCapsule(position, rotation, radius, height, color, duration));
    }

    /// <summary>
    /// Draw a capsule between points A and B with the desired radius.
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// <para>
    /// A capsule will not be displayed if the distance between points A and B or the radius is approximately equal to or less than zero.
    /// </para>
    ///
    /// </summary>
    /// <param name="a">First pole of the capsule</param>
    /// <param name="b">Second pole of the capsule</param>
    /// <param name="radius">Capsule radius</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawCapsuleAb(Vector3 a, Vector3 b, real_t radius = 0.5f, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawCapsuleAb(a, b, radius, color, duration));
    }

    /// <summary>
    /// Draw a vertical cylinder with radius 1.0 (x, z) and height 1.0 (y)
    /// </summary>
    /// <param name="transform">Cylinder transform</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawCylinder(Transform3D transform, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawCylinder(transform, color, duration));
    }

    /// <summary>
    /// Draw a cylinder between points A and B with a certain radius.
    ///
    /// <para>
    /// A cylinder will not be displayed if the distance between points A and B is approximately zero.
    /// </para>
    /// </summary>
    /// <param name="a">Bottom point of the Cylinder</param>
    /// <param name="b">Top point of the Cylinder</param>
    /// <param name="radius">Cylinder radius</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawCylinderAb(Vector3 a, Vector3 b, real_t radius = 0.5f, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawCylinderAb(a, b, radius, color, duration));
    }

    /// <summary>
    /// Draw a box
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// </summary>
    /// <param name="position">Position of the Box</param>
    /// <param name="rotation">Rotation of the box</param>
    /// <param name="size">Size of the Box</param>
    /// <param name="color">Primary color</param>
    /// <param name="is_box_centered">Set where the center of the box will be. In the center or in the bottom corner</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawBox(Vector3 position, Quaternion rotation, Vector3 size, Color? color = null, bool is_box_centered = false, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawBox(position, rotation, size, color, is_box_centered, duration));
    }

    /// <summary>
    /// Draw a box between points A and B by rotating and scaling based on the up vector
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// <para>
    /// A box will not be displayed if its dimensions are close to zero or if the up vector is approximately zero.
    /// </para>
    ///
    /// </summary>
    /// <param name="a">Start position</param>
    /// <param name="b">End position</param>
    /// <param name="up">Vertical vector by which the box will be aligned</param>
    /// <param name="color">Primary color</param>
    /// <param name="is_ab_diagonal">Set uses the diagonal between the corners or the diagonal between the centers of two edges</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawBoxAb(Vector3 a, Vector3 b, Vector3 up, Color? color = null, bool is_ab_diagonal = true, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawBoxAb(a, b, up, color, is_ab_diagonal, duration));
    }

    /// <summary>
    /// Draw a box as in DebugDraw3D.draw_box
    ///
    /// </summary>
    /// <param name="transform">Box transform</param>
    /// <param name="color">Primary color</param>
    /// <param name="is_box_centered">Set where the center of the box will be. In the center or in the bottom corner</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawBoxXf(Transform3D transform, Color? color = null, bool is_box_centered = true, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawBoxXf(transform, color, is_box_centered, duration));
    }

    /// <summary>
    /// Draw a box as in DebugDraw3D.draw_box, but based on the AABB
    ///
    /// </summary>
    /// <param name="aabb">AABB</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawAabb(Aabb aabb, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawAabb(aabb, color, duration));
    }

    /// <summary>
    /// Draw the box as in DebugDraw3D.draw_aabb, but AABB is defined by the diagonal AB
    ///
    /// </summary>
    /// <param name="a">Start position</param>
    /// <param name="b">End position</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawAabbAb(Vector3 a, Vector3 b, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawAabbAb(a, b, color, duration));
    }

    /// <summary>
    /// Draw line separated by hit point (billboard square) or not separated if `is_hit = false`.
    ///
    /// Some of the default settings can be overridden in DebugDraw3DConfig.
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="hit">Hit point</param>
    /// <param name="is_hit">Whether to draw the collision point</param>
    /// <param name="hit_size">Size of the hit point</param>
    /// <param name="hit_color">Color of the hit point and line before hit</param>
    /// <param name="after_hit_color">Color of line after hit position</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawLineHit(Vector3 start, Vector3 end, Vector3 hit, bool is_hit, real_t hit_size = 0.25f, Color? hit_color = null, Color? after_hit_color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawLineHit(start, end, hit, is_hit, hit_size, hit_color, after_hit_color, duration));
    }

    /// <summary>
    /// Draw line separated by hit point.
    ///
    /// Similar to DebugDraw3D.draw_line_hit, but instead of a hit point, an offset from the start point is used.
    ///
    /// Some of the default settings can be overridden in DebugDraw3DConfig.
    ///
    /// </summary>
    /// <param name="start">Start point</param>
    /// <param name="end">End point</param>
    /// <param name="is_hit">Whether to draw the collision point</param>
    /// <param name="unit_offset_of_hit">Unit offset on the line where the collision occurs</param>
    /// <param name="hit_size">Size of the hit point</param>
    /// <param name="hit_color">Color of the hit point and line before hit</param>
    /// <param name="after_hit_color">Color of line after hit position</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawLineHitOffset(Vector3 start, Vector3 end, bool is_hit, real_t unit_offset_of_hit = 0.5f, real_t hit_size = 0.25f, Color? hit_color = null, Color? after_hit_color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawLineHitOffset(start, end, is_hit, unit_offset_of_hit, hit_size, hit_color, after_hit_color, duration));
    }

    /// <summary>
    /// Draw a single line
    /// </summary>
    /// <param name="a">Start point</param>
    /// <param name="b">End point</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawLine(Vector3 a, Vector3 b, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawLine(a, b, color, duration));
    }

    /// <summary>
    /// Draw a ray.
    ///
    /// Same as DebugDraw3D.draw_line, but uses origin, direction and length instead of A and B.
    ///
    /// </summary>
    /// <param name="origin">Origin</param>
    /// <param name="direction">Direction</param>
    /// <param name="length">Length</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawRay(Vector3 origin, Vector3 direction, real_t length, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawRay(origin, direction, length, color, duration));
    }

    /// <summary>
    /// Draw an array of lines. Each line is two points, so the array must be of even size.
    /// </summary>
    /// <param name="lines">An array of points of lines. 1 line = 2 vectors3. The array size must be even.</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawLines(Vector3[] lines, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawLines(lines, color, duration));
    }

    /// <summary>
    /// Draw an array of lines.
    ///
    /// Unlike DebugDraw3D.draw_lines, here lines are drawn between each point in the array.
    ///
    /// The array can be of any size.
    ///
    /// <para>
    /// If the path size is equal to 1, then DebugDraw3D.draw_square will be used instead of drawing a line.
    /// </para>
    ///
    /// </summary>
    /// <param name="path">Sequence of points</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawLinePath(Vector3[] path, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawLinePath(path, color, duration));
    }

    /// <summary>
    /// Draw the arrowhead
    ///
    /// </summary>
    /// <param name="transform">godot::Transform3D of the Arrowhead</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawArrowhead(Transform3D transform, Color? color = null, real_t duration = 0)
    {
        EnqueueAction((() => DebugDraw3D.DrawArrowhead(transform, color, duration)));
    }

    /// <summary>
    /// Draw line with arrowhead
    /// <para>
    /// An arrow will not be displayed if the distance between points a and b is approximately zero.
    /// </para>
    ///
    /// </summary>
    /// <param name="a">Start point</param>
    /// <param name="b">End point</param>
    /// <param name="color">Primary color</param>
    /// <param name="arrow_size">Size of the arrow</param>
    /// <param name="is_absolute_size">Is `arrow_size` absolute or relative to the length of the string?</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawArrow(Vector3 a, Vector3 b, Color? color = null, real_t arrow_size = 0.5f, bool is_absolute_size = false, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawArrow(a, b, color, arrow_size, is_absolute_size, duration));
    }

    /// <summary>
    /// Same as DebugDraw3D.draw_arrow, but uses origin, direction and length instead of A and B.
    ///
    /// </summary>
    /// <param name="origin">Origin</param>
    /// <param name="direction">Direction</param>
    /// <param name="length">Length</param>
    /// <param name="color">Primary color</param>
    /// <param name="arrow_size">Size of the arrow</param>
    /// <param name="is_absolute_size">Is `arrow_size` absolute or relative to the line length?</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawArrowRay(Vector3 origin, Vector3 direction, real_t length, Color? color = null, real_t arrow_size = 0.5f, bool is_absolute_size = false, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawArrowRay(origin, direction, length, color, arrow_size, is_absolute_size, duration));
    }

    /// <summary>
    /// Draw a sequence of points connected by lines with arrows like DebugDraw3D.draw_line_path.
    /// <para>
    /// If the path size is equal to 1, then DebugDraw3D.draw_square will be used instead of drawing a line.
    /// </para>
    ///
    /// </summary>
    /// <param name="path">Sequence of points</param>
    /// <param name="color">Primary color</param>
    /// <param name="arrow_size">Size of the arrow</param>
    /// <param name="is_absolute_size">Is the `arrow_size` absolute or relative to the length of the line?</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawArrowPath(Vector3[] path, Color? color = null, real_t arrow_size = 0.75f, bool is_absolute_size = true, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawArrowPath(path, color, arrow_size, is_absolute_size, duration));
    }

    /// <summary>
    /// Draw a sequence of points connected by lines using billboard squares or spheres like DebugDraw3D.draw_line_path.
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// <para>
    /// If the path size is equal to 1, then DebugDraw3D.draw_square or DebugDraw3D.draw_sphere will be used instead of drawing a line.
    /// </para>
    ///
    /// </summary>
    /// <param name="path">Sequence of points</param>
    /// <param name="type">Type of points</param>
    /// <param name="points_color">Color of points</param>
    /// <param name="lines_color">Color of lines</param>
    /// <param name="size">Size of squares</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawPointPath(Vector3[] path, DebugDraw3D.PointType type = DebugDraw3D.PointType.TypeSquare, real_t size = 0.25f, Color? points_color = null, Color? lines_color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawPointPath(path, type, size, points_color, lines_color, duration));
    }

    /// <summary>
    /// Draw a sequence of points using billboard squares or spheres.
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// </summary>
    /// <param name="points">Sequence of points</param>
    /// <param name="type">Type of points</param>
    /// <param name="size">Size of squares</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawPoints(Vector3[] points, DebugDraw3D.PointType type = DebugDraw3D.PointType.TypeSquare, real_t size = 0.25f, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawPoints(points, type, size, color, duration));
    }

    /// <summary>
    /// Draw a square that will always be turned towards the camera
    ///
    /// </summary>
    /// <param name="position">Center position of square</param>
    /// <param name="size">Square size</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawSquare(Vector3 position, real_t size = 0.2f, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawSquare(position, size, color, duration));
    }

    /// <summary>
    /// Draws a plane of non-infinite size relative to the position of the current camera.
    ///
    /// The plane size is set based on the `Far` parameter of the current camera or with DebugDraw3DScopeConfig.set_plane_size.
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// </summary>
    /// <param name="plane">Plane data</param>
    /// <param name="color">Primary color</param>
    /// <param name="anchor_point">A point that is projected onto a Plane, and its projection is used as the center of the drawn plane</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawPlane(Plane plane, Color? color = null, Vector3? anchor_point = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawPlane(plane, color, anchor_point, duration));
    }

    /// <summary>
    /// Draw 3 intersecting lines with the given transformations
    ///
    /// [THERE WAS AN IMAGE]
    ///
    /// </summary>
    /// <param name="transform">godot::Transform3D of lines</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawPosition(Transform3D transform, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawPosition(transform, color, duration));
    }

    /// <summary>
    /// Draw 3 lines with the given transformations and arrows at the ends
    /// </summary>
    /// <param name="transform">godot::Transform3D of lines</param>
    /// <param name="color">Primary color</param>
    /// <param name="is_centered">If `true`, then the lines will intersect in the center of the transform</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawGizmo(Transform3D transform, Color? color = null, bool is_centered = false, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawGizmo(transform, color, is_centered, duration));
    }

    /// <summary>
    /// Draw simple grid with given size and subdivision
    /// </summary>
    /// <param name="origin">Grid origin</param>
    /// <param name="x_size">Direction and size of the X side. As an axis in the Basis.</param>
    /// <param name="y_size">Direction and size of the Y side. As an axis in the Basis.</param>
    /// <param name="subdivision">Number of cells for the X and Y axes</param>
    /// <param name="color">Primary color</param>
    /// <param name="is_centered">Draw lines relative to origin</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawGrid(Vector3 origin, Vector3 x_size, Vector3 y_size, Vector2I subdivision, Color? color = null, bool is_centered = true, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawGrid(origin, x_size, y_size, subdivision, color, is_centered, duration));
    }

    /// <summary>
    /// Draw a simple grid with a given transform and subdivision.
    ///
    /// Like DebugDraw3D.draw_grid, but instead of origin, x_size and y_size, a single transform is used.
    ///
    /// </summary>
    /// <param name="transform">godot::Transform3D of the Grid</param>
    /// <param name="p_subdivision">Number of cells for the X and Y axes</param>
    /// <param name="color">Primary color</param>
    /// <param name="is_centered">Draw lines relative to origin</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawGridXf(Transform3D transform, Vector2I p_subdivision, Color? color = null, bool is_centered = true, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawGridXf(transform, p_subdivision, color, is_centered, duration));
    }

    /// <summary>
    /// Draw camera frustum area.
    /// </summary>
    /// <param name="camera">Camera node</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawCameraFrustum(Camera3D camera, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawCameraFrustum(camera, color, duration));
    }

    /// <summary>
    /// Draw the frustum area of the camera based on an array of 6 planes.
    ///
    /// </summary>
    /// <param name="camera_frustum">Array of frustum planes</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawCameraFrustumPlanes(Plane[] camera_frustum, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawCameraFrustumPlanes(camera_frustum, color, duration));
    }

    /// <summary>
    /// Draw text using Label3D.
    ///
    /// <para>
    /// Outline can be changed using DebugDraw3DScopeConfig.set_text_outline_color and DebugDraw3DScopeConfig.set_text_outline_size.
    /// The font can be changed using DebugDraw3DScopeConfig.set_text_font.
    /// The text can be made to stay the same size regardless of distance using DebugDraw3DScopeConfig.set_text_fixed_size.
    /// </para>
    /// </summary>
    /// <param name="position">Center position of Label</param>
    /// <param name="text">Label's text</param>
    /// <param name="size">Font size</param>
    /// <param name="color">Primary color</param>
    /// <param name="duration">The duration of how long the object will be visible</param>
    public static void DrawText(Vector3 position, string text, int size = 32, Color? color = null, real_t duration = 0)
    {
        EnqueueAction(() => DebugDraw3D.DrawText(position, text, size, color, duration));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnqueueAction(Action action)
    {
        if (Engine.IsInPhysicsFrame()) Instance.physicsProcessCalls.Add(action);
        else Instance.processCalls.Add(action);
    }

    
    public override void _Process(double delta)
    {
        foreach (var call in processCalls)
        {
            call.Invoke();
        }
        processCalls.Clear();
    }
    
    public override void _PhysicsProcess(double delta)
    {
        foreach (var call in physicsProcessCalls)
        {
            call.Invoke();
        }
        physicsProcessCalls.Clear();
    }
}
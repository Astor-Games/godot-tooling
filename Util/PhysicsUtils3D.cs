using System.Runtime.CompilerServices;
using GodotLib.ProjectConstants;
using static System.Runtime.CompilerServices.MethodImplOptions;
using static Godot.PhysicsServer3D.AreaParameter;
using static GodotLib.Debug.Assertions;

namespace GodotLib.Util;

public static class PhysicsUtils3D
{
    public static PhysicsDirectSpaceState3D DefaultSpaceState => spaceState ??= EngineUtils.SceneTree.Root.GetWorld3D().DirectSpaceState;
    private static PhysicsDirectSpaceState3D spaceState;


     /// Use global coordinates, not local to node
     public static RaycastResult Raycast(Vector3 origin, Vector3 direction, PhysicsLayers3D mask = PhysicsLayers3D.All, bool collideWithAreas = false, bool hitFromInside = false)
     {
         return RaycastTo(DefaultSpaceState, origin, origin + direction, (uint)mask, collideWithAreas, hitFromInside);
     }
    
    /// Use global coordinates, not local to node
    public static RaycastResult RaycastTo(Vector3 from, Vector3 to, PhysicsLayers3D mask = PhysicsLayers3D.All, bool collideWithAreas = false, bool hitFromInside = false)
    {
        return RaycastTo(DefaultSpaceState, from, to, (uint)mask, collideWithAreas, hitFromInside);
    }
    
    /// Use global coordinates, not local to node
    public static RaycastResult Raycast(PhysicsDirectSpaceState3D space, Vector3 origin, Vector3 direction, PhysicsLayers3D mask = PhysicsLayers3D.All, bool collideWithAreas = false, bool hitFromInside = false)
    {
        return RaycastTo(space, origin, origin + direction, (uint)mask, collideWithAreas, hitFromInside);
    }
    
    /// Use global coordinates, not local to node
    public static RaycastResult RaycastTo(PhysicsDirectSpaceState3D space,Vector3 from, Vector3 to, PhysicsLayers3D mask = PhysicsLayers3D.All, bool collideWithAreas = false, bool hitFromInside = false)
    {
        return RaycastTo(space, from, to, (uint)mask, collideWithAreas, hitFromInside);
    }

    /// Use global coordinates, not local to node
    private static RaycastResult RaycastTo(PhysicsDirectSpaceState3D space, Vector3 from, Vector3 to, uint mask, bool collideWithAreas, bool hitFromInside)
    {
        AssertTrue(!from.IsEqualApprox(to), "Raycast distance is too small");

        var query = PhysicsRayQueryParameters3D.Create(from, to, mask);
        query.HitFromInside = hitFromInside;
        query.CollideWithAreas = collideWithAreas;
        var result = new RaycastResult(space.IntersectRay(query));
        DebugDraw3D.DrawArrow(from, to, result.IsHit ? Colors.LimeGreen : Colors.Red, 0.2f, true);
        return result;
    }

    [MethodImpl(AggressiveInlining)]
    public static Transform3D BodyGetTransform(ResourceId body)
    {
        return PhysicsServer3D.BodyGetState(body, PhysicsServer3D.BodyState.Transform).AsTransform3D();
    }
    
    [MethodImpl(AggressiveInlining)]
    public static void BodySetTransform(ResourceId body, Transform3D transform)
    {
        PhysicsServer3D.BodySetState(body, PhysicsServer3D.BodyState.Transform, transform);
    }

    public static ResourceId CreateSphereShape(float radius)
    {
        var shapeRid = PhysicsServer3D.SphereShapeCreate();
        PhysicsServer3D.ShapeSetData(shapeRid, radius);
        return shapeRid;
    }

    public static ResourceId CreateBoxShape(Vector3 size)
    {
        var shapeRid = PhysicsServer3D.BoxShapeCreate();
        PhysicsServer3D.ShapeSetData(shapeRid, size / 2.0f);
        return shapeRid;
    }

    public static ResourceId CreateCapsuleShape(float height, float radius)
    {
        var shapeRid = PhysicsServer3D.CapsuleShapeCreate();
        var data = new GodotDictionary
        {
            { "radius", radius },
            { "height", height }
        };
        PhysicsServer3D.ShapeSetData(shapeRid, data);
        return shapeRid;
    }

    public static ResourceId CreateCylinderShape(float height, float radius)
    {
        var shapeRid = PhysicsServer3D.CylinderShapeCreate();
        var data = new GodotDictionary
        {
            { "radius", radius },
            { "height", height }
        };
        PhysicsServer3D.ShapeSetData(shapeRid, data);
        return shapeRid;
    }

    public static ResourceId CreateConcavePolygonShape(Vector3[] faces, bool backfaceCollision)
    {
        var shapeRid = PhysicsServer3D.ConcavePolygonShapeCreate();
        var data = new GodotDictionary
        {
            { "faces", faces },
            { "backface_collision", backfaceCollision }
        };
        PhysicsServer3D.ShapeSetData(shapeRid, data);
        return shapeRid;
    }

    public static ResourceId CreateConvexPolygonShape(Vector3[] points)
    {
        var shapeRid = PhysicsServer3D.ConvexPolygonShapeCreate();
        PhysicsServer3D.ShapeSetData(shapeRid, points);
        return shapeRid;
    }

    public static ResourceId CreateHeightMapShape(int mapWidth, int mapDepth, float[] mapData)
    {
        var shapeRid = PhysicsServer3D.HeightmapShapeCreate();

        var minHeight = float.MaxValue;
        var maxHeight = float.MinValue;

        foreach (var v in mapData)
        {
            if (v < minHeight) minHeight = v;
            if (v > maxHeight) maxHeight = v;
        }

        var data = new GodotDictionary
        {
            { "width", mapWidth },
            { "depth", mapDepth },
            { "heights", mapData },
            { "min_height", minHeight },
            { "max_height", maxHeight }
        };
        PhysicsServer3D.ShapeSetData(shapeRid, data);
        return shapeRid;
    }

    public static ResourceId CreateSpace(bool active = false, float? defaultGravity = null, Vector3? defaultGravityVector = null, float? defaultLinearDamp = null, float? defaultAngularDamp = null)
    {
        var space  = PhysicsServer3D.SpaceCreate();
        PhysicsServer3D.SpaceSetActive(space, active);
        
        PhysicsServer3D.AreaSetParam(space, Gravity, defaultGravity ?? ProjectSettings.GetSettingWithOverride("physics/3d/default_gravity"));
        PhysicsServer3D.AreaSetParam(space, GravityVector, defaultGravityVector ?? ProjectSettings.GetSettingWithOverride("physics/3d/default_gravity_vector"));
        PhysicsServer3D.AreaSetParam(space, LinearDamp, defaultLinearDamp ?? ProjectSettings.GetSettingWithOverride("physics/3d/default_linear_damp"));
        PhysicsServer3D.AreaSetParam(space, AngularDamp, defaultAngularDamp ?? ProjectSettings.GetSettingWithOverride("physics/3d/default_angular_damp"));

        return space;
    }
}
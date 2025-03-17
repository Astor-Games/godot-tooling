using Godot;
using Godot.Collections;
using GodotLib.ProjectConstants;

namespace GodotLib.Util;

public static class PhysicsUtils3D
{
    public static PhysicsDirectSpaceState3D DefaultSpaceState => spaceState ??= ((SceneTree)Engine.GetMainLoop()).Root.GetWorld3D().DirectSpaceState;
    private static PhysicsDirectSpaceState3D spaceState;


     /// Use global coordinates, not local to node
     public static RaycastResult Raycast(Vector3 origin, Vector3 direction, PhysicsLayers3D mask = PhysicsLayers3D.All, bool collideWithAreas = false, bool hitFromInside = false)
     {
         return RaycastTo(origin, origin + direction, (uint)mask, collideWithAreas, hitFromInside);
     }
    
    /// Use global coordinates, not local to node
    public static RaycastResult RaycastTo(Vector3 from, Vector3 to, PhysicsLayers3D mask = PhysicsLayers3D.All, bool collideWithAreas = false, bool hitFromInside = false)
    {
        return RaycastTo(from, to, (uint)mask, collideWithAreas, hitFromInside);
    }

    /// Use global coordinates, not local to node
    private static RaycastResult RaycastTo(Vector3 from, Vector3 to, uint mask, bool collideWithAreas, bool hitFromInside)
    {
        var query = PhysicsRayQueryParameters3D.Create(from, to, mask);
        query.HitFromInside = hitFromInside;
        query.CollideWithAreas = collideWithAreas;
        var result = new RaycastResult(DefaultSpaceState.IntersectRay(query));
        DebugDraw3D.DrawLine(from, to, result.IsHit ? Colors.Green : Colors.Red);
        return result;
    }
}

public struct RaycastResult
{
    private readonly Dictionary dictionary;

    public static implicit operator bool(RaycastResult result)
    {
        return result.IsHit;
    }
    
    public bool IsHit => isHit ??= dictionary.Count > 0;
    public Vector3 Position => IsHit ? position ??= (Vector3) dictionary["position"] : Vector3.Zero;
    public Vector3 Normal => IsHit ? normal ??= (Vector3) dictionary["normal"] : Vector3.Zero;
    public Node3D Collider => IsHit ? collider ??= (Node3D) dictionary["collider"] : null;

    private bool? isHit = null;
    private Vector3? position = null;
    private Vector3? normal = null;
    private Node3D collider = null;

    // Constructor to initialize the class with data
    public RaycastResult(Dictionary dictionary)
    {
        this.dictionary = dictionary;
    }
}
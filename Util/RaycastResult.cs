namespace GodotLib.Util;

public record struct RaycastResult
{
    private readonly GodotDictionary dictionary;

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
    public RaycastResult(GodotDictionary dictionary)
    {
        this.dictionary = dictionary;
    }
}
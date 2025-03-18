using GodotLib.Pooling;
using GodotLib.Util;
using static Godot.Mathf;
using static GodotLib.UI.WorldSpaceIndicator.OffScreenBehavior;

namespace GodotLib.UI;

[GlobalClass, Icon("res://addons/godot_lib/Editor/Icons/indicator.svg")]
public partial class WorldSpaceIndicator : Control, Resettable
{
    private const float ArrowOffset = Tau / 4; // 90 degrees;

    public enum OffScreenBehavior
    {
        Hide,
        ShowDirectionalArrow
    }
    
    [Export] public string Text
    {
        get => __text;
        set
        {
            __text = value;
            if (label != null) label.Text = value;
        }
    }

    [Export] public Node3D Target;
    [Export] public OffScreenBehavior WhenOffScreen = OffScreenBehavior.Hide;
    [Export] protected bool StartEnabled = true;
    [Export] private float arrowRadius = 65.0f;
   
    
    private float minimumDistance, minDistSquared;
    private float maximumDistance, maxDistSquared;

    public bool IsOnScreen { get; private set; }

    [Export] public float MinimumDistance
    {
        get => minimumDistance;
        set
        {
            minimumDistance = value;
            minDistSquared = MinimumDistance * MinimumDistance;
        }
    }

    [Export] public float MaximumDistance
    {
        get => maximumDistance;
        set
        {
            maximumDistance = value;
            maxDistSquared = MaximumDistance > MinimumDistance ? MaximumDistance * MaximumDistance : float.MaxValue;
        }
    }

    public bool Enabled;
    
    private CollisionShape2D debugShape;
    private Fader arrow, content, onlyOnScreenContent;
    
    private Label label;
    private string __text;

    public override void _Ready()
    {
        content = GetNode<Fader>("%Content");
        onlyOnScreenContent = GetNode<Fader>("%Content/OnlyOnScreen");
        label = GetNode<Label>("%Content/OnlyOnScreen/Label");
        arrow = GetNode<Fader>("%DirectionArrow");
        Reset();
        
        if (StartEnabled) 
            Show();
    }

    public override void _Process(double delta)
    {
        if(!Enabled)
        {
            content.BeVisible = false;
            arrow.BeVisible = false;
            return;
        }
        
        var currentCamera = GetViewport().GetCamera3D();
        var cameraPos = currentCamera.GlobalPosition;
        var targetPos = Target.GlobalPosition;
        var distance = targetPos.DistanceSquaredTo(cameraPos);
        
        // Case 1: The object is out of the visible range, hide it
        
        if (distance < minDistSquared || distance > maxDistSquared)
        {
            content.BeVisible = false;
            arrow.BeVisible = false;
            return;
        }
        
        var viewportRect = GetViewportRect();
        var camToObj = cameraPos - targetPos;
        var camForward = currentCamera.GlobalTransform.Forward(); 
        var dot = camToObj.Normalized().Dot(camForward);

        var screenPos = currentCamera.UnprojectPosition(targetPos);
        
        // Case 2: The object is in range and within screen bounds, show normally
        if (dot < 0 && viewportRect.DistanceTo(screenPos) < -arrowRadius) 
        {
            IsOnScreen = true;
            content.BeVisible = true;
            onlyOnScreenContent.BeVisible = true;
            arrow.BeVisible = false;
            Position = screenPos - PivotOffset;
            return;   
        }
        
        IsOnScreen = false;
        
        // Case 3: Camera is facing away from object, show directional arrow
        if (WhenOffScreen == ShowDirectionalArrow && dot <= 0.98)
        {
            var localPos = currentCamera.ToLocal(targetPos);
            localPos.Z = currentCamera.Near + 0.05f;
            localPos.X *= 100;
            localPos.Y *= 100;

            screenPos = LocalToViewportPoint(currentCamera, localPos);
            Position = screenPos - PivotOffset;

            content.BeVisible = true;
            onlyOnScreenContent.BeVisible = false;
            arrow.BeVisible = true;
            arrow.Rotation = viewportRect.GetCenter().AngleToPoint(screenPos) + ArrowOffset;
            return;
        }
        
        //Case 4: The indicator is set to Hide or the angle is so extreme that math breaks
        content.BeVisible = false;
        arrow.BeVisible = false;
    }

    private Vector2 LocalToViewportPoint(Camera3D camera, Vector3 localPoint)
        {
            var viewportSize = GetViewport().GetVisibleRect().Size;

            // In camera space (Godot), the camera faces -Z.
            // We want to find t such that (t * localPoint).z == -camera.Near.
            // That is, t = (-camera.Near / localPoint.z).
            var t = -camera.Near / localPoint.Z;
            var localIntersection = t * localPoint;

            // Now convert the local intersection point to normalized device coordinates (NDC)
            // using the camera’s projection matrix.
            var projection = camera.GetCameraProjection();
            var ndc = projection * localIntersection;
            
            // Map NDC [-1, 1] to viewport pixel coordinates.
            var screenX = (1.0f - (ndc.X * 0.5f + 0.5f)) * viewportSize.X;
            var screenY = (ndc.Y * 0.5f + 0.5f) * viewportSize.Y;
            
            
            var screenPoint = new Vector2(screenX, screenY);
            
            // If the point is offscreen, we want to push it back toward the center
            // along the same direction so that it lands on the edge.
            // Compute the offset from screen center.
            var center = viewportSize * 0.5f;
            var offset = screenPoint - center;
            
            var limit = center - Vector2.One * arrowRadius;
            
            // If the offset exceeds the available half-size, scale it down.
            if (Abs(offset.X) > limit.X || Abs(offset.Y) > limit.Y)
            {
                var scaleX = limit.X / Abs(offset.X);
                var scaleY = limit.Y / Abs(offset.Y);
                var scale = Min(scaleX, scaleY);
                offset *= scale;
                screenPoint = center + offset;
            }

            return screenPoint;
        }
  
    public void Reset()
    {
        content.BeVisible = false;
        content.Visible = false;
        
        arrow.BeVisible = false;
        arrow.Visible = false;
        
        Enabled = false;
    }

    public override void _EnterTree()
    {
        Text = __text; // In case it was set before _ready
    }
}
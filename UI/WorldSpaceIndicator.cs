using GodotLib.Pooling;
using GodotLib.Util;
using static Godot.Mathf;
using static GodotLib.UI.WorldSpaceIndicator.OffScreenBehavior;

namespace GodotLib.UI;

[GlobalClass, Icon("res://tools/Editor/Icons/indicator.svg")]
public partial class WorldSpaceIndicator : Control, Resettable
{
    private const float ArrowOffset = Tau / 4; // 90 degreess;

    public enum OffScreenBehavior
    {
        Hide,
        ShowDirectionalArrow
    }

    [Export] public Node3D Target;
    [Export] public OffScreenBehavior WhenOffScreen = OffScreenBehavior.Hide;
    [Export] protected bool StartEnabled = true;
    [Export] private float arrowRadius = 65.0f;
    
    public bool IsOnScreen { get; private set; }
    public bool Enabled;
    
    private CollisionShape2D debugShape;
    private Fader arrow, content, onlyOnScreenContent;

    public override void _Ready()
    {
        content = GetNode<Fader>("%Content");
        onlyOnScreenContent = GetNode<Fader>("%Content/OnlyOnScreen");
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
        
        var viewportRect = GetViewportRect();
        var currentCamera = GetViewport().GetCamera3D();
        
        var cameraPos = currentCamera.GlobalTransform.Origin;
        var camToObj = cameraPos - Target.GlobalPosition;
        var camForward = currentCamera.GlobalTransform.Forward(); 
        var dot = camToObj.Normalized().Dot(camForward);

        var screenPos = currentCamera.UnprojectPosition(Target.GlobalPosition);
        
        // Case 1: The object is within bounds, show normally
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
        
        // Case 2: Camera is facing away from object, show directional arrow
        if (WhenOffScreen == ShowDirectionalArrow && dot <= 0.98)
        {
            var localPos = currentCamera.ToLocal(Target.GlobalPosition);
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
        
        //Case 3: The indicator is set to Hide or the angle is so extreme that math breaks
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
}
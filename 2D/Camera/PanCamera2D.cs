using GodotLib;
using GodotLib.Util;
using static GodotLib.ProjectConstants.InputActions;

[GlobalClass]
public partial class PanCamera2D : Camera2D
{
    [ExportGroup("Action-based Movement", "keyMovement")]
    [Export(PropertyHint.GroupEnable)] private bool keyMovementEnabled = true;
    [Export] private float keyMovementSpeed = 12;
    [Export(PropertyHint.Range, "0, 10")] private float keyMovementSmoothing = 0.1f;
    
    [ExportGroup("Edge Panning", "edgePanning")]
    [Export(PropertyHint.GroupEnable)] private bool edgePanningEnabled = true;
    [Export] private float edgePanningSensitivity;
    [Export(PropertyHint.Range, "0, 10")] private float edgePanningSmoothing = 0.1f;
    [Export] private float edgePanningMargins = 0.08f;
    
    [ExportGroup("Mouse Dragging", "dragging")]
    [Export(PropertyHint.GroupEnable)] private bool draggingEnabled = true;

    [Export]
    public bool DebugEnabled
    {
        get;
        set
        {
            field = value;
            QueueRedraw();
        }
    }

    private Vector2 currentKeySpeed, currentEdgeSpeed;
    private float screenMargin;
    private Rect2 screenRect;
    private Vector2 closestMouse;
    private bool isDragging, isPanning, canEdgePan = true;
    private Viewport viewport;
    private Window window;

    public override void _Ready()
    {
        viewport = GetViewport();
        window = viewport.GetWindow();
        
        // Input.MouseMode = Input.MouseModeEnum.Confined;
        viewport.SizeChanged += RecalculateBounds;
        RecalculateBounds();
    }

    public override void _Input(InputEvent @event)
    {
        if (!window.HasFocus() || !draggingEnabled) return;
        
        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Middle)
        {
            isDragging = mouseButton.Pressed;
            if (!isDragging) canEdgePan = false; // Disable edge panning when drag ends
        }
    
        else if (@event is InputEventMouseMotion mouseMotion && isDragging)
        {
            Position -= mouseMotion.ScreenRelative;
        }
    }

    public override void _Process(double delta)
    {
        if (!window.HasFocus()) return;
        
        var finalSpeed = Vector2.Zero;
        
        if (keyMovementEnabled && !isDragging)
        {
            var movementDirection = Input.GetVector(Left, Right, Forward, Back).LimitLength();
            currentKeySpeed = currentKeySpeed.DampTowards(movementDirection * keyMovementSpeed, keyMovementSmoothing/1000, delta);
            finalSpeed += currentKeySpeed;
        }
        
        if (edgePanningEnabled && !isDragging)
        {
            var mousePos = viewport.GetMousePosition();
            if (screenRect.HasPoint(mousePos))
            {
                canEdgePan = true; // Re-enable edge panning when mouse returns to safe zone
                closestMouse = mousePos;
                currentEdgeSpeed = currentEdgeSpeed.DampTowards(Vector2.Zero, edgePanningSmoothing/1000, delta);
                finalSpeed += currentEdgeSpeed;
            }
            else if (canEdgePan)
            {
                closestMouse = screenRect.ClosestPointOnBorder(mousePos);
                var mouseDirection = (mousePos - closestMouse).LimitLength(screenMargin);
                currentEdgeSpeed = currentEdgeSpeed.DampTowards(mouseDirection * edgePanningSensitivity, edgePanningSmoothing/1000, delta);
                finalSpeed += currentEdgeSpeed;
            }
        }
        
        Position += finalSpeed;

        if (DebugEnabled)
        {
            QueueRedraw();
        }
    }

    private void RecalculateBounds()
    {
        var viewportRect = GetViewportRect();
        screenMargin = edgePanningMargins * Mathf.Min(viewportRect.Size.X, viewportRect.Size.Y); // Scale margins relative to smallest dimension
        screenRect = viewportRect.Grow(-screenMargin);
        
        if (DebugEnabled)
        {
            QueueRedraw();
        }
    }

    public override void _Draw()
    {
        if (!DebugEnabled) return;

        if (edgePanningEnabled)
        {
            var offset = GetViewportRect().GetCenter();
            var spRect = screenRect;

            spRect.Position -= offset;

            DrawRect(spRect, Colors.Red, false, 2);
            DrawCircle(closestMouse - offset, 4, Colors.Red);
        }
    }
}
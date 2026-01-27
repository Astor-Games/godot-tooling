namespace GodotLib.Debug;

[GlobalClass]
public partial class DockableWindow : PanelContainer
{
    public virtual string WindowName { get; }
    
    private string VisibleKey => $"window_{WindowName}_visible";
    private string SizeKey => $"window_{WindowName}_size";
    private string PositionKey => $"window_{WindowName}_position";
    private string DockedSideKey => $"window_{WindowName}_docked_side";
    private string DockedWidthKey => $"window_{WindowName}_docked_width";
    private string DockedHeightKey => $"window_{WindowName}_docked_height";
    
    private const int DockSnapDistance = 50;
    private const int ResizeMargin = 8;
    private readonly Vector2I MinSize = new(200, 100);
    private const int TitleBarHeight = 31;
    private const float DockedSizeProportion = 0.25f;

    private Label titleLabel;
    private Button closeButton;

    [Export] private DockedSide dockedSide = DockedSide.None;
    private Vector2 undockedSize;
    private Vector2 undockedPosition;
    private float dockedWidth;  // Width when docked left/right
    private float dockedHeight; // Height when docked top/bottom

    private bool isDraggingWindow = false;
    private Vector2 dragOffset;

    private bool isResizing = false;
    private ResizeEdge resizeEdge = ResizeEdge.None;
    private Vector2 resizeStartSize;
    private Vector2 resizeStartPos;
    private Vector2 resizeStartMouse;

    private enum DockedSide
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    [Flags]
    private enum ResizeEdge
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }
    
    public StringName Title
    {
        get => titleLabel.Text;
        set => titleLabel.Text = value;
    }

    public override void _EnterTree()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Ready()
    {
        titleLabel = GetNode<Label>("%Title");
        closeButton = GetNode<Button>("%CloseButton");
        closeButton.Pressed += ToggleVisibility;
        
        undockedSize = (Vector2I)CustomMinimumSize;
        if (undockedSize.X == 0) undockedSize = new Vector2I(400, 300);
    }

    public void ToggleVisibility()
    {
        Visible = !Visible;
        SaveState();
    }
    
    public override void _GuiInput(InputEvent evt)
    {
        if (!Visible) return;

        if (evt is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left)
        {
            var localPos = mouseButton.Position;

            if (mouseButton.Pressed)
            {
                // Check if clicking edge for resizing (priority over dragging)
                resizeEdge = GetResizeEdge(localPos);
                if (resizeEdge != ResizeEdge.None)
                {
                    isResizing = true;
                    resizeStartSize = (Vector2I)Size;
                    resizeStartPos = (Vector2I)Position;
                    resizeStartMouse = GetParentViewport().GetMousePosition();
                    GetViewport().SetInputAsHandled();
                }
                // Check if clicking title bar for dragging
                else if (localPos.Y <= TitleBarHeight)
                {
                    isDraggingWindow = true;
                    dragOffset = localPos;
                    MouseDefaultCursorShape = CursorShape.Drag;
                    GetViewport().SetInputAsHandled();
                }
            }
            else
            {
                if (isDraggingWindow)
                {
                    isDraggingWindow = false;
                    MouseDefaultCursorShape = CursorShape.Arrow;

                    // Save position if not docked
                    if (dockedSide == DockedSide.None)
                    {
                        undockedPosition = (Vector2I)Position;
                        undockedSize = (Vector2I)Size;
                        SaveState();
                    }

                    GetViewport().SetInputAsHandled();
                }
                if (isResizing)
                {
                    isResizing = false;

                    switch (dockedSide)
                    {
                        // Save the appropriate size based on docked state
                        case DockedSide.Left or DockedSide.Right:
                            dockedWidth = (int)Size.X;
                            break;
                        
                        case DockedSide.Top or DockedSide.Bottom:
                            dockedHeight = (int)Size.Y;
                            break;
                        
                        default:
                            
                            undockedSize = (Vector2I)Size;
                            undockedPosition = (Vector2I)Position;
                            break;
                    }

                    SaveState();
                    GetViewport().SetInputAsHandled();
                }

                if (dockedSide == DockedSide.None)
                {
                    ClampWindow(Size, Position);
                }
            }
        }
        else if (evt is InputEventMouseMotion mouseMotion)
        {
            if (isDraggingWindow)
            {
                // Get parent-relative mouse position
                var mousePos = GetParentViewport().GetMousePosition();
                var viewportSize = GetViewportRect().Size;

                // Check if we should undock while dragging
                if (dockedSide != DockedSide.None)
                {
                    if (ShouldUndock(mousePos, viewportSize))
                    {
                        Undock();
                        // Recalculate drag offset so panel stays under cursor
                        dragOffset = new Vector2(undockedSize.X / 2f, TitleBarHeight / 2f);
                        undockedPosition = (Vector2I)(mousePos - dragOffset);
                        Position = undockedPosition;
                    }
                }
                else
                {
                    // Try to dock if near edge and not already docked
                    if (dockedSide != DockedSide.None || !TryDock(mousePos, viewportSize))
                    {
                        // Apply position immediately when not docked
                        var newPos = mousePos - dragOffset;
                        Position = newPos;
                    }
                }

                GetViewport().SetInputAsHandled();
            }
            else if (isResizing)
            {
                var mousePos = GetParentViewport().GetMousePosition();
                HandleResize(mousePos);
                GetViewport().SetInputAsHandled();
            }
            else
            {
                // Update cursor based on edge
                var edge = GetResizeEdge(mouseMotion.Position);
                UpdateCursor(edge);
            }
        } 
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;

        // Update docked window size if viewport changes
        if (dockedSide != DockedSide.None)
        {
            var viewportSize = GetViewportRect().Size;
            UpdateDockedWindow(viewportSize);
        }
    }

    private ResizeEdge GetResizeEdge(Vector2 localPos)
    {
        var edge = ResizeEdge.None;
        var size = Size;

        var nearBottom = localPos.Y >= size.Y - ResizeMargin;
        var nearTop = localPos.Y <= ResizeMargin;
        var nearRight = localPos.X >= size.X - ResizeMargin;
        var nearLeft = localPos.X <= ResizeMargin;


        // When docked, only allow resizing from the opposite edge
        
        switch (dockedSide)
        {
            case DockedSide.Left:
                if (nearRight) edge |= ResizeEdge.Right;
                break;
            
            case DockedSide.Right:
                if (nearLeft) edge |= ResizeEdge.Left;
                break;
            
            case DockedSide.Top:
                if (nearBottom) edge |= ResizeEdge.Bottom;
                break;
            
            case DockedSide.Bottom:
                if (nearTop) edge |= ResizeEdge.Top;
                break;
            
            case DockedSide.None:
                // When not docked, allow all edges
                if (nearLeft) edge |= ResizeEdge.Left;
                else if (nearRight) edge |= ResizeEdge.Right;

                if (nearTop) edge |= ResizeEdge.Top;
                else if (nearBottom) edge |= ResizeEdge.Bottom;
                break;
        }

        return edge;
    }

    private void UpdateCursor(ResizeEdge edge)
    {
        var cursor = edge switch
        {
            ResizeEdge.Left or ResizeEdge.Right => CursorShape.Hsize,
            ResizeEdge.Top or ResizeEdge.Bottom => CursorShape.Vsize,
            ResizeEdge.Left | ResizeEdge.Top or ResizeEdge.Right | ResizeEdge.Bottom => CursorShape.Fdiagsize,
            ResizeEdge.Right | ResizeEdge.Top or ResizeEdge.Left | ResizeEdge.Bottom => CursorShape.Bdiagsize,
            _ => CursorShape.Arrow
        };
        MouseDefaultCursorShape = cursor;
    }

    private void HandleResize(Vector2 mousePos)
    {
        var delta = mousePos - resizeStartMouse;
        var newSize = resizeStartSize;
        var newPos = resizeStartPos;

        if (resizeEdge.HasFlag(ResizeEdge.Right))
        {
            newSize.X = Mathf.Max(MinSize.X, resizeStartSize.X + delta.X);
        }
        if (resizeEdge.HasFlag(ResizeEdge.Left))
        {
            var deltaX = delta.X;
            var maxDelta = resizeStartSize.X - MinSize.X;
            deltaX = Mathf.Clamp(deltaX, -maxDelta, resizeStartSize.X - MinSize.X);
            newPos.X = resizeStartPos.X + deltaX;
            newSize.X = resizeStartSize.X - deltaX;
        }
        if (resizeEdge.HasFlag(ResizeEdge.Bottom))
        {
            newSize.Y = Mathf.Max(MinSize.Y, resizeStartSize.Y + delta.Y);
        }
        if (resizeEdge.HasFlag(ResizeEdge.Top))
        {
            var deltaY = delta.Y;
            var maxDelta = resizeStartSize.Y - MinSize.Y;
            deltaY = Mathf.Clamp(deltaY, -maxDelta, resizeStartSize.Y - MinSize.Y);
            newPos.Y = resizeStartPos.Y + deltaY;
            newSize.Y = resizeStartSize.Y - deltaY;
        }

        Position = newPos;
        Size = newSize;
        
        // Save the appropriate size based on docked state
        switch (dockedSide)
        {
            case DockedSide.Left or DockedSide.Right:
                dockedWidth = newSize.X;
                break;
                        
            case DockedSide.Top or DockedSide.Bottom:
                dockedHeight = newSize.Y;
                break;
                        
            default:
                            
                undockedSize = (Vector2I)Size;
                undockedPosition = (Vector2I)Position;
                break;
        }
    }

    protected virtual void SaveState()
    {
        DebugTools.SaveConfig(VisibleKey, Visible);
        DebugTools.SaveConfig(SizeKey, undockedSize);
        DebugTools.SaveConfig(PositionKey, undockedPosition);
        DebugTools.SaveConfig(DockedWidthKey, dockedWidth);
        DebugTools.SaveConfig(DockedHeightKey, dockedHeight);

        DebugTools.SaveConfig(DockedSideKey, dockedSide);
    }

    private bool TryDock(Vector2 pos, Vector2 viewportSize)
    {
        // Check left edge
        if (pos.X <= DockSnapDistance)
        {
            DockToSide(DockedSide.Left, viewportSize);
            return true;
        }
        // Check right edge

        if (pos.X >= viewportSize.X - DockSnapDistance)
        {
            DockToSide(DockedSide.Right, viewportSize);
            return true;
        }
        
        // Check top edge
        if (pos.Y <= DockSnapDistance)
        {
            DockToSide(DockedSide.Top, viewportSize);
            return true;
        }
        
        // Check bottom edge

        if (pos.Y >= viewportSize.Y - DockSnapDistance)
        {
            DockToSide(DockedSide.Bottom, viewportSize);
            return true;
        }
        
        return false;
    }

    private bool ShouldUndock(Vector2 pos, Vector2 viewportSize)
    {
        const int undockThreshold = DockSnapDistance; // Drag this far from edge to undock

        return dockedSide switch
        {
            DockedSide.Left => pos.X > undockThreshold,
            DockedSide.Right => pos.X < viewportSize.X - undockThreshold,
            DockedSide.Top => pos.Y > undockThreshold,
            DockedSide.Bottom => pos.Y < viewportSize.Y - undockThreshold,
            _ => false
        };
    }

    private void DockToSide(DockedSide side, Vector2 viewportSize)
    {
        if (dockedSide == DockedSide.None)
        {
            undockedSize = (Vector2I)Size;
            undockedPosition = (Vector2I)Position;
        }

        var previousSide = dockedSide;
        dockedSide = side;

        // Initialize docked sizes if not set
        if (dockedWidth == 0) dockedWidth = (int)(viewportSize.X * DockedSizeProportion);
        if (dockedHeight == 0) dockedHeight = (int)(viewportSize.Y * DockedSizeProportion);

        // When switching between incompatible sides, use default proportional size
        var isVerticalToPreviousVertical = side is DockedSide.Left or DockedSide.Right &&
                                           previousSide is DockedSide.Left or DockedSide.Right;
        var isHorizontalToPreviousHorizontal = side is DockedSide.Top or DockedSide.Bottom &&
                                               previousSide is DockedSide.Top or DockedSide.Bottom;

        if (!isVerticalToPreviousVertical && side is DockedSide.Left or DockedSide.Right)
        {
            dockedWidth = (int)(viewportSize.X * DockedSizeProportion);
        }
        if (!isHorizontalToPreviousHorizontal && side is DockedSide.Top or DockedSide.Bottom)
        {
            dockedHeight = (int)(viewportSize.Y * DockedSizeProportion);
        }

        DebugTools.SaveConfig(DockedSideKey, (int)side);
        UpdateDockedWindow(viewportSize);
    }

    private void Undock()
    {
        dockedSide = DockedSide.None;
        Size = undockedSize;
        Position = undockedPosition;
        DebugTools.SaveConfig(DockedSideKey, (int)DockedSide.None);
    }

    private void UpdateDockedWindow(Vector2 viewportSize)
    {
        switch (dockedSide)
        {
            case DockedSide.Left:
                Position = new Vector2(0, 0);
                Size = new Vector2(dockedWidth, viewportSize.Y);
                break;
            case DockedSide.Right:
                Position = new Vector2(viewportSize.X - dockedWidth, 0);
                Size = new Vector2(dockedWidth, viewportSize.Y);
                break;
            case DockedSide.Top:
                Position = new Vector2(0, 0);
                Size = new Vector2(viewportSize.X, dockedHeight);
                break;
            case DockedSide.Bottom:
                Position = new Vector2(0, viewportSize.Y - dockedHeight);
                Size = new Vector2(viewportSize.X, dockedHeight);
                break;
        }
    }

    public virtual void RestoreState()
    {
        Visible = DebugTools.LoadConfig(VisibleKey, false);
        var lastSize = DebugTools.LoadConfig(SizeKey, undockedSize);
        var lastPos = DebugTools.LoadConfig(PositionKey, new Vector2I(100, 100));

        dockedSide = (DockedSide)DebugTools.LoadConfig(DockedSideKey, (int)DockedSide.None);
        undockedSize = lastSize;
        undockedPosition = lastPos;

        // Load docked sizes
        var viewportSize = GetViewportRect().Size;
        dockedWidth = DebugTools.LoadConfig(DockedWidthKey, (int)(viewportSize.X * DockedSizeProportion));
        dockedHeight = DebugTools.LoadConfig(DockedHeightKey, (int)(viewportSize.Y * DockedSizeProportion));

        if (dockedSide != DockedSide.None)
        {
            UpdateDockedWindow(viewportSize);
        }
        else
        {
            ClampWindow(lastSize, lastPos);
        }
    }

    private void ClampWindow(Vector2 lastSize, Vector2 lastPosition)
    {
        var viewportSize = GetViewportRect().Size;

        lastSize = new Vector2(
            Mathf.Clamp(lastSize.X, MinSize.X, (int)viewportSize.X - 30),
            Mathf.Clamp(lastSize.Y, MinSize.Y, (int)viewportSize.Y - 30)
        );

        lastPosition = new Vector2(
            Mathf.Clamp(lastPosition.X, 0, Mathf.Max(0, (int)viewportSize.X - lastSize.X)),
            Mathf.Clamp(lastPosition.Y, 0, Mathf.Max(0, (int)viewportSize.Y - lastSize.Y))
        );

        Size = lastSize;
        Position = lastPosition;
    }

    private Viewport GetParentViewport()
    {
        return GetParent().GetViewport();
    }
}
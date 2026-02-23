using GodotLib.Debug;
using Turtles.addons.godot_lib.UI;
using ZLinq;

namespace GodotLib.UI;

[GlobalClass]
public partial class DockablePanel : PanelContainer
{
    private string VisibleKey => $"window_{Name}_visible";
    private string SizeKey => $"window_{Name}_size";
    private string PositionKey => $"window_{Name}_position";
    private string DockedSideKey => $"window_{Name}_docked_side";
    private string DockedWidthKey => $"window_{Name}_docked_width";
    private string DockedHeightKey => $"window_{Name}_docked_height";
    
    [Export] public bool VisibleByDefault;
    
    private const int DockSnapDistance = 50;
    private const int ResizeMargin = 8;
    private const int TitleBarHeight = 31;
    private const float DockedSizeProportion = 0.25f;

    private Label titleLabel;
    private Button closeButton;
    private MenuButton menuButton;

    private DockPosition dockPosition = DockPosition.Undocked;
    private Vector2 undockedSize;
    private Vector2 undockedPosition;
    public float dockedWidth;  // Width when docked left/right
    public float dockedHeight; // Height when docked top/bottom

    private bool isDraggingWindow = false;
    private Vector2 dragOffset;

    private bool isResizing = false;
    private ResizeEdge resizeEdge = ResizeEdge.None;
    private Vector2 resizeStartSize;
    private Vector2 resizeStartPos;
    private Vector2 resizeStartMouse;
    private PopupMenu optionsMenu;

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

    public override void _ExitTree()
    {
    }

    public override void _Ready()
    {
        titleLabel = GetNode<Label>("%Title");
        closeButton = GetNode<Button>("%CloseButton");
        closeButton.Pressed += ToggleVisibility;
        
        menuButton = GetNode<MenuButton>("%OptionsMenuButton");
        menuButton.Visible = false;
        
        undockedSize = (Vector2I)CustomMinimumSize;
        if (undockedSize.X == 0) undockedSize = new Vector2I(400, 300);
        
        // Connect focus signals for all child controls
        ConnectChildFocusSignals(this);

        CallDeferred(MethodName.RestoreState);
    }

    private void ConnectChildFocusSignals(Node node)
    {
        foreach (var child in node.DescendantsAndSelf())
        {
            if (child is Control control)
            {
                control.FocusEntered += OnFocusEntered;
            }
        }
    }

    public virtual void OnFocusEntered()
    {
        MoveToFront();
        DockSurface.Instance?.InformFocused(this);
    }

    public virtual void OnFocusExited()
    {
        
    }

    public virtual void ToggleVisibility()
    {
        Visible = !Visible;
        SaveState();
    }

    protected void SetTitle(string title)
    {
        titleLabel.Text = title;
    }

    protected PopupMenu UseOptionsMenu()
    {
        menuButton.Visible = true;
        optionsMenu = menuButton.GetPopup();
        optionsMenu.IdPressed += id => OnOptionsMenuItemClicked((int)id);
        
        return optionsMenu;
    }
    
    public override void _GuiInput(InputEvent evt)
    {
        if (!Visible) return;

        switch (evt)
        {
            case InputEventMouseButton mouseButton when mouseButton.ButtonIndex == MouseButton.Left:
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
                        if (dockPosition == DockPosition.Undocked)
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

                        switch (dockPosition)
                        {
                            // Save the appropriate size based on docked state
                            case DockPosition.Left or DockPosition.Right:
                                dockedWidth = (int)Size.X;
                                break;

                            case DockPosition.Top or DockPosition.Bottom:
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

                    if (dockPosition == DockPosition.Undocked)
                    {
                        ClampWindow(Size, Position);
                    }
                    else
                    {
                        // Update dock manager layout when resizing finished
                        DockSurface.Instance?.UpdateLayout();
                    }
                }

                break;
            }
            case InputEventMouseMotion when isDraggingWindow:
            {
                // Get parent-relative mouse position
                var mousePos = GetParentViewport().GetMousePosition();
                var viewportSize = GetViewportRect().Size;

                // Check if we should undock while dragging
                if (dockPosition != DockPosition.Undocked)
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
                    if (dockPosition != DockPosition.Undocked || !TryDock(mousePos, viewportSize))
                    {
                        // Apply position immediately when not docked
                        var newPos = mousePos - dragOffset;
                        Position = newPos;
                    }
                }

                GetViewport().SetInputAsHandled();
                break;
            }
            case InputEventMouseMotion when isResizing:
            {
                var mousePos = GetParentViewport().GetMousePosition();
                HandleResize(mousePos);
                GetViewport().SetInputAsHandled();
                break;
            }
            case InputEventMouseMotion mouseMotion:
            {
                // Update cursor based on edge
                var edge = GetResizeEdge(mouseMotion.Position);
                UpdateCursor(edge);
                break;
            }
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

        // When docked, check for neighboring panels
        if (dockPosition != DockPosition.Undocked)
        {
            var panelInfo = DockSurface.Instance?.GetPanelInfo(this);
            if (panelInfo != null)
            {
                var panelCount = DockSurface.Instance?.GetPanelCount(dockPosition) ?? 0;
                var isFirst = panelInfo.Index == 0;
                var isLast = panelInfo.Index == panelCount - 1;

                switch (dockPosition)
                {
                    case DockPosition.Left:
                        // Can resize from right edge (opposite of dock)
                        if (nearRight) edge |= ResizeEdge.Right;
                        // Can resize from bottom edge if not last panel (neighbor below)
                        if (nearBottom && !isLast) edge |= ResizeEdge.Bottom;
                        break;

                    case DockPosition.Right:
                        // Can resize from left edge (opposite of dock)
                        if (nearLeft) edge |= ResizeEdge.Left;
                        // Can resize from bottom edge if not last panel (neighbor below)
                        if (nearBottom && !isLast) edge |= ResizeEdge.Bottom;
                        break;

                    case DockPosition.Top:
                        // Can resize from bottom edge (opposite of dock)
                        if (nearBottom) edge |= ResizeEdge.Bottom;
                        // Can resize from right edge if not last panel (neighbor to right)
                        if (nearRight && !isLast) edge |= ResizeEdge.Right;
                        break;

                    case DockPosition.Bottom:
                        // Can resize from top edge (opposite of dock)
                        if (nearTop) edge |= ResizeEdge.Top;
                        // Can resize from right edge if not last panel (neighbor to right)
                        if (nearRight && !isLast) edge |= ResizeEdge.Right;
                        break;
                }
            }
        }
        else
        {
            // When not docked, allow all edges
            if (nearLeft) edge |= ResizeEdge.Left;
            else if (nearRight) edge |= ResizeEdge.Right;

            if (nearTop) edge |= ResizeEdge.Top;
            else if (nearBottom) edge |= ResizeEdge.Bottom;
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
        var minSize = this.GetCombinedMinimumSize();
        
        var delta = mousePos - resizeStartMouse;

        if (dockPosition != DockPosition.Undocked)
        {
            // When docked, only update the stored sizes and let DockSurface handle layout
            var isVertical = dockPosition is DockPosition.Left or DockPosition.Right;

            if (isVertical)
            {
                // For vertical docks (left/right)
                if (resizeEdge.HasFlag(ResizeEdge.Right) || resizeEdge.HasFlag(ResizeEdge.Left))
                {
                    // Perpendicular axis (width) - update and notify
                    var newWidth = resizeEdge.HasFlag(ResizeEdge.Right) 
                        ? Mathf.Max(minSize.X, resizeStartSize.X + delta.X)
                        : Mathf.Max(minSize.X, resizeStartSize.X - delta.X);
                    dockedWidth = newWidth;
                    DockSurface.Instance?.OnPanelResized(this);
                }
                else if (resizeEdge.HasFlag(ResizeEdge.Bottom) || resizeEdge.HasFlag(ResizeEdge.Top))
                {
                    // Parallel axis (height) - update and notify
                    var newHeight = resizeEdge.HasFlag(ResizeEdge.Bottom)
                        ? Mathf.Max(minSize.Y, resizeStartSize.Y + delta.Y)
                        : Mathf.Max(minSize.Y, resizeStartSize.Y - delta.Y);
                    dockedHeight = newHeight;
                    DockSurface.Instance?.OnPanelResized(this);
                }
            }
            else
            {
                // For horizontal docks (top/bottom)
                if (resizeEdge.HasFlag(ResizeEdge.Top) || resizeEdge.HasFlag(ResizeEdge.Bottom))
                {
                    // Perpendicular axis (height) - update and notify
                    var newHeight = resizeEdge.HasFlag(ResizeEdge.Bottom)
                        ? Mathf.Max(minSize.Y, resizeStartSize.Y + delta.Y)
                        : Mathf.Max(minSize.Y, resizeStartSize.Y - delta.Y);
                    dockedHeight = newHeight;
                    DockSurface.Instance?.OnPanelResized(this);
                }
                else if (resizeEdge.HasFlag(ResizeEdge.Right) || resizeEdge.HasFlag(ResizeEdge.Left))
                {
                    // Parallel axis (width) - update and notify
                    var newWidth = resizeEdge.HasFlag(ResizeEdge.Right)
                        ? Mathf.Max(minSize.X, resizeStartSize.X + delta.X)
                        : Mathf.Max(minSize.X, resizeStartSize.X - delta.X);
                    dockedWidth = newWidth;
                    DockSurface.Instance?.OnPanelResized(this);
                }
            }
        }
        else
        {
            // When not docked, handle resize normally
            var newSize = resizeStartSize;
            var newPos = resizeStartPos;

            if (resizeEdge.HasFlag(ResizeEdge.Right))
            {
                newSize.X = Mathf.Max(minSize.X, resizeStartSize.X + delta.X);
            }
            if (resizeEdge.HasFlag(ResizeEdge.Left))
            {
                var deltaX = delta.X;
                var maxDelta = resizeStartSize.X - minSize.X;
                deltaX = Mathf.Clamp(deltaX, -maxDelta, resizeStartSize.X - minSize.X);
                newPos.X = resizeStartPos.X + deltaX;
                newSize.X = resizeStartSize.X - deltaX;
            }
            if (resizeEdge.HasFlag(ResizeEdge.Bottom))
            {
                newSize.Y = Mathf.Max(minSize.Y, resizeStartSize.Y + delta.Y);
            }
            if (resizeEdge.HasFlag(ResizeEdge.Top))
            {
                var deltaY = delta.Y;
                var maxDelta = resizeStartSize.Y - minSize.Y;
                deltaY = Mathf.Clamp(deltaY, -maxDelta, resizeStartSize.Y - minSize.Y);
                newPos.Y = resizeStartPos.Y + deltaY;
                newSize.Y = resizeStartSize.Y - deltaY;
            }

            Position = newPos;
            Size = newSize;
            undockedSize = (Vector2I)Size;
            undockedPosition = (Vector2I)Position;
        }
    }

    public virtual void SaveState()
    {
        DebugManager.SaveConfig(VisibleKey, Visible);
        DebugManager.SaveConfig(SizeKey, undockedSize);
        DebugManager.SaveConfig(PositionKey, undockedPosition);
        DebugManager.SaveConfig(DockedWidthKey, dockedWidth);
        DebugManager.SaveConfig(DockedHeightKey, dockedHeight);

        DebugManager.SaveConfig(DockedSideKey, dockPosition);
    }

    private bool TryDock(Vector2 pos, Vector2 viewportSize)
    {
        // Check left edge
        if (pos.X <= DockSnapDistance)
        {
            DockToSide(DockPosition.Left, viewportSize, pos);
            return true;
        }
        // Check right edge
        if (pos.X >= viewportSize.X - DockSnapDistance)
        {
            DockToSide(DockPosition.Right, viewportSize, pos);
            return true;
        }

        // Check top edge
        if (pos.Y <= DockSnapDistance)
        {
            DockToSide(DockPosition.Top, viewportSize, pos);
            return true;
        }

        // Check bottom edge
        if (pos.Y >= viewportSize.Y - DockSnapDistance)
        {
            DockToSide(DockPosition.Bottom, viewportSize, pos);
            return true;
        }

        return false;
    }

    private bool ShouldUndock(Vector2 pos, Vector2 viewportSize)
    {
        const int undockThreshold = DockSnapDistance; // Drag this far from edge to undock

        return dockPosition switch
        {
            DockPosition.Left => pos.X > undockThreshold,
            DockPosition.Right => pos.X < viewportSize.X - undockThreshold,
            DockPosition.Top => pos.Y > undockThreshold,
            DockPosition.Bottom => pos.Y < viewportSize.Y - undockThreshold,
            _ => false
        };
    }

    private void DockToSide(DockPosition position, Vector2 viewportSize, Vector2? cursorPos = null)
    {
        if (dockPosition == DockPosition.Undocked)
        {
            undockedSize = (Vector2I)Size;
            undockedPosition = (Vector2I)Position;
        }

        var previousSide = dockPosition;
        dockPosition = position;

        // Initialize docked sizes if not set
        if (dockedWidth == 0) dockedWidth = (int)(viewportSize.X * DockedSizeProportion);
        if (dockedHeight == 0) dockedHeight = (int)(viewportSize.Y * DockedSizeProportion);

        // When switching between incompatible sides, use default proportional size
        var isVerticalToPreviousVertical = position is DockPosition.Left or DockPosition.Right &&
                                           previousSide is DockPosition.Left or DockPosition.Right;
        var isHorizontalToPreviousHorizontal = position is DockPosition.Top or DockPosition.Bottom &&
                                               previousSide is DockPosition.Top or DockPosition.Bottom;

        if (!isVerticalToPreviousVertical && position is DockPosition.Left or DockPosition.Right)
        {
            dockedWidth = (int)(viewportSize.X * DockedSizeProportion);
        }
        if (!isHorizontalToPreviousHorizontal && position is DockPosition.Top or DockPosition.Bottom)
        {
            dockedHeight = (int)(viewportSize.Y * DockedSizeProportion);
        }

        DebugManager.SaveConfig(DockedSideKey, (int)position);

        // Register with DockSurface, passing cursor position for smart insertion
        DockSurface.Instance?.UpdatePanelDocking(this, position, cursorPos);
    }

    private void Undock()
    {
        DockSurface.Instance?.UnregisterPanel(this);

        dockPosition = DockPosition.Undocked;
        Size = undockedSize;
        Position = undockedPosition;
        DebugManager.SaveConfig(DockedSideKey, dockPosition);
    }

    public virtual void RestoreState()
    {
        Visible = DebugManager.LoadConfig(VisibleKey, VisibleByDefault);
        var lastSize = DebugManager.LoadConfig(SizeKey, undockedSize);
        var lastPos = DebugManager.LoadConfig(PositionKey, new Vector2I(100, 100));

        dockPosition = (DockPosition)DebugManager.LoadConfig(DockedSideKey, (int)DockPosition.Undocked);
        undockedSize = lastSize;
        undockedPosition = lastPos;

        // Load docked sizes
        var viewportSize = GetViewportRect().Size;
        dockedWidth = DebugManager.LoadConfig(DockedWidthKey, (int)(viewportSize.X * DockedSizeProportion));
        dockedHeight = DebugManager.LoadConfig(DockedHeightKey, (int)(viewportSize.Y * DockedSizeProportion));

        if (dockPosition != DockPosition.Undocked)
        {
            DockSurface.Instance?.UpdatePanelDocking(this, dockPosition);
        }
        else
        {
            ClampWindow(lastSize, lastPos);
        }
    }

    private void ClampWindow(Vector2 lastSize, Vector2 lastPosition)
    {
        var viewportSize = GetViewportRect().Size;
        var minSize = GetCombinedMinimumSize();

        lastSize = new Vector2(
            Mathf.Clamp(lastSize.X, minSize.X, (int)viewportSize.X - 30),
            Mathf.Clamp(lastSize.Y, minSize.Y, (int)viewportSize.Y - 30)
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
    
    private void OnOptionsMenuItemClicked(int menuId)
    {
        var index = optionsMenu.GetItemIndex(menuId);
        var isChecked = optionsMenu.HandleCheckState(index);
        OnOptionsMenuItemClicked(menuId, isChecked);
    }

    protected virtual void OnOptionsMenuItemClicked(int menuId, bool isChecked) { }
}
using System.Collections.Generic;
using System.Linq;

namespace GodotLib.UI;

[GlobalClass]
public partial class DockSurface : Control
{
    private static DockSurface instance;
    public static DockSurface Instance => instance;

    private readonly Dictionary<DockPosition, List<DockData>> dockedPanels = new();
    private readonly List<DockablePanel> nonDockedPanels = new();
    private readonly Dictionary<string, DockablePanel> allPanels = new(); // Key: panel name/id
    private Logger log = new(nameof(DockSurface));
    
    public class DockData
    {
        public DockablePanel Panel { get; init; }
        public DockPosition Position { get; init; }
        public int Index { get; set; } // Stack order on the same side
    }

    public override void _EnterTree()
    {
        instance = this;

        dockedPanels[DockPosition.Left] = new List<DockData>();
        dockedPanels[DockPosition.Right] = new List<DockData>();
        dockedPanels[DockPosition.Top] = new List<DockData>();
        dockedPanels[DockPosition.Bottom] = new List<DockData>();
    }

    public override void _ExitTree()
    {
        foreach (var panel in allPanels.Values)
        {
            panel.SaveState();
        }

        if (instance == this)
            instance = null;
    }
    
    public DockablePanel CreatePanel(string scenePath) => CreatePanel<DockablePanel>(scenePath);

    /// <summary>
    /// Creates a panel from a scene file and adds it to the DockSurface.
    /// </summary>
    /// <param name="scenePath">Path to the .tscn file</param>
    /// <returns>The created panel instance</returns>
    public T CreatePanel<T>(string scenePath) where T : DockablePanel
    {
        var scene = GD.Load<PackedScene>(scenePath);
        if (scene == null)
        {
            log.Error($"Failed to load scene: {scenePath}");
            return null;
        }

        var panel = scene.Instantiate<T>();
        if (panel == null)
        {
            log.Error($"Scene does not contain a DockablePanel: {scenePath}");
            return null;
        }

        // Use provided ID or generate from scene path
        var id = panel.PanelId;
        
        if (allPanels.TryGetValue(id, out var existing))
        {
            log.Warning($"Panel with ID '{id}' already exists. Returning existing panel.");
            return existing as T;
        }

        // Add as child
        AddChild(panel);
        
        // Track the panel
        allPanels[id] = panel;
        nonDockedPanels.Add(panel);

        return panel;
    }

    /// <summary>
    /// Removes a panel from the DockSurface and frees it.
    /// </summary>
    public void RemovePanel(string panelId)
    {
        if (!allPanels.TryGetValue(panelId, out var panel))
        {
            log.Warning($"Panel with ID '{panelId}' not found.");
            return;
        }

        // Unregister from docked panels
        UnregisterPanel(panel);
        
        // Remove from non-docked list
        nonDockedPanels.Remove(panel);
        
        // Remove from all panels
        allPanels.Remove(panelId);
        
        // Remove from scene tree and free
        RemoveChild(panel);
        panel.QueueFree();
    }

    /// <summary>
    /// Gets a panel by its ID.
    /// </summary>
    public DockablePanel GetPanel(string panelId)
    {
        return allPanels.GetValueOrDefault(panelId);
    }

    private Vector2 lastViewportSize = Vector2.Zero;

    public override void _Process(double delta)
    {
        // Check if viewport size changed
        if (GetViewport() == null) return;
        
        var currentSize = GetViewport().GetVisibleRect().Size;
        if (currentSize != lastViewportSize)
        {
            lastViewportSize = currentSize;
            UpdateLayout();
        }
    }

    public void UpdatePanelDocking(DockablePanel panel, DockPosition position, Vector2? cursorPos = null)
    {
        if (position == DockPosition.None)
        {
            // Move to non-docked if it was docked
            UnregisterPanel(panel);
            if (!nonDockedPanels.Contains(panel))
            {
                nonDockedPanels.Add(panel);
            }
            return;
        }

        // Remove from old side if exists
        UnregisterPanel(panel);
        
        // Remove from non-docked list
        nonDockedPanels.Remove(panel);

        // Determine insertion index based on cursor position
        var insertIndex = dockedPanels[position].Count;

        if (cursorPos.HasValue && dockedPanels[position].Count > 0)
        {
            insertIndex = CalculateInsertionIndex(position, cursorPos.Value);
        }

        var dockable = new DockData
        {
            Panel = panel,
            Position = position,
            Index = insertIndex
        };

        dockedPanels[position].Insert(insertIndex, dockable);

        // Reindex all panels on this side
        for (var i = 0; i < dockedPanels[position].Count; i++)
        {
            dockedPanels[position][i].Index = i;
        }

        UpdateLayout();
    }

    private int CalculateInsertionIndex(DockPosition position, Vector2 cursorPos)
    {
        var panels = dockedPanels[position];
        var isVertical = position is DockPosition.Left or DockPosition.Right;

        // Find which panel the cursor is over or nearest to
        for (var i = 0; i < panels.Count; i++)
        {
            var panel = panels[i];
            var rect = panel.Panel.GetGlobalRect();

            if (isVertical)
            {
                // For left/right, check Y position
                if (cursorPos.Y < rect.Position.Y + rect.Size.Y / 2)
                {
                    return i; // Insert before this panel
                }
            }
            else
            {
                // For top/bottom, check X position
                if (cursorPos.X < rect.Position.X + rect.Size.X / 2)
                {
                    return i; // Insert before this panel
                }
            }
        }

        return panels.Count; // Insert at end
    }

    public void UnregisterPanel(Control panel)
    {
        foreach (var side in dockedPanels.Keys)
        {
            var removed = dockedPanels[side].RemoveAll(p => p.Panel == panel) > 0;
            if (removed)
            {
                // Reindex remaining panels
                for (var i = 0; i < dockedPanels[side].Count; i++)
                {
                    dockedPanels[side][i].Index = i;
                }
                UpdateLayout();
                break;
            }
        }
    }

    public void OnPanelResized(Control panel)
    {
        // A panel was manually resized, adjust neighboring panels accordingly
        var panelInfo = GetPanelInfo(panel);
        if (panelInfo == null) return;

        var panels = dockedPanels[panelInfo.Position];
        var panelIndex = panels.IndexOf(panelInfo);
        if (panelIndex < 0) return;

        var dockableWindow = panel as DockablePanel;
        if (dockableWindow == null) return;

        var isVertical = panelInfo.Position is DockPosition.Left or DockPosition.Right;

        // When resizing in perpendicular axis, all panels on the same side share the new size
        if (isVertical)
        {
            // For vertical sides (left/right), share the width
            foreach (var p in panels)
            {
                var otherWindow = p.Panel as DockablePanel;
                if (otherWindow != null)
                {
                    otherWindow.dockedWidth = dockableWindow.dockedWidth;
                }
            }
        }
        else
        {
            // For horizontal sides (top/bottom), share the height
            foreach (var p in panels)
            {
                var otherWindow = p.Panel as DockablePanel;
                if (otherWindow != null)
                {
                    otherWindow.dockedHeight = dockableWindow.dockedHeight;
                }
            }
        }

        // For parallel axis resizing with multiple panels, redistribute space
        if (panels.Count > 1)
        {
            if (isVertical)
            {
                // Get total available height
                var viewportSize = GetViewport().GetVisibleRect().Size;
                var totalHeight = viewportSize.Y;
                var remainingHeight = totalHeight - dockableWindow.dockedHeight;

                // Distribute remaining space among other panels
                var otherPanelCount = panels.Count - 1;
                if (otherPanelCount > 0)
                {
                    var heightPerOther = remainingHeight / otherPanelCount;

                    foreach (var p in panels)
                    {
                        if (p == panelInfo) continue;
                        var otherWindow = p.Panel as DockablePanel;
                        if (otherWindow != null)
                        {
                            otherWindow.dockedHeight = heightPerOther;
                        }
                    }
                }
            }
            else
            {
                // Get total available width
                var viewportSize = GetViewport().GetVisibleRect().Size;
                var leftWidth = GetSideWidth(DockPosition.Left);
                var rightWidth = GetSideWidth(DockPosition.Right);
                var totalWidth = viewportSize.X - leftWidth - rightWidth;
                var remainingWidth = totalWidth - dockableWindow.dockedWidth;

                // Distribute remaining space among other panels
                var otherPanelCount = panels.Count - 1;
                if (otherPanelCount > 0)
                {
                    var widthPerOther = remainingWidth / otherPanelCount;

                    foreach (var p in panels)
                    {
                        if (p == panelInfo) continue;
                        var otherWindow = p.Panel as DockablePanel;
                        if (otherWindow != null)
                        {
                            otherWindow.dockedWidth = widthPerOther;
                        }
                    }
                }
            }
        }

        UpdateLayout();
    }

    public void UpdateLayout()
    {
        if (GetViewport() == null) return;

        var viewportSize = GetViewport().GetVisibleRect().Size;

        // Calculate layout with corner precedence (left/right take full height)
        LayoutSide(DockPosition.Left, new Vector2(0, 0), new Vector2(0, viewportSize.Y));
        LayoutSide(DockPosition.Right, new Vector2(viewportSize.X, 0), new Vector2(viewportSize.X, viewportSize.Y));

        // Get the width consumed by left/right panels
        var leftWidth = GetSideWidth(DockPosition.Left);
        var rightWidth = GetSideWidth(DockPosition.Right);

        // Top/bottom get the remaining horizontal space
        LayoutSide(DockPosition.Top, new Vector2(leftWidth, 0), new Vector2(viewportSize.X - rightWidth, 0));
        LayoutSide(DockPosition.Bottom, new Vector2(leftWidth, viewportSize.Y), new Vector2(viewportSize.X - rightWidth, viewportSize.Y));
    }

    private void LayoutSide(DockPosition position, Vector2 startPos, Vector2 endPos)
    {
        var panels = dockedPanels[position];
        if (panels.Count == 0) return;

        var isVertical = position is DockPosition.Left or DockPosition.Right;
        var totalSpace = isVertical ? (endPos.Y - startPos.Y) : (endPos.X - startPos.X);

        // Get shared perpendicular axis size (first panel's size or default)
        float sharedPerpendicularSize;
        if (panels.Count > 0)
        {
            var firstWindow = panels[0].Panel as DockablePanel;
            if (isVertical)
            {
                // For vertical sides, share width
                sharedPerpendicularSize = firstWindow?.dockedWidth ?? 0;
                if (sharedPerpendicularSize == 0)
                {
                    sharedPerpendicularSize = GetViewport().GetVisibleRect().Size.X * 0.25f;
                }
            }
            else
            {
                // For horizontal sides, share height
                sharedPerpendicularSize = firstWindow?.dockedHeight ?? 0;
                if (sharedPerpendicularSize == 0)
                {
                    sharedPerpendicularSize = GetViewport().GetVisibleRect().Size.Y * 0.25f;
                }
            }
        }
        else
        {
            return;
        }

        if (panels.Count == 1)
        {
            // Single panel - take full space
            var panel = panels[0];
            var dockableWindow = panel.Panel as DockablePanel;

            if (isVertical)
            {
                // Left/Right - full height, use shared width
                var x = position == DockPosition.Left ? startPos.X : endPos.X - sharedPerpendicularSize;
                panel.Panel.Position = new Vector2(x, startPos.Y);
                panel.Panel.Size = new Vector2(sharedPerpendicularSize, totalSpace);
                
                if (dockableWindow != null)
                {
                    dockableWindow.dockedWidth = sharedPerpendicularSize;
                }
            }
            else
            {
                // Top/Bottom - full width, use shared height
                var y = position == DockPosition.Top ? startPos.Y : endPos.Y - sharedPerpendicularSize;
                panel.Panel.Position = new Vector2(startPos.X, y);
                panel.Panel.Size = new Vector2(endPos.X - startPos.X, sharedPerpendicularSize);
                
                if (dockableWindow != null)
                {
                    dockableWindow.dockedHeight = sharedPerpendicularSize;
                }
            }
        }
        else
        {
            // Multiple panels - check if we need equal division or use stored sizes
            float totalRequestedSize = 0;
            foreach (var panel in panels)
            {
                var dockableWindow = panel.Panel as DockablePanel;
                if (isVertical)
                {
                    totalRequestedSize += dockableWindow?.dockedHeight ?? 0;
                }
                else
                {
                    totalRequestedSize += dockableWindow?.dockedWidth ?? 0;
                }
            }

            // Use equal division if total is 0 or very close to 0
            var useEqualDivision = totalRequestedSize < 1.0f;
            var spacePerPanel = totalSpace / panels.Count;
            var scale = useEqualDivision ? 1.0f : totalSpace / totalRequestedSize;

            float currentOffset = 0;

            foreach (var panel in panels.OrderBy(p => p.Index))
            {
                var dockableWindow = panel.Panel as DockablePanel;

                if (isVertical)
                {
                    // Left/Right: stack vertically, all share the same width
                    var height = useEqualDivision ? spacePerPanel : (dockableWindow?.dockedHeight ?? spacePerPanel) * scale;
                    var x = position == DockPosition.Left ? startPos.X : endPos.X - sharedPerpendicularSize;

                    panel.Panel.Position = new Vector2(x, startPos.Y + currentOffset);
                    panel.Panel.Size = new Vector2(sharedPerpendicularSize, height);

                    if (dockableWindow != null)
                    {
                        dockableWindow.dockedWidth = sharedPerpendicularSize;
                        dockableWindow.dockedHeight = height;
                    }

                    currentOffset += height;
                }
                else
                {
                    // Top/Bottom: stack horizontally, all share the same height
                    var width = useEqualDivision ? spacePerPanel : (dockableWindow?.dockedWidth ?? spacePerPanel) * scale;
                    var y = position == DockPosition.Top ? startPos.Y : endPos.Y - sharedPerpendicularSize;

                    panel.Panel.Position = new Vector2(startPos.X + currentOffset, y);
                    panel.Panel.Size = new Vector2(width, sharedPerpendicularSize);

                    if (dockableWindow != null)
                    {
                        dockableWindow.dockedWidth = width;
                        dockableWindow.dockedHeight = sharedPerpendicularSize;
                    }

                    currentOffset += width;
                }
            }
        }
    }

    private float GetSideWidth(DockPosition position)
    {
        var panels = dockedPanels[position];
        if (panels.Count == 0) return 0;

        // Return the maximum width of panels on this side
        return panels.Max(p => p.Panel.Size.X);
    }

    public int GetPanelCount(DockPosition position)
    {
        return dockedPanels[position].Count;
    }

    public DockData GetPanelInfo(Control panel)
    {
        foreach (var side in dockedPanels.Keys)
        {
            var found = dockedPanels[side].FirstOrDefault(p => p.Panel == panel);
            if (found != null) return found;
        }
        return null;
    }
}

public enum DockPosition
{
    None,
    Left,
    Right,
    Top,
    Bottom
}

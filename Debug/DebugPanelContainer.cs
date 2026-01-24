using System.Collections.Generic;
using Turtles.Core;

namespace GodotLib.Debug;

[GlobalClass]
public partial class DebugPanelContainer : Window
{
    private const string VisibleKey = "debug_panel_visible";
    private const string SizeKey = "debug_panel_size";
    private const string PositionKey = "debug_panel_position";
    private const string SelectedTabKey = "debug_panel_tab";
    private TabContainer tabContainer;
    
    public override void _EnterTree()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _ShortcutInput(InputEvent evt)
    {
        if (Game.IsPaused && Visible && evt.IsActionPressed("exit"))
        {
            Game.SceneRoot.GetWindow().GrabFocus();
            Game.Resume();
        }
    }

    public override void _Ready()
    {
        tabContainer = GetNode<TabContainer>("%TabContainer");
        
        FocusEntered += () =>
        {
            tabContainer.TabsVisible = true;
            ClampWindow(Size, Position);
        };
        
        FocusExited += () =>
        {
            tabContainer.TabsVisible = false;
            DebugTools.SaveConfig(SizeKey, Size);
            DebugTools.SaveConfig(PositionKey, Position);
            
            ClampWindow(Size, Position);
        };
        CloseRequested += Toggle;
    }

    public void Toggle()
    {
        Visible = !Visible;
        DebugTools.SaveConfig(VisibleKey, Visible);
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;
    }

    public void AddTab(IDebugPanel panel, string name)
    {
        var control = (Control)panel;
        
        var margins = new MarginContainer();
        margins.Set("theme_override_constants/margin_left", 7);
        margins.Set("theme_override_constants/margin_right", 7);
        margins.Set("theme_override_constants/margin_top", 7);
        margins.Set("theme_override_constants/margin_bottom", 7);

        margins.Name = name;

        tabContainer.AddChild(margins);
        margins.AddChild(control);
    }

    public void RestoreState()
    {
        tabContainer.CurrentTab = DebugTools.LoadConfig(SelectedTabKey, -1);
        tabContainer.TabChanged += idx => DebugTools.SaveConfig(SelectedTabKey, idx);
        
        Visible = DebugTools.LoadConfig(VisibleKey, false);
        var lastSize = DebugTools.LoadConfig(SizeKey, Size);
        var lastPos = DebugTools.LoadConfig(PositionKey, Position);
        
        ClampWindow(lastSize, lastPos);
    }

    private void ClampWindow(Vector2I lastSize, Vector2I lastPostiion)
    {
        var viewport = GetParent().GetViewport();
        var viewportSize = viewport.GetVisibleRect().Size;
        
        lastSize = new Vector2I(
            Mathf.Min(lastSize.X, (int)viewportSize.X - 30),
            Mathf.Min(lastSize.Y, (int)viewportSize.Y - 30)
        );
        
        lastPostiion = new Vector2I(
            Mathf.Clamp(lastPostiion.X, 0, Mathf.Max(0, (int)viewportSize.X - lastSize.X)),
            Mathf.Clamp(lastPostiion.Y, 31, Mathf.Max(0, (int)viewportSize.Y - lastSize.Y))
        );

        Size = lastSize;
        Position = lastPostiion;
    }
}
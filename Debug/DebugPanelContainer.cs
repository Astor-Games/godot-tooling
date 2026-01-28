using GodotLib.UI;
using Turtles.Core;

namespace GodotLib.Debug;

[GlobalClass]
public partial class DebugPanelContainer : DockablePanel
{
    public override string PanelId => "debug";
    private string SelectedTabKey => $"window_{PanelId}_tab";
    private TabContainer tabContainer;

    public override void _Ready()
    {
        tabContainer = GetNode<TabContainer>("%TabContainer");
        
        // Set up focus tracking for tab visibility
        GetViewport().GuiFocusChanged += OnFocusChanged;
        
        base._Ready();
    }


    private void OnFocusChanged(Control focusedControl)
    {
        var areWeFocused = focusedControl == this || IsAncestorOf(focusedControl);
        tabContainer.TabsVisible = areWeFocused;
    }
    
    public override void _Input(InputEvent evt)
    {
        if (evt is InputEventKey key && key.IsActionPressed("exit") && Game.IsPaused)
        {
            ReleaseFocus();
            tabContainer.TabsVisible = false;
            Game.Resume();
            GetViewport().SetInputAsHandled();
        }
    }
    
    public void AddTab(IDebugPanel panel, string name)
    {
        var control = (Control)panel;
        
        var margins = new MarginContainer();
        margins.Set("theme_override_constants/margin_left", 7);
        margins.Set("theme_override_constants/margin_right", 7);
        margins.Set("theme_override_constants/margin_top", 7);
        margins.Set("theme_override_constants/margin_bottom", 7);

        control.Name = name;
        margins.Name = name;

        tabContainer.AddChild(margins);
        margins.AddChild(control);
    }

    public override void SaveState()
    {
        base.SaveState();
        DebugTools.SaveConfig(SelectedTabKey, tabContainer.CurrentTab);
    }

    public override void RestoreState()
    {
        base.RestoreState();
        
        tabContainer.TabChanged += idx => OnTabChanged((int)idx);
        tabContainer.CurrentTab = DebugTools.LoadConfig(SelectedTabKey, -1);
    }

    private void OnTabChanged(int idx)
    {
        var control = tabContainer.GetTabControl(idx);
        DebugTools.SaveConfig(SelectedTabKey, idx);
        Title = control.Name;
    }
}
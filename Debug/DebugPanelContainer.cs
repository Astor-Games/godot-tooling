using GodotLib.UI;
using Turtles.Core;

namespace GodotLib.Debug;

[GlobalClass]
public partial class DebugPanelContainer : DockablePanel
{
    private string SelectedTabKey => $"window_{Name}_selected_tab";
    private TabContainer tabContainer;

    public override void _Ready()
    {
        tabContainer = GetNode<TabContainer>("%TabContainer");
        base._Ready();
    }

    public override void OnFocusEntered()
    {
        base.OnFocusEntered();
        tabContainer.TabsVisible = true;
    }

    public override void OnFocusExited()
    {
        tabContainer.TabsVisible = false;
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
        
        var idx = DebugTools.LoadConfig(SelectedTabKey, -1);
        if (idx >= 0)
        {
            tabContainer.CurrentTab = idx;
        }
    }

    private void OnTabChanged(int idx)
    {
        var control = tabContainer.GetTabControl(idx);
        DebugTools.SaveConfig(SelectedTabKey, idx);
        Title = control.Name;
    }
}
using GodotLib.UI;

namespace GodotLib.Debug;

public partial class QuickHelp : DockablePanel
{
    private RichTextLabel label;
    
    public override void _Ready()
    { 
        base._Ready();
        label = new RichTextLabel();
        label.FitContent = true;
        label.AutowrapMode = TextServer.AutowrapMode.Off;
        GetNode("%Contents").AddChild(label);
        label.PushTable(2);
        CallDeferred(MethodName.PrintShortcuts);
    }

    private void PrintShortcuts()
    {
        foreach (var (key, actionName) in DebugManager.ListShortcuts())
        {
            label.PushCell();
            label.PushColor(Colors.DarkGray);
            label.AddText(actionName);
            label.Pop();
            label.Pop();
        
            label.PushCell();
            label.PushColor(Colors.White);
            label.AddText(key);
            label.Pop();
            label.Pop();
        }
    }

    private void PrintShortcut(string shortcut, string description)
    {
        label.PushCell();
        label.PushColor(Colors.DarkGray);
        label.AddText(description);
        label.Pop();
        label.Pop();
        
        label.PushCell();
        label.PushColor(Colors.White);
        label.AddText(shortcut);
        label.Pop();
        label.Pop();
    }
}
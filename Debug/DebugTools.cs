using System.Collections.Generic;
using GodotLib.UI;

namespace GodotLib.Debug;

using GodotStringArray = Godot.Collections.Array<string>;

public partial class DebugTools : Node
{
    public const string QuickLoadScenesKey = "godot_lib/quick_load/scene_paths";
    private const string DebugCfgPath = "user://debug.cfg";
    private const KeyModifierMask quickLoadModifiers = KeyModifierMask.MaskAlt;
    
    private static readonly Dictionary<Key, Action> actions = new();
    private static readonly ConfigFile config = new();
    private static List<IDebugPanel> registeredPanels = new();
    private static Console console;
    private static DebugPanelContainer panelContainer;
    private static DockSurface dockSurface;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        config.Load(DebugCfgPath);

        // Create DockSurface
        dockSurface = new DockSurface();
        AddChild(dockSurface);

        var scenePaths = ProjectSettings.GetSetting(QuickLoadScenesKey).As<GodotStringArray>();
        if (scenePaths == null) return;
        
        for (var i = 0; i < Mathf.Min(scenePaths.Count, 9); i++)
        {
            var scene = scenePaths[i];
            AddDebugShortcut(() => QuickLoad(scene), Key.Key1 + i, quickLoadModifiers);
        }

        console = dockSurface.CreatePanel<Console>("uid://s8wks02elbo6");
        console.Visible = false;
        AddDebugShortcut(console.ToggleVisibility, Key.Quoteleft);
        
        panelContainer = dockSurface.CreatePanel<DebugPanelContainer>("uid://bay1hwklweai3");
        panelContainer.Visible = false;
        AddDebugShortcut(panelContainer.ToggleVisibility, Key.F12);

        dockSurface.CreatePanel("uid://rs11fd5jwql7");
        
        GetParent().ChildEnteredTree += _ => GetParent().MoveChild(this, -1);
    }

    private void QuickLoad(string scenePath)
    {
        var result = GetTree().ChangeSceneToFile(scenePath);
        Print($"Loading scene {scenePath}...{result}");
    }

    public override void _Notification(int notification)
    {
        if (notification == NotificationWMCloseRequest || notification == NotificationExitTree )
        {
            SaveConfig();
        }
    }

    private static void SaveConfig()
    {
        config.Save(DebugCfgPath);
    }

    protected void AddDebugShortcut(Action action, Key keycode, KeyModifierMask keyModifiers = 0)
    {
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        var keycodeWithModifiers = keycode | (Key)keyModifiers;
        actions.Add(keycodeWithModifiers, action);
    }

    protected void AddConsoleCommand(Delegate action, string name, string description = "")
    {
        console.AddCommand(name, action, description);
    }

    public override void _ShortcutInput(InputEvent evt)
    {
        if (evt is not InputEventKey eventKey || eventKey.Pressed) return;

        var keycodeWithModifiers = eventKey.GetKeycodeWithModifiers();
        
        if (!actions.TryGetValue(keycodeWithModifiers, out var action)) return;
        
        action?.Invoke();
        GetViewport().SetInputAsHandled();
    }

    public static T LoadConfig<[MustBeVariant] T>(string key, T defaultValue = default)
    {
        return config.GetValue("Debug", key, Variant.From(defaultValue)).As<T>();
    }

    public static void SaveConfig<[MustBeVariant]T>(string key, T value)
    {
        config.SetValue("Debug", key, Variant.From(value));   
        SaveConfig();
    }
    
    public static void AddDebugPanel(IDebugPanel panel, string name, bool embed = true)
    {
        Assertions.AssertTrue(panel is Control);
        registeredPanels.Add(panel);

        if (embed)
        {
            panelContainer.AddTab(panel, name);
        }
    }

    public static void EmbedPanel(IDebugPanel panel)
    {
        Assertions.AssertTrue(panel is Control);
    }
    
    public static void PopPanel(IDebugPanel panel)
    {
        Assertions.AssertTrue(panel is Control);
    }
}
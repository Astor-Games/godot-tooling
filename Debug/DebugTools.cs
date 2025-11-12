using Godot.Collections;
using GodotLib.UI.Settings;

namespace GodotLib.Debug;

public partial class DebugTools : Node
{
    public const string QuickLoadScenesKey = "godot_lib/quick_load/scene_paths";
    
    private const string DebugCfgPath = "user://debug.cfg";
    private readonly System.Collections.Generic.Dictionary<Key, Action> actions = new();
    private static readonly ConfigFile config = new();
    private KeyModifierMask quickLoadModifiers = KeyModifierMask.MaskAlt;
    private Console console;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        config.Load(DebugCfgPath);

        var scenePaths = ProjectSettings.GetSetting(QuickLoadScenesKey).As<Array<string>>();
        if (scenePaths == null) return;
        
        for (var i = 0; i < Mathf.Min(scenePaths.Count, 9); i++)
        {
            var scene = scenePaths[i];
            AddDebugShortcut(() => QuickLoad(scene), Key.Key1 + i, quickLoadModifiers);
        }

        console = Load<PackedScene>("uid://s8wks02elbo6").Instantiate<Console>();
        console.Visible = false;
        AddChild(console);
        AddDebugShortcut(ToggleConsole, Key.Quoteleft);
    }

    private void QuickLoad(string scenePath)
    {
        var result = GetTree().ChangeSceneToFile(scenePath);
        Print($"Loading scene {scenePath}...{result}");
    }

    private void ToggleConsole()
    {
        console.Toggle();
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

    public static T GetSavedConfig<[MustBeVariant] T>(string key, T defaultValue = default)
    {
        return config.GetValue("Debug", key, Variant.From(defaultValue)).As<T>();
    }

    public static void SaveConfig<[MustBeVariant]T>(string key, T value)
    {
        config.SetValue("Debug", key, Variant.From(value));   
        SaveConfig();
    }
}
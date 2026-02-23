using System.Collections.Generic;
using System.Linq;
using AstorGames.EcsTools;
using GodotLib.UI;
using GodotLib.Util;

namespace GodotLib.Debug;

using GodotStringArray = Godot.Collections.Array<string>;

public partial class DebugManager : Node
{
    public const string QuickLoadScenesKey = "godot_lib/quick_load/scene_paths";
    private const string DebugCfgPath = "user://debug.cfg";
    private const KeyModifierMask quickLoadModifiers = KeyModifierMask.MaskAlt;
    
    private static readonly Dictionary<Key, DebugShortcut> shortcuts = new();
    private static readonly ConfigFile config = new();
    private static List<IDebugTool> registeredTools = new();
    private static Console console;
    private static DebugToolbox toolbox;
    protected static DockSurface DockSurface;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        config.Load(DebugCfgPath);

        // Create DockSurface
        DockSurface = new DockSurface();
        AddChild(DockSurface);

        var scenePaths = ProjectSettings.GetSetting(QuickLoadScenesKey).As<GodotStringArray>();
        if (scenePaths == null) return;
        
        for (var i = 0; i < Mathf.Min(scenePaths.Count, 9); i++)
        {
            var scene = scenePaths[i];
            AddDebugShortcut(() => QuickLoad(scene), Key.Key1 + i, quickLoadModifiers, $"Load scene {i+1}");
        }
        
        var quickHelp = DockSurface.CreatePanel<QuickHelp>("uid://4xhtd33lo64t", "quick_help");
        AddDebugShortcut(quickHelp.ToggleVisibility, Key.F1, description: "Quick help");

        console = DockSurface.CreatePanel<Console>("uid://dycmuxkbddnip", "console");
        AddDebugShortcut(console.ToggleVisibility, Key.F2, description: "Open console");
        
        var entityExplorer = DockSurface.CreatePanel<EntityExplorer>("uid://ce43m4m1dijmv", "entity_explorer");
        AddDebugShortcut(entityExplorer.ToggleVisibility, Key.F9, description: "Open entity explorer");
        
        var entityInspector = DockSurface.CreatePanel<EntityInspector>("uid://dlp8t5gkbh231", "entity_inspector");
        AddDebugShortcut(entityInspector.ToggleVisibility, Key.F10, description: "Open entity inspector");
        
        toolbox = DockSurface.CreatePanel<DebugToolbox>("uid://3p1cuqxlxa4t", "debug");
        AddDebugShortcut(toolbox.ToggleVisibility, Key.F12, description: "Open debug tools");

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

    protected void AddDebugShortcut(Action action, Key keycode, KeyModifierMask keyModifiers = 0, string description = null)
    {
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        var keycodeWithModifiers = keycode | (Key)keyModifiers;
        
        var shortcut = new DebugShortcut
        {
            Action = action,
            Description = description ?? action.Method.Name.FormatSentence()
        };

       shortcuts.Add(keycodeWithModifiers, shortcut);
    }

    protected void AddConsoleCommand(Delegate action, string name, string description = "")
    {
        console.AddCommand(name, action, description);
    }

    public override void _ShortcutInput(InputEvent evt)
    {
        if (evt is not InputEventKey eventKey || eventKey.Pressed) return;

        var keycodeWithModifiers = eventKey.GetKeycodeWithModifiers();
        
        if (!shortcuts.TryGetValue(keycodeWithModifiers, out var shortcut)) return;
        
        shortcut.Action?.Invoke();
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
    
    public static void AddDebugTool(IDebugTool tool, string name, bool embed = true)
    {
        Assertions.AssertTrue(tool is Control);
        registeredTools.Add(tool);

        if (embed)
        {
            toolbox.AddTab(tool, name);
        }
    }

    public static void EmbedPanel(IDebugTool tool)
    {
        Assertions.AssertTrue(tool is Control);
    }
    
    public static void PopPanel(IDebugTool tool)
    {
        Assertions.AssertTrue(tool is Control);
    }

    public static IEnumerable<(string, string)> ListShortcuts()
    {
        foreach (var (key, shortcut) in shortcuts.OrderBy(x => x.Value.Description))
        {
            var keycode = OS.GetKeycodeString(key);
            
            yield return (keycode, shortcut.Description);
        }
    }

    private struct DebugShortcut
    {
        public Action Action;
        public string Description;
    }
}
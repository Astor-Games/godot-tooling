using GodotLib.Debug;
using GodotLib.Util;

namespace GodotLib;
using Godot;

#if TOOLS

[Tool]
public partial class GodotLibPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        RegisterLoggingCategories();
        RegisterQuickLoadScenes();
    }

    private void RegisterLoggingCategories()
    {
        if (!ProjectSettings.HasSetting(LogUtils.QuickLoadScenesKey))
        {
            ProjectSettings.SetSetting(LogUtils.QuickLoadScenesKey, 0);
        }

        var categories = string.Join(',', Enum.GetNames<LoggingCategories>());
        ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary
        {
            { "name", LogUtils.QuickLoadScenesKey },
            { "type", (int)Variant.Type.Int },
            { "hint", (int)PropertyHint.Flags },  // Allows selecting files in the editor
            { "hint_string", categories }   // Restricts to scene files
        });
    }

    private static void RegisterQuickLoadScenes()
    {
        if (!ProjectSettings.HasSetting(DebugTools.QuickLoadScenesKey))
        {
            ProjectSettings.SetSetting(DebugTools.QuickLoadScenesKey, new Godot.Collections.Array<string>());
        }

        ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary
        {
            { "name", DebugTools.QuickLoadScenesKey },
            { "type", (int)Variant.Type.Array },
            { "hint", (int)PropertyHint.File },  // Allows selecting files in the editor
            { "hint_string", "*.tscn,*.scn" }   // Restricts to scene files
        });
    }

    public override void _ExitTree()
    {
        // Optional: Remove the setting when the addon is disabled
        ProjectSettings.Clear(DebugTools.QuickLoadScenesKey);
    }
}

#endif
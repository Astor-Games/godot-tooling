using GodotLib.Debug;

namespace GodotLib;
using Godot;

#if TOOLS

[Tool]
public partial class GodotLibPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        RegisterQuickLoadScenes();
    }

    private static void RegisterQuickLoadScenes()
    {
       if (!ProjectSettings.HasSetting(DebugManager.QuickLoadScenesKey))
       {
           ProjectSettings.SetSetting(DebugManager.QuickLoadScenesKey, new string[1]);
       }
       ProjectSettings.SetInitialValue(DebugManager.QuickLoadScenesKey, new string[1]);

       ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary
       {
           { "name", DebugManager.QuickLoadScenesKey },
           { "type", (int)Variant.Type.PackedStringArray },
           { "hint", (int)PropertyHint.TypeString },
           { "hint_string", $"{Variant.Type.String:D}/{PropertyHint.File:D}:*.tscn,*.scn" } // Array of strings (file paths). }
       });
    }

    public override void _ExitTree()
    {
        // Optional: Remove the setting when the addon is disabled
        ProjectSettings.Clear(DebugManager.QuickLoadScenesKey);
    }
}

#endif
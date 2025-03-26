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
        if (!ProjectSettings.HasSetting(QuickLoadScenesKey))
        {
            ProjectSettings.SetSetting(QuickLoadScenesKey, 0);
        }
        ProjectSettings.SetInitialValue(QuickLoadScenesKey, 0);

        var categories = string.Join(',', Enum.GetNames<LoggingCategories>());
        ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary
        {
            { "name", QuickLoadScenesKey },
            { "type", (int)Variant.Type.Int },
            { "hint", (int)PropertyHint.Flags },  // Allows selecting files in the editor
            { "hint_string", categories }   // Restricts to scene files
        });
    }

    private static void RegisterQuickLoadScenes()
    {
       if (!ProjectSettings.HasSetting(DebugTools.QuickLoadScenesKey))
       {
           ProjectSettings.SetSetting(DebugTools.QuickLoadScenesKey, new string[1]);
       }
       ProjectSettings.SetInitialValue(DebugTools.QuickLoadScenesKey, new string[1]);

       ProjectSettings.AddPropertyInfo(new Godot.Collections.Dictionary
       {
           { "name", DebugTools.QuickLoadScenesKey },
           { "type", (int)Variant.Type.PackedStringArray },
           { "hint", (int)PropertyHint.TypeString },
           { "hint_string", $"{Variant.Type.String:D}/{PropertyHint.File:D}:*.tscn,*.scn" } // Array of strings (file paths). }
       });
    }

    public override void _ExitTree()
    {
        // Optional: Remove the setting when the addon is disabled
        ProjectSettings.Clear(DebugTools.QuickLoadScenesKey);
    }
}

#endif
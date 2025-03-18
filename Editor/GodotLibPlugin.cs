namespace GodotLib.Debug;
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
        // Check if the setting already exists
        if (!ProjectSettings.HasSetting(DebugTools.QuickLoadScenesKey))
        {
            // Default value: an empty array
            ProjectSettings.SetSetting(DebugTools.QuickLoadScenesKey, new Godot.Collections.Array<string>());
        }

        // Make it visible in the editor (optional)
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
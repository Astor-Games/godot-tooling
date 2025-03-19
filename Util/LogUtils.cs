namespace GodotLib.Util;

public static class LogUtils
{
    public const string QuickLoadScenesKey = "godot_lib/logging/enabled_categories";
    
    public static LoggingCategories EnabledCategories;
    private static bool loaded;
    
    public static void PrintCategory(string text, LoggingCategories category)
    {
        if (!loaded)
        {
            EnabledCategories = ProjectSettings.GetSetting(QuickLoadScenesKey).As<LoggingCategories>();
            //Print($"Loading categories: {EnabledCategories}");
            loaded = true;
        }
        
        if (EnabledCategories.HasFlag(category))
        {
            Print($"[{category}] {text}");
        }
    }
}

[Flags]
public enum LoggingCategories
{
    Game = 1 << 0,
    CharacterState = 1 << 1,
    CharacterForces = 1 << 2,
    CharacterInteraction = 1 << 3,
}
namespace GodotLib.Util;

public static class LogUtils
{
    public static LoggingCategories EnabledCategories = LoggingCategories.CharacterInteraction | LoggingCategories.CharacterState;
    
    public static void PrintCategory(string text, LoggingCategories category)
    {
        if (EnabledCategories.HasFlag(category))
        {
            Print($"[{category}] {text}");
        }
    }
}

[Flags]
public enum LoggingCategories
{
    CharacterState = 1 << 0,
    CharacterForces = 1 << 1,
    CharacterInteraction = 1 << 2,
}
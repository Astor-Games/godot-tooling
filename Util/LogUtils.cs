using System.Collections.Generic;
using System.Text;

using Arg = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace GodotLib.Util;

public static class LogUtils
{
    public const string QuickLoadScenesKey = "godot_lib/logging/enabled_categories";
    
    public static LoggingCategories EnabledCategories;
    private static bool loaded;
    private static readonly StringBuilder sb = new();

    private static readonly List<string> metadata = [];
    private static string metadataStr = string.Empty;

    public static void AddMetadata(string value)
    {
        metadata.Add(value);
        metadataStr = string.Join(",", metadata);
    }
    
    public static void Log(string message, LoggingCategories category = General)
    {
        if (!loaded)
        {
            EnabledCategories = ProjectSettings.GetSetting(QuickLoadScenesKey).As<LoggingCategories>();
            //Print($"Loading categories: {EnabledCategories}");
            loaded = true;
        }

        if (!EnabledCategories.HasFlag(category)) return;

        if (metadataStr.IsNullOrEmpty())
        {
            Print($"[{category}] {message}");
        }
        else
        {
            Print($"[{metadataStr}] [{category}] {message}");
        }
    }

    public static void Log(object variable, LoggingCategories category = General, [Arg("variable")] string variableName = null)
    {
        var message = $"{variableName}: {variable}";
        Log(message, category);
    }
    
    public static void Log(object var1, object var2, LoggingCategories category = General, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null)
    {
        sb.Append($"{name1}: {var1}, ");
        sb.Append($"{name2}: {var2}");
        Log(sb.ToString(), category);
        sb.Clear();
    }
    
    public static void Log(object var1, object var2, object var3, LoggingCategories category = General, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null)
    {
        sb.Append($"{name1}: {var1}, ");
        sb.Append($"{name2}: {var2}, ");
        sb.Append($"{name3}: {var3}");
        Log(sb.ToString(), category);
        sb.Clear();
    }
    
    public static void Log(object var1, object var2, object var3, object var4, LoggingCategories category = General, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null, [Arg("var4")] string name4 = null)
    {
        sb.Append($"{name1}: {var1}, ");
        sb.Append($"{name2}: {var2}, ");
        sb.Append($"{name3}: {var3}, ");
        sb.Append($"{name4}: {var4}");
        Log(sb.ToString(), category);
        sb.Clear();
    }
}

[Flags]
public enum LoggingCategories
{
    General = 1 << 0,
    Multiplayer = 1 << 1,
    CharacterState = 1 << 2,
    CharacterForces = 1 << 3,
    CharacterInteraction = 1 << 4,
}
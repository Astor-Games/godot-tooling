using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Arg = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

// ReSharper disable once CheckNamespace
namespace GodotLib.Logging;

[Flags]
public enum SeverityLevel
{
    None = 0,
    
    Trace = 1 << 0,
    Info = 1 << 1,
    Warning = 1 << 2,
    Error = 1 << 3,
    
    All = Trace | Info | Warning | Error
}

public class Logger
{
    public SeverityLevel Level = SeverityLevel.Info;
    
    public string Name { get; }
    
    private readonly StringBuilder sb = new(256);

    public static Logger For<T>() => For(typeof(T).Name);
    
    public static Logger For(string name)
    {
        if (LogManager.Instance.TryGetLogger(name, out var logger))
        {
            return logger;
        }

        return new Logger(name);
    }
    
    private Logger(string name)
    {
        Name = name;
        LogManager.Instance.RegisterLogger(this);
    }

    [Conditional("DEBUG")]
    public void Trace(string message)
    {
        if (Level > SeverityLevel.Trace) return;
        sb.Append(message);
        PrintLog(SeverityLevel.Trace);
    }

    [Conditional("DEBUG")]
    public void Trace(string message, object variable, [Arg("variable")] string variableName = null)
    {
        if (Level > SeverityLevel.Trace) return;
        sb.Append(message);
        sb.Append("\n\t");
        sb.AppendVar(variableName, variable);
        PrintLog(SeverityLevel.Trace);
    }
    
    public void Info(string message)
    {
        if (Level > SeverityLevel.Info) return;
        sb.Append(message);
        PrintLog(SeverityLevel.Info);
    }
    
    public void Info(string message, object variable, [Arg("variable")] string variableName = null)
    {
        if (Level > SeverityLevel.Info) return;
        sb.Append(message);
        sb.Append("\n\t");
        sb.AppendVar(variableName, variable);
        PrintLog(SeverityLevel.Info);
    }
    
    public void Warning(string message)
    {
        if (Level > SeverityLevel.Warning) return;
        sb.Append(message);
        PrintLog(SeverityLevel.Warning);
    }
    
    public void Warning(string message, object variable, [Arg("variable")] string variableName = null)
    {
        if (Level > SeverityLevel.Warning) return;
        sb.Append(message);
        sb.Append("\n\t");
        sb.AppendVar(variableName, variable);
        PrintLog(SeverityLevel.Warning);
    }
    
    public void Error(string message)
    {
        if (Level > SeverityLevel.Error) return;
        sb.Append(message);
        PrintLog(SeverityLevel.Error);
    }
    
    public void Error(string message, object variable, [Arg("variable")] string variableName = null)
    {
        if (Level > SeverityLevel.Error) return;
        sb.Append(message);
        sb.Append("\n\t");
        sb.AppendVar(variableName, variable);
        PrintLog(SeverityLevel.Error);
    }
    
    public void PrintVar(object variable, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("variable")] string variableName = null)
    {
        if (Level > severityLevel) return;
        sb.AppendVar(variableName, variable);
        PrintLog(severityLevel);
    }
    
    public void PrintVars(object var1, object var2, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null)
    {
        if (Level > severityLevel) return;
        
        sb.AppendVar(name1, var1).AppendLine();
        sb.AppendVar(name2, var2);
        PrintLog(severityLevel);
    }
    
    public void PrintVars(object var1, object var2, object var3, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null)
    {
        if (Level > severityLevel) return;
        
        sb.AppendVar(name1, var1).AppendLine();
        sb.AppendVar(name2, var2).AppendLine();
        sb.AppendVar(name3, var3);
        PrintLog(severityLevel);
    }
    
    public void PrintVars(object var1, object var2, object var3, object var4, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null, [Arg("var4")] string name4 = null)
    {
        if (Level > severityLevel) return;
        
        sb.AppendVar(name1, var1).AppendLine();
        sb.AppendVar(name2, var2).AppendLine();
        sb.AppendVar(name3, var3).AppendLine();
        sb.AppendVar(name4, var4);
        PrintLog(severityLevel);
    }
    
    private void PrintLog(SeverityLevel severityLevel)
    {
        var message = sb.ToString();
        LogManager.Instance.AddLog(this, severityLevel, message);
        sb.Clear();
    }
}

public static class StringBuilderExt
{
    private static readonly Dictionary<Type, bool> customToStringCache = new();
    private static readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };
    
    public static StringBuilder AppendVar(this StringBuilder sb, string varName, object var)
    {
        if (HasCustomToString(var))
        {
            return sb.Append(varName).Append(": ").Append(var);
        }

        var json = JsonSerializer.Serialize(var, jsonOptions);
        return sb.Append(varName).Append(": ").Append(json);
    }
    
    private static bool HasCustomToString(object var)
    {
        var type = var.GetType();

        if (customToStringCache.TryGetValue(type, out var result)) return result;
        var toStringMethod = type.GetMethod("ToString", Type.EmptyTypes);
        var hasCustomToString = toStringMethod?.DeclaringType != typeof(object);
        customToStringCache.Add(type, hasCustomToString);
        return hasCustomToString;
    }
}
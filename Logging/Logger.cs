using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Utf8Json;
using Arg = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

// ReSharper disable once CheckNamespace
namespace GodotLib.Logging;

public class Logger(string name)
{
    private static readonly Stopwatch stopwatch;
    
    public enum LogOutput
    {
        GodotConsole,
        DotNetConsole
    }
    
    public enum SeverityLevel
    {
        Debug = 1,
        Info = 2,
        Warning = 3,
        Error = 4
    }

    public static LogOutput Output;
    public SeverityLevel Level = SeverityLevel.Info;
    
    
    public string Name { get; init; } = name;
    private readonly StringBuilder sb = new(256);
    private static readonly Lock printLock = new();

    static Logger()
    {
        Output = LogOutput.DotNetConsole;
        stopwatch = Stopwatch.StartNew();
    }

    [Conditional("DEBUG")]
    public void Debug(string message)
    {
        if (Level > SeverityLevel.Debug) return;
        sb.Append(message);
        PrintLog(SeverityLevel.Debug);
    }

    [Conditional("DEBUG")]
    public void Debug(string message, object variable, [Arg("variable")] string variableName = null)
    {
        if (Level > SeverityLevel.Debug) return;
        sb.Append(message);
        sb.Append(" | ");
        sb.AppendVar(variableName, variable);
        PrintLog(SeverityLevel.Debug);
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
        sb.Append(" | ");
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
        sb.Append(" | ");
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
        sb.Append(" | ");
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
        
        sb.AppendVar(name1, var1).Append(", ");
        sb.AppendVar(name2, var2);
        PrintLog(severityLevel);
    }
    
    public void PrintVars(object var1, object var2, object var3, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null)
    {
        if (Level > severityLevel) return;
        
        sb.AppendVar(name1, var1).Append(", ");
        sb.AppendVar(name2, var2).Append(", ");
        sb.AppendVar(name3, var3);
        PrintLog(severityLevel);
    }
    
    public void PrintVars(object var1, object var2, object var3, object var4, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null, [Arg("var4")] string name4 = null)
    {
        if (Level > severityLevel) return;
        
        sb.AppendVar(name1, var1).Append(", ");
        sb.AppendVar(name2, var2).Append(", ");
        sb.AppendVar(name3, var3).Append(", ");
        sb.AppendVar(name4, var4);
        PrintLog(severityLevel);
    }
    
    private void PrintLog(SeverityLevel severityLevel)
    {
        var timestamp = stopwatch.Elapsed.ToString(@"h\:mm\:ss\.fff");
        var formattedMessage = $"[{timestamp}] [{Name}] [{severityLevel}] {sb}";
        
        switch (Output)
        {
            case LogOutput.GodotConsole:
                Print(formattedMessage);
                break;
            
            case LogOutput.DotNetConsole:
                
                var ansiColor = severityLevel switch
                {
                    SeverityLevel.Debug => ConsoleColor.Gray,
                    SeverityLevel.Info => ConsoleColor.Black,
                    SeverityLevel.Warning => ConsoleColor.Yellow,
                    SeverityLevel.Error => ConsoleColor.Red,
                    _ => ConsoleColor.White
                };
                
                lock (printLock)
                {
                    Console.ForegroundColor = ansiColor;
                    Console.WriteLine(formattedMessage);
                    Console.ResetColor();
                }
                break;
        }
        
        sb.Clear();
    }
}

public static class StringBuilderExt
{
    public static StringBuilder AppendVar(this StringBuilder sb, string varName, object var)
    {
        var json = JsonSerializer.PrettyPrint(JsonSerializer.Serialize(var));
        return sb.Append(varName).Append(": ").Append(json);
    }
}
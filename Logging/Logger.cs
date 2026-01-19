using System.Text;

using Arg = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

// ReSharper disable once CheckNamespace
namespace GodotLib.Logging;

public class Logger
{
    public enum SeverityLevel
    {
        Verbose = 1,
        Info = 2,
        Warning = 3,
        Error = 4
    }
    
    public SeverityLevel Level = SeverityLevel.Info;
    public string Name { get; init; }
    private readonly StringBuilder message = new(256);
    private readonly StringBuilder prefix = new(64);

    public Logger(string name)
    {
        Name = name;
    }
    
    public void LogVerbose(string message)
    {
        if (Level < SeverityLevel.Verbose) return;
        this.message.Append(message);
        Print(SeverityLevel.Verbose);
    }
    
    public void LogWarning(string message)
    {
        if (Level < SeverityLevel.Warning) return;
        this.message.Append(message);
        Print(SeverityLevel.Warning);
    }
    
    public void LogError(string message)
    {
        if (Level < SeverityLevel.Error) return;
        this.message.Append(message);
        Print(SeverityLevel.Error);
    }
    
    public void Log(string message, SeverityLevel severityLevel = SeverityLevel.Info)
    {
        if (Level < severityLevel) return;
        this.message.Append(message);
        Print(severityLevel);
    }

    public void Log(object variable, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("variable")] string variableName = null)
    {
       if (Level < severityLevel) return;
        
        message.Append(variableName).Append(": ").Append(variable);
        Print(severityLevel);
    }
    
    public void Log(object var1, object var2, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null)
    {
       if (Level < severityLevel) return;
        
        message.Append(name1).Append(": ").Append(var1).Append(", ");
        message.Append(name2).Append(": ").Append(var2);
        Print(severityLevel);
    }
    
    public void Log(object var1, object var2, object var3, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null)
    {
       if (Level < severityLevel) return;
        
        message.Append(name1).Append(": ").Append(var1).Append(", ");
        message.Append(name2).Append(": ").Append(var2).Append(", ");
        message.Append(name3).Append(": ").Append(var3);
        Print(severityLevel);
    }
    
    public void Log(object var1, object var2, object var3, object var4, SeverityLevel severityLevel = SeverityLevel.Info, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null, [Arg("var4")] string name4 = null)
    {
       if (Level < severityLevel) return;
        
        message.Append(name1).Append(": ").Append(var1).Append(", ");
        message.Append(name2).Append(": ").Append(var2).Append(", ");
        message.Append(name3).Append(": ").Append(var3).Append(", ");
        message.Append(name4).Append(": ").Append(var4);
        Print(severityLevel);
    }
    
    private void Print(SeverityLevel severityLevel)
    {
        if (severityLevel == SeverityLevel.Warning)
        {
            prefix.Append("[color=").Append(Colors.Gold).Append(']');
            message.Append("[color=").Append(Colors.Gold).Append(']');
        } else if (severityLevel == SeverityLevel.Error)
        {
            prefix.Append("[color=").Append(Colors.Crimson).Append(']');
            message.Append("[color=").Append(Colors.Crimson).Append(']');
        }

        prefix.Append('[').Append(severityLevel).Append("] [").Append(Name).Append("] ");
        
        var finalString = prefix.Append(message).ToString();
        PrintRich(finalString);
        
        prefix.Clear();
        message.Clear();
    }
}
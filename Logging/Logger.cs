using System.Collections.Generic;
using System.Text;

using Arg = System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

// ReSharper disable once CheckNamespace
namespace GodotLib.Logging;

public class Logger
{
    public bool Enabled { get; set; } = true;
    public string Name { get; init; }
    public static readonly Dictionary<string, object> GlobalMetadata = new();
    public readonly Dictionary<string, object> Metadata = new();
    private readonly StringBuilder sb = new(256);

    public Logger(string name)
    {
        Name = name;
    }
    
    public void Log(string message)
    {
        if (!Enabled) return;
        sb.Append(message);
        Print();
    }

    public void Log(object variable, [Arg("variable")] string variableName = null)
    {
        if (!Enabled) return;
        
        sb.Append(variableName).Append(": ").Append(variable);
        Print();
    }
    
    public void Log(object var1, object var2, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null)
    {
        if (!Enabled) return;
        
        sb.Append(name1).Append(": ").Append(var1).Append(", ");
        sb.Append(name2).Append(": ").Append(var2);
        Print();
    }
    
    public void Log(object var1, object var2, object var3, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null)
    {
        if (!Enabled) return;
        
        sb.Append(name1).Append(": ").Append(var1).Append(", ");
        sb.Append(name2).Append(": ").Append(var2).Append(", ");
        sb.Append(name3).Append(": ").Append(var3);
        Print();
    }
    
    public void Log(object var1, object var2, object var3, object var4, [Arg("var1")] string name1 = null, [Arg("var2")] string name2 = null, [Arg("var3")] string name3 = null, [Arg("var4")] string name4 = null)
    {
        if (!Enabled) return;
        
        sb.Append(name1).Append(": ").Append(var1).Append(", ");
        sb.Append(name2).Append(": ").Append(var2).Append(", ");
        sb.Append(name3).Append(": ").Append(var3).Append(", ");
        sb.Append(name4).Append(": ").Append(var4);
        Print();
    }

    private void Print()
    {
        sb.Append('[').Append(Name).Append("] ");
        foreach (var (key, value) in GlobalMetadata)
        {
            sb.Append('[').Append(key).Append(": ").Append(value).Append("] ");
        }
        foreach (var (key, value) in Metadata)
        {
            sb.Append('[').Append(key).Append(": ").Append(value).Append("] ");
        }
        GD.Print(sb.ToString());
        sb.Clear();
    }
}
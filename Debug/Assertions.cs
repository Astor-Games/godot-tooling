using System.Diagnostics;
using JetBrains.Annotations;

using Arg =  System.Runtime.CompilerServices.CallerArgumentExpressionAttribute;

namespace GodotLib.Debug;

public static class Assertions
{
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("obj:null => halt")]
    public static void AssertNotNull(object obj, string message = null, bool fatal = true, [Arg(nameof(obj))]string objName = null)
    {
        AssertInternal(obj != null, message ?? $"{objName} was null.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("obj:notnull => halt")]
    public static void AssertNull(object obj, string message = null, bool fatal = true, [Arg(nameof(obj))]string objName = null)
    {
        AssertInternal(obj == null, message ?? $"{objName} was not null.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod]
    public static void AssertEqual(object obj1, object obj2, string message = null, bool fatal = true, [Arg(nameof(obj1))]string obj1Name = null, [Arg(nameof(obj2))]string obj2Name = null)
    {
        AssertInternal(obj1.Equals(obj2), message ?? $"{obj1Name} was not equal to {obj2Name}.\nExpected {obj2}, but was {obj1}", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    public static void AssertNotEqual(object obj1, object obj2, string message = null, bool fatal = true, [Arg(nameof(obj1))]string obj1Name = null, [Arg(nameof(obj2))]string obj2Name = null)
    {
        AssertInternal(!obj1.Equals(obj2), message ?? $"{obj1Name} was equal to {obj2Name}.", fatal);
    }

    public static void AssertInRange(int value, int min, int max, string message = null, bool fatal = true, [Arg(nameof(value))]string valueName = null)
    {
        AssertInternal(value >= min && value < max, message ?? $"{valueName} ({value}) was outside the range [{min}-{max}].", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("condition: false => halt")]
    public static void AssertTrue(bool condition, string message = null, [Arg(nameof(condition))] string expression = null, bool fatal = true)
    {
        AssertInternal(condition, message ?? $"{expression} should be true, was false.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("condition: true => halt")]
    public static void AssertFalse(bool condition, string message = null, [Arg(nameof(condition))] string expression = null, bool fatal = true)
    {
        AssertInternal(!condition, message ?? $"{expression} should be false, was true.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    private static void AssertInternal(bool condition, string message, bool fatal)
    {
        if (condition) return;

        if (!fatal)
        {
            PushWarning("Assertion failed: " + message);
            return;
        }
        
        PushError("Assertion failed: " + message);
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
        if (EngineDebugger.IsActive())
        {
            EngineDebugger.Debug();
        }
        else
        {
            var method = new StackTrace().GetFrame(2)!.GetMethod()!;
            var messageAndSource = $"{message}\n\nat {method.DeclaringType!.FullName}.{method.Name}";
            OS.Alert(EscapeForGtkAlert(messageAndSource), "Assertion failed!");
        }
    }
    
    static string EscapeForGtkAlert(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}
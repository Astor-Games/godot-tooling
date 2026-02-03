using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GodotLib.Debug;

public static class Assertions
{
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("obj:null => halt")]
    public static void AssertNotNull(object obj, string message = null, bool fatal = true, [CallerArgumentExpression(nameof(obj))]string objName = null)
    {
        AssertInternal(obj != null, message ?? $"{objName} was null.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("obj:notnull => halt")]
    public static void AssertNull(object obj, string message = null, bool fatal = true, [CallerArgumentExpression(nameof(obj))]string objName = null)
    {
        AssertInternal(obj == null, message ?? $"{objName} was not null.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod]
    public static void AssertEqual(object obj1, object obj2, string message = null, bool fatal = true, [CallerArgumentExpression(nameof(obj1))]string obj1Name = null, [CallerArgumentExpression(nameof(obj2))]string obj2Name = null)
    {
        AssertInternal(obj1.Equals(obj2), message ?? $"{obj1Name} was not equal to {obj2Name}.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    public static void AssertNotEqual(object obj1, object obj2, string message = null, bool fatal = true, [CallerArgumentExpression(nameof(obj1))]string obj1Name = null, [CallerArgumentExpression(nameof(obj2))]string obj2Name = null)
    {
        AssertInternal(!obj1.Equals(obj2), message ?? $"{obj1Name} was equal to {obj2Name}.", fatal);
    }
    
    
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("condition: false => halt")]
    public static void AssertTrue(bool condition, string message = null, [CallerArgumentExpression(nameof(condition))] string expression = null, bool fatal = true)
    {
        AssertInternal(condition, message ?? $"{expression} should be true, was false.", fatal);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("condition: true => halt")]
    public static void AssertFalse(bool condition, string message = null, [CallerArgumentExpression(nameof(condition))] string expression = null, bool fatal = true)
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
            var frame = new StackTrace().GetFrame(3);
            OS.Alert(EscapeForGtkAlert(message + $"\n\n{frame!.GetMethod()}\n\n{frame.GetMethod().DeclaringType.FullName}"), "Assertion failed!");
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
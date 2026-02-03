using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace GodotLib.Debug;

public static class Assertions
{
    private static bool initialized, haltRequested;
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("obj:null => halt")]
    public static void AssertNotNull(object obj, string message = null, [CallerArgumentExpression(nameof(obj))]string objName = null)
    {
        AssertInternal(obj != null, message ?? $"{objName} was null.");
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("obj:notnull => halt")]
    public static void AssertNull(object obj, string message = null, [CallerArgumentExpression(nameof(obj))]string objName = null)
    {
        AssertInternal(obj == null, message ?? $"{objName} was not null.");
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod]
    public static void AssertEqual(object obj1, object obj2, string message = null, [CallerArgumentExpression(nameof(obj1))]string obj1Name = null, [CallerArgumentExpression(nameof(obj2))]string obj2Name = null)
    {
        AssertInternal(obj1.Equals(obj2), message ?? $"{obj1Name} was not equal to {obj2Name}.");
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    public static void AssertNotEqual(object obj1, object obj2, string message = null, [CallerArgumentExpression(nameof(obj1))]string obj1Name = null, [CallerArgumentExpression(nameof(obj2))]string obj2Name = null)
    {
        AssertInternal(!obj1.Equals(obj2), message ?? $"{obj1Name} was equal to {obj2Name}.");
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("condition: false => halt")]
    public static void AssertTrue(bool condition, [CallerArgumentExpression(nameof(condition))] string message = null)
    {
        AssertInternal(condition, message);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    [AssertionMethod, ContractAnnotation("condition: true => halt")]
    public static void AssertFalse(bool condition, [CallerArgumentExpression(nameof(condition))] string message = null)
    {
        AssertInternal(!condition, message);
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    private static void AssertInternal(bool condition, string message)
    {
        if (condition) return;

        PushError("Assertion failed: " + message);
        if (EngineDebugger.IsActive())
        {
            EngineDebugger.Debug();
        }
        else
        {
            OS.Alert(message, "Assertion failed!");
        }
    }
}
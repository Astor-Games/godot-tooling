using System.Diagnostics;
using System.Runtime.CompilerServices;
using Turtles;

namespace GodotLib.Debug;

public static class Assertions
{
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    public static void AssertNotNull(object obj, string message = null, [CallerArgumentExpression(nameof(obj))]string objName = null)
    {
        AssertInternal(obj != null, message ?? $"{objName} was null.");
    }
    
    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    public static void Assert(bool condition, [CallerArgumentExpression(nameof(condition))] string message = null)
    {
        AssertInternal(condition, message);
    }

    [Conditional("DEBUG")]
    [DebuggerHidden, StackTraceHidden]
    private static void AssertInternal(bool condition, string message)
    {
        if (condition) return;
        EngineDebugger.Debug();
        throw new AssertionException("Assert failed: " + message);
    }
}

[StackTraceHidden]
public class AssertionException(string message) : Exception(message);
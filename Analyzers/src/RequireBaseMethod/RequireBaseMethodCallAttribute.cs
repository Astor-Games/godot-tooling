using System;

namespace GodotLib.Analyzers;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class RequireBaseMethodCallAttribute : Attribute
{
}
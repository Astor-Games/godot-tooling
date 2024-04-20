using System;

namespace DotNetTooling.Analyzers.RequireBaseMethod;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class RequireBaseMethodCallAttribute : Attribute
{
}
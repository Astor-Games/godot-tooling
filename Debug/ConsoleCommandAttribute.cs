using System;
using System.Linq;
using System.Reflection;

namespace GodotLib.Debug;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ConsoleCommandAttribute(string name = null, string description = "") : Attribute
{
    public readonly string Name = name;
    public readonly string Description = description;
}

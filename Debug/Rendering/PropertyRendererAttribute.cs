namespace GodotLib.Debug;

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PropertyRendererAttribute(params Type[] renderedTypes) : Attribute
{
    public Type[] RenderedTypes { get; } = renderedTypes;
}
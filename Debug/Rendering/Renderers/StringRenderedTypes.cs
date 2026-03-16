using System.Runtime.CompilerServices;
using Turtles.Core;

namespace GodotLib.Debug;

public static class StringRenderedTypes
{
    [ModuleInitializer]
    public static void RegisterTypes()
    {
        PropertyTreeRendering.RenderTypeAsString<Aabb>();
        PropertyTreeRendering.RenderTypeAsString<Quaternion>();
        PropertyTreeRendering.RenderTypeAsString<EventId>();
        PropertyTreeRendering.RenderTypeAsString<ComponentId>();
        PropertyTreeRendering.RenderTypeAsString<PlayerId>();
    }
}
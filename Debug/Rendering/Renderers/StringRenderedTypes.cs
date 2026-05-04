using System.Runtime.CompilerServices;

namespace GodotLib.Debug;

public static class StringRenderedTypes
{
    [ModuleInitializer]
    public static void RegisterTypes()
    {
        PropertyTreeRendering.RenderTypeAsString<Aabb>();
        PropertyTreeRendering.RenderTypeAsString<Quaternion>();
    }
}
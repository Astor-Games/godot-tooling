namespace GodotLib.Debug;

public static class StringRenderedTypes
{
    public static void RegisterTypes()
    {
        PropertyTreeRendering.RenderTypeAsString<Aabb>();
        PropertyTreeRendering.RenderTypeAsString<Quaternion>();
    }
}
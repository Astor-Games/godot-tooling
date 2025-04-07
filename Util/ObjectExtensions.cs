namespace GodotLib.Util;

public static class ObjectExtensions
{
    public static T GetMeta<[MustBeVariant] T>(this GodotObject obj, string name, T defaultValue = default)
    {
        return obj.GetMeta(name, Variant.From(defaultValue)).As<T>();
    }
}
namespace GodotLib.Util;

public static class ObjectExtensions
{
    public static T Get<[MustBeVariant] T>(this GodotObject obj, StringName name)
    {
        return obj.Get(name).As<T>();
    }
    
    public static T GetMeta<[MustBeVariant] T>(this GodotObject obj, StringName name, T defaultValue = default)
    {
        return obj.GetMeta(name, Variant.From(defaultValue)).As<T>();
    }
}
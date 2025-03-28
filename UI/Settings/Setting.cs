namespace GodotLib.UI.Settings;

public class Setting(StringName key, Variant defaultValue)
{
    public readonly StringName Key = key;
    public readonly Variant DefaultValue = defaultValue;
    
    public Variant Value = defaultValue;
}

public class Setting<[MustBeVariant]T>(StringName key, T defaultValue) : Setting(key, Variant.From(defaultValue))
{
    public new T Value;
}

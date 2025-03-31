namespace GodotLib.UI.Settings;

[Flags]
public enum SettingFlags
{
    None = 1 << 0,
    Delayed = 1 << 1,
}

public abstract class Setting(StringName key, Variant defaultValue, SettingFlags flags, params DisplayOption[] displayOptions)
{
    public readonly StringName Key = key;

    public Variant Value => ValueInternal;

    public readonly Variant DefaultValue = defaultValue;
    public readonly SettingFlags Flags = flags;
    public readonly DisplayOption[] DisplayDisplayOptions = displayOptions;

    protected Variant ValueInternal = defaultValue;

    public virtual void SetValue(Variant newValue)
    {
        ValueInternal = newValue;
    }

    public virtual void NotifyChanges()
    {
    }

    public bool DisplayAs<T>(out T displayOption) where T : DisplayOption
    {
        foreach (var option in DisplayDisplayOptions)
        {
            if (option is T tOption)
            {
                displayOption = tOption;
                return true;
            }
        }
        displayOption = default;    
        return false;
    }
}

public class Setting<[MustBeVariant]T>(StringName key, T defaultValue, SettingFlags flags = SettingFlags.None, params DisplayOption[] displayOptions) : Setting(key, Variant.From(defaultValue), flags, displayOptions)
{
    public event Action<T> OnValueChanged;
    
    public new T Value => ValueInternal.As<T>();

    private bool changed;

    public override void SetValue(Variant newValue)
    {
        base.SetValue(newValue);
        if (Flags.HasFlag(SettingFlags.Delayed))
            changed = true;
        else
            OnValueChanged?.Invoke(ValueInternal.As<T>());
    }

    public override void NotifyChanges()
    {
        if (changed && Flags.HasFlag(SettingFlags.Delayed))
        {
            OnValueChanged?.Invoke(ValueInternal.As<T>());
            changed = false;
        }
    }

    public static implicit operator T(Setting<T> setting) => setting.Value;
}

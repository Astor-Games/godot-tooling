namespace GodotLib.UI.Settings;

[Flags]
public enum PropertyFlags
{
    None = 1 << 0,
    Delayed = 1 << 1,
}

public abstract class Property(StringName key, Variant defaultValue, PropertyFlags flags, params DisplayOption[] displayOptions)
{
    public readonly StringName Key = key;

    public Variant Value => ValueInternal;

    public readonly Variant DefaultValue = defaultValue;
    public readonly PropertyFlags Flags = flags;
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

    public override string ToString()
    {
        return Value.ToString();
    }
}

public class Property<[MustBeVariant]T>(StringName key, T defaultValue, PropertyFlags flags = PropertyFlags.None, params DisplayOption[] displayOptions) : Property(key, Variant.From(defaultValue), flags, displayOptions)
{
    public event Action<T> ValueChanged;
    
    public new T Value => ValueInternal.As<T>();

    private bool changed;

    public override void SetValue(Variant newValue)
    {
        base.SetValue(newValue);
        if (Flags.HasFlag(PropertyFlags.Delayed))
            changed = true;
        else
            ValueChanged?.Invoke(ValueInternal.As<T>());
    }

    public override void NotifyChanges()
    {
        if (changed && Flags.HasFlag(PropertyFlags.Delayed))
        {
            ValueChanged?.Invoke(ValueInternal.As<T>());
            changed = false;
        }
    }

    public static implicit operator T(Property<T> property) => property.Value;
}

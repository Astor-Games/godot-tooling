#if TOOLS
using Godot;
using System;

namespace GodotLib;

[Tool]
public partial class DateTimeEditorPlugin : EditorPlugin
{
    private DateTimeInspectorPlugin _inspector;

    public override void _EnterTree()
    {
        _inspector = new DateTimeInspectorPlugin();
        AddInspectorPlugin(_inspector);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(_inspector);
        _inspector = null;
    }
}

// ── Inspector plugin — decides which properties get custom editors ────────────

public partial class DateTimeInspectorPlugin : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object) => true;

    public override bool _ParseProperty(
        GodotObject @object,
        Variant.Type type,
        string name,
        PropertyHint hintType,
        string hintString,
        PropertyUsageFlags usageFlags,
        bool wide)
    {
        if (type != Variant.Type.Object) return false;

        EditorProperty editor = hintString switch
        {
            nameof(DateTime) => new DateTimePropertyEditor(),
            nameof(Date) => new DatePropertyEditor(),
            nameof(Time) => new TimePropertyEditor(),
            _ => null
        };

        if (editor == null) return false;

        AddPropertyEditor(name, editor);
        return true;
    }
}

// ────────────────────────────────────────────────────────────────────────────
// GodotDateTime editor — year / month / day | hour : minute : second  [kind]
// ────────────────────────────────────────────────────────────────────────────

public partial class DateTimePropertyEditor : EditorProperty
{
    private SpinBox _year, _month, _day, _hour, _minute, _second;
    private Label   _kindLabel;
    private bool    _updating;

    public override void _Ready()
    {
        var hbox = new HBoxContainer();
        AddChild(hbox);

        _year   = MakeSpinBox(hbox, 1, 9999, "Y");
        Separator(hbox);
        _month  = MakeSpinBox(hbox, 1, 12,   "M");
        Separator(hbox);
        _day    = MakeSpinBox(hbox, 1, 31,   "D");

        AddSpacer(hbox, 6);

        _hour   = MakeSpinBox(hbox, 0, 23, "h");
        hbox.AddChild(new Label { Text = ":" });
        _minute = MakeSpinBox(hbox, 0, 59, "m");
        hbox.AddChild(new Label { Text = ":" });
        _second = MakeSpinBox(hbox, 0, 59, "s");

        AddSpacer(hbox, 6);

        _kindLabel = new Label();
        _kindLabel.AddThemeColorOverride("font_color", Colors.Gray);
        hbox.AddChild(_kindLabel);

        foreach (var sb in new[] { _year, _month, _day, _hour, _minute, _second })
            sb.ValueChanged += _ => OnChanged();
    }

    public override void _UpdateProperty()
    {
        var resource = GetEditedObject().Get(GetEditedProperty()).As<DateTime>();
        if (resource == null) return;

        _updating = true;
        var dt = resource.Value;
        _year.Value   = dt.Year;
        _month.Value  = dt.Month;
        _day.Value    = dt.Day;
        _hour.Value   = dt.Hour;
        _minute.Value = dt.Minute;
        _second.Value = dt.Second;
        _kindLabel.Text = dt.Kind switch
        {
            DateTimeKind.Utc   => "UTC",
            DateTimeKind.Local => "LOC",
            _                  => "UNS"
        };
        _updating = false;
    }

    private void OnChanged()
    {
        if (_updating) return;
        var resource = GetEditedObject().Get(GetEditedProperty()).As<DateTime>();
        if (resource == null) return;

        // Clamp day to valid range for selected month/year
        int maxDay = System.DateTime.DaysInMonth((int)_year.Value, (int)_month.Value);
        if (_day.Value > maxDay) _day.Value = maxDay;

        resource.Value = new System.DateTime(
            (int)_year.Value, (int)_month.Value, (int)_day.Value,
            (int)_hour.Value, (int)_minute.Value, (int)_second.Value,
            resource.Value.Kind);

        EmitChanged(GetEditedProperty(), resource);
    }
}

// ────────────────────────────────────────────────────────────────────────────
// GodotDateOnly editor — year / month / day
// ────────────────────────────────────────────────────────────────────────────

public partial class DatePropertyEditor : EditorProperty
{
    private SpinBox _year, _month, _day;
    private bool    _updating;

    public override void _Ready()
    {
        var hbox = new HBoxContainer();
        AddChild(hbox);

        _year  = MakeSpinBox(hbox, 1, 9999, "Y");
        Separator(hbox);
        _month = MakeSpinBox(hbox, 1, 12,   "M");
        Separator(hbox);
        _day   = MakeSpinBox(hbox, 1, 31,   "D");

        foreach (var sb in new[] { _year, _month, _day })
            sb.ValueChanged += _ => OnChanged();
    }

    public override void _UpdateProperty()
    {
        var resource = GetEditedObject().Get(GetEditedProperty()).As<Date>();
        if (resource == null) return;

        _updating = true;
        var d = resource.Value;
        _year.Value  = d.Year;
        _month.Value = d.Month;
        _day.Value   = d.Day;
        _updating = false;
    }

    private void OnChanged()
    {
        if (_updating) return;
        var resource = GetEditedObject().Get(GetEditedProperty()).As<Date>();
        if (resource == null) return;

        int maxDay = System.DateTime.DaysInMonth((int)_year.Value, (int)_month.Value);
        if (_day.Value > maxDay) _day.Value = maxDay;

        resource.Value = new System.DateOnly((int)_year.Value, (int)_month.Value, (int)_day.Value);
        EmitChanged(GetEditedProperty(), resource);
    }
}

// ────────────────────────────────────────────────────────────────────────────
// GodotTimeSpan editor — [±] days | hours : minutes : seconds
// ────────────────────────────────────────────────────────────────────────────

public partial class TimePropertyEditor : EditorProperty
{
    private CheckBox _negative;
    private SpinBox  _days, _hours, _minutes, _seconds;
    private bool     _updating;

    public override void _Ready()
    {
        var hbox = new HBoxContainer();
        AddChild(hbox);

        _negative = new CheckBox { Text = "−" };
        _negative.Toggled += _ => OnChanged();
        hbox.AddChild(_negative);

        AddSpacer(hbox, 4);

        _days    = MakeSpinBox(hbox, 0, 36500, "d");
        Separator(hbox);
        _hours   = MakeSpinBox(hbox, 0, 23, "h");
        hbox.AddChild(new Label { Text = ":" });
        _minutes = MakeSpinBox(hbox, 0, 59, "m");
        hbox.AddChild(new Label { Text = ":" });
        _seconds = MakeSpinBox(hbox, 0, 59, "s");

        foreach (var sb in new[] { _days, _hours, _minutes, _seconds })
            sb.ValueChanged += _ => OnChanged();
    }

    public override void _UpdateProperty()
    {
        var resource = GetEditedObject().Get(GetEditedProperty()).As<Time>();
        if (resource == null) return;

        _updating = true;
        var ts = resource.Value;
        bool neg = ts.Ticks < 0;
        if (neg) ts = ts.Duration();

        _negative.ButtonPressed = neg;
        _days.Value    = ts.Days;
        _hours.Value   = ts.Hours;
        _minutes.Value = ts.Minutes;
        _seconds.Value = ts.Seconds;
        _updating = false;
    }

    private void OnChanged()
    {
        if (_updating) return;
        var resource = GetEditedObject().Get(GetEditedProperty()).As<Time>();
        if (resource == null) return;

        var ts = new TimeSpan((int)_days.Value, (int)_hours.Value, (int)_minutes.Value, (int)_seconds.Value);
        if (_negative.ButtonPressed) ts = -ts;

        resource.Value = ts;
        EmitChanged(GetEditedProperty(), resource);
    }
}

// ────────────────────────────────────────────────────────────────────────────
// Shared helpers (extension-style statics — can't use extensions on partial classes here)
// ────────────────────────────────────────────────────────────────────────────

public static class EditorPropertyHelpers
{
    public static SpinBox MakeSpinBox(HBoxContainer parent, int min, int max, string tooltip)
    {
        var label = new Label { Text = tooltip };
        label.AddThemeColorOverride("font_color", Colors.DimGray);
        parent.AddChild(label);

        var sb = new SpinBox
        {
            MinValue = min,
            MaxValue = max,
            Step     = 1,
            AllowGreater = false,
            AllowLesser  = false,
            CustomMinimumSize = new Vector2(60, 0),
            TooltipText = tooltip
        };
        parent.AddChild(sb);
        return sb;
    }

    public static void Separator(HBoxContainer parent)
    {
        var sep = new Label { Text = "/" };
        sep.AddThemeColorOverride("font_color", Colors.DimGray);
        parent.AddChild(sep);
    }

    public static void AddSpacer(HBoxContainer parent, int width)
    {
        var spacer = new Control { CustomMinimumSize = new Vector2(width, 0) };
        parent.AddChild(spacer);
    }
}

// Pull helpers into each editor via using static or just call them directly.
// Duplicating calls here keeps partial class constraints simple.
public partial class DateTimePropertyEditor
{
    private static SpinBox MakeSpinBox(HBoxContainer p, int min, int max, string tip)
        => EditorPropertyHelpers.MakeSpinBox(p, min, max, tip);
    private static void Separator(HBoxContainer p) => EditorPropertyHelpers.Separator(p);
    private static void AddSpacer(HBoxContainer p, int w) => EditorPropertyHelpers.AddSpacer(p, w);
}
public partial class DatePropertyEditor
{
    private static SpinBox MakeSpinBox(HBoxContainer p, int min, int max, string tip)
        => EditorPropertyHelpers.MakeSpinBox(p, min, max, tip);
    private static void Separator(HBoxContainer p) => EditorPropertyHelpers.Separator(p);
}
public partial class TimePropertyEditor
{
    private static SpinBox MakeSpinBox(HBoxContainer p, int min, int max, string tip)
        => EditorPropertyHelpers.MakeSpinBox(p, min, max, tip);
    private static void Separator(HBoxContainer p) => EditorPropertyHelpers.Separator(p);
    private static void AddSpacer(HBoxContainer p, int w) => EditorPropertyHelpers.AddSpacer(p, w);
}

#endif

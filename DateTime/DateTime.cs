using Godot;
using System;
using System.Globalization;

namespace GodotLib;

/// <summary>
/// Godot Resource wrapping System.DateTime.
/// Export this type on nodes/resources — the editor plugin renders a friendly date-time picker.
/// Serialized as ISO 8601 round-trip string ("O") so it survives save/load intact.
/// </summary>
[GlobalClass]
public partial class DateTime : Resource
{
    // ── Serialized backing field ─────────────────────────────────────────────

    [Export]
    public string Serialized
    {
        get => _serialized;
        set
        {
            if (_serialized == value) return;
            _serialized = value;
            EmitChanged();
        }
    }

    private string _serialized = System.DateTime.UtcNow.ToString("O");

    // ── .NET value accessor ──────────────────────────────────────────────────

    public System.DateTime Value
    {
        get => System.DateTime.Parse(_serialized, null, DateTimeStyles.RoundtripKind);
        set
        {
            var s = value.ToString("O");
            if (_serialized == s) return;
            _serialized = s;
            EmitChanged();
        }
    }

    // ── Convenience parts (read-only, computed) ──────────────────────────────

    public int Year    => Value.Year;
    public int Month   => Value.Month;
    public int Day     => Value.Day;
    public int Hour    => Value.Hour;
    public int Minute  => Value.Minute;
    public int Second  => Value.Second;
    public DateTimeKind Kind => Value.Kind;

    // ── Constructors ─────────────────────────────────────────────────────────

    public DateTime() { }

    public DateTime(System.DateTime dt) => Value = dt;

    public DateTime(int year, int month, int day,
                         int hour = 0, int minute = 0, int second = 0,
                         DateTimeKind kind = DateTimeKind.Utc)
        => Value = new System.DateTime(year, month, day, hour, minute, second, kind);

    // ── Operators & conversions ──────────────────────────────────────────────

    public static implicit operator System.DateTime(DateTime g)    => g.Value;
    public static implicit operator DateTime(System.DateTime dt)   => new(dt);

    public static DateTime operator +(DateTime g, TimeSpan t) => new(g.Value + t);
    public static DateTime operator -(DateTime g, TimeSpan t) => new(g.Value - t);
    public static TimeSpan      operator -(DateTime a, DateTime b) => a.Value - b.Value;

    // ── Utility ──────────────────────────────────────────────────────────────

    /// Returns a copy converted to UTC.
    public DateTime ToUtc()   => new(Value.Kind == DateTimeKind.Utc ? Value : Value.ToUniversalTime());

    /// Returns a copy converted to local time.
    public DateTime ToLocal() => new(Value.ToLocalTime());

    public override string ToString() => Value.ToString("u");
}

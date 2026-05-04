using Godot;
using System;

namespace GodotLib;

/// <summary>
/// Godot Resource wrapping System.DateOnly.
/// Use when time-of-day is irrelevant (calendar dates, birthdays, events, etc.).
/// Serialized as "yyyy-MM-dd".
/// </summary>
[GlobalClass]
public partial class Date : Resource
{
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

    private string _serialized = System.DateOnly.FromDateTime(System.DateTime.Today).ToString("yyyy-MM-dd");

    // ── .NET value accessor ──────────────────────────────────────────────────

    public System.DateOnly Value
    {
        get => System.DateOnly.ParseExact(_serialized, "yyyy-MM-dd");
        set
        {
            var s = value.ToString("yyyy-MM-dd");
            if (_serialized == s) return;
            _serialized = s;
            EmitChanged();
        }
    }

    // ── Convenience parts ────────────────────────────────────────────────────

    public int Year      => Value.Year;
    public int Month     => Value.Month;
    public int Day       => Value.Day;
    public DayOfWeek DayOfWeek => Value.DayOfWeek;

    // ── Constructors ─────────────────────────────────────────────────────────

    public Date() { }

    public Date(DateOnly d) => Value = d;

    public Date(int year, int month, int day) => Value = new System.DateOnly(year, month, day);

    public static Date Today => new(System.DateOnly.FromDateTime(System.DateTime.Today));

    // ── Operators & conversions ──────────────────────────────────────────────

    public static implicit operator System.DateOnly(Date g)   => g.Value;
    public static implicit operator Date(System.DateOnly d)   => new(d);

    /// Add days.
    public static Date operator +(Date g, int days) => new(g.Value.AddDays(days));
    public static Date operator -(Date g, int days) => new(g.Value.AddDays(-days));
    public static int           operator -(Date a, Date b)
        => a.Value.DayNumber - b.Value.DayNumber;

    // ── Utility ──────────────────────────────────────────────────────────────

    /// Combine with a TimeOnly to produce a DateTime.
    public System.DateTime ToDateTime(TimeOnly time, DateTimeKind kind = DateTimeKind.Utc)
        => Value.ToDateTime(time, kind);

    public override string ToString() => Value.ToString("D"); // "Monday, 1 January 2024"
}

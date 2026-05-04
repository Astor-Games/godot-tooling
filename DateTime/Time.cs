using Godot;
using System;

namespace GodotLib;

/// <summary>
/// Godot Resource wrapping System.TimeSpan.
/// Use for durations and offsets (cooldowns, delays, elapsed time, etc.).
/// Serialized as constant ("c") format: [-][d'.']hh':'mm':'ss['.'fffffff]
/// </summary>
[GlobalClass]
public partial class Time : Resource
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

    private string _serialized = TimeSpan.Zero.ToString("c");

    // ── .NET value accessor ──────────────────────────────────────────────────

    public TimeSpan Value
    {
        get => TimeSpan.Parse(_serialized);
        set
        {
            var s = value.ToString("c");
            if (_serialized == s) return;
            _serialized = s;
            EmitChanged();
        }
    }

    // ── Convenience parts ────────────────────────────────────────────────────

    public int    Days         => Value.Days;
    public int    Hours        => Value.Hours;
    public int    Minutes      => Value.Minutes;
    public int    Seconds      => Value.Seconds;
    public double TotalSeconds => Value.TotalSeconds;
    public double TotalMinutes => Value.TotalMinutes;
    public double TotalHours   => Value.TotalHours;
    public double TotalDays    => Value.TotalDays;
    public bool   IsNegative   => Value.Ticks < 0;

    // ── Constructors ─────────────────────────────────────────────────────────

    public Time() { }

    public Time(TimeSpan ts) => Value = ts;

    public Time(int days, int hours, int minutes, int seconds)
        => Value = new TimeSpan(days, hours, minutes, seconds);

    // ── Named constructors ───────────────────────────────────────────────────

    public static Time FromSeconds(double s) => new(TimeSpan.FromSeconds(s));
    public static Time FromMinutes(double m) => new(TimeSpan.FromMinutes(m));
    public static Time FromHours(double h)   => new(TimeSpan.FromHours(h));
    public static Time FromDays(double d)    => new(TimeSpan.FromDays(d));
    public static Time Zero                  => new(TimeSpan.Zero);

    // ── Operators & conversions ──────────────────────────────────────────────

    public static implicit operator TimeSpan(Time g)    => g.Value;
    public static implicit operator Time(TimeSpan ts)   => new(ts);

    public static Time operator +(Time a, Time b) => new(a.Value + b.Value);
    public static Time operator -(Time a, Time b) => new(a.Value - b.Value);
    public static Time operator *(Time g, double factor)   => new(g.Value * factor);
    public static Time operator -(Time g)                  => new(-g.Value);

    // ── Utility ──────────────────────────────────────────────────────────────

    public Time Abs() => new(Value.Duration());

    public override string ToString()
    {
        var ts = Value;
        if (ts.Days != 0)
            return $"{ts.Days}d {ts.Hours:D2}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
        if (ts.Hours != 0)
            return $"{ts.Hours}h {ts.Minutes:D2}m {ts.Seconds:D2}s";
        return $"{ts.Minutes}m {ts.Seconds:D2}s";
    }
}

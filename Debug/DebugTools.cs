using System;
using System.Collections.Generic;
using Godot;

namespace GodotLib.Debug;

public partial class DebugTools : Node
{
    private const string DebugCfgPath = "user://debug.cfg";
    private readonly Dictionary<Key, Action> actions = new();
    private ConfigFile config = new();

    public override void _Ready()
    {
        config.Load(DebugCfgPath);
    }

    public override void _Notification(int notification)
    {
        if (notification == NotificationWMCloseRequest || notification == NotificationExitTree )
        {
            SaveConfig();
        }
    }

    private void SaveConfig()
    {
        config.Save(DebugCfgPath);
    }

    protected void AddDebugShortcut(Action action, Key keycode, KeyModifierMask keyModifiers = 0)
    {
        // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
        var keycodeWithModifiers = keycode | (Key)keyModifiers;
        actions.Add(keycodeWithModifiers, action);
    }

    public override void _ShortcutInput(InputEvent evt)
    {
        if (evt is not InputEventKey eventKey || eventKey.Pressed) return;

        var keycodeWithModifiers = eventKey.GetKeycodeWithModifiers();
        if (!actions.TryGetValue(keycodeWithModifiers, out var action)) return;
        
        action?.Invoke();
        GetViewport().SetInputAsHandled();
    }

    protected T GetSavedConfig<[MustBeVariant] T>(string key, T defaultValue = default)
    {
        return config.GetValue("Debug", key, Variant.From(defaultValue)).As<T>();
    }

    protected void SaveConfig<[MustBeVariant]T>(string key, T value)
    {
        config.SetValue("Debug", key, Variant.From(value));   
        SaveConfig();
    }
}
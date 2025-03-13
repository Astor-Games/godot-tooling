using System.Collections.Generic;
using Godot;
using GodotLib.Util;

namespace GodotLib.Input;


[Icon("editor/icons/gamepad.svg")]
public partial class InputHelper : Node
{
    public static InputHelper Instance => autoload ??= NodeUtils.GetAutoload<InputHelper>();
    private static InputHelper autoload;
	
    [Signal]
    public delegate void InputModeChangedEventHandler(InputMode mode);

    private static InputMode inputMode;
    public static Vector2 MouseMotion { get; private set; }
    
    public override void _EnterTree()
    {
        //Process at the very last to make sure mouse input is cleaned after all uses
        ProcessPriority = int.MaxValue;
    }
    
    public override void _Process(double delta)
    {
        MouseMotion = Vector2.Zero;
    }

    public override void _Input(InputEvent evt)
    {
        switch (evt)
        {
            case InputEventMouseMotion mouseMotion:
                MouseMotion += mouseMotion.ScreenRelative;
                SwitchInputMode(InputMode.MouseAndKeyboard);
                break;
            
            case InputEventMouseButton or InputEventKey:
                SwitchInputMode(InputMode.MouseAndKeyboard);
                break;
            
            case InputEventJoypadButton or InputEventJoypadMotion:
                SwitchInputMode(InputMode.Joystick);
                break;
        }
    }
    
    public static InputMode GetInputMode(int device = 0)
    {
        // Only the first player can switch to mouse and keyboard
        return device == 0 ? inputMode : InputMode.Joystick;
    }

    private void SwitchInputMode(InputMode mode)
    {
        if (mode == inputMode) return;
        Print($"InputManager: Switching to {mode}");
        inputMode = mode;
        EmitSignalInputModeChanged(mode);
    }

    public static StringName DuplicateAction(string baseAction, int device)
    {
        if (device == 0) return baseAction;
    
        var newAction = $"{baseAction}_{device}";
        
        if (InputMap.GetActions().Contains(newAction)) return newAction;
            
        foreach (var evt in InputMap.ActionGetEvents(baseAction))
        {
            if (evt is InputEventJoypadButton or InputEventJoypadMotion)
            {
                if (!InputMap.HasAction(newAction))
                {
                    var baseDeadzone = InputMap.ActionGetDeadzone(baseAction);
                    InputMap.AddAction(newAction, baseDeadzone);
                }
    
                var newEvent = (InputEvent)evt.Duplicate();
                newEvent.Device = device;
    
                InputMap.ActionAddEvent(newAction, newEvent);
            }
        }
        
        return newAction;
    }
}
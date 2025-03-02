using System;
using Godot;
using static Godot.GD;

namespace GodotLib.Input;

[Icon("editor/icons/gamepad.svg")]
public partial class InputDevice : Node
{
    public event Action<InputMode> InputModeChanged; 
    
    public InputMode InputMode { get; private set; }
    public Vector2 MouseMotion { get; private set; }

    private int index;
    private string suffix;
    private bool CanUseMouse => index == 0;

    public InputDevice(int deviceIndex)
    {
        if (deviceIndex == -1)
        {
            PushError("Cannot set up DeviceInput for all devices");
            return;
        }

        index = deviceIndex;
        suffix = $"_device_{deviceIndex}";
        SwitchInputMode(InputMode.MouseAndKeyboard);
        
        //Process at the very last to make sure mouse input is cleaned after all uses
        ProcessPriority = int.MaxValue;
    }

    public override void _UnhandledInput(InputEvent evt)
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

    public override void _Process(double delta)
    {
       MouseMotion = Vector2.Zero;
    }

    public InputVector AddVector(StringName negativeX, StringName positiveX, StringName negativeY, StringName positiveY)
    {
        negativeX = DuplicateAction(negativeX);
        positiveX = DuplicateAction(positiveX);
        negativeY = DuplicateAction(negativeY);
        positiveY = DuplicateAction(positiveY);
        
        var vector = new InputVector(negativeX, positiveX, negativeY, positiveY);
        AddChild(vector);
        return vector;
    }

    public InputButton AddButton(StringName name)
    {
        name = DuplicateAction(name);
        var button = new InputButton(name);
        AddChild(button);
        return button;
    }
    
    public InputHoldButton AddHoldButton(StringName name, float longPressTime)
    {
        name = DuplicateAction(name);
        var button = new InputHoldButton(name, longPressTime);
        AddChild(button);
        return button;
    }

    public InputToggleButton AddToggleButton(StringName name)
    {
        name = DuplicateAction(name);
        var toggle = new InputToggleButton(name);
        AddChild(toggle);
        return toggle;
    }
    
    private StringName DuplicateAction(string baseAction)
    {
        if (index == 0) return baseAction;

        // Check if the action has been duplicated already
        var newAction = GetName(baseAction);
        if ( InputMap.GetActions().Contains(newAction)) return newAction;
        
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
                newEvent.Device = index;

                InputMap.ActionAddEvent(newAction, newEvent);
            }
        }

        return newAction;
    }

    private StringName GetName(string action)
    {
        if (index == 0)
        {
            return action;
        }

        return action + suffix;
    }

    private void SwitchInputMode(InputMode mode)
    {
        if (!CanUseMouse || mode == InputMode) return;
        
        Print($"Device {index}: Switching to {mode}");
        InputMode = mode;
        InputModeChanged?.Invoke(InputMode);
    }
}

public enum InputMode
{
    MouseAndKeyboard,
    Joystick
}

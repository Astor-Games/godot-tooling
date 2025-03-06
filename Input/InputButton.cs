using System;
using Godot;

namespace GodotLib.Input;

public partial class InputButton : Node
{
    [Signal] public delegate void PressedEventHandler();
        
    [Signal] public delegate void ReleasedEventHandler();

    public ButtonState State { get; private set; }
    
    private readonly StringName name;

    public InputButton(StringName name)
    {
        this.name = name;
    }
    
    public override void _Process(double delta)
    {
        if (Godot.Input.IsActionJustPressed(name))
        {
            State = ButtonState.Pressed;
            EmitSignalPressed();
            return;
        }
        
        if (Godot.Input.IsActionJustReleased(name))
        {
            State = ButtonState.Released;
            EmitSignalReleased();
            return;
        }
    }
}

public enum ButtonState
{
    Pressed,
    Released
}
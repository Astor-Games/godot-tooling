using System;
using Godot;

namespace GodotLib.Input;

public partial class InputButton : Node
{
    public event Action<ButtonState> StateChanged;
    public ButtonState State { get; private set; }
    
    private readonly StringName name;

    public InputButton(StringName name)
    {
        this.name = name;
    }
    
    public override void _Process(double delta)
    {
        var previousState = State;
        
        if (Godot.Input.IsActionJustReleased(name))
        {
            State = ButtonState.Released;
        }

        if (Godot.Input.IsActionPressed(name))
        {
            State = ButtonState.Pressed;
        }

        if (State != previousState)
        {
            StateChanged?.Invoke(State);
        }
    }
}

public enum ButtonState
{
    Pressed,
    Released
}
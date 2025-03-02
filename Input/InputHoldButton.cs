using System;
using Godot;

namespace GodotLib.Input;

public partial class InputHoldButton : Node
{
    public event Action<HoldButtonState> StateChanged;
    public HoldButtonState State { get; private set; }
    
    private readonly StringName name;
    private readonly float holdTime;

    private float heldTimeCounter;

    public InputHoldButton(StringName name, float holdTime)
    {
        this.name = name;
        this.holdTime = holdTime;
    }
    
    public override void _Process(double delta)
    {
        var previousState = State;
        
        if (Godot.Input.IsActionJustReleased(name))
        {
            if (previousState == HoldButtonState.Held) State = HoldButtonState.HoldReleased;
            else State = HoldButtonState.Released;
            heldTimeCounter = 0.0f;
        }

        if (Godot.Input.IsActionPressed(name))
        {
            heldTimeCounter += (float)delta;
            if (heldTimeCounter >= holdTime) State = HoldButtonState.Held;
            else State = HoldButtonState.Pressed;
        }

        if (State != previousState)
        {
            StateChanged?.Invoke(State);
        }
    }
}

public enum HoldButtonState
{
    Pressed,
    Held,
    Released,
    HoldReleased
}
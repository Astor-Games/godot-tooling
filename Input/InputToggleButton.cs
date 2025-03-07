using Godot;

namespace GodotLib.Input;

public partial class InputToggleButton : Node
{
    [Signal] public delegate void StateChangedEventHandler(bool active);
    
    public bool Active { get; private set; }
    
    private readonly StringName name;

    public InputToggleButton(StringName name)
    {
        this.name = name;
    }

    public override void _Process(double delta)
    {
        if (Godot.Input.IsActionJustPressed(name))
        {
            Active = !Active;
            EmitSignalStateChanged(Active);
        }
    }
}
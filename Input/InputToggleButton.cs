using Godot;

namespace GodotLib.Input;

public partial class InputToggleButton : Node
{
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
        }
    }
}
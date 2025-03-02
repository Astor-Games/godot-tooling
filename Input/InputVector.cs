using Godot;

namespace GodotLib.Input;

public partial class InputVector : Node
{
    public Vector2 Value { get; private set; }
    public float X => Value.X;
    public float Y => Value.Y;

    private readonly StringName negativeX;
    private readonly StringName positiveX;
    private readonly StringName negativeY;
    private readonly StringName positiveY;

    private bool useMouse;
    private InputMode inputMode;
    
    public InputVector(StringName negativeX, StringName positiveX, StringName negativeY, StringName positiveY, bool useMouse = false)
    {
        this.negativeX = negativeX;
        this.positiveX = positiveX;
        this.negativeY = negativeY;
        this.positiveY = positiveY;
        this.useMouse = useMouse;
    }

    public override void _Process(double delta)
    {
        // TODO fix or workaround https://github.com/godotengine/godot/issues/93396
        Value = Godot.Input.GetVector(negativeX, positiveX, negativeY, positiveY);
    }

    
}
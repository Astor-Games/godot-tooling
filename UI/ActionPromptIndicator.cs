namespace GodotLib.UI;

[GlobalClass]
public partial class ActionPromptIndicator : WorldSpaceIndicator
{
    [Export] public StringName InputAction
    {
        get => inputAction;
        set
        {
            inputAction = value;
            promptInput?.Set("action", value);
        }
    }

    private StringName inputAction;
    private Control promptInput;

    public override void _Ready()
    {
        promptInput = GetNode<Control>("%Content/Texture");
        base._Ready();
    }
    
    public override void _EnterTree(){
        base._EnterTree();
        InputAction = inputAction; // In case it was set before _ready
    }
}
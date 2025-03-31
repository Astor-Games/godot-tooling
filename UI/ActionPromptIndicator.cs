using Turtles;

namespace GodotLib.UI;

[GlobalClass]
public partial class ActionPromptIndicator : WorldSpaceIndicator
{
    [Export] public StringName Input
    {
        get => input;
        set
        {
            input = value;
            promptInput?.Set("action", value);
        }
    }

    private StringName input;
    private Control promptInput;

    public override void _Ready()
    {
        promptInput = GetNode<Control>("%Content/Texture");
        base._Ready();
    }
    
    public override void _EnterTree(){
        base._EnterTree();
        Input = input; // In case it was set before _ready
    }
}
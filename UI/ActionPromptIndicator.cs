namespace GodotLib.UI;

[GlobalClass]
public partial class ActionPromptIndicator : WorldSpaceIndicator
{
    [Export]
    public StringName Input
    {
        get;
        set
        {
            field = value;
            promptInput?.Set("action", field);
        }
    }
    private Control promptInput;

    public override void _Ready()
    {
        promptInput = GetNode<Control>("%Content/Texture");
        base._Ready();
    }
    
    public override void _EnterTree(){
        base._EnterTree();
        promptInput?.Set("action", Input); // In case it was set before _ready
    }
}
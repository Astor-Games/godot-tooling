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

    [Export]  public string Prompt
    {
        get => prompt;
        set
        {
            prompt = value;
            if (promptLabel != null) promptLabel.Text = value;
        }
    }

    private StringName inputAction;
    private string prompt;
    
    private Control promptInput;
    private Label promptLabel;

    public override void _Ready()
    {
        promptInput = GetNode<Control>("%Content/Texture");
        promptLabel = GetNode<Label>("%Content/OnlyOnScreen/Label");
        base._Ready();
    }
    
    public override void _EnterTree(){
        // In case these were set before _ready
        Prompt = prompt;
        InputAction = inputAction;
    }
}
namespace GodotLib.UI;

[GlobalClass, Icon("res://addons/godot_lib/Editor/Icons/circle_dashed.svg")]
public partial class Fader : Control
{
    [Signal]
    delegate void FadeCompletedEventHandler(bool visible);
    
    private Tween fadeInTween, fadeOutTween;
    
    
    [Export] public float FadeDuration = 0.1f;

    [Export] public bool BeVisible
    {
        get => __beVisible;
        set
        {
            __beVisible = value;
            UpdateVis();
        }
    }
    private bool __beVisible;

    public override void _EnterTree()
    {
       UpdateVis();
    }
    
    private void UpdateVis()
    {
        if (!IsInsideTree()) return;

        if (!Visible && BeVisible)
            FadeIn();

        else if (Visible && !BeVisible) 
            FadeOut();
    }

    private void FadeIn()
    {
        fadeOutTween?.Kill();
        Visible = true;
        
        if (fadeInTween?.IsRunning() == true) return;

        fadeInTween = CreateTween();
        fadeInTween.TweenProperty(this, "modulate:a", 1, FadeDuration).From(0);
        fadeInTween.Finished += () => EmitSignalFadeCompleted(true);
    }

    private void FadeOut()
    {
        fadeInTween?.Kill();
        if (!Visible || fadeOutTween?.IsRunning() == true) return;
        
        fadeOutTween = CreateTween();
        fadeOutTween.TweenProperty(this, "modulate:a", 0, FadeDuration).From(1);
        fadeOutTween.Finished += () =>
        {
            Visible = false;
            EmitSignalFadeCompleted(false);
        };
    }
}
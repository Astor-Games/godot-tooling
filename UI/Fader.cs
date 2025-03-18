namespace GodotLib.UI;

[GlobalClass, Icon("res://addons/godot_lib/Editor/Icons/circle_dashed.svg")]
public partial class Fader : Control
{
    [Signal]
    delegate void FadeCompletedEventHandler(bool visible);
    
    private Tween fadeInTween, fadeOutTween;
    
    [Export] public bool BeVisible;
    [Export] private double fadeDuration = 0.1f;

    public override void _Process(double delta)
    {
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
        fadeInTween.TweenProperty(this, "modulate:a", 1, fadeDuration).From(0);
        fadeInTween.Finished += () => EmitSignalFadeCompleted(true);
    }

    private void FadeOut()
    {
        fadeInTween?.Kill();
        if (!Visible || fadeOutTween?.IsRunning() == true) return;
        
        fadeOutTween = CreateTween();
        fadeOutTween.TweenProperty(this, "modulate:a", 0, fadeDuration).From(1);
        fadeOutTween.Finished += () =>
        {
            Visible = false;
            EmitSignalFadeCompleted(false);
        };
    }
}
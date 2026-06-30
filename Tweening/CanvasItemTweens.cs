using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class CanvasItemTweens
{
    private static readonly NodePath modulate = new(CanvasItem.PropertyName.Modulate);
    private static readonly NodePath modulateAlpha = new($"{CanvasItem.PropertyName.Modulate}:a");
    private static readonly NodePath selfModulate = new(CanvasItem.PropertyName.SelfModulate);
    private static readonly NodePath selfModulateAlpha = new($"{CanvasItem.PropertyName.SelfModulate}:a");

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<CanvasItem>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Modulate(Color color, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, modulate, color, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Alpha(float alpha, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, modulateAlpha, alpha, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener SelfModulate(Color color, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, selfModulate, color, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener SelfAlpha(float alpha, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, selfModulateAlpha, alpha, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public Tweener GradientModulate(Gradient gradient, float duration)
        {
            return tween.Tween.TweenMethod(Callable.From<float>(t => tween.Node.Modulate = gradient.Sample(t)), 0f, 1f, duration);
        }
    }
}

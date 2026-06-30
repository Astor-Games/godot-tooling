using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using Range = Godot.Range;

namespace GodotLib.Tweening;

public static class RangeTweens
{
    private static readonly NodePath value = new(Range.PropertyName.Value);
    private static readonly NodePath ratio = new(Range.PropertyName.Ratio);

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<Range>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Value(float value, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, RangeTweens.value, value, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Ratio(float ratio, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, RangeTweens.ratio, ratio, duration);
        }
    }
}

using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class LabelTweens
{
    private static readonly NodePath visibleRatio = new(Label.PropertyName.VisibleRatio);

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<Label>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener VisibleRatio(float ratio, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, visibleRatio, ratio, duration);
        }
    }
}

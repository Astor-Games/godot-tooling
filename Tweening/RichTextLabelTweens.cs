using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class RichTextLabelTweens
{
    private static readonly NodePath visibleRatio = new(RichTextLabel.PropertyName.VisibleRatio);

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<RichTextLabel>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener VisibleRatio(float ratio, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, visibleRatio, ratio, duration);
        }
    }
}

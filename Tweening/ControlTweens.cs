using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class ControlTweens
{
    private static readonly NodePath size = new(Control.PropertyName.Size);

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<Control>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Size(Vector2 size, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, ControlTweens.size, size, duration);
        }
    }
}

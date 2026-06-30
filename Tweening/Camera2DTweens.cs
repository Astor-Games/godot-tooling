using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class Camera2DTweens
{
    private static readonly NodePath zoom = new(Camera2D.PropertyName.Zoom);

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<Camera2D>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Zoom(Vector2 zoom, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, Camera2DTweens.zoom, zoom, duration);
        }
    }
}

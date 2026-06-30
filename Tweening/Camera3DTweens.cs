using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class Camera3DTweens
{
    private static readonly NodePath fov = new(Camera3D.PropertyName.Fov);

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<Camera3D>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener FieldOfView(float fov, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, Camera3DTweens.fov, fov, duration);
        }
    }
}

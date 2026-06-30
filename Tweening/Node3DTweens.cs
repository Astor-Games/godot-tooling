using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class Node3DTweens
{
    private static readonly NodePath globalPosition = new(Node3D.PropertyName.GlobalPosition);
    private static readonly NodePath localPosition = new(Node3D.PropertyName.Position);
    private static readonly NodePath rotation = new(Node3D.PropertyName.Rotation);
    private static readonly NodePath scale = new(Node3D.PropertyName.Scale);
    
    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<Node3D>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener GlobalPosition(Vector3 position, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, globalPosition, position, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener LocalPosition(Vector3 position, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, localPosition, position, duration);
        }
        
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Rotation(Vector3 eulerRadians, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, rotation, eulerRadians, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Scale(Vector3 scale, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, Node3DTweens.scale, scale, duration);
        }
    }
}
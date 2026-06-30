using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class Node2DTweens
{
    private static readonly NodePath globalPosition = new(Node2D.PropertyName.GlobalPosition);
    private static readonly NodePath localPosition = new(Node2D.PropertyName.Position);
    private static readonly NodePath rotation = new(Node2D.PropertyName.Rotation);
    private static readonly NodePath scale = new(Node2D.PropertyName.Scale);
    
    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<Node2D>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener GlobalPosition(Vector2 position, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, globalPosition, position, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener LocalPosition(Vector2 position, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, localPosition, position, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Rotation(float radians, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, rotation, radians, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Scale(Vector2 scale, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, Node2DTweens.scale, scale, duration);
        }
    }
}
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class TweenExtensions
{
    extension(Tween tween)
    {
        public static TweenFor<T> For<T>(T node) where T : Node
        {
            return new TweenFor<T>(node.CreateTween(), node);
        }
    }
    
    extension<TSelf>(TSelf tween) where TSelf : struct, ITween
    {
        [MethodImpl(AggressiveInlining)]
        public TSelf SetEase(Tween.EaseType ease, Tween.TransitionType transition)
        {
            tween.Tween.SetEase(ease);
            tween.Tween.SetTrans(transition);
            return tween;
        }

        [MethodImpl(AggressiveInlining)]
        public TSelf Append()
        {
            tween.Tween.Chain();
            return tween;
        }

        [MethodImpl(AggressiveInlining)]
        public TSelf Interval(double interval)
        {
            tween.Tween.TweenInterval(interval);
            return tween;
        }

        [MethodImpl(AggressiveInlining)]
        public TSelf Await(Signal signal)
        {
            tween.Tween.TweenAwait(signal);
            return tween;
        }

        [MethodImpl(AggressiveInlining)]
        public TSelf Callback(Callable callback)
        {
            tween.Tween.TweenCallback(callback);
            return tween;
        }

        [MethodImpl(AggressiveInlining)]
        public TSelf Callback(Action callback)
        {
            tween.Tween.TweenCallback(Callable.From(callback));
            return tween;
        }

        [MethodImpl(AggressiveInlining)]
        public TSelf Do<[MustBeVariant]T>(Action<float> callback, T from, T to, float duration)
        {
            tween.Tween.TweenMethod(Callable.From(callback), Variant.From(from), Variant.From(to), duration);
            return tween;
        }
    }
    
    extension(PropertyTweener tweener) 
    {
        // Misc
        
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener SetEase(Tween.EaseType ease, Tween.TransitionType transition)
        {
            tweener.SetEase(ease);
            tweener.SetTrans(transition);
            return tweener;
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener SetCurve(Curve curve)
        {
            tweener.SetTrans(Tween.TransitionType.Linear);
            tweener.SetCustomInterpolator(Callable.From<float>(t => curve.Sample(t)));
            return tweener;
        }
    }
    
    extension(MethodTweener tweener) 
    {
        // Misc
        
        [MethodImpl(AggressiveInlining)]
        public MethodTweener SetEase(Tween.EaseType ease, Tween.TransitionType transition)
        {
            tweener.SetEase(ease);
            tweener.SetTrans(transition);
            return tweener;
        }
    }
}
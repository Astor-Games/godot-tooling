namespace GodotLib.Tweening;
public interface ITween
{
    Tween Tween { get; }
}

public interface ITweenFor<out TNode> : ITween where TNode : Node
{
    TNode Node { get; }
}

public readonly struct TweenFor<T>(Tween tween, T node) : ITweenFor<T> where T : Node
{
    public Tween Tween { get; } = tween;
    public T Node { get; } = node;
        
    public TweenFor<TOther> For<TOther>(TOther other) where TOther : Node
    {
        return new TweenFor<TOther>(Tween, other);
    }
}
using Collections.Pooled;
using NativeCollections;

namespace GodotLib.Debug;

using Godot;

public abstract class SpanRenderer<T> : IPropertyTreeRenderer
{
    public abstract void Render(TreeItem rootItem, object property, RenderingParameters parameters);

    protected static void RenderSpan(ReadOnlySpan<T> span, TreeItem rootItem, RenderingParameters parameters)
    {
        var length = span.Length;
        switch (length)
        {
            case 0:
                rootItem.SetText(1, "No elements");
                rootItem.SetCustomColor(1, RendererConsts.DefaultValueColor);
                break;
            case 1:
                rootItem.SetText(1, "1 element");
                break;
            default:
                rootItem.SetText(1, $"{length} elements");
                break;
        }
        
        using var names = new PooledList<string>(length);
        using var values = new PooledList<object>(length);
        
        for (var i = 0; i < length; i++)
        {

            names.Add(i.ToString());
            values.Add(span[i]);
        }
        PropertyTreeRendering.Render(rootItem, names.Span, values.Span, parameters);
    }
}
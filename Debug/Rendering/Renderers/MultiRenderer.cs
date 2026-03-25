using NativeCollections;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(Multi<>))]
public class MultiRenderer<T> : SpanRenderer<T> where T : struct
{
    public override void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var span = ((Multi<T>)property).AsSpan();
        RenderSpan(span, rootItem, parameters);
    }
}

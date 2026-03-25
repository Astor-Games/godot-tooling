using NativeCollections;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(NativeArray<>))]
public class NativeArrayRenderer<T> : SpanRenderer<T> where T : unmanaged
{
    public override void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var span = ((NativeArray<T>)property).AsSpan();
        RenderSpan(span, rootItem, parameters);
    }
}
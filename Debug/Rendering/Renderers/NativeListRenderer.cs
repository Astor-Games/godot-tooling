using NativeCollections;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(NativeList<>))]
public class NativeListRenderer<T> : SpanRenderer<T> where T : unmanaged, IEquatable<T>
{
    public override void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var span = ((NativeList<T>)property).AsSpan();
        RenderSpan(span, rootItem, parameters);
    }
}
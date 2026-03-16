using NativeCollections;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(NativeArray<>), typeof(NativeList<>), typeof(Multi<>))]
public class UnsafeCollectionRenderer<T> : IPropertyTreeRenderer where T : unmanaged, IEquatable<T>
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        Span<T> data;
        switch (property)
        {
            case NativeArray<T> array:
                data = array.AsSpan();
                break;
            case NativeList<T> list:
                data = list.AsSpan();
                break;
            case Multi<T> multi:
                data = multi.AsSpan();
                break;
            default:
                return;
        }

        rootItem.SetText(1, $"{data.Length} elements");

        for (var i = 0; i < data.Length; i++)
        {
            var item = data[i];
            PropertyTreeRendering.Render(rootItem, i, item,i.ToString(), parameters);
        }
    }
}
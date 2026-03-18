using System.Collections;
using Collections.Pooled;

namespace GodotLib.Debug;

public class ListRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var list = (IList)property;
        
        switch (list.Count)
        {
            case 0:
                rootItem.SetText(1, "No elements");
                rootItem.SetCustomColor(1, RendererConsts.DefaultValueColor);
                break;
            case 1:
                rootItem.SetText(1, "1 element");
                break;
            default:
                rootItem.SetText(1, $"{list.Count} elements");
                break;
        }
        
        using var names = new PooledList<string>(list.Count);
        using var values = new PooledList<object>(list.Count);
        
        for (var i = 0; i < list.Count; i++)
        {

            names.Add(i.ToString());
            values.Add(list[i]);
        }

        PropertyTreeRendering.Render(rootItem, names.Span, values.Span, parameters);
    }
}
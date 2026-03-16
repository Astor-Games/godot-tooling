using System.Collections;

namespace GodotLib.Debug;

public class ListRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var list = (IList)property;


        if (list.Count == 0)
        {
            rootItem.SetText(1, "No elements");
            rootItem.SetCustomColor(1, RendererConsts.DefaultValueColor);
        } 
        else if (list.Count == 1)
        {
            rootItem.SetText(1, "1 element");
        }
        else
        {
            rootItem.SetText(1, $"{list.Count} elements");
        }
        
        var existingChildren = rootItem.GetChildren();

        //clean up old list items
        for (var i = list.Count; i < existingChildren.Count; i++)
        {
            existingChildren[i].Free();
        }
        
        //update / add list items
        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];
            PropertyTreeRendering.Render(rootItem, i, item,i.ToString(), parameters);
        }
    }
}
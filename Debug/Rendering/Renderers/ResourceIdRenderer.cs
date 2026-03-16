using static GodotLib.Debug.RendererConsts;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(ResourceId))]
public class ResourceIdRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var rid = (ResourceId)property;
        
        if (rid.IsValid)
        {
            rootItem.SetText(1, rid.Id.ToString());
        }
        else
        {
            rootItem.SetText(1, "Invalid");
            rootItem.SetCustomColor(1, ErrorColor);
        }
    }
}

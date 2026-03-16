using GodotLib.Util;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(Type))]
public class TypeRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var vector = (Type)property;
        rootItem.SetText(1, vector.GetHumanReadableName());
    }
}

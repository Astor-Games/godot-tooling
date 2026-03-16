namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(Vector3))]
public class Vector3Renderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var vector = (Vector3)property;
        rootItem.SetText(1, vector.ToString("F2"));
    }
}

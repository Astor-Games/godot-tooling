namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(Vector2))]
public class Vector2Renderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var vector = (Vector2)property;
        rootItem.SetText(1, vector.ToString("F2"));
    }
}

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(Color))]
public class ColorRenderer : IPropertyTreeRenderer
{
    private static readonly Texture2D colorCircle = Load<Texture2D>("uid://bb3jtvmjwlnr0");
    
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var color = (Color)property;

        if (parameters.IsNew)
        {
            rootItem.SetIcon(1, colorCircle);
            rootItem.SetIconMaxWidth(1, 12);
        }
        rootItem.SetText(1, $"{color.R8}, {color.G8}, {color.B8}, {color.A8}");
        rootItem.SetIconModulate(1, color);
    }
}

using GodotLib.Util;

namespace GodotLib.Debug;


[PropertyRenderer(typeof(Transform2D))]
public class Transform2DRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var transform2D = (Transform2D)property;
        
        rootItem.CreateOrGetChild(0, out var row1);
        rootItem.CreateOrGetChild(1, out var row2);
        rootItem.CreateOrGetChild(2, out var row3);
        
        row1.SetText(0, "Position");
        row1.SetText(1, transform2D.Origin.ToString());
        row2.SetText(0, "Rotation");
        row2.SetText(1, $"{Mathf.RadToDeg(transform2D.Rotation)}°");
        row3.SetText(0, "Scale");
        row3.SetText(1, transform2D.Scale.ToString());
    }
}
using GodotLib.Util;
using static Godot.Mathf;

namespace GodotLib.Debug;


[PropertyRenderer(typeof(Basis))]
public class BasisRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var basis = (Basis)property;
        var euler = basis.GetEuler();
        
        rootItem.CreateOrGetChild(0, out var row1);
        rootItem.CreateOrGetChild(1, out var row2);
        
        row1.SetText(0, "Rotation");
        row1.SetText(1, $"{RadToDeg(euler.X):F1}°, {RadToDeg(euler.Y):F1}°, {RadToDeg(euler.Z):F1}°");
        row2.SetText(0, "Scale");
        row2.SetText(1, basis.Scale.ToString("F2"));
    }
}
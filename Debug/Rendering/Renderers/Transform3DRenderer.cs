using GodotLib.Util;
using static Godot.Mathf;

namespace GodotLib.Debug;


[PropertyRenderer(typeof(Transform3D))]
public class Transform3DRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var transform3D = (Transform3D)property;
        var euler = transform3D.Basis.GetEuler();
        
        rootItem.CreateOrGetChild(0, out var row1);
        rootItem.CreateOrGetChild(1, out var row2);
        rootItem.CreateOrGetChild(2, out var row3);
        
        row1.SetText(0, "Position");
        row1.SetText(1, transform3D.Origin.ToString("F2"));
        row2.SetText(0, "Rotation");
        row2.SetText(1, $"{RadToDeg(euler.X):F1}°, {RadToDeg(euler.Y):F1}°, {RadToDeg(euler.Z):F1}°");
        row3.SetText(0, "Scale");
        row3.SetText(1, transform3D.Basis.Scale.ToString("F2"));
    }
}
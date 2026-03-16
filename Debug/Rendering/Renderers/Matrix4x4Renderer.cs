using GodotLib.Util;

namespace GodotLib.Debug;

using Godot;
using System.Numerics;


[PropertyRenderer(typeof(Matrix4x4))]
public class Matrix4x4Renderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        if (parameters.IsNew)
            rootItem.Collapsed = true;

        var matrix4X4 = (Matrix4x4)property;
        rootItem.SetTooltipText(0, matrix4X4.ToString());

        // Render each row of the matrix as a child item
        rootItem.CreateOrGetChild(0, out var row1);
        rootItem.CreateOrGetChild(1, out var row2);
        rootItem.CreateOrGetChild(2, out var row3);
        rootItem.CreateOrGetChild(3, out var row4);
        
        row1.SetText(1, $"[{matrix4X4.M11:F2}, {matrix4X4.M12:F2}, {matrix4X4.M13:F2}, {matrix4X4.M14:F2}]");
        row2.SetText(1, $"[{matrix4X4.M21:F2}, {matrix4X4.M22:F2}, {matrix4X4.M23:F2}, {matrix4X4.M24:F2}]");
        row3.SetText(1, $"[{matrix4X4.M31:F2}, {matrix4X4.M32:F2}, {matrix4X4.M33:F2}, {matrix4X4.M34:F2}]");
        row4.SetText(1, $"[{matrix4X4.M41:F2}, {matrix4X4.M42:F2}, {matrix4X4.M43:F2}, {matrix4X4.M44:F2}]");
    }
}
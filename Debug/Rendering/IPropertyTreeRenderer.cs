namespace GodotLib.Debug;

using Godot;

public interface IPropertyTreeRenderer
{
    void Render(TreeItem rootItem, object property, RenderingParameters parameters);
}

public static class RendererConsts
{
    public static readonly Color ErrorColor = Color.Color8(219, 51, 51);
    public static readonly Color WarningColor = Color.Color8(243, 204, 89);
    public static readonly Color DefaultValueColor = Color.Color8(95, 100, 103);
}
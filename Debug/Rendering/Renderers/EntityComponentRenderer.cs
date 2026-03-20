using AstorGames.EcsTools;
using Turtles.Core;
using static GodotLib.Debug.RendererConsts;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(DebuggerComponent))]
public class EntityComponentRenderer : IPropertyTreeRenderer
{
    private static readonly Texture2D linkIcon = Load<Texture2D>("uid://kuiptts1ywp4");
    
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var component = (DebuggerComponent)property;
        

        if (component.IsDisabled)
        {
            rootItem.SetText(0, $"{component.Name} (disabled)");
            rootItem.SetCustomColor(0, DisabledColor);
        }
        else
        {
            rootItem.SetText(0, component.Name);
            rootItem.ClearCustomColor(0);
        }
        
        PropertyTreeRendering.RenderFields(rootItem, component.Value, parameters);
    }
}
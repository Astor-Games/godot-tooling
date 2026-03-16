using AstorGames.EcsTools;
using Turtles.Core;
using static GodotLib.Debug.RendererConsts;

namespace GodotLib.Debug;

using Godot;

[PropertyRenderer(typeof(EntityId))]
public class EntityIdRenderer : IPropertyTreeRenderer
{
    private static readonly Texture2D linkIcon = Load<Texture2D>("uid://kuiptts1ywp4");
    
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var entityId = (EntityId)property;
        
        if (parameters.IsNew || rootItem.GetButtonById(1, 0) == -1)
        {
            rootItem.AddButton(1, linkIcon);
        }
        
        if (entityId == EntityId.None)
        {
            rootItem.SetText(1, "None");
            rootItem.SetCustomColor(1, DefaultValueColor);
            rootItem.SetButtonColor(1, 0, DefaultValueColor);
            rootItem.SetButtonDisabled(1, 0 , true);
            return;
        }
        
        if (!entityId.IsValid())
        {
            rootItem.SetText(1, $"Invalid entity ({entityId.Id})");
            rootItem.SetCustomColor(1, ErrorColor);
            rootItem.SetButtonColor(1, 0, DefaultValueColor);
            rootItem.SetButtonDisabled(1, 0 , true);
            return;
        }
        
        if (!entityId.IsLoaded())
        {
            rootItem.SetText(1, $"Unloaded entity ({entityId.Id})");
            rootItem.SetCustomColor(1, WarningColor);
            return;
        }
        
        rootItem.SetText(1, entityId.ToString());
        rootItem.SetMeta(EntityDebuggerCommon.IdKey, entityId.Raw);
        rootItem.ClearCustomBgColor(1);
        
        foreach (var child in rootItem.GetChildren())
        {
            child.Free();
        }
    }
}
using GodotLib.Debug;

namespace Turtles.addons.godot_lib.UI;

[Tool]
public partial class ViewportDisplay : MeshInstance3D
{
    [Export] private int surfaceIndex = 0;    
    private SubViewport viewport;
    
    public override void _EnterTree()
    {
        var material = GetActiveMaterial(surfaceIndex) as BaseMaterial3D;
        Assertions.AssertNotNull(material);
        
        viewport = GetNode<SubViewport>("Viewport");
        material.AlbedoTexture = viewport.GetTexture();
    }
}

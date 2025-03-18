using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotLib.ProjectConstants;

namespace GodotLib.Util;

public static class NodeUtils
{
    public static T GetAutoload<T>() where T : Node
    {
        var tree = (SceneTree)Engine.GetMainLoop();
        return tree.Root.GetNode<T>($"{typeof(T).Name}");
    }
    
    public static IEnumerable<T> FindChildren<[MustBeVariant] T>(this Node node, bool recursive = false) where T : Node
    {
        // this should work but doesn't because of https://github.com/godotengine/godot/issues/85690
        // return node.FindChildren("", nameof(T), recursive).Cast<T>();
        
        return node.FindChildren("", nameof(Node), recursive).OfType<T>();
    }

    public static T GetAncestor<T>(this Node node)
    {
        var parent = node.GetParent();
        while (parent != null)
        {
            if (parent is T pt) return pt;
            parent = parent.GetParent();
        }
        return default;
    }
    
    public static void SetCullMaskValue(this Camera3D camera, RenderLayers3D layer, bool value)
    {
        camera.CullMask = SetMaskValue(camera.CullMask, (uint)layer, value);
    }

    private static uint SetMaskValue(uint mask, uint layer, bool value)
    {
        if (value)
            mask |= layer; // Set the bit
        else
            mask &= ~layer; // Clear the bit

        return mask;
    }
}

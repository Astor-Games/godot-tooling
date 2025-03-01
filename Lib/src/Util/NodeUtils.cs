using System.Collections.Generic;
using System.Linq;
using Godot;

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
}

using System.Collections.Generic;

namespace GodotLib.Pooling;

public class NodePool<T> where T : Node, Resettable
{
    private readonly PackedScene scene;
    private readonly Queue<T> pool = new();
    private readonly Node parent;

    public NodePool(PackedScene scene, Node parent, int preloadCount = 0)
    {
        this.scene = scene;
        this.parent = parent;
        Preload(preloadCount);
    }

    private void Preload(int count)
    {
        for (var i = 0; i < count; i++)
        {
            var instance = CreateInstance();
            pool.Enqueue(instance);
        }
    }

    private T CreateInstance()
    {
        var instance = scene.Instantiate<T>();
        return instance;
    }

    public T Take()
    {
        var instance = pool.Count > 0 ? pool.Dequeue() : CreateInstance();
        parent.AddChild(instance);
        return instance;
    }

    public void Return(T instance)
    {
        try
        {
            instance.Reset();
            parent.RemoveChild(instance);
            pool.Enqueue(instance);
        }
        catch (Exception e)
        {
            PushError(e);
            instance.QueueFree();
        }
    }
}
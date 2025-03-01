using Godot;

namespace GodotLib.Util;

public class Ref<T> where T : Resource
{
    public readonly string Path;

    public Ref(string path)
    {
        Path = path;
    }

    public T Load() => GD.Load<T>(Path);
}
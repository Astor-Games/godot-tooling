namespace GodotLib.Util;

public static class EngineUtils
{
    public static SceneTree SceneTree => sceneTree ??= (SceneTree)Engine.GetMainLoop();
    private static SceneTree sceneTree;
}
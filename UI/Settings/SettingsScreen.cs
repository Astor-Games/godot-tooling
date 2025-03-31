using GodotLib.ProjectConstants;
using Turtles;

namespace GodotLib.UI.Settings;

[GlobalClass]
public partial class SettingsScreen : Control
{
    private static string scenePath = "uid://c6onjm3wu6yo8";
    
    [Export] private PackedScene settingsPanel;
    private TabContainer categoriesContainer;

    private static SettingsScreen instance;

    public override void _Ready()
    {
        categoriesContainer = GetNode<TabContainer>("%CategoriesContainer");
        Load(new TurtlesSettings());
    }

    public override void _Input(InputEvent evt)
    {
        if (evt.IsActionReleased(InputActions.Exit))
            Close();
    }

    public static void Open()
    {
        if (instance == null)
        {
            var sceneTree = (SceneTree)Engine.GetMainLoop();
            var scene = GD.Load<PackedScene>(scenePath);
            instance = scene.Instantiate<SettingsScreen>();
            sceneTree.Root.AddChild(instance);
        }
    }

    public static void Close()
    {
        instance.Hide();
        instance.QueueFree();
        instance = null;
    }

    private void Load(Settings settings)
    {
        settings.Load();
        
        foreach (var category in settings.GetCategories())
        {
            Log($"Loading {category} settings");
            var panel = settingsPanel.Instantiate<SettingsPanel>();
            categoriesContainer.AddChild(panel);
            panel.Name = category;
            panel.Load(settings.GetSettings(category));
        }
        
        //categoriesContainer.TabChanged += _ => settings.Save();
        TreeExiting += settings.Save;
    }
}

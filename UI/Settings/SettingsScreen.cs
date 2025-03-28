using Turtles;

namespace GodotLib.UI.Settings;

[GlobalClass]
public partial class SettingsScreen : Control
{
    [Export] private PackedScene settingsPanel;
    private TabContainer categoriesContainer;

    public override void _Ready()
    {
        categoriesContainer = GetNode<TabContainer>("CategoriesContainer");
        
        LoadSettings(new TurtlesSettings());
    }

    public void LoadSettings(Settings settings)
    {
        settings.Load();
        foreach (var category in settings.GetCategories())
        {
            var panel = settingsPanel.Instantiate<SettingsPanel>();
            categoriesContainer.AddChild(panel);
            panel.Name = category;
            panel.Load(settings.GetSettings(category));
        }
        categoriesContainer.TabChanged += tab => settings.Save();
    }
}

using System.Collections.Generic;

namespace GodotLib.UI.Settings;

public abstract class Settings
{
    private readonly string settingsPath = "user://settings.cfg";
    
    private readonly ConfigFile settingsFile = new();
    
    private readonly Dictionary<StringName, Dictionary<StringName, Setting>> settingsByCategory = new();

    protected abstract void Init();
    
    public void Load()
    {
        Init();
        settingsFile.Load(settingsPath);
        
        foreach (var category in settingsFile.GetSections())
        {
            var settings = settingsByCategory[category];
            foreach (var key in settingsFile.GetSectionKeys(category))
            {
                if (settings.TryGetValue(key, out var setting))
                {
                    setting.Value = settingsFile.GetValue(category, key, setting.DefaultValue);
                }
                else
                {
                    PushError($"Found unknown setting: {category}/{key}");
                }
            }
        }
    }

    public void Save()
    {
        foreach (var category in GetCategories())
        foreach (var setting in GetSettings(category))
        {
            settingsFile.SetValue(category, setting.Key, setting.Value);
        }

        settingsFile.Save(settingsPath);
    }

    public IEnumerable<StringName> GetCategories()
    {
        return settingsByCategory.Keys;
    }

    public IEnumerable<Setting> GetSettings(StringName category)
    {
        return settingsByCategory[category].Values;
    }

    protected void Register(StringName category, IEnumerable<Setting> settingList)
    {
        if (!settingsByCategory.TryGetValue(category, out var settings))
        {
            settings = new Dictionary<StringName, Setting>();
            settingsByCategory.Add(category, settings);
        }

        foreach (var setting in settingList)
        {
            if (!settings.TryAdd(setting.Key, setting))
            {
                PushError($"Duplicated setting found: {category}/{setting.Key}");
            }
        }
    }
}
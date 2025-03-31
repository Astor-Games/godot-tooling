using System.Collections.Generic;
using GodotLib.Util;

namespace GodotLib.UI.Settings;

public partial class Settings : Node
{
    private readonly string settingsPath = "user://settings.cfg";
    
    private readonly ConfigFile settingsFile = new();
    
    private readonly Dictionary<StringName, Dictionary<StringName, Setting>> settingsByCategory = new();

    public static Settings Instance => instance ??= NodeUtils.GetAutoload<Settings>();
    private static Settings instance;

    public override void _EnterTree()
    {
        instance = this;
        RegisterSettings();
        settingsFile.Load(settingsPath);
        
        foreach (var category in settingsFile.GetSections())
        {
            var settings = settingsByCategory[category];
            foreach (var key in settingsFile.GetSectionKeys(category))
            {
                if (settings.TryGetValue(key, out var setting))
                {
                    var value = settingsFile.GetValue(category, key, setting.DefaultValue);
                    setting.SetValue(value);
                }
                else
                {
                    PushError($"Found unknown setting: {category}/{key}");
                }
            }
        }
    }
    partial void RegisterSettings();

    public void Save()
    {
        foreach (var (category, settings) in settingsByCategory)
        foreach (var setting in settings.Values)
        {
            setting.NotifyChanges();
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

    private void Register(StringName category, IEnumerable<Setting> settingList)
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
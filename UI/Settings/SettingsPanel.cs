using System.Collections.Generic;

namespace GodotLib.UI.Settings;

[GlobalClass]
public partial class SettingsPanel : Control
{
    private Control container;

    public override void _EnterTree()
    {
        container = GetNode<Container>("%Container");
    }

    public void Load(IEnumerable<Setting> settings)
    {
        foreach (var setting in settings)
        {
            var label = new Label();
            label.Text = setting.Key;
            
            var settingRow = new HBoxContainer();
            var control = GetControlFor(setting);
            
            settingRow.AddChild(label);
            settingRow.AddChild(control);
            
            container.AddChild(settingRow);
        }
    }

    private Control GetControlFor(Setting setting)
    {
        Log(setting.DefaultValue, setting.Value);
        switch (setting.Value.VariantType)
        {
            case Variant.Type.Bool:
                var checkBox = new CheckBox();
                checkBox.SetPressedNoSignal(setting.Value.As<bool>());
                checkBox.Toggled += val => setting.Value = val;
                return checkBox;
            
            case Variant.Type.Float:
                var spinbox = new SpinBox();
                spinbox.SetValueNoSignal(setting.Value.As<float>());
                spinbox.ValueChanged += val => setting.Value = val;
                return spinbox;
            
            default:
                var label = new Label();
                label.Text = setting.Value.ToString();
                return label;
        }
    }
}

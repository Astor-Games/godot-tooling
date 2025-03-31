using System.Collections.Generic;
using System.Globalization;

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
            label.Text = StringExtensions.Capitalize(setting.Key);
            
            var settingRow = new HBoxContainer();
            var controls = GetControlsFor(setting);
            
            settingRow.AddThemeConstantOverride("separation", 10);
            settingRow.AddChild(label);
            foreach (var control in controls)
            {
                settingRow.AddChild(control);
            }
            
            container.AddChild(settingRow);
        }
    }

    private Control[] GetControlsFor(Setting setting)
    {
        Log(setting.DefaultValue, setting.Value);
        switch (setting.Value.VariantType)
        {
            case Variant.Type.Bool:
                var checkBox = new CheckBox();
                checkBox.SetPressedNoSignal(setting.Value.As<bool>());
                checkBox.Toggled += val => setting.SetValue(val);
                return [checkBox];
            
            case Variant.Type.Float when setting.DisplayAs(out Range range):

                var value = setting.Value.As<float>();
                var label = new Label();
                var decimals = range.StepDecimalPlaces;
                label.Text = value.ToString($"F{decimals}");
                var slider = new HSlider();
                slider.CustomMinimumSize = new Vector2(100, 10);
                slider.MinValue = range.Min;
                slider.MaxValue = range.Max;
                slider.Step = range.Step;
                slider.SetValueNoSignal(value);
                slider.ValueChanged += val =>
                {
                    label.Text = val.ToString($"F{decimals}");
                    setting.SetValue(val);
                };
                
                return [slider, label];
            
            case Variant.Type.Float:
                var spinbox = new SpinBox();
                spinbox.SetValueNoSignal(setting.Value.As<float>());
                spinbox.ValueChanged += val => setting.SetValue(val);
                return [spinbox];
            
            default:
                label = new Label();
                label.Text = setting.Value.ToString();
                return [label];
        }
    }
}

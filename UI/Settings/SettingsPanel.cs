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

    public void Load(IEnumerable<Property> settings)
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

    private Control[] GetControlsFor(Property property)
    {
        Log(property.DefaultValue, property.Value);
        switch (property.Value.VariantType)
        {
            case Variant.Type.Bool:
                var checkBox = new CheckBox();
                checkBox.SetPressedNoSignal(property.Value.As<bool>());
                checkBox.Toggled += val => property.SetValue(val);
                return [checkBox];
            
            case Variant.Type.Float when property.DisplayAs(out Range range):

                var value = property.Value.As<float>();
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
                    property.SetValue(val);
                };
                
                return [slider, label];
            
            case Variant.Type.Float:
                var spinbox = new SpinBox();
                spinbox.SetValueNoSignal(property.Value.As<float>());
                spinbox.ValueChanged += val => property.SetValue(val);
                return [spinbox];
            
            default:
                label = new Label();
                label.Text = property.Value.ToString();
                return [label];
        }
    }
}

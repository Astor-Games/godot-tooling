namespace GodotLib.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TooltipAttribute : Attribute
{
    private readonly string tooltip;

    public TooltipAttribute(string tooltip)
    {
        this.tooltip = tooltip;
    }
}
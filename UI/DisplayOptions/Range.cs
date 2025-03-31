namespace GodotLib.UI;

public readonly struct Range(float min, float max, float step = 0) : DisplayOption
{
    public readonly float Min = min, Max = max, Step = step;
    
    // Count the number of decimal places in the interval
    public readonly int StepDecimalPlaces
    {
        get
        {
            var parts = Step.ToString("0.################").Split('.');
            return parts.Length > 1 ? parts[1].Length : 0;
        }
    }
}
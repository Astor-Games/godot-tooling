using GodotLib.Util;
using Turtles.Core;
using static GodotLib.Debug.RendererConsts;
namespace GodotLib.Debug;

[PropertyRenderer(typeof(Timer))]
public class TimerRenderer : IPropertyTreeRenderer
{
    public void Render(TreeItem rootItem, object property, RenderingParameters parameters)
    {
        var timer = (Timer)property;

        if (timer == Timer.None)
        {
            rootItem.SetText(1, "None");
            rootItem.SetCustomColor(1, DefaultValueColor);
            return;
        }

        string state;
        
        if (Client.GameInstance.TimerIsFinished(timer)) state = "Finished";
        else if (Client.GameInstance.TimerIsStarted(timer)) state = "Started";
        else state = "Not Started";
        
        rootItem.SetText(1, state);
        
        var remaining = Client.GameInstance.TimerGetRemaining(timer);
        
        rootItem.CreateOrGetChild(0, out var row1);
        rootItem.CreateOrGetChild(1, out var row2);
        rootItem.CreateOrGetChild(1, out var row3);
        rootItem.CreateOrGetChild(1, out var row4);
        
        row1.SetText(0, "Start Tick");
        row1.SetText(1, timer.StartTick.ToString());
        row2.SetText(0, "Duration");
        row2.SetText(1, timer.Duration.ToString());
        row3.SetText(0, "Remaining Ticks");
        row3.SetText(1, remaining.ToString());
        row4.SetText(0, "Interval");
        row4.SetText(1, timer.Interval.ToString("F2"));
    }
}
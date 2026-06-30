using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class Audio2DTweens
{
    private static readonly NodePath volumeDbPath = new(AudioStreamPlayer2D.PropertyName.VolumeDb);
    private static readonly NodePath pitchPath = new(AudioStreamPlayer2D.PropertyName.PitchScale);
    
    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<AudioStreamPlayer2D>
    {
        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Volume(float volumeDb, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, volumeDbPath, volumeDb, duration);
        }

        [MethodImpl(AggressiveInlining)]
        public PropertyTweener Pitch(float pitch, float duration)
        {
            return tween.Tween.TweenProperty(tween.Node, pitchPath, pitch, duration);
        }
    }
}

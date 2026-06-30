using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace GodotLib.Tweening;

public static class AudioTweens
{
    private static readonly NodePath volumeDbPath = new(AudioStreamPlayer.PropertyName.VolumeDb);
    private static readonly NodePath pitchPath = new(AudioStreamPlayer.PropertyName.PitchScale);

    extension<TSelf>(TSelf tween) where TSelf : struct, ITweenFor<AudioStreamPlayer>
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

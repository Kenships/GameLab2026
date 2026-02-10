using UnityEngine;

namespace _Project.Scripts.Core.AudioPooling.Implement
{
    public class AudioConfig2D : AudioConfigBase<AudioConfig2D>
    {
        public AudioConfig2D(AudioPooler audioPooler, AudioClip audioClip) : base(audioPooler, audioClip)
        {
            SpatialBlend = 0f;
            Position = Vector3.zero;
        }
    }
}

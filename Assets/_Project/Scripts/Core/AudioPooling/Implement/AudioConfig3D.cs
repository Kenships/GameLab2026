using UnityEngine;

namespace _Project.Scripts.Core.AudioPooling.Implement
{
    public class AudioConfig3D : AudioConfigBase<AudioConfig3D>
    {
        public AudioConfig3D(AudioPooler audioPooler, AudioClip audioClip) : base(audioPooler, audioClip)
        {
            SpatialBlend = 1f;
        }
        
        public AudioConfig3D AtPosition(Vector3 position)
        {
            Position = position;
            return this;
        }
        
        public AudioConfig3D WithMinDistance(float minDistance)
        {
            MinDistance = minDistance;
            return this;
        }
        
        public AudioConfig3D WithMaxDistance(float maxDistance)
        {
            MaxDistance = maxDistance;
            return this;
        }

        public AudioConfig3D BypassReverbZones()
        {
            IsBypassReverbZones = true;
            return this;
        }
    }
}

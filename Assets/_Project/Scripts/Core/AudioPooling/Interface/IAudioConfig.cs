using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Core.AudioPooling.Interface
{
    public enum AudioType
    {
        Music,
        UI,
        Sfx
    }
    
    public interface IAudioConfig
    {
        public int SceneBuildIndex { get; set; }
        public AudioClip Clip { get; set; }
        public AudioType AudioType { get; set; }
        public int Priority { get; set; }
        public AudioMixerGroup AudioMixerGroup { get; set; }
        public Vector3 Position { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }
        public float SpatialBlend { get; set; }
        public bool Loop { get; set; }
        public float MinDistance { get; set; }
        public float MaxDistance { get; set; }
        public bool IsBypassReverbZones { get; set; }
    }
}

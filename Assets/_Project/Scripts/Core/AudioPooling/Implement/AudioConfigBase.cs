using _Project.Scripts.Core.AudioPooling.Interface;
using UnityEngine;
using UnityEngine.Audio;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.AudioPooling.Implement
{
    public abstract class AudioConfigBase<TSelf> : IAudioConfig where TSelf : AudioConfigBase<TSelf>
    {
        private readonly AudioPooler _audioPooler;
        public int SceneBuildIndex { get; set; } = 0;
        public AudioClip Clip { get; set; }
        public AudioType AudioType { get; set; } = AudioType.Sfx;
        public int Priority { get; set; } = 1;
        public AudioMixerGroup AudioMixerGroup { get; set; }
        public Vector3 Position { get; set; } = Vector3.zero;
        public float Pitch { get; set; } = 1f;
        public float Volume { get; set; } = 1f;
        public float SpatialBlend { get; set; } = 1f;
        public float MinDistance { get; set; } = 1f;
        public float MaxDistance { get; set; } = 500f;
        public bool IsBypassReverbZones { get; set; } = false;
        public bool Loop { get; set; } = false;

        protected AudioConfigBase(AudioPooler audioPooler, AudioClip audioClip)
        {
            _audioPooler = audioPooler;
            Clip = audioClip;
            
            // Default mixer group
            AudioMixerGroup = _audioPooler.GetMixerFor(AudioType.Sfx);
        }

        public TSelf AddToScene(int sceneBuildIndex)
        {
            SceneBuildIndex = sceneBuildIndex;
            return (TSelf) this;
        }
        
        public TSelf OnChannel(AudioType audioType)
        {
            AudioType = audioType;
            AudioMixerGroup = _audioPooler.GetMixerFor(audioType);
            return (TSelf) this;
        }

        public TSelf SetVolume(float volume)
        {
            Volume = volume;
            return (TSelf) this;
        }

        public TSelf SetPitch(float pitch)
        {
            Pitch = pitch;
            return (TSelf) this;
        }
        
        public TSelf RandomizePitch(float min = 0.05f, float max = 0.05f)
        {
            return SetPitch(1f + Random.Range(min, max));
        }

        public TSelf LoopAudio()
        {
            Loop = true;
            return (TSelf) this;
        }

        public TSelf AddPriority(int priority)
        {
            Priority = priority;
            return (TSelf) this;
        }

        public TSelf MarkFrequent()
        {
            Priority = 0;
            return (TSelf) this;
        }

        public IAudioPlayer Play() => _audioPooler.Play(this);
    }
}

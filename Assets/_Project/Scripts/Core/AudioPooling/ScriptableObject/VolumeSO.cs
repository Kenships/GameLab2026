using Obvious.Soap;
using Sisus.Init;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Core.AudioPooling.ScriptableObject
{
    [Service(typeof(VolumeSO), ResourcePath = "ScriptableObjects/Audio/VolumeSO")]
    [CreateAssetMenu(fileName = "VolumeSO", menuName = "Scriptable Objects/Audio/VolumeSO")]
    public class VolumeSO : UnityEngine.ScriptableObject
    {
        [SerializeField] private BoolVariable mute;
        [SerializeField] private FloatVariable masterVolume;
        [SerializeField] private FloatVariable musicVolume;
        [SerializeField] private FloatVariable sfxVolume;
        [SerializeField] private AudioMixer mixer;

        public bool Mute
        {
            get => mute.Value;
            set => mute.Value = value;
        }
        public float MasterVolume {
            get => masterVolume.Value;
            set => masterVolume.Value = value;
        }
        public float MusicVolume {
            get => musicVolume.Value;
            set => musicVolume.Value = value;
        }
        public float SfxVolume {
            get => sfxVolume.Value;
            set => sfxVolume.Value = value;
        }
        
       

        private void OnEnable()
        {
            masterVolume.OnValueChanged += MasterVolumeOnValueChanged;
            musicVolume.OnValueChanged += MusicVolumeOnValueChanged;
            sfxVolume.OnValueChanged += SfxVolumeOnValueChanged;
            mute.OnValueChanged += MuteOnValueChanged;
            InitializeMixer();
        }

        private void OnDisable()
        {
            masterVolume.OnValueChanged -= MasterVolumeOnValueChanged;
            musicVolume.OnValueChanged -= MusicVolumeOnValueChanged;
            sfxVolume.OnValueChanged -= SfxVolumeOnValueChanged;
            mute.OnValueChanged -= MuteOnValueChanged;
        }

        public void InitializeMixer()
        {
            MasterVolumeOnValueChanged(MasterVolume);
            MusicVolumeOnValueChanged(MusicVolume);
            SfxVolumeOnValueChanged(SfxVolume);
            MuteOnValueChanged(Mute);
        }
        
        private void MuteOnValueChanged(bool isMute)
        {
            mixer.SetFloat("Master", isMute ? LinearToDb(0) : LinearToDb(MasterVolume));
        }

        private void SfxVolumeOnValueChanged(float volume)
        {
            mixer.SetFloat("Sfx", LinearToDb(volume));
        }

        private void MusicVolumeOnValueChanged(float volume)
        {
            mixer.SetFloat("Music", LinearToDb(volume));
        }

        private void MasterVolumeOnValueChanged(float volume)
        {
            mixer.SetFloat("Master", LinearToDb(volume));
        }

        private float LinearToDb(float volume)
        {
            float volumePercent = volume / 100f;
            volumePercent = Mathf.Clamp(volumePercent, 0.0001f, 1f);
            return Mathf.Log10(volumePercent) * 20f;
        }
    }
}

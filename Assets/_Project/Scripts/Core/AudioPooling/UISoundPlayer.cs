using Sisus.Init;
using System.Collections;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.AudioPooling
{
    public class UISoundPlayer : MonoBehaviour<AudioPooler>
    {
        [SerializeField] private float delay = 1f;
        private AudioPooler _audioPooler;
        private AudioClip _clip;
        protected override void Init(AudioPooler playerReader)
        {
            _audioPooler = playerReader;
        }
    
        public void PlaySound(AudioClip clip)
        {
            _audioPooler.New2DAudio(clip)
                .OnChannel(AudioType.UI)
                .RandomizePitch()
                .Play();
        }
        public void PlaySoundWithDelay(AudioClip clip)
        {
            _clip = clip;
            Invoke(nameof(DelaySound), delay);
        }
        private void DelaySound()
        {
            PlaySound(_clip);
        }
    }
}

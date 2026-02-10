using System;
using _Project.Scripts.Core.AudioPooling.Interface;
using UnityEngine;

namespace _Project.Scripts.Core.AudioPooling.Implement
{
    /// <summary>
    /// Wrapper class for any IAudioPlayer
    /// </summary>
    public class AudioPlayer : IAudioPlayer
    {
        public event Action OnAudioFinished;
        private IAudioPlayer _audioSource;
        private Coroutine _playingCoroutine;
        
        public AudioPlayer(IAudioPlayer audioSource)
        {
            _audioSource = audioSource;
            
            _audioSource.OnAudioFinished += AudioSourceOnAudioFinished;
        }

        private void AudioSourceOnAudioFinished()
        {
            if (_audioSource == null)
            {
                return;
            }
            OnAudioFinished?.Invoke();
            _audioSource.OnAudioFinished -= AudioSourceOnAudioFinished;
            _audioSource = null;
        }

        public void FadeVolume(float volume, float duration = 0, bool stopOnSilent = true)
        {
            _audioSource.FadeVolume(volume, duration, stopOnSilent);
        }

        public void Play()
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("AudioPlayer Trying to Play from an Expired Source");
                return;
            }
            
            _audioSource.Play();
        }

        public void Stop()
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("AudioPlayer Trying to Stop an Expired Source");
                return;
            }
            _audioSource.Stop();
        }

        public void Pause()
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("AudioPlayer Trying to Pause an Expired Source");
                return;
            }
            _audioSource.Pause();
        }

        public void Resume()
        {
            if (_audioSource == null)
            {
                Debug.LogWarning("AudioPlayer Trying to Resume an Expired Source");
                return;
            }
            _audioSource.Play();
        }
    }
}

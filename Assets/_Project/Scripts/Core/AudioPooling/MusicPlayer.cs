using System;
using _Project.Scripts.Core.AudioPooling.Interface;
using Sisus.Init;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.AudioPooling
{
    public class MusicPlayer : MonoBehaviour<AudioPooler>
    {
        [SerializeField] private AudioClip music;
        [SerializeField] private float musicVolume = 1;

        private AudioPooler _audioPooler;
        
        private IAudioPlayer _audioPlayer;

        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }

        private void OnDestroy()
        {
            
        }

        private void Start()
        {
            _audioPlayer = _audioPooler.New2DAudio(music)
                .OnChannel(AudioType.Music)
                .SetVolume(musicVolume)
                .AddToScene(gameObject.scene.buildIndex)
                .LoopAudio()
                .Play();
        }
    }
}

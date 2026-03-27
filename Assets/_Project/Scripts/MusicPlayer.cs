using _Project.Scripts.Core.AudioPooling;
using Sisus.Init;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

public class MusicPlayer : MonoBehaviour<AudioPooler>
{
    [SerializeField] private AudioClip music;
    [SerializeField] private float musicvolume = 1;

    protected AudioPooler _audioPooler;

    protected override void Init(AudioPooler audioPooler)
    {
        _audioPooler = audioPooler;
    }

    void Start()
    {
        _audioPooler.New2DAudio(music).LoopAudio().OnChannel(AudioType.Music).SetVolume(musicvolume).Play();
    }

}

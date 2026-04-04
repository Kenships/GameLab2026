using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;
using Sisus.Init;
using _Project.Scripts.Core.AudioPooling;

public class PlaySoundOnDestroy : MonoBehaviour<AudioPooler>
{
    [SerializeField] private AudioClip sound;
    [SerializeField] private float volume;
    private AudioPooler _pooler;
    protected override void Init(AudioPooler argument)
    {
        _pooler = argument;
    }

    private void OnDestroy()
    {
        _pooler.New2DAudio(sound).OnChannel(AudioType.Sfx).SetVolume(volume).Play();
    }
}

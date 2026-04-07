using _Project.Scripts.Core.AudioPooling;
using Sisus.Init;
using UnityEngine;

public class ForceStopAllMusic : MonoBehaviour<AudioPooler>
{
    private AudioPooler _audioPooler;

    protected override void Init(AudioPooler audioPooler)
    {
        _audioPooler = audioPooler;
    }
    public void StopAllMusic()
    {
        _audioPooler.StopAllMusic();
    }
}

using System;

namespace _Project.Scripts.Core.AudioPooling.Interface
{
    public interface IAudioPlayer
    {
        event Action OnAudioFinished;
        void Play();
        void Stop();
        void FadeVolume(float volume, float duration = 0, bool stopOnSilent = true);
        void Pause();
        void Resume();
    }
}

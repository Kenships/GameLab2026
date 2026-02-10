using _Project.Scripts.Core.AudioPooling.Interface;

namespace _Project.Scripts.Core.AudioPooling
{
    public partial class AudioPooler : IAudioPoolSceneController
    {
        public void FadeAllVolumeFromScene(int sceneBuildIndex, float volume, float duration)
        {
            if (activeSourcesBySceneIndex.TryGetValue(sceneBuildIndex, out var audioSources))
            {
                foreach (var audioSource in audioSources)
                {
                    audioSource.FadeVolume(volume, duration);
                }
            }
        }
    }
}

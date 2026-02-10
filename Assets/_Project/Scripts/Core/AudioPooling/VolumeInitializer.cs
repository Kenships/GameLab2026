using _Project.Scripts.Core.AudioPooling.ScriptableObject;
using Sisus.Init;

namespace _Project.Scripts.Core.AudioPooling
{
    public class VolumeInitializer : MonoBehaviour<VolumeSO>

    {
        private VolumeSO _volume;
        protected override void Init(VolumeSO playerReader)
        {
            _volume = playerReader;
        }

        private void Start()
        {
            _volume.InitializeMixer();
        }
    }
}

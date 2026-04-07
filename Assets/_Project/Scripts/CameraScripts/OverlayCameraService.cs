using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.CameraScripts
{
    [RequireComponent(typeof(Camera)), Service(typeof(OverlayCameraService), LoadScene = 0)]
    public class OverlayCameraService : MonoBehaviour
    {
        public Camera Camera => GetComponent<Camera>();
    }
}

using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Project.Scripts.CameraScripts
{
    [RequireComponent(typeof(Camera))]
    public class BuildCameraStack : MonoBehaviour
    {
        public void Start()
        {
            var overlayCamera = GetComponent<Camera>();
            
            var data = Camera.main.GetUniversalAdditionalCameraData();
            if (!data.cameraStack.Contains(overlayCamera))
                data.cameraStack.Add(overlayCamera);
        }

        private void OnDestroy()
        {
            var overlayCamera = GetComponent<Camera>();
            var data = Camera.main.GetUniversalAdditionalCameraData();
            if (data.cameraStack.Contains(overlayCamera))
                data.cameraStack.Remove(overlayCamera);
        }

        void OnValidate()
        {
            #if UNITY_EDITOR
            var overlayCamera = GetComponent<Camera>();
            
            var data = Camera.main.GetUniversalAdditionalCameraData();
            if (!data.cameraStack.Contains(overlayCamera))
                data.cameraStack.Add(overlayCamera);
            #endif
        }
    }
}

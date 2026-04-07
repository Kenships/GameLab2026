using System;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class FaceCamera : MonoBehaviour
    {
        private Camera _cam;
        
        private void Start()
        {
            _cam = Camera.main;
        }

        private void Update()
        {
            if (!_cam) return;

            transform.forward = -_cam.transform.forward;
        }

        [ContextMenu("Face Camera")]
        public void FaceCameraOnClick()
        {
            _cam = Camera.main;
            transform.forward = -_cam.transform.forward;
        }
    }
}

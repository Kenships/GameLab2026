using System;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.CameraScripts
{
    [RequireComponent(typeof(Canvas))]
    public class CameraCanvasBind : MonoBehaviour<OverlayCameraService>
    {
        private Canvas _canvas;
        
        protected override void Init(OverlayCameraService overlayCameraService)
        {
            GetComponent<Canvas>().worldCamera = overlayCameraService.Camera;
        }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            _canvas ??= GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            #endif
        }
    }
}

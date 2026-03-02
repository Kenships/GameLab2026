using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.CameraScripts
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasBindCamera : MonoBehaviour<Camera>
    {
        protected override void Init(Camera argument)
        {
            var canvas = GetComponent<Canvas>();
            canvas.worldCamera = argument;
        }
    }
}

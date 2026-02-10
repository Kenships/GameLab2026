using _Project.Scripts.UI.Interfaces;
using PrimeTween;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.UI.Implement
{
    [Service(typeof(ITransition), FindFromScene = true)]
    [RequireComponent(typeof(CanvasGroup))]
    public class LoadingOverlay : MonoBehaviour, ITransition
    {
        [SerializeField] private float fadeDuration = 0.25f; 
        
        public float TransitionDuration => fadeDuration;
        private CanvasGroup _canvasGroup;
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show()
        {
            Tween.StopAll(_canvasGroup);
            
            Tween.Alpha(
                target: _canvasGroup,
                endValue: 1f,
                duration: fadeDuration
            );
        }
        
        public void Hide()
        {
            Tween.StopAll(_canvasGroup);
            
            Tween.Alpha(
                target: _canvasGroup,
                endValue: 0f,
                duration: fadeDuration
            );
        }
    }
}

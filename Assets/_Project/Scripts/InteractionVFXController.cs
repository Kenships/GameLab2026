using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Modules;
using _Project.Scripts.Core.Modules.Base_Class;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    public class InteractionVFXController : MonoBehaviour
    {
        public enum AbilityMode
        {
            FastForward,
            Rewind
        }

        private AbilityMode currentMode = AbilityMode.FastForward;
        
        [SerializeField] private IconManager iconManager;
        [SerializeField] private ModuleInfoManager moduleInfoManager; 

        [Header("Player Reference")]
        [Tooltip("Drag the player GameObject here")]
        [SerializeField] private Transform playerTransform;

        [Header("Animation Prefabs")]
        [Tooltip("Blue animation prefab for Fast Forward")]
        [SerializeField] private GameObject fastForwardPrefab;
        [Tooltip("Red animation prefab for Rewind")]
        [SerializeField] private GameObject rewindPrefab;
        
        [Header("Animation Settings")]
        [Tooltip("Diameter of the animation in world units")]
        [SerializeField] private float diameter = 4f;
        [Tooltip("Height offset above the ground (small value to prevent z-fighting)")]
        [SerializeField] private float yOffset = 0.05f;
        [Tooltip("Rotation offset on Y axis (to orient the arrows)")]
        [SerializeField] private float rotationOffset = 0f;

        [Header("Fade Settings")]
        [SerializeField] private float fadeInDuration = 0.15f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        [Header("Pulse Settings")]
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseMinAlpha = 0.6f;
        [SerializeField] private float pulseMaxAlpha = 1f;
        [SerializeField] private bool pulseScale = false;
        [SerializeField] private float pulseScaleMin = 0.95f;
        [SerializeField] private float pulseScaleMax = 1.05f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        [Header("ModuleIcons")]
        [SerializeField] private Texture2D turretIcon;
        [SerializeField] private Texture2D flameIcon;
        [SerializeField] private Texture2D lazerIcon;
        [SerializeField] private Texture2D carIcon;
        [SerializeField] private Texture2D discIcon;
        [SerializeField] private Texture2D explosiveIcon;

        private Canvas worldCanvas;
        private GameObject canvasGameObject;
        private GameObject currentAnimationInstance;
        private RectTransform currentAnimationRect;
        private CanvasGroup canvasGroup;
        private StopMotionUI stopMotionUI;
        private Image animationImage;
        
        private float fadeProgress = 0f;
        private bool isFadingIn = false;
        private bool isFadingOut = false;
        private bool isVisible = false;
        private float pulseTime = 0f;
        private Vector3 originalScale = Vector3.one;

        private string currentModuleName; 
        
        private void Awake()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                    DebugLog("Player found by tag: " + player.name);
                }
                else
                {
                    playerTransform = transform;
                    DebugLog("No player found, using this transform: " + gameObject.name);
                }
            }
            else
            {
                DebugLog("Player transform assigned: " + playerTransform.name);
            }

            CreateWorldSpaceCanvas();
        }

        private void DebugLog(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log("[InteractionVFXController] " + message);
            }
        }

        private void CreateWorldSpaceCanvas()
        {
            DebugLog("Creating world space canvas...");

            canvasGameObject = new GameObject("AbilityAnimationCanvas");
            canvasGameObject.transform.position = playerTransform.position + new Vector3(0f, yOffset, 0f);
            canvasGameObject.transform.rotation = Quaternion.Euler(90f, rotationOffset, 0f);

            worldCanvas = canvasGameObject.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.sortingOrder = 0;

            RectTransform canvasRect = canvasGameObject.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(100f, 100f);
            
            float canvasScale = diameter / 100f;
            canvasGameObject.transform.localScale = new Vector3(canvasScale, canvasScale, canvasScale);

            CanvasScaler scaler = canvasGameObject.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 1f;

            DebugLog("World space canvas created. Scale: " + canvasScale + ", Position: " + canvasGameObject.transform.position);
        }

        private void Update()
        {
            UpdateCanvasPosition();
            UpdatePulse();
            UpdateFade();
        }

        private void UpdateCanvasPosition()
        {
            if (worldCanvas == null || playerTransform == null) return;

            canvasGameObject.transform.position = playerTransform.position + new Vector3(0f, yOffset, 0f);
            canvasGameObject.transform.rotation = Quaternion.Euler(90f, rotationOffset, 0f);
        }

        private void UpdatePulse()
        {
            if (!enablePulse || !isVisible || canvasGroup == null) return;

            pulseTime += Time.deltaTime * pulseSpeed;

            // Scale pulse (optional)
            if (pulseScale && currentAnimationRect != null)
            {
                float t = (Mathf.Sin(pulseTime * Mathf.PI * 2f) + 1f) * 0.5f; // 0 to 1
                float scale = Mathf.Lerp(pulseScaleMin, pulseScaleMax, t);
                
                Vector3 newScale = new Vector3(
                    originalScale.x * scale,
                    originalScale.y * scale,
                    originalScale.z * scale
                );
                currentAnimationRect.localScale = newScale;
            }
        }

        private void UpdateFade()
        {
            if (canvasGroup == null) return;

            // Calculate pulse alpha
            float pulseAlpha = pulseMaxAlpha;
            if (enablePulse)
            {
                float t = (Mathf.Sin(pulseTime * Mathf.PI * 2f) + 1f) * 0.5f; // 0 to 1
                pulseAlpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t);
            }

            if (isFadingIn)
            {
                fadeProgress += Time.deltaTime / fadeInDuration;
                
                if (fadeProgress >= 1f)
                {
                    fadeProgress = 1f;
                    isFadingIn = false;
                    DebugLog("Fade in complete");
                }

                // Multiply fade progress with pulse alpha
                canvasGroup.alpha = fadeProgress * pulseAlpha;
            }
            else if (isFadingOut)
            {
                fadeProgress -= Time.deltaTime / fadeOutDuration;
                
                if (fadeProgress <= 0f)
                {
                    fadeProgress = 0f;
                    isFadingOut = false;
                    DestroyCurrentAnimation();
                    DebugLog("Fade out complete, animation destroyed");
                }
                else
                {
                    // Multiply fade progress with pulse alpha
                    canvasGroup.alpha = fadeProgress * pulseAlpha;
                }
            }
            else if (isVisible)
            {
                // Just pulsing, no fade
                canvasGroup.alpha = pulseAlpha;
            }
        }

        private void SpawnAnimation(AbilityMode mode)
        {
            DebugLog("SpawnAnimation called. Mode: " + mode);

            DestroyCurrentAnimation();

            // Select the correct prefab based on mode
            GameObject prefabToUse = null;
            
            if (mode == AbilityMode.FastForward)
            {
                prefabToUse = fastForwardPrefab;
                DebugLog("Using Fast Forward (blue) prefab");
            }
            else if (mode == AbilityMode.Rewind)
            {
                prefabToUse = rewindPrefab;
                DebugLog("Using Rewind (red) prefab");
            }

            if (prefabToUse == null)
            {
                Debug.LogError("[InteractionVFXController] " + mode + " prefab is not assigned!");
                return;
            }

            if (worldCanvas == null)
            {
                Debug.LogError("[InteractionVFXController] World Canvas was not created!");
                return;
            }

            DebugLog("Instantiating animation prefab: " + prefabToUse.name);

            currentAnimationInstance = Instantiate(prefabToUse);
            currentAnimationInstance.transform.SetParent(worldCanvas.transform, false);
            currentAnimationInstance.name = "ActiveAbilityAnimation_" + mode;

            DebugLog("Animation instance created: " + currentAnimationInstance.name);

            currentAnimationRect = currentAnimationInstance.GetComponent<RectTransform>();
            if (currentAnimationRect != null)
            {
                currentAnimationRect.anchorMin = new Vector2(0.5f, 0.5f);
                currentAnimationRect.anchorMax = new Vector2(0.5f, 0.5f);
                currentAnimationRect.pivot = new Vector2(0.5f, 0.5f);
                currentAnimationRect.anchoredPosition = Vector2.zero;
                currentAnimationRect.sizeDelta = new Vector2(100f, 100f);
                
                // Flip horizontally for Rewind mode (arrows point opposite direction)
                if (mode == AbilityMode.Rewind)
                {
                    currentAnimationRect.localScale = new Vector3(-1f, 1f, 1f);
                    DebugLog("Rewind mode - flipped horizontally");
                }
                else
                {
                    currentAnimationRect.localScale = Vector3.one;
                }

                // Store original scale for pulse
                originalScale = currentAnimationRect.localScale;

                DebugLog("RectTransform configured. Size: " + currentAnimationRect.sizeDelta);
            }
            else
            {
                Debug.LogError("[InteractionVFXController] Animation prefab has no RectTransform!");
            }

            animationImage = currentAnimationInstance.GetComponent<Image>();
            if (animationImage != null)
            {
                animationImage.preserveAspect = true;
                DebugLog("Image component found. Sprite: " + (animationImage.sprite != null ? animationImage.sprite.name : "NULL"));
            }
            else
            {
                Debug.LogError("[InteractionVFXController] Animation prefab has no Image component!");
            }

            canvasGroup = currentAnimationInstance.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = currentAnimationInstance.AddComponent<CanvasGroup>();
                DebugLog("Added CanvasGroup component");
            }
            canvasGroup.alpha = 0f;

            stopMotionUI = currentAnimationInstance.GetComponent<StopMotionUI>();
            if (stopMotionUI != null)
            {
                DebugLog("StopMotionUI found. Frames count: " + (stopMotionUI.frames != null ? stopMotionUI.frames.Length.ToString() : "NULL"));
                
                if (stopMotionUI.frames == null || stopMotionUI.frames.Length == 0)
                {
                    Debug.LogError("[InteractionVFXController] StopMotionUI has no frames assigned!");
                }
                else
                {
                    if (stopMotionUI.frames[0] == null)
                    {
                        Debug.LogError("[InteractionVFXController] First frame in StopMotionUI is null!");
                    }
                    else
                    {
                        DebugLog("First frame: " + stopMotionUI.frames[0].name);
                    }
                }

                stopMotionUI.loop = true;
                stopMotionUI.destroyOnEnd = false;
                
                DebugLog("Starting animation playback...");
                stopMotionUI.Play();
            }
            else
            {
                Debug.LogError("[InteractionVFXController] Animation prefab doesn't have StopMotionUI component!");
            }

            // Reset pulse and fade state
            pulseTime = 0f;
            fadeProgress = 0f;
            isFadingIn = true;
            isFadingOut = false;

            DebugLog("SpawnAnimation completed");
        }

        private void DestroyCurrentAnimation()
        {
            if (currentAnimationInstance != null)
            {
                DebugLog("Destroying current animation");

                if (stopMotionUI != null)
                {
                    stopMotionUI.StopAllCoroutines();
                }
                
                Destroy(currentAnimationInstance);
                currentAnimationInstance = null;
                currentAnimationRect = null;
                canvasGroup = null;
                stopMotionUI = null;
                animationImage = null;
            }
            
            isFadingIn = false;
            isFadingOut = false;
        }

        public void ShowHeldObject(GameObject heldObject)
        {
            Debug.Log(heldObject);
            if (!heldObject.TryGetComponent(out Module module))
            {
                Debug.LogError("[InteractionVFXController] Module not found!");
                return;
            }
            
            moduleInfoManager.ShowInfo(module.moduleSprite, module.nameKey);
        }

        public void HideHeldObject()
        {
            moduleInfoManager.Hide();
        }

        public void ShowWind(AbilityMode mode = AbilityMode.FastForward)
        {
            DebugLog("ShowWind called. Mode: " + mode);

            currentMode = mode;

            SpawnAnimation(mode);

            if (mode == AbilityMode.FastForward && iconManager != null)
            {
                iconManager.ShowFastForward();
            }
            else if (mode == AbilityMode.Rewind && iconManager != null)
            {
                iconManager.ShowRewind();
            }

            isVisible = true;

            DebugLog("ShowWind completed. isVisible: " + isVisible);
        }

        public void HideWind()
        {
            DebugLog("HideWind called");

            isVisible = false;
            isFadingIn = false;
            isFadingOut = true;
            
            if (iconManager != null)
            {
                iconManager.Hide();
            }
        }

        public void Toggle(AbilityMode mode = AbilityMode.FastForward)
        {
            if (isVisible) HideWind();
            else ShowWind(mode);
        }

        public void SetDiameter(float newDiameter)
        {
            diameter = newDiameter;
            
            if (canvasGameObject != null)
            {
                float canvasScale = diameter / 100f;
                canvasGameObject.transform.localScale = new Vector3(canvasScale, canvasScale, canvasScale);
            }
        }

        public void SetYOffset(float newOffset)
        {
            yOffset = newOffset;
        }

        public void SetPlayerTransform(Transform newPlayerTransform)
        {
            playerTransform = newPlayerTransform;
            DebugLog("Player transform updated to: " + playerTransform.name);
        }

        /// <summary>
        /// Enable or disable pulse at runtime
        /// </summary>
        public void SetPulseEnabled(bool enabled)
        {
            enablePulse = enabled;
            
            if (!enabled && currentAnimationRect != null)
            {
                currentAnimationRect.localScale = originalScale;
            }
        }

        /// <summary>
        /// Set pulse speed at runtime
        /// </summary>
        public void SetPulseSpeed(float speed)
        {
            pulseSpeed = speed;
        }

        /// <summary>
        /// Set pulse alpha range at runtime
        /// </summary>
        public void SetPulseAlphaRange(float min, float max)
        {
            pulseMinAlpha = min;
            pulseMaxAlpha = max;
        }

        /// <summary>
        /// Set pulse scale range at runtime
        /// </summary>
        public void SetPulseScaleRange(float min, float max)
        {
            pulseScaleMin = min;
            pulseScaleMax = max;
        }

        /// <summary>
        /// Reset pulse time to sync with another effect
        /// </summary>
        public void ResetPulseTime()
        {
            pulseTime = 0f;
        }

        /// <summary>
        /// Get current pulse time for syncing
        /// </summary>
        public float GetPulseTime()
        {
            return pulseTime;
        }

        /// <summary>
        /// Set pulse time to match another effect
        /// </summary>
        public void SetPulseTime(float time)
        {
            pulseTime = time;
        }

        private void OnDisable()
        {
            DestroyCurrentAnimation();
        }

        private void OnDestroy()
        {
            DestroyCurrentAnimation();
            
            if (canvasGameObject != null)
            {
                Destroy(canvasGameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Transform targetTransform = playerTransform != null ? playerTransform : transform;
            
            Gizmos.color = Color.cyan;
            Vector3 center = targetTransform.position + new Vector3(0f, yOffset, 0f);
            
            int segments = 32;
            float radius = diameter / 2f;
            
            Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }

            Gizmos.DrawLine(center - new Vector3(0.5f, 0f, 0f), center + new Vector3(0.5f, 0f, 0f));
            Gizmos.DrawLine(center - new Vector3(0f, 0f, 0.5f), center + new Vector3(0f, 0f, 0.5f));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (canvasGameObject != null)
            {
                float canvasScale = diameter / 100f;
                canvasGameObject.transform.localScale = new Vector3(canvasScale, canvasScale, canvasScale);
                canvasGameObject.transform.rotation = Quaternion.Euler(90f, rotationOffset, 0f);
            }
        }
#endif
    }
}

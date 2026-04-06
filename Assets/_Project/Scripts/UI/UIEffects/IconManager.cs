using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconManager : MonoBehaviour
{
    [Header("=== Animation Prefab References ===")]
    [SerializeField] private GameObject fastForwardPrefab;
    [SerializeField] private GameObject rewindPrefab;
    
    [Header("=== Canvas Reference ===")]
    [SerializeField] private Transform iconContainer;

    [Header("=== Size & Position Settings ===")]
    [SerializeField] private Vector2 iconSize = new Vector2(100f, 100f);
    [SerializeField] private float rotationOffset = 0f;

    [Header("=== Fade Settings ===")]
    [SerializeField] private float fadeInDuration = 0.15f;
    [SerializeField] private float fadeOutDuration = 0.2f;
    [SerializeField] private float switchFadeDuration = 0.1f;

    [Header("=== Pulse Settings ===")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinAlpha = 0.6f;
    [SerializeField] private float pulseMaxAlpha = 1f;
    [SerializeField] private bool pulseScale = false;
    [SerializeField] private float pulseScaleMin = 0.95f;
    [SerializeField] private float pulseScaleMax = 1.05f;

    // State
    private enum IconState
    {
        None,
        FastForward,
        Rewind
    }

    private IconState currentState = IconState.None;
    private GameObject activeIconInstance = null;
    private CanvasGroup activeCanvasGroup = null;
    private StopMotionUI activeStopMotion = null;
    private Coroutine transitionCoroutine;
    private Coroutine pulseCoroutine;
    private bool isTransitioning = false;

    // Original scale for pulse
    private Vector3 originalScale = Vector3.one;

    void Awake()
    {
        // If no container assigned, use this transform
        if (iconContainer == null)
        {
            iconContainer = transform;
        }
    }

    public void ShowFastForward()
    {
        if (currentState == IconState.FastForward && !isTransitioning)
            return;

        StopAllTransitions();
        transitionCoroutine = StartCoroutine(TransitionToIcon(IconState.FastForward));
    }

    public void ShowRewind()
    {
        if (currentState == IconState.Rewind && !isTransitioning)
            return;

        StopAllTransitions();
        transitionCoroutine = StartCoroutine(TransitionToIcon(IconState.Rewind));
    }

    public void Hide()
    {
        if (currentState == IconState.None && !isTransitioning)
            return;

        StopAllTransitions();
        transitionCoroutine = StartCoroutine(TransitionToIcon(IconState.None));
    }

    void StopAllTransitions()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        StopPulse();
    }

    void StopPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        // Reset scale
        if (activeIconInstance != null)
        {
            activeIconInstance.transform.localScale = originalScale;
        }
    }

    IEnumerator TransitionToIcon(IconState targetState)
    {
        isTransitioning = true;

        GameObject targetPrefab = GetPrefabForState(targetState);
        bool wasSwitching = currentState != IconState.None && targetState != IconState.None;

        // Fade out and destroy current icon if showing
        if (activeIconInstance != null && activeCanvasGroup != null && activeCanvasGroup.alpha > 0f)
        {
            float fadeDuration = wasSwitching ? switchFadeDuration : fadeOutDuration;
            yield return StartCoroutine(FadeCanvasGroup(activeCanvasGroup, activeCanvasGroup.alpha, 0f, fadeDuration));
            
            // Destroy the old instance
            DestroyActiveIcon();
        }

        // Update state
        currentState = targetState;

        // Spawn and fade in new icon if not hiding
        if (targetPrefab != null)
        {
            // Spawn new instance
            SpawnIcon(targetPrefab, targetState);

            // Fade in
            yield return StartCoroutine(FadeCanvasGroup(activeCanvasGroup, 0f, pulseMaxAlpha, fadeInDuration));

            // Start pulse
            if (enablePulse)
            {
                pulseCoroutine = StartCoroutine(PulseRoutine());
            }
        }

        isTransitioning = false;
        transitionCoroutine = null;
    }

    void SpawnIcon(GameObject prefab, IconState state)
    {
        // Instantiate the prefab
        activeIconInstance = Instantiate(prefab, iconContainer);
        activeIconInstance.name = "ActiveIcon_" + state;

        // Configure RectTransform
        RectTransform rect = activeIconInstance.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = iconSize;
            
            // Apply rotation offset and flip for rewind
            if (state == IconState.Rewind)
            {
                // Flip horizontally and apply rotation
                rect.localScale = new Vector3(-1f, 1f, 1f);
                rect.localRotation = Quaternion.Euler(0f, 0f, -rotationOffset);
            }
            else
            {
                rect.localScale = Vector3.one;
                rect.localRotation = Quaternion.Euler(0f, 0f, rotationOffset);
            }
        }

        // Store original scale for pulse
        originalScale = activeIconInstance.transform.localScale;

        // Get or add CanvasGroup
        activeCanvasGroup = activeIconInstance.GetComponent<CanvasGroup>();
        if (activeCanvasGroup == null)
        {
            activeCanvasGroup = activeIconInstance.AddComponent<CanvasGroup>();
        }
        activeCanvasGroup.alpha = 0f;

        // Get StopMotionUI and start playing
        activeStopMotion = activeIconInstance.GetComponent<StopMotionUI>();
        if (activeStopMotion != null)
        {
            activeStopMotion.loop = true;
            activeStopMotion.destroyOnEnd = false;
            activeStopMotion.Play();
        }
        else
        {
            Debug.LogWarning("[IconManager] Prefab doesn't have StopMotionUI component: " + prefab.name);
        }
    }

    void DestroyActiveIcon()
    {
        if (activeIconInstance != null)
        {
            if (activeStopMotion != null)
            {
                activeStopMotion.StopAllCoroutines();
            }

            Destroy(activeIconInstance);
            activeIconInstance = null;
            activeCanvasGroup = null;
            activeStopMotion = null;
        }
    }

    IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float fromAlpha, float toAlpha, float duration)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = toAlpha;
    }

    IEnumerator PulseRoutine()
    {
        if (activeCanvasGroup == null) yield break;

        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * pulseSpeed;
            float t = (Mathf.Sin(time * Mathf.PI * 2f) + 1f) * 0.5f; // 0 to 1

            // Alpha pulse
            float alpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t);
            if (activeCanvasGroup != null)
            {
                activeCanvasGroup.alpha = alpha;
            }

            // Scale pulse (optional)
            if (pulseScale && activeIconInstance != null)
            {
                float scale = Mathf.Lerp(pulseScaleMin, pulseScaleMax, t);
                activeIconInstance.transform.localScale = originalScale * scale;
            }

            yield return null;
        }
    }

    GameObject GetPrefabForState(IconState state)
    {
        return state switch
        {
            IconState.FastForward => fastForwardPrefab,
            IconState.Rewind => rewindPrefab,
            _ => null
        };
    }

    /// <summary>
    /// Set rotation offset at runtime
    /// </summary>
    public void SetRotationOffset(float newOffset)
    {
        rotationOffset = newOffset;
        
        // Update current icon if one is active
        if (activeIconInstance != null)
        {
            RectTransform rect = activeIconInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                if (currentState == IconState.Rewind)
                {
                    rect.localRotation = Quaternion.Euler(0f, 0f, -rotationOffset);
                }
                else
                {
                    rect.localRotation = Quaternion.Euler(0f, 0f, rotationOffset);
                }
            }
        }
    }

    /// <summary>
    /// Set icon size at runtime
    /// </summary>
    public void SetIconSize(Vector2 newSize)
    {
        iconSize = newSize;
        
        // Update current icon if one is active
        if (activeIconInstance != null)
        {
            RectTransform rect = activeIconInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = iconSize;
            }
        }
    }

    void OnDisable()
    {
        StopAllTransitions();
        DestroyActiveIcon();
        currentState = IconState.None;
    }

    void OnDestroy()
    {
        DestroyActiveIcon();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Update rotation in editor when value changes
        if (activeIconInstance != null)
        {
            RectTransform rect = activeIconInstance.GetComponent<RectTransform>();
            if (rect != null)
            {
                if (currentState == IconState.Rewind)
                {
                    rect.localRotation = Quaternion.Euler(0f, 0f, -rotationOffset);
                }
                else if (currentState == IconState.FastForward)
                {
                    rect.localRotation = Quaternion.Euler(0f, 0f, rotationOffset);
                }
            }
        }
    }
#endif

    // Public getters
    public bool IsShowingFastForward => currentState == IconState.FastForward;
    public bool IsShowingRewind => currentState == IconState.Rewind;
    public bool IsHidden => currentState == IconState.None;
    public bool IsTransitioning => isTransitioning;
}
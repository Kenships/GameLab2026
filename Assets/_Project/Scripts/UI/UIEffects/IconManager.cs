using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IconManager : MonoBehaviour
{
    [Header("=== Icon References ===")]
    [SerializeField] private Image fastForwardIcon;
    [SerializeField] private Image rewindIcon;

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
    private enum IconState { None, FastForward, Rewind }
    private IconState currentState = IconState.None;
    private Image activeIcon = null;
    private Coroutine transitionCoroutine;
    private Coroutine pulseCoroutine;
    private bool isTransitioning = false;

    // Original scales
    private Vector3 fastForwardOriginalScale;
    private Vector3 rewindOriginalScale;

    void Awake()
    {
        CacheOriginalScales();
        InitializeIcons();
    }

    void CacheOriginalScales()
    {
        if (fastForwardIcon != null)
            fastForwardOriginalScale = fastForwardIcon.transform.localScale;
        
        if (rewindIcon != null)
            rewindOriginalScale = rewindIcon.transform.localScale;
    }

    void InitializeIcons()
    {
        // Start with both icons hidden
        SetIconAlpha(fastForwardIcon, 0f);
        SetIconAlpha(rewindIcon, 0f);
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

        // Reset scales
        if (fastForwardIcon != null)
            fastForwardIcon.transform.localScale = fastForwardOriginalScale;
        
        if (rewindIcon != null)
            rewindIcon.transform.localScale = rewindOriginalScale;
    }

    IEnumerator TransitionToIcon(IconState targetState)
    {
        isTransitioning = true;

        Image targetIcon = GetIconForState(targetState);
        Image previousIcon = activeIcon;
        bool wasSwitching = currentState != IconState.None && targetState != IconState.None;

        // Fade out current icon if showing
        if (previousIcon != null && GetIconAlpha(previousIcon) > 0f)
        {
            float fadeDuration = wasSwitching ? switchFadeDuration : fadeOutDuration;
            yield return StartCoroutine(FadeIcon(previousIcon, GetIconAlpha(previousIcon), 0f, fadeDuration));
        }

        // Update state
        currentState = targetState;
        activeIcon = targetIcon;

        // Fade in new icon if not hiding
        if (targetIcon != null)
        {
            yield return StartCoroutine(FadeIcon(targetIcon, 0f, pulseMaxAlpha, fadeInDuration));

            // Start pulse
            if (enablePulse)
            {
                pulseCoroutine = StartCoroutine(PulseRoutine(targetIcon));
            }
        }

        isTransitioning = false;
        transitionCoroutine = null;
    }

    IEnumerator FadeIcon(Image icon, float fromAlpha, float toAlpha, float duration)
    {
        if (icon == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(fromAlpha, toAlpha, t);
            SetIconAlpha(icon, alpha);
            yield return null;
        }

        SetIconAlpha(icon, toAlpha);
    }

    IEnumerator PulseRoutine(Image icon)
    {
        if (icon == null) yield break;

        Vector3 originalScale = icon == fastForwardIcon ? fastForwardOriginalScale : rewindOriginalScale;
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * pulseSpeed;
            float t = (Mathf.Sin(time * Mathf.PI * 2f) + 1f) * 0.5f; // 0 to 1

            // Alpha pulse
            float alpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t);
            SetIconAlpha(icon, alpha);

            // Scale pulse (optional)
            if (pulseScale)
            {
                float scale = Mathf.Lerp(pulseScaleMin, pulseScaleMax, t);
                icon.transform.localScale = originalScale * scale;
            }

            yield return null;
        }
    }

    Image GetIconForState(IconState state)
    {
        return state switch
        {
            IconState.FastForward => fastForwardIcon,
            IconState.Rewind => rewindIcon,
            _ => null
        };
    }

    void SetIconAlpha(Image icon, float alpha)
    {
        if (icon == null) return;

        Color color = icon.color;
        color.a = alpha;
        icon.color = color;
    }

    float GetIconAlpha(Image icon)
    {
        if (icon == null) return 0f;
        return icon.color.a;
    }

    void OnDisable()
    {
        StopAllTransitions();
        
        // Reset icons
        SetIconAlpha(fastForwardIcon, 0f);
        SetIconAlpha(rewindIcon, 0f);
        
        currentState = IconState.None;
        activeIcon = null;
    }

    // Public getters
    public bool IsShowingFastForward => currentState == IconState.FastForward;
    public bool IsShowingRewind => currentState == IconState.Rewind;
    public bool IsHidden => currentState == IconState.None;
    public bool IsTransitioning => isTransitioning;
}
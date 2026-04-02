using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ModuleInfoManager : MonoBehaviour
{
    [Header("=== References ===")]
    [SerializeField] private Image moduleIcon;
    [SerializeField] private TMP_Text moduleNameText;

    [Header("=== Fade Settings ===")]
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float fadeOutDuration = 0.15f;
    [SerializeField] private float switchFadeDuration = 0.1f;

    [Header("=== Slide Settings ===")]
    [SerializeField] private bool enableSlide = true;
    [SerializeField] private float slideDistance = 50f;
    [SerializeField] private float slideDuration = 0.2f;
    [SerializeField] private AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve slideOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("=== Pulse Settings ===")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinAlpha = 0.7f;
    [SerializeField] private float pulseMaxAlpha = 1f;

    [Header("=== Scale Settings ===")]
    [SerializeField] private bool enableScalePulse = true;
    [SerializeField] private float scaleSpeed = 2f;
    [SerializeField] private float scaleMin = 0.95f;
    [SerializeField] private float scaleMax = 1.05f;

    // State
    private bool isShowing = false;
    private bool isTransitioning = false;
    private Coroutine transitionCoroutine;
    private Coroutine pulseCoroutine;

    // Cached values
    private RectTransform iconRect;
    private RectTransform textRect;
    private Vector2 iconOriginalPosition;
    private Vector2 textOriginalPosition;
    private Vector3 iconOriginalScale;
    private Vector3 textOriginalScale;

    // Current info
    private Texture2D currentTexture;
    private string currentName;

    void Awake()
    {
        CacheComponents();
        InitializeElements();
    }

    void CacheComponents()
    {
        if (moduleIcon != null)
        {
            iconRect = moduleIcon.GetComponent<RectTransform>();
            iconOriginalPosition = iconRect.anchoredPosition;
            iconOriginalScale = iconRect.localScale;
        }

        if (moduleNameText != null)
        {
            textRect = moduleNameText.GetComponent<RectTransform>();
            textOriginalPosition = textRect.anchoredPosition;
            textOriginalScale = textRect.localScale;
        }
    }

    void InitializeElements()
    {
        SetAlpha(0f);
        isShowing = false;
    }

    public void ShowInfo(Texture2D texture, string moduleName)
    {
        if (isShowing && currentTexture == texture && currentName == moduleName)
            return;

        StopAllTransitions();
        transitionCoroutine = StartCoroutine(TransitionToNewInfo(texture, moduleName));
    }

    public void Hide()
    {
        if (!isShowing && !isTransitioning)
            return;

        StopAllTransitions();
        transitionCoroutine = StartCoroutine(HideRoutine());
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

        ResetScales();
    }

    void ResetScales()
    {
        if (iconRect != null)
            iconRect.localScale = iconOriginalScale;

        if (textRect != null)
            textRect.localScale = textOriginalScale;
    }

    IEnumerator TransitionToNewInfo(Texture2D texture, string moduleName)
    {
        isTransitioning = true;

        // If already showing, slide out first
        if (isShowing)
        {
            yield return StartCoroutine(SlideOutAndFade());
        }

        // Update content
        ApplyInfo(texture, moduleName);

        // Slide in and fade
        yield return StartCoroutine(SlideInAndFade());

        // Start pulsing
        isShowing = true;
        isTransitioning = false;

        if (enablePulse || enableScalePulse)
        {
            pulseCoroutine = StartCoroutine(PulseRoutine());
        }

        transitionCoroutine = null;
    }

    IEnumerator HideRoutine()
    {
        isTransitioning = true;

        yield return StartCoroutine(SlideOutAndFade());

        isShowing = false;
        isTransitioning = false;
        currentTexture = null;
        currentName = null;
        transitionCoroutine = null;
    }

    void ApplyInfo(Texture2D texture, string moduleName)
    {
        currentTexture = texture;
        currentName = moduleName;

        if (moduleIcon != null && texture != null)
        {
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );
            moduleIcon.sprite = sprite;
        }

        if (moduleNameText != null)
        {
            moduleNameText.text = moduleName;
        }
    }

    IEnumerator SlideInAndFade()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(slideDuration, fadeInDuration);

        // Start positions (offset to the left)
        Vector2 iconStartPos = iconOriginalPosition - new Vector2(slideDistance, 0f);
        Vector2 textStartPos = textOriginalPosition - new Vector2(slideDistance, 0f);

        if (enableSlide)
        {
            if (iconRect != null) iconRect.anchoredPosition = iconStartPos;
            if (textRect != null) textRect.anchoredPosition = textStartPos;
        }

        SetAlpha(0f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Fade
            float fadeT = Mathf.Clamp01(elapsed / fadeInDuration);
            float alpha = Mathf.Lerp(0f, pulseMaxAlpha, fadeT);
            SetAlpha(alpha);

            // Slide
            if (enableSlide)
            {
                float slideT = slideInCurve.Evaluate(Mathf.Clamp01(elapsed / slideDuration));

                if (iconRect != null)
                    iconRect.anchoredPosition = Vector2.Lerp(iconStartPos, iconOriginalPosition, slideT);

                if (textRect != null)
                    textRect.anchoredPosition = Vector2.Lerp(textStartPos, textOriginalPosition, slideT);
            }

            yield return null;
        }

        // Ensure final values
        SetAlpha(pulseMaxAlpha);

        if (enableSlide)
        {
            if (iconRect != null) iconRect.anchoredPosition = iconOriginalPosition;
            if (textRect != null) textRect.anchoredPosition = textOriginalPosition;
        }
    }

    IEnumerator SlideOutAndFade()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(slideDuration, isShowing ? switchFadeDuration : fadeOutDuration);
        float fadeDuration = isShowing ? switchFadeDuration : fadeOutDuration;

        // End positions (offset to the right)
        Vector2 iconEndPos = iconOriginalPosition + new Vector2(slideDistance, 0f);
        Vector2 textEndPos = textOriginalPosition + new Vector2(slideDistance, 0f);

        Vector2 iconStartPos = iconRect != null ? iconRect.anchoredPosition : iconOriginalPosition;
        Vector2 textStartPos = textRect != null ? textRect.anchoredPosition : textOriginalPosition;
        float startAlpha = GetCurrentAlpha();

        StopPulse();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Fade
            float fadeT = Mathf.Clamp01(elapsed / fadeDuration);
            float alpha = Mathf.Lerp(startAlpha, 0f, fadeT);
            SetAlpha(alpha);

            // Slide
            if (enableSlide)
            {
                float slideT = slideOutCurve.Evaluate(Mathf.Clamp01(elapsed / slideDuration));

                if (iconRect != null)
                    iconRect.anchoredPosition = Vector2.Lerp(iconStartPos, iconEndPos, slideT);

                if (textRect != null)
                    textRect.anchoredPosition = Vector2.Lerp(textStartPos, textEndPos, slideT);
            }

            yield return null;
        }

        // Ensure final values
        SetAlpha(0f);

        // Reset positions for next show
        if (iconRect != null) iconRect.anchoredPosition = iconOriginalPosition;
        if (textRect != null) textRect.anchoredPosition = textOriginalPosition;
    }

    IEnumerator PulseRoutine()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime;

            // Alpha pulse
            if (enablePulse)
            {
                float alphaT = (Mathf.Sin(time * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
                float alpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, alphaT);
                SetAlpha(alpha);
            }

            // Scale pulse
            if (enableScalePulse)
            {
                float scaleT = (Mathf.Sin(time * scaleSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
                float scale = Mathf.Lerp(scaleMin, scaleMax, scaleT);

                if (iconRect != null)
                    iconRect.localScale = iconOriginalScale * scale;

                if (textRect != null)
                    textRect.localScale = textOriginalScale * scale;
            }

            yield return null;
        }
    }

    void SetAlpha(float alpha)
    {
        if (moduleIcon != null)
        {
            Color color = moduleIcon.color;
            color.a = alpha;
            moduleIcon.color = color;
        }

        if (moduleNameText != null)
        {
            Color color = moduleNameText.color;
            color.a = alpha;
            moduleNameText.color = color;
        }
    }

    float GetCurrentAlpha()
    {
        if (moduleIcon != null)
            return moduleIcon.color.a;

        if (moduleNameText != null)
            return moduleNameText.color.a;

        return 0f;
    }

    void OnDisable()
    {
        StopAllTransitions();
        SetAlpha(0f);
        ResetScales();

        if (iconRect != null) iconRect.anchoredPosition = iconOriginalPosition;
        if (textRect != null) textRect.anchoredPosition = textOriginalPosition;

        isShowing = false;
        isTransitioning = false;
        currentTexture = null;
        currentName = null;
    }

    // Public getters
    public bool IsShowing => isShowing;
    public bool IsTransitioning => isTransitioning;
    public string CurrentModuleName => currentName;
    public Texture2D CurrentTexture => currentTexture;
}
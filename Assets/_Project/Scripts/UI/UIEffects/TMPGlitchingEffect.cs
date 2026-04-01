using UnityEngine;
using TMPro;

public class TMPGlitchingEffect : MonoBehaviour
{
    [Header("=== Timing ===")]
    [SerializeField] private float glitchInterval = 0.05f;
    [SerializeField] private bool smoothReturn = true;
    [SerializeField] private float smoothSpeed = 15f;

    [Header("=== Position ===")]
    [SerializeField] private bool enablePositionGlitch = true;
    [SerializeField] private float positionIntensity = 3f;

    [Header("=== Scale ===")]
    [SerializeField] private bool enableScaleGlitch = true;
    [SerializeField] private float scaleIntensity = 0.01f;

    [Header("=== Rotation ===")]
    [SerializeField] private bool enableRotationGlitch = true;
    [SerializeField] private float rotationIntensity = 0.5f;

    [Header("=== Font Color ===")]
    [SerializeField] private bool enableColorGlitch = false;
    [SerializeField][Range(0f, 1f)] private float colorGlitchIntensity = 0.1f;

    [Header("=== Big Glitch Spikes ===")]
    [SerializeField][Range(0f, 1f)] private float bigGlitchChance = 0.05f;
    [SerializeField] private float bigGlitchMultiplier = 4f;

    // Components
    private RectTransform rectTransform;
    private TMP_Text tmpText;

    // Original values
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Color baseColor;

    // Target values
    private Vector2 targetPositionOffset;
    private float targetScaleOffset;
    private float targetRotationOffset;
    private Color targetColor;

    // Timer
    private float timer;

    // Public properties
    public float PositionIntensity { get => positionIntensity; set => positionIntensity = value; }
    public float ScaleIntensity { get => scaleIntensity; set => scaleIntensity = value; }
    public float RotationIntensity { get => rotationIntensity; set => rotationIntensity = value; }
    public float ColorGlitchIntensity { get => colorGlitchIntensity; set => colorGlitchIntensity = value; }
    public float BigGlitchChance { get => bigGlitchChance; set => bigGlitchChance = Mathf.Clamp01(value); }
    public float BigGlitchMultiplier { get => bigGlitchMultiplier; set => bigGlitchMultiplier = value; }
    public float GlitchInterval { get => glitchInterval; set => glitchInterval = value; }
    public Color BaseColor => baseColor;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tmpText = GetComponent<TMP_Text>();
    }

    void Start()
    {
        CacheOriginalValues();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= glitchInterval)
        {
            GenerateGlitch();
            timer = 0f;
        }

        ApplyGlitch();
    }

    void CacheOriginalValues()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;
        
        if (tmpText != null)
            baseColor = tmpText.color;
    }

    void GenerateGlitch()
    {
        float multiplier = Random.value < bigGlitchChance ? bigGlitchMultiplier : 1f;

        if (enablePositionGlitch)
        {
            targetPositionOffset = new Vector2(
                Random.Range(-positionIntensity, positionIntensity),
                Random.Range(-positionIntensity, positionIntensity)
            ) * multiplier;
        }

        if (enableScaleGlitch)
        {
            targetScaleOffset = Random.Range(-scaleIntensity, scaleIntensity) * multiplier;
        }

        if (enableRotationGlitch)
        {
            targetRotationOffset = Random.Range(-rotationIntensity, rotationIntensity) * multiplier;
        }

        if (enableColorGlitch && tmpText != null)
        {
            float intensity = colorGlitchIntensity * multiplier;
            targetColor = new Color(
                Mathf.Clamp01(baseColor.r + Random.Range(-intensity, intensity)),
                Mathf.Clamp01(baseColor.g + Random.Range(-intensity, intensity)),
                Mathf.Clamp01(baseColor.b + Random.Range(-intensity, intensity)),
                baseColor.a
            );
        }
    }

    void ApplyGlitch()
    {
        float t = smoothReturn ? Time.deltaTime * smoothSpeed : 1f;

        if (enablePositionGlitch)
        {
            Vector2 currentOffset = rectTransform.anchoredPosition - originalPosition;
            Vector2 newOffset = Vector2.Lerp(currentOffset, targetPositionOffset, t);
            rectTransform.anchoredPosition = originalPosition + newOffset;
        }

        if (enableScaleGlitch)
        {
            Vector3 targetScale = originalScale + Vector3.one * targetScaleOffset;
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, t);
        }

        if (enableRotationGlitch)
        {
            Quaternion targetRot = originalRotation * Quaternion.Euler(0f, 0f, targetRotationOffset);
            rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, targetRot, t);
        }

        if (enableColorGlitch && tmpText != null)
        {
            tmpText.color = Color.Lerp(tmpText.color, targetColor, t);
        }
    }

    void OnDisable()
    {
        ResetToOriginal();
    }

    public void ResetToOriginal()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localScale = originalScale;
            rectTransform.localRotation = originalRotation;
        }

        if (tmpText != null)
            tmpText.color = baseColor;
    }

    public void TriggerBigGlitch()
    {
        float multiplier = bigGlitchMultiplier * 2f;

        if (enablePositionGlitch)
        {
            targetPositionOffset = new Vector2(
                Random.Range(-positionIntensity, positionIntensity),
                Random.Range(-positionIntensity, positionIntensity)
            ) * multiplier;
        }

        if (enableScaleGlitch)
            targetScaleOffset = Random.Range(-scaleIntensity, scaleIntensity) * multiplier;

        if (enableRotationGlitch)
            targetRotationOffset = Random.Range(-rotationIntensity, rotationIntensity) * multiplier;

        if (enableColorGlitch && tmpText != null)
        {
            float intensity = colorGlitchIntensity * multiplier;
            targetColor = new Color(
                Mathf.Clamp01(baseColor.r + Random.Range(-intensity, intensity)),
                Mathf.Clamp01(baseColor.g + Random.Range(-intensity, intensity)),
                Mathf.Clamp01(baseColor.b + Random.Range(-intensity, intensity)),
                baseColor.a
            );
        }
    }

    public void SetBaseColor(Color color)
    {
        baseColor = color;
        
        if (tmpText != null)
            tmpText.color = color;
    }

    public void SetAllIntensities(float positionInt, float scaleInt, float rotationInt, float colorInt, float bigChance, float bigMult)
    {
        positionIntensity = positionInt;
        scaleIntensity = scaleInt;
        rotationIntensity = rotationInt;
        colorGlitchIntensity = colorInt;
        bigGlitchChance = Mathf.Clamp01(bigChance);
        bigGlitchMultiplier = bigMult;
    }
}
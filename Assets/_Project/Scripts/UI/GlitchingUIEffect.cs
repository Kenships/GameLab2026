using UnityEngine;
using UnityEngine.UI;

public class GlitchingUIEffect : MonoBehaviour
{
    
    [SerializeField] private float positionIntensity = 3f;
    [SerializeField] private float glitchInterval = 0.05f;
    [SerializeField] private bool enableScaleGlitch = true;
    [SerializeField] private float scaleIntensity = 0.01f;
    [SerializeField] private bool enableRotationGlitch = true;
    [SerializeField] private float rotationIntensity = 0.5f;
    [SerializeField][Range(0f, 1f)] private float bigGlitchChance = 0.05f;
    [SerializeField] private float bigGlitchMultiplier = 4f;
    [SerializeField] private bool enableColorGlitch = false;
    [SerializeField][Range(0f, 0.2f)] private float colorGlitchIntensity = 0.05f;
    [SerializeField] private Image targetImage;
    [SerializeField] private Text targetText; // Use TMPro if you have TextMeshPro
    [SerializeField] private bool smoothReturn = true;
    [SerializeField] private float smoothSpeed = 15f;

    // Private variables
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Color originalImageColor;
    private Color originalTextColor;
    
    private float glitchTimer;
    private Vector2 targetOffset;
    private float targetScaleOffset;
    private float targetRotationOffset;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Store original values
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;
        
        // Get image/text components if not assigned
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        if (targetText == null)
            targetText = GetComponentInChildren<Text>();
            
        if (targetImage != null)
            originalImageColor = targetImage.color;
        if (targetText != null)
            originalTextColor = targetText.color;
    }

    void Update()
    {
        glitchTimer += Time.deltaTime;
        
        if (glitchTimer >= glitchInterval)
        {
            GenerateGlitchValues();
            glitchTimer = 0f;
        }
        
        ApplyGlitch();
    }

    void GenerateGlitchValues()
    {
        // Check for big glitch
        float multiplier = Random.value < bigGlitchChance ? bigGlitchMultiplier : 1f;
        
        // Generate random offset values
        targetOffset = new Vector2(
            Random.Range(-positionIntensity, positionIntensity),
            Random.Range(-positionIntensity, positionIntensity)
        ) * multiplier;
        
        if (enableScaleGlitch)
        {
            targetScaleOffset = Random.Range(-scaleIntensity, scaleIntensity) * multiplier;
        }
        
        if (enableRotationGlitch)
        {
            targetRotationOffset = Random.Range(-rotationIntensity, rotationIntensity) * multiplier;
        }
        
        // Color glitch
        if (enableColorGlitch)
        {
            ApplyColorGlitch(multiplier);
        }
    }

    void ApplyGlitch()
    {
        if (smoothReturn)
        {
            // Smoothly interpolate to target values
            Vector2 currentOffset = rectTransform.anchoredPosition - originalPosition;
            Vector2 newOffset = Vector2.Lerp(currentOffset, targetOffset, Time.deltaTime * smoothSpeed);
            rectTransform.anchoredPosition = originalPosition + newOffset;
            
            if (enableScaleGlitch)
            {
                Vector3 currentScale = rectTransform.localScale;
                Vector3 targetScale = originalScale + new Vector3(targetScaleOffset, targetScaleOffset, 0);
                rectTransform.localScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * smoothSpeed);
            }
            
            if (enableRotationGlitch)
            {
                Quaternion targetRot = originalRotation * Quaternion.Euler(0, 0, targetRotationOffset);
                rectTransform.localRotation = Quaternion.Lerp(rectTransform.localRotation, targetRot, Time.deltaTime * smoothSpeed);
            }
        }
        else
        {
            // Instant application
            rectTransform.anchoredPosition = originalPosition + targetOffset;
            
            if (enableScaleGlitch)
            {
                rectTransform.localScale = originalScale + new Vector3(targetScaleOffset, targetScaleOffset, 0);
            }
            
            if (enableRotationGlitch)
            {
                rectTransform.localRotation = originalRotation * Quaternion.Euler(0, 0, targetRotationOffset);
            }
        }
    }

    void ApplyColorGlitch(float multiplier)
    {
        if (targetImage != null)
        {
            Color glitchedColor = originalImageColor;
            glitchedColor.r += Random.Range(-colorGlitchIntensity, colorGlitchIntensity) * multiplier;
            glitchedColor.g += Random.Range(-colorGlitchIntensity, colorGlitchIntensity) * multiplier;
            glitchedColor.b += Random.Range(-colorGlitchIntensity, colorGlitchIntensity) * multiplier;
            targetImage.color = glitchedColor;
        }
        
        if (targetText != null)
        {
            Color glitchedColor = originalTextColor;
            glitchedColor.r += Random.Range(-colorGlitchIntensity, colorGlitchIntensity) * multiplier;
            glitchedColor.g += Random.Range(-colorGlitchIntensity, colorGlitchIntensity) * multiplier;
            glitchedColor.b += Random.Range(-colorGlitchIntensity, colorGlitchIntensity) * multiplier;
            targetText.color = glitchedColor;
        }
    }

    // Reset to original state
    void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
            rectTransform.localScale = originalScale;
            rectTransform.localRotation = originalRotation;
        }
        
        if (targetImage != null)
            targetImage.color = originalImageColor;
        if (targetText != null)
            targetText.color = originalTextColor;
    }
    
    // Public method to trigger a manual big glitch
    public void TriggerBigGlitch()
    {
        targetOffset = new Vector2(
            Random.Range(-positionIntensity, positionIntensity),
            Random.Range(-positionIntensity, positionIntensity)
        ) * bigGlitchMultiplier * 2f;
    }
}
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core.HealthManagement;

public class ScoreAdjectiveManager : MonoBehaviour
{
    [Header("=== References ===")]
    [SerializeField] private TMPGlitchingEffect glitchEffect;
    [SerializeField] private Health health;
    
    [Header("=== Full Health Settings ===")]
    [SerializeField] private string fullHealthText = "Full Health";
    [SerializeField] private Color fullHealthColor = new Color(0.12f, 1.00f, 0.00f); // Green
    
    [Header("=== Adjectives (Stage 1-9) ===")]
    [SerializeField] private List<string> adjectives = new List<string>
    {
        "Horrid",    // Stage 1
        "Garbage",   // Stage 2
        "Okay",      // Stage 3
        "Great",     // Stage 4
        "Excellent", // Stage 5
        "Radical",   // Stage 6
        "Wicked",    // Stage 7
        "Tubular",   // Stage 8
        "Bomb"       // Stage 9
    };

    [Header("=== Rarity Colors (Stage 1-9) ===")]
    [SerializeField] private List<Color> rarityColors = new List<Color>
    {
        new Color(1.00f, 1.00f, 1.00f), // Stage 1 - White (Common)
        new Color(0.62f, 0.62f, 0.62f), // Stage 2 - Gray (Poor)
        new Color(0.12f, 1.00f, 0.00f), // Stage 3 - Green (Uncommon)
        new Color(0.00f, 0.44f, 0.87f), // Stage 4 - Blue (Rare)
        new Color(0.64f, 0.21f, 0.93f), // Stage 5 - Purple (Epic)
        new Color(1.00f, 0.50f, 0.00f), // Stage 6 - Orange (Legendary)
        new Color(1.00f, 0.00f, 0.50f), // Stage 7 - Hot Pink (Mythic)
        new Color(0.00f, 1.00f, 1.00f), // Stage 8 - Cyan (Cosmic)
        new Color(1.00f, 0.84f, 0.00f)  // Stage 9 - Gold (Divine)
    };

    [Header("=== Intensity Settings ===")]
    [SerializeField] private GlitchIntensitySettings minIntensity = new GlitchIntensitySettings
    {
        position = 0.5f,
        scale = 0.002f,
        rotation = 0.1f,
        color = 0.02f,
        bigGlitchChance = 0.01f,
        bigGlitchMultiplier = 2f
    };

    [SerializeField] private GlitchIntensitySettings maxIntensity = new GlitchIntensitySettings
    {
        position = 8f,
        scale = 0.03f,
        rotation = 2f,
        color = 0.2f,
        bigGlitchChance = 0.15f,
        bigGlitchMultiplier = 6f
    };

    [Header("=== Transition Settings ===")]
    [SerializeField] private bool enableTransition = true;
    [SerializeField] private float slideOutDuration = 0.15f;
    [SerializeField] private float slideInDuration = 0.2f;
    [SerializeField] private float slideDistance = 300f;
    [SerializeField] private AnimationCurve slideOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private AnimationCurve slideInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool addPunchScale = true;
    [SerializeField] private float punchScaleAmount = 0.2f;

    [Header("=== Display Duration Settings ===")]
    [SerializeField] private float adjectiveDisplayDuration = 3f; // How long the adjective stays visible

    [Header("=== Update Settings ===")]
    [SerializeField] private float updateInterval = 0.25f;

    [Header("=== Debug ===")]
    [SerializeField] private bool debugMode = false;
    [SerializeField] private float debugDamagePerSecond = 50f;

    // Components
    private TMP_Text tmpText;
    private RectTransform rectTransform;
    
    // State
    private int currentStage = 0;
    private float updateTimer;
    private float debugTimer;
    private float displayTimer;
    private Vector2 originalPosition;
    private bool isTransitioning = false;
    private bool isFirstUpdate = true;
    private bool hasEverTakenDamage = false;
    private bool isCurrentlyVisible = true;
    private bool isHiddenOffScreen = false;
    private float initialMaxHealth;
    private Coroutine transitionCoroutine;
    private Coroutine hideCoroutine;

    // Constants
    private const int MIN_STAGE = 1;
    private const int MAX_STAGE = 9;

    [System.Serializable]
    public struct GlitchIntensitySettings
    {
        public float position;
        public float scale;
        public float rotation;
        public float color;
        public float bigGlitchChance;
        public float bigGlitchMultiplier;

        public static GlitchIntensitySettings Lerp(GlitchIntensitySettings a, GlitchIntensitySettings b, float t)
        {
            return new GlitchIntensitySettings
            {
                position = Mathf.Lerp(a.position, b.position, t),
                scale = Mathf.Lerp(a.scale, b.scale, t),
                rotation = Mathf.Lerp(a.rotation, b.rotation, t),
                color = Mathf.Lerp(a.color, b.color, t),
                bigGlitchChance = Mathf.Lerp(a.bigGlitchChance, b.bigGlitchChance, t),
                bigGlitchMultiplier = Mathf.Lerp(a.bigGlitchMultiplier, b.bigGlitchMultiplier, t)
            };
        }
    }

    void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        
        // Store initial max health for comparison
        if (health != null)
        {
            initialMaxHealth = health.MaxHealth;
        }
        
        // Initialize with "Full Health" display
        ApplyFullHealthSettings();
    }

    void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval)
        {
            CheckForDamageAndUpdateStage();
            updateTimer = 0f;
        }

        if (debugMode)
        {
            HandleDebugDamage();
        }
    }

    void HandleDebugDamage()
    {
        if (health == null) return;

        debugTimer += Time.deltaTime;

        if (debugTimer >= 1f)
        {
            health.AddToHealth(-debugDamagePerSecond);
            debugTimer = 0f;
        }
    }

    void CheckForDamageAndUpdateStage()
    {
        if (health == null) return;

        // Check if damage has been taken for the first time
        if (!hasEverTakenDamage)
        {
            if (health.CurrentHealth < initialMaxHealth)
            {
                hasEverTakenDamage = true;
                // Transition from "Full Health" to the adjective system
                TransitionToAdjectiveSystem();
            }
            return;
        }

        // Normal stage update logic (only runs after damage has been taken)
        UpdateStage();
    }

    void TransitionToAdjectiveSystem()
    {
        int newStage = health.GetStageIndexFromHealth(health.CurrentHealth);
        newStage = Mathf.Clamp(newStage, MIN_STAGE, MAX_STAGE);
        currentStage = newStage;

        if (enableTransition && !isTransitioning)
        {
            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine);
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);
                
            transitionCoroutine = StartCoroutine(TransitionToAdjective());
        }
        else
        {
            ApplyStageSettings();
            StartHideTimer();
        }
    }

    void UpdateStage()
    {
        if (health == null) return;

        int newStage = health.GetStageIndexFromHealth(health.CurrentHealth);
        newStage = Mathf.Clamp(newStage, MIN_STAGE, MAX_STAGE);

        if (newStage != currentStage)
        {
            currentStage = newStage;
            
            if (enableTransition && !isTransitioning)
            {
                if (transitionCoroutine != null)
                    StopCoroutine(transitionCoroutine);
                if (hideCoroutine != null)
                    StopCoroutine(hideCoroutine);
                    
                transitionCoroutine = StartCoroutine(TransitionText());
            }
            else
            {
                ApplyStageSettings();
                StartHideTimer();
            }
        }
    }

    void ApplyFullHealthSettings()
    {
        // Set "Full Health" text
        if (tmpText != null)
        {
            tmpText.text = fullHealthText;
        }

        // Set full health color
        if (glitchEffect != null)
        {
            glitchEffect.SetBaseColor(fullHealthColor);
            
            // Use minimal glitch for full health (calm state)
            glitchEffect.SetAllIntensities(
                minIntensity.position,
                minIntensity.scale,
                minIntensity.rotation,
                minIntensity.color,
                minIntensity.bigGlitchChance,
                minIntensity.bigGlitchMultiplier
            );
        }

        isCurrentlyVisible = true;
        isHiddenOffScreen = false;
    }

    IEnumerator TransitionToAdjective()
    {
        isTransitioning = true;

        // Slide out "Full Health"
        yield return StartCoroutine(SlideOut());

        // Apply new adjective settings while off-screen
        ApplyStageSettings();

        // Slide in with new adjective
        yield return StartCoroutine(SlideIn());

        // Punch scale effect
        if (addPunchScale)
        {
            yield return StartCoroutine(PunchScale());
        }

        isTransitioning = false;
        isCurrentlyVisible = true;
        isHiddenOffScreen = false;

        // Start the hide timer
        StartHideTimer();
    }

    IEnumerator TransitionText()
    {
        isTransitioning = true;

        // If currently hidden, slide in from hidden position
        if (isHiddenOffScreen)
        {
            ApplyStageSettings();
            yield return StartCoroutine(SlideIn());
        }
        else
        {
            // Slide out to the right
            yield return StartCoroutine(SlideOut());

            // Apply new text and settings while off-screen
            ApplyStageSettings();

            // Slide in from the left
            yield return StartCoroutine(SlideIn());
        }

        // Punch scale effect
        if (addPunchScale)
        {
            yield return StartCoroutine(PunchScale());
        }

        isTransitioning = false;
        isCurrentlyVisible = true;
        isHiddenOffScreen = false;

        // Start the hide timer
        StartHideTimer();
    }

    void StartHideTimer()
    {
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
            
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(adjectiveDisplayDuration);

        // Only hide if we've taken damage (don't hide "Full Health")
        if (hasEverTakenDamage && !isTransitioning)
        {
            yield return StartCoroutine(SlideOutAndHide());
        }
    }

    IEnumerator SlideOutAndHide()
    {
        isTransitioning = true;

        yield return StartCoroutine(SlideOut());

        isCurrentlyVisible = false;
        isHiddenOffScreen = true;
        isTransitioning = false;
    }

    IEnumerator SlideOut()
    {
        float elapsed = 0f;
        Vector2 startPos = originalPosition;
        Vector2 endPos = originalPosition + new Vector2(slideDistance, 0f);

        while (elapsed < slideOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideOutCurve.Evaluate(elapsed / slideOutDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
    }

    IEnumerator SlideIn()
    {
        float elapsed = 0f;
        Vector2 startPos = originalPosition - new Vector2(slideDistance, 0f);
        Vector2 endPos = originalPosition;

        // Start from the left
        rectTransform.anchoredPosition = startPos;

        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideInCurve.Evaluate(elapsed / slideInDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
    }

    IEnumerator PunchScale()
    {
        float elapsed = 0f;
        float duration = 0.15f;
        Vector3 originalScale = rectTransform.localScale;
        Vector3 punchScale = originalScale * (1f + punchScaleAmount);

        // Scale up
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            rectTransform.localScale = Vector3.Lerp(originalScale, punchScale, t);
            yield return null;
        }

        // Scale back down
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            rectTransform.localScale = Vector3.Lerp(punchScale, originalScale, t);
            yield return null;
        }

        rectTransform.localScale = originalScale;
    }

    void ApplyStageSettings()
    {
        int index = currentStage - 1;

        // Update adjective text
        if (tmpText != null && index >= 0 && index < adjectives.Count)
        {
            tmpText.text = adjectives[index];
        }

        // Update rarity color
        if (glitchEffect != null && index >= 0 && index < rarityColors.Count)
        {
            glitchEffect.SetBaseColor(rarityColors[index]);
        }

        // Update glitch intensity
        if (glitchEffect != null)
        {
            float t = (currentStage - MIN_STAGE) / (float)(MAX_STAGE - MIN_STAGE);
            GlitchIntensitySettings settings = GlitchIntensitySettings.Lerp(minIntensity, maxIntensity, t);
            
            glitchEffect.SetAllIntensities(
                settings.position,
                settings.scale,
                settings.rotation,
                settings.color,
                settings.bigGlitchChance,
                settings.bigGlitchMultiplier
            );
        }
    }

    public void ForceUpdate()
    {
        currentStage = 0;
        CheckForDamageAndUpdateStage();
    }

    public void ResetToFullHealth()
    {
        hasEverTakenDamage = false;
        currentStage = 0;
        
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);
        if (transitionCoroutine != null)
            StopCoroutine(transitionCoroutine);

        // Reset position and apply full health settings
        rectTransform.anchoredPosition = originalPosition;
        ApplyFullHealthSettings();
    }

    public void ToggleDebugMode()
    {
        debugMode = !debugMode;
        debugTimer = 0f;
    }

    public void SetDebugMode(bool enabled)
    {
        debugMode = enabled;
        debugTimer = 0f;
    }

    public int GetCurrentStage() => currentStage;

    public bool HasTakenDamage() => hasEverTakenDamage;

    public bool IsVisible() => isCurrentlyVisible;

    public string GetCurrentAdjective()
    {
        if (!hasEverTakenDamage)
            return fullHealthText;
            
        int index = currentStage - 1;
        if (index >= 0 && index < adjectives.Count)
            return adjectives[index];
        return string.Empty;
    }

    public Color GetCurrentColor()
    {
        if (!hasEverTakenDamage)
            return fullHealthColor;
            
        int index = currentStage - 1;
        if (index >= 0 && index < rarityColors.Count)
            return rarityColors[index];
        return Color.white;
    }

    public void TriggerTransition()
    {
        if (!isTransitioning && hasEverTakenDamage)
        {
            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine);
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);
                
            transitionCoroutine = StartCoroutine(TransitionText());
        }
    }

    public void ForceShowAdjective()
    {
        if (hasEverTakenDamage && isHiddenOffScreen && !isTransitioning)
        {
            if (hideCoroutine != null)
                StopCoroutine(hideCoroutine);
                
            StartCoroutine(SlideInAndShow());
        }
    }

    IEnumerator SlideInAndShow()
    {
        isTransitioning = true;
        
        yield return StartCoroutine(SlideIn());
        
        if (addPunchScale)
        {
            yield return StartCoroutine(PunchScale());
        }
        
        isTransitioning = false;
        isCurrentlyVisible = true;
        isHiddenOffScreen = false;
        
        StartHideTimer();
    }
}
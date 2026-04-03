using UnityEngine;
using System.Collections;

public class ShakeEffect : MonoBehaviour
{
    [Header("=== Shake Settings ===")]
    [SerializeField] private float baseMagnitude = 0.5f;
    [SerializeField] private float baseDuration = 0.3f;
    [SerializeField] private float frequency = 25f;
    
    [Header("=== Damping ===")]
    [SerializeField] private bool useDamping = true;
    [SerializeField] private AnimationCurve dampingCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    
    [Header("=== Axis Control ===")]
    [SerializeField] private bool shakeX = true;
    [SerializeField] private bool shakeY = true;
    [SerializeField] private bool shakeZ = false;
    
    [Header("=== Rotation Shake ===")]
    [SerializeField] private bool enableRotationShake = false;
    [SerializeField] private float rotationMagnitude = 2f;

    // State
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine shakeCoroutine;
    private float currentIntensity = 0f;
    private bool isShaking = false;

    void Awake()
    {
        CacheOriginalTransform();
    }

    void CacheOriginalTransform()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    public void Shake(float intensity = 1f)
    {
        if (shakeCoroutine != null)
        {
            // If new shake is stronger, override current shake
            if (intensity > currentIntensity)
            {
                StopCoroutine(shakeCoroutine);
                shakeCoroutine = StartCoroutine(ShakeRoutine(intensity));
            }
        }
        else
        {
            shakeCoroutine = StartCoroutine(ShakeRoutine(intensity));
        }
    }

    public void ShakeWithDuration(float intensity, float duration)
    {
        if (shakeCoroutine != null)
        {
            if (intensity > currentIntensity)
            {
                StopCoroutine(shakeCoroutine);
                shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
            }
        }
        else
        {
            shakeCoroutine = StartCoroutine(ShakeRoutine(intensity, duration));
        }
    }

    IEnumerator ShakeRoutine(float intensity, float? customDuration = null)
    {
        isShaking = true;
        currentIntensity = intensity;
        
        float duration = customDuration ?? baseDuration;
        float elapsed = 0f;
        float magnitude = baseMagnitude * intensity;
        float rotMagnitude = rotationMagnitude * intensity;
        float seed = Random.value * 100f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            
            // Apply damping
            float dampMultiplier = useDamping ? dampingCurve.Evaluate(progress) : 1f;
            float currentMagnitude = magnitude * dampMultiplier;
            float currentRotMagnitude = rotMagnitude * dampMultiplier;

            // Calculate position offset using Perlin noise for smooth shake
            Vector3 offset = Vector3.zero;
            
            if (shakeX)
                offset.x = (Mathf.PerlinNoise(seed, elapsed * frequency) - 0.5f) * 2f * currentMagnitude;
            if (shakeY)
                offset.y = (Mathf.PerlinNoise(seed + 1f, elapsed * frequency) - 0.5f) * 2f * currentMagnitude;
            if (shakeZ)
                offset.z = (Mathf.PerlinNoise(seed + 2f, elapsed * frequency) - 0.5f) * 2f * currentMagnitude;

            transform.localPosition = originalPosition + offset;

            // Apply rotation shake
            if (enableRotationShake)
            {
                float rotX = (Mathf.PerlinNoise(seed + 3f, elapsed * frequency) - 0.5f) * 2f * currentRotMagnitude;
                float rotY = (Mathf.PerlinNoise(seed + 4f, elapsed * frequency) - 0.5f) * 2f * currentRotMagnitude;
                float rotZ = (Mathf.PerlinNoise(seed + 5f, elapsed * frequency) - 0.5f) * 2f * currentRotMagnitude;
                
                transform.localRotation = originalRotation * Quaternion.Euler(rotX, rotY, rotZ);
            }

            yield return null;
        }

        // Reset to original
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        
        isShaking = false;
        currentIntensity = 0f;
        shakeCoroutine = null;
    }

    public void StopShake()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            shakeCoroutine = null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        
        isShaking = false;
        currentIntensity = 0f;
    }

    public void UpdateOriginalPosition()
    {
        if (!isShaking)
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
        }
    }

    public bool IsShaking => isShaking;
    public float CurrentIntensity => currentIntensity;

    void OnDisable()
    {
        StopShake();
    }
}
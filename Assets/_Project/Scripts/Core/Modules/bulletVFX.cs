using UnityEngine;

public class BulletVFX : MonoBehaviour
{
    [Header("Effect Toggles")]
    [SerializeField] private bool enableGlow = true;
    [SerializeField] private bool enableTrail = true;

    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] [Range(0f, 10f)] private float glowIntensity = 2f;
    [SerializeField] [Range(0.5f, 10f)] private float glowRange = 2f;

    [Header("Trail Settings")]
    [SerializeField] private Color trailStartColor = Color.yellow;
    [SerializeField] private Color trailEndColor = Color.clear;
    [SerializeField] [Range(0.05f, 2f)] private float trailTime = 0.3f; 
    [SerializeField] [Range(0.01f, 1f)] private float trailStartWidth = 0.2f; //controlled by bullet preset type
    [SerializeField] [Range(0f, 0.5f)] private float trailEndWidth = 0f;//controlled by bullet preset type
    [SerializeField] private Material trailMaterial;

    [Header("Bullet Type Preset")]
    [SerializeField] private BulletType bulletType = BulletType.Regular;

    private enum BulletType
    {
        Regular,
        Disc,
        Custom
    }

    private Renderer bulletRenderer;
    private TrailRenderer trailRenderer;
    private Light pointLight;

    private void Awake()
    {
        bulletRenderer = GetComponent<Renderer>();
        
        if (bulletType != BulletType.Custom)
        {
            ApplyPreset(bulletType);
        }
    }

    private void Start()
    {
        if (enableGlow) SetupGlow();
        if (enableTrail) SetupTrail();
    }

    private void ApplyPreset(BulletType type)
    {
        switch (type)
        {
            case BulletType.Regular:
                trailStartWidth = 0.08f;
                trailEndWidth = 0f;
                break;

            case BulletType.Disc:
                trailStartWidth = 0.3f;
                trailEndWidth = 0.05f;
                break;
        }
    }

    private void SetupGlow()
    {
        pointLight = gameObject.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.color = glowColor;
        pointLight.intensity = glowIntensity;
        pointLight.range = glowRange;
        pointLight.renderMode = LightRenderMode.Auto;

        if (bulletRenderer != null)
        {
            bulletRenderer.material.EnableKeyword("_EMISSION");
            bulletRenderer.material.SetColor("_EmissionColor", glowColor * glowIntensity);
        }
    }

    private void SetupTrail()
    {
        trailRenderer = gameObject.AddComponent<TrailRenderer>();
        
        trailRenderer.time = trailTime;
        trailRenderer.startWidth = trailStartWidth;
        trailRenderer.endWidth = trailEndWidth;
        trailRenderer.minVertexDistance = 0.05f;
        
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(trailStartColor, 0f),
                new GradientColorKey(trailEndColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trailRenderer.colorGradient = gradient;

        if (trailMaterial != null)
        {
            trailRenderer.material = trailMaterial;
        }
        else
        {
            trailRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        }

        trailRenderer.sortingOrder = 100;
    }
}
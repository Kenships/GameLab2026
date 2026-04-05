using _Project.Scripts.Core.HealthManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float duration = 0.1f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private Health health;
    private float prevHealth;

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }
    private void Start()
    {
        health = GetComponent<Health>();
        health.OnHealthChanged += Flash;
    }
    private void OnDisable()
    {
        health.OnHealthChanged -= Flash;
    }

    public void Flash(float useless)
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        propBlock.SetColor(ColorId, flashColor);
        foreach (var r in renderers)
        {
            r.SetPropertyBlock(propBlock);
        }

        yield return new WaitForSeconds(duration);

        propBlock.Clear();
        foreach (var r in renderers)
        {
            r.SetPropertyBlock(propBlock);
        }
    }
}
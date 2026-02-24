using _Project.Scripts.Core.HealthManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : PickupObjectBase
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float emissionRateToAngleRatio = 14f;
    [SerializeField] private float angleMultiplier = 0.6f;
    [SerializeField] private float radiusMultiplier = 1.2f;

    [Header("Flamethrower Settings")]
    [SerializeField] private float normalDamage = 10f;
    [SerializeField] private float timesOfNormalDamagePerSec = 4f;
    [SerializeField] private float fastForwardingDamage = 15f;
    [SerializeField] private float timesOfFastForwardingDamagePerSec = 8f;

    [Header("Optimization")]
    [SerializeField] private float detectionInterval = 0.25f;
    private float detectionTimer;

    private float currentDamage;
    private float currentTimesOfDamagePerSec;
    private Color originalColor;
    private RangeDetector rangeDetector; // rangeType is sector
    private List<Transform> enemies;

    private bool isDamagingEnemies = false;

    private void Start()
    {
        currentDamage = normalDamage;
        currentTimesOfDamagePerSec = timesOfNormalDamagePerSec;

        var main = particle.main;
        originalColor = main.startColor.color;

        enemies = new List<Transform>();
        rangeDetector = GetComponent<RangeDetector>();
        if (!rangeDetector)
        {
            Debug.Log("missing rangeDetector");
            return;
        }
        if (!particle)
        {
            Debug.Log("missing particle");
            return;
        }

        UpdateAngle(rangeDetector.angle * angleMultiplier);
        UpdateDistance(rangeDetector.radius * radiusMultiplier);
    }

    private void UpdateAngle(float angle)
    {
        var shape = particle.shape;
        shape.angle = angle;

        var emission = particle.emission;
        emission.rateOverTime = angle * emissionRateToAngleRatio;
    }

    private void UpdateDistance(float distance)
    {
        var main = particle.main;

        float currentLifetime = main.startLifetime.constant;

        main.startSpeed = distance / currentLifetime;
    }
    private void Update()
    {
        detectionTimer += Time.deltaTime;
        if (detectionTimer >= detectionInterval)
        {
            detectionTimer = 0f;

            if (!isDamagingEnemies && currentDamage != 0)
            {
                enemies = rangeDetector.GetTransformsInRange();
                if (enemies.Count > 0)
                {
                    isDamagingEnemies = true;
                    StartCoroutine(DamageEnemiesCycle());
                }
            }
        }
    }
    private IEnumerator DamageEnemiesCycle()
    {
        while (true)
        {
            enemies = rangeDetector.GetTransformsInRange();

            if (enemies.Count <= 0 || currentDamage == 0)
            {
                isDamagingEnemies = false;
                yield break;
            }

            foreach(Transform t in enemies)
            {
                t.GetComponent<IDamageable>().Damage(currentDamage);
            }

            yield return new WaitForSeconds(1f / currentTimesOfDamagePerSec);
        }
    }
    protected override void ActByState()
    {
        switch (state)
        {
            case State.Load:
                RestoreOriginalColor();
                particle.Play();
                currentDamage = normalDamage;
                currentTimesOfDamagePerSec = timesOfNormalDamagePerSec;
                break;
            case State.Attack:
                MakeDarkRed();
                currentDamage = fastForwardingDamage;
                currentTimesOfDamagePerSec = timesOfFastForwardingDamagePerSec;
                break;
            case State.Used:
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                currentDamage = 0;
                break;
        }
    }
    private void MakeDarkRed()
    {
        var main = particle.main;
        Color currentColor = main.startColor.color;

        main.startColor = new Color(
            Mathf.Min(currentColor.r * 1.2f, 1f),
            currentColor.g * 0.2f,
            currentColor.b * 0.2f,
            currentColor.a
        );
    }

    private void RestoreOriginalColor()
    {
        var main = particle.main;
        main.startColor = originalColor;
    }
}

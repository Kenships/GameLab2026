using _Project.Scripts.Core.HealthManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamethrower : PickupObjectBase
{
    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private float emissionRateToAngleRatio = 16f;
    // The relationship between rangeDetector.angle and particle angle can't be represented by a simple function, 
    // so everytime you change rangeDetector.angle, you have to also adjust angleMultiplier to have desired particle effect
    [SerializeField] private float angleMultiplier = 0.6f;
    // The relationship between rangeDetector.radius and particle radius can't be represented by a simple function, 
    // so everytime you change rangeDetector.angle, you have to also adjust angleMultiplier to have desired particle effect
    [SerializeField] private float radiusMultiplier = 3f;
    [SerializeField] private float emissionRateToAngleRatio_fast = 32f;
    [SerializeField] private float radiusMultiplier_fast = 3.75f;

    [Header("Flamethrower Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float dps_normal = 4f;
    [SerializeField] private float dps_fast = 8f;
    [SerializeField] private float radius_fast = 10;
    [SerializeField] protected float attackStateDuration = 15f;

    private float currentDamage;
    private float currentDps;
    private float radius_normal;
    private RangeDetector rangeDetector; // rangeType is sector
    private List<Transform> enemies;
    private Coroutine attackCoroutine;
    private bool isDamagingEnemies = false;

    private void Start()
    {
        currentDamage = damage;
        currentDps = dps_normal;
        var main = particle.main;
        enemies = new List<Transform>();

        rangeDetector = GetComponent<RangeDetector>();
        if (!rangeDetector)
        {
            Debug.Log("missing rangeDetector");
            return;
        }

        radius_normal = rangeDetector.radius;

        if (!particle)
        {
            Debug.Log("missing particle");
            return;
        }

        UpdateParticleAngle(rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio);
        UpdateDistance(rangeDetector.radius * radiusMultiplier);
    }

    private void UpdateParticleAngle(float angle, float emissionRateToAngleRatio)
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

            yield return new WaitForSeconds(1f / currentDps);
        }
    }
    protected override void LoadState()
    {
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        UpdateParticleAngle(rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio);
        rangeDetector.radius = radius_normal;
        UpdateDistance(rangeDetector.radius * radiusMultiplier);
        particle.Play();
        currentDamage = damage;
        currentDps = dps_normal;
    }
    protected override void AttackState()
    {
        currentDps = dps_fast;
        UpdateParticleAngle(rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio_fast);
        rangeDetector.radius = radius_fast;
        UpdateDistance(rangeDetector.radius * radiusMultiplier_fast);
        attackCoroutine = StartCoroutine(AttackStateCoroutine());
    }
    protected override void UsedState()
    {
        particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        currentDamage = 0;
        attackCoroutine = null;
    }
    private IEnumerator AttackStateCoroutine()
    {
        yield return new WaitForSeconds(attackStateDuration);
        state = State.Used;
        ActByState();
    }
}

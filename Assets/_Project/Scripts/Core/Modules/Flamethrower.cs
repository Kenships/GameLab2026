using _Project.Scripts.Core.HealthManagement;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Util.Timer.Timers;
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

    private float _currentDamage;
    private float _currentDps;
    private float _normalRadius;
    private RangeDetector _rangeDetector; // rangeType is sector
    private List<Transform> _enemies;
    private Coroutine _attackCoroutine;
    private bool _isDamagingEnemies;
    private CountdownTimer _attackCooldownTimer;

    private void Start()
    {
        _currentDamage = damage;
        _currentDps = dps_normal;
        _attackCooldownTimer = new CountdownTimer(1f/_currentDps);
        _enemies = new List<Transform>();

        _rangeDetector = GetComponent<RangeDetector>();
        if (!_rangeDetector)
        {
            Debug.Log("missing rangeDetector");
            return;
        }

        _normalRadius = _rangeDetector.radius;

        if (!particle)
        {
            Debug.Log("missing particle");
            return;
        }

        UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio);
        UpdateDistance(_rangeDetector.radius * radiusMultiplier);
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
    private void FixedUpdate()
    {
        if (_currentDamage == 0 || _attackCooldownTimer.IsRunning)
        {
            return;
        }
        _enemies = _rangeDetector.GetTransformsInRange();

        if (_enemies.Count <= 0)
        {
            return;
        }

        foreach(Transform t in _enemies)
        {
            //potentially cache IDamageables for better performance
            t.GetComponent<IDamageable>().Damage(_currentDamage);
            _attackCooldownTimer.Reset(1f/_currentDps);
        }
    }
    
    protected override void LoadState()
    {
        if (_attackCoroutine != null) StopCoroutine(_attackCoroutine);
        UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio);
        _rangeDetector.radius = _normalRadius;
        UpdateDistance(_rangeDetector.radius * radiusMultiplier);
        particle.Play();
        _currentDamage = damage;
        _currentDps = dps_normal;
    }
    protected override void AttackState()
    {
        _currentDps = dps_fast;
        UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio_fast);
        _rangeDetector.radius = radius_fast;
        UpdateDistance(_rangeDetector.radius * radiusMultiplier_fast);
        _attackCoroutine = StartCoroutine(AttackStateCoroutine());
    }
    protected override void UsedState()
    {
        particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        _currentDamage = 0;
        _attackCoroutine = null;
    }
    private IEnumerator AttackStateCoroutine()
    {
        yield return new WaitForSeconds(attackStateDuration);
        state = State.Used;
        ActByState();
    }
}

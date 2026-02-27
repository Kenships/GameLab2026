using System;
using _Project.Scripts.Core.HealthManagement;
using System.Collections.Generic;
using _Project.Scripts.Util.ExtensionMethods;
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

    [Header("Time Settings")]
    
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float defaultRecoverySpeed = 10f;
    [SerializeField] private float rewindRecoveryMultiplier = 4f;
    [SerializeField] private float defaultDecaySpeed = 5f;
    [SerializeField] private float attackStateDecayMultiplier = 4f;
    

    private float _currentDamage;
    private float _currentDps;
    private float _normalRadius;
    private RangeDetector _rangeDetector; // rangeType is sector
    private List<Transform> _enemies;
    private bool _isDamagingEnemies;
    private CountdownTimer _attackCooldownTimer;
    
    private Health _health;

    private bool _isRewinding;

    private void Awake()
    {
        _health = gameObject.GetOrAdd<Health>();
        _health.Initialize(maxHealth);
    }

    private void Start()
    {
        _health.OnDeath += OnDeath;
        _health.OnFullHp += OnFullHp;
        
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

        state = ModuleState.Load;
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

    private void PerformAttack()
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

    private void OnDeath()
    {
        state = ModuleState.Used;
    }

    private void OnFullHp()
    {
        state = ModuleState.Load;
    }

    #region State Methods
    protected override void LoadState()
    {
        PerformAttack();

        if (_isRewinding)
        {
            _health.AddToHealth(defaultRecoverySpeed * rewindRecoveryMultiplier * Time.deltaTime);
        }
        else
        {
            _health.AddToHealth(- defaultDecaySpeed * Time.deltaTime);
        }
        
    }
    protected override void AttackState()
    {
        PerformAttack();
        _health.AddToHealth(-defaultDecaySpeed * attackStateDecayMultiplier * Time.deltaTime);
    }
    protected override void UsedState()
    {
        if (_isRewinding)
        {
            _health.AddToHealth(defaultRecoverySpeed * rewindRecoveryMultiplier * Time.deltaTime);
        }
        else
        {
            _health.AddToHealth(defaultRecoverySpeed * Time.deltaTime);
        }
    }
    
    protected override void OnStateChanged(ModuleState newState)
    {
        switch (newState)
        {
            case ModuleState.Load :
                UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio);
                _rangeDetector.radius = _normalRadius;
                UpdateDistance(_rangeDetector.radius * radiusMultiplier);
                particle.Play();
                _currentDamage = damage;
                _currentDps = dps_normal;
                break;
            case ModuleState.Attack :
                _currentDps = dps_fast;
                UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio_fast);
                _rangeDetector.radius = radius_fast;
                UpdateDistance(_rangeDetector.radius * radiusMultiplier_fast);
                break;
            case ModuleState.Used :
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                _currentDamage = 0;
                break;
        }
    }

    #endregion

    public override void Rewind()
    {
        transform.localScale = 1.05f * Vector3.one;
        _isRewinding = !_isRewinding && state != ModuleState.Attack;
    }

    public override void CancelRewind()
    {
        transform.localScale = Vector3.one;
        _isRewinding = false;
    }
    
    public override void FastForward()
    {
        if (_isRewinding || state == ModuleState.Used)
            return;
        
        transform.localScale = 1.05f * Vector3.one;
        
        state = ModuleState.Attack;
    }

    public override void CancelFastForward()
    {
        if (state == ModuleState.Used)
            return;
        
        transform.localScale = Vector3.one;
        
        state = ModuleState.Load;
    }

    
}

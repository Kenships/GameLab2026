using System;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.HealthManagement;
using UnityEngine;

public class Car : Module
{

    [SerializeField] private float fastForwardSpeed = 15f;
    [SerializeField] private float rewindSpeed = 10f;
    [SerializeField] private float rewindDistanceLimit = 5f;
    [SerializeField] private float rewindDamage = 10f;
    [SerializeField] private float fastFowardDamage = 20f;

    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private RangeDetector _rangeDetector;

    private Vector3 _rewindLimitPosition;
    private Vector3 _originalPosition;
    private Coroutine _rewindCoroutine;
    private Coroutine _fastForwardCoroutine;
    private bool fastForwarding = false;
    private bool rewinding = false;
    private List<IDamageable> _enemies;
    private float damage = 0f;
    void Start()
    {
        _enemies = new List<IDamageable>();
        _originalPosition = transform.position;
        _rewindLimitPosition = _originalPosition + new Vector3(0, 0, rewindDistanceLimit);
        _rangeDetector.OnObjectEnter += OnEnter;
    }

    protected override void LoadState()
    {

    }

    protected override void AttackState()
    {
        attack();
    }

    protected override void UsedState()
    {

    }

    protected override void OnStateChanged(ModuleState newState)
    {

    }

    public override void FastForward()
    {
        if (_fastForwardCoroutine != null)
        {
            StopCoroutine(_fastForwardCoroutine);
        }

        fastForwarding = true;
        state = ModuleState.Attack;
        _fastForwardCoroutine = StartCoroutine(FastForwardCoroutine());
    }

    public override void CancelFastForward()
    {
        if (_fastForwardCoroutine != null)
        {
            StopCoroutine(_fastForwardCoroutine);
            _fastForwardCoroutine = null;
        }

        fastForwarding = false;
        state = ModuleState.None;
        damage = 0;

    }

    private IEnumerator FastForwardCoroutine()
    {
        while (transform.position != _originalPosition)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _originalPosition,
                fastForwardSpeed * Time.deltaTime
            );

            yield return null;
        }

        _fastForwardCoroutine = null;
    }

    public override void Rewind()
    {
        if (_rewindCoroutine != null)
        {
            StopCoroutine(_rewindCoroutine);
        }

        rewinding = true;
        state = ModuleState.Attack;
        _rewindCoroutine = StartCoroutine(RewindCoroutine());
        
    }

    public override void CancelRewind()
    {
        if (_rewindCoroutine != null)
        {
            StopCoroutine(_rewindCoroutine);
            _rewindCoroutine = null;
        }

        rewinding = false;
        state = ModuleState.None;
        damage = 0;
    }

    private IEnumerator RewindCoroutine()
    {
        while (transform.position != _rewindLimitPosition)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _rewindLimitPosition,
                rewindSpeed * Time.deltaTime
            );

            yield return null;
        }

        _rewindCoroutine = null;
    }

    private void OnCollisionEnter(Collision other)
    {

        if ((_enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("enemy detected");

            if (other.gameObject.TryGetComponent(out IDamageable damageable))
            {
                if (fastForwarding == true)
                {
                    damageable.Damage(fastFowardDamage);
                }

                if (rewinding == true)
                {
                    damageable.Damage(rewindDamage);
                }

            }
        }
    }
    
    public override void ShowVisual(PlayerData.PlayerID playerID)
    {
        
    }

    public override void HideVisual(PlayerData.PlayerID playerID)
    {
      
    }

    private void attack()
    {
        
        if (fastForwarding == true)
        {
            damage = fastFowardDamage;
        }

        if (rewinding == true)
        {
            damage = rewindDamage;
        }
        
        _rangeDetector.GetObjectTypeInRangeNoAlloc(_enemies);

        
    }

    private void OnEnter(Collider col)
    {
        if (_enemies.Count <= 0)
        {
            return;
        }

        foreach(IDamageable enemy in _enemies)
        {
            enemy.Damage(50);
        }
    }
}

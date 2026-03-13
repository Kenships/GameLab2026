using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Core;
using _Project.Scripts.Core.HealthManagement;
using UnityEngine;
using UnityEngine.Serialization;

public class Car : Module
{
    [SerializeField] private Transform destination;
    [SerializeField] private float fastForwardSpeed = 15f;
    [SerializeField] private float rewindSpeed = 10f;
    [SerializeField] private float rewindDamage = 10f;
    [SerializeField] private float fastForwardDamage = 20f;
    [SerializeField] private RangeDetector backDetector;
    [SerializeField] private RangeDetector frontDetector;
    
    
    private Vector3 _originalPosition;
    private bool _fastForwarding = false;
    private bool _rewinding = false;
    private List<IDamageable> _enemies;
    private float _damage;
    
    void Start()
    {
        _enemies = new List<IDamageable>();
        _originalPosition = transform.position;
        destination.transform.position = new Vector3(0, 0, destination.transform.position.z);
        backDetector.OnObjectEnter += OnEnter;
        frontDetector.OnObjectEnter += OnEnter;
    }

    protected override void LoadState()
    {
        
    }

    protected override void AttackState()
    {
        if (_fastForwarding)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                destination.transform.position,
                rewindSpeed * Time.deltaTime
            );
        }

        if (_rewinding)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _originalPosition,
                fastForwardSpeed * Time.deltaTime
            );
        }
        
    }

    protected override void UsedState()
    {

    }

    protected override void OnStateChanged(ModuleState newState)
    {

    }

    public override void FastForward()
    {
        // if (_fastForwardCoroutine != null)
        // {
        //     StopCoroutine(_fastForwardCoroutine);
        // }
        //
        // _fastForwarding = true;
        // state = ModuleState.Attack;
        // _fastForwardCoroutine = StartCoroutine(FastForwardCoroutine());
        
        
    }

    public override void CancelFastForward()
    {
        _fastForwarding = false;
        state = ModuleState.None;
        _damage = 0;
    }

    public override void Rewind()
    {
        // if (_rewindCoroutine != null)
        // {
        //     StopCoroutine(_rewindCoroutine);
        // }
        //
        // _rewinding = true;
        // state = ModuleState.Attack;
        // _rewindCoroutine = StartCoroutine(RewindCoroutine());
        //

        state = ModuleState.Attack;
        _rewinding = true;
    }

    public override void CancelRewind()
    {
        state = ModuleState.Attack;
        _rewinding = false;
    }
    
    
    public override void ShowVisual(PlayerData.PlayerID playerID)
    {
        
    }

    public override void HideVisual(PlayerData.PlayerID playerID)
    {
      
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

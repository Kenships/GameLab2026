using System;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using System.Collections;
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
    
    private Vector3 _rewindLimitPosition;
    private bool fullyRewound = false; 
    private Vector3 _originalPosition;
    private ModuleState _state = ModuleState.Used;
    private Coroutine _rewindCoroutine;
    private Coroutine _fastForwardCoroutine;
    private bool fastForwarding = false;
    private bool rewinding = false;
    
    void Start()
    {
        _originalPosition = transform.position;
        _rewindLimitPosition = _originalPosition + new Vector3(0,0,rewindDistanceLimit);
        
    }
    protected override void LoadState()
    {
        
    }

    protected override void AttackState()
    {
        
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
        _rewindCoroutine = StartCoroutine(RewindCoroutine());
    }

    public override void CancelRewind()
    {
        if (_rewindCoroutine != null)
        {
            StopCoroutine(_rewindCoroutine);
            _rewindCoroutine = null;
        }
        rewinding =  false;
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

    private void OnTriggerEnter(Collider other)
    {
        if ((_enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log(other.gameObject.name);
            if (other.TryGetComponent(out IDamageable damageable))
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
    

    
}

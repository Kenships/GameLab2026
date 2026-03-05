using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Inflictors;
using _Project.Scripts.Util.ExtensionMethods;
using Sisus.Init;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

public class RecordDiscBullet : MonoBehaviour<AudioPooler>
{
    private float _rotateSpeed = 200f; //takes from disc shooter
    private float _wobbleAmount = 0f; //takes from disc shooter 
    [SerializeField] private EnemyEffectInflictor inflictor;
    [SerializeField] private float hitCooldown = 0.2f; //cooldown before it starts detecting another trigger on hit
    [SerializeField] private float enemySearchRadius = 30f; //the radius that it searches the next closest enemy if theres none it will go back to previous enemy
    [SerializeField] private float bulletLifetime = 3.5f; // max seconds until bullet is destroyed regardless of how many targets it hit
    [SerializeField] private int bulletDamage = -35;
    private Transform _target;
    private Transform _lastKnownTarget;
    private float _speed;
    private int _maxTargets;
    private int _hitCount;
    private float _hitTimer;
    private Rigidbody _rb;
    private LayerMask _enemyLayer;
    private bool _isNormalTurret;
    
    private AudioPooler _audioPooler;
    
    protected override void Init(AudioPooler audioPooler)
    {
        _audioPooler = audioPooler;
    }

    public void Initialize(Transform target, float speed, int maxTargets, float rotateSpeed, float wobbleAmount, LayerMask enemyLayer, bool isNormal)
    {
        _rotateSpeed = rotateSpeed;
        _target = target;
        _lastKnownTarget = target;
        _speed = speed;
        _maxTargets = maxTargets;
        _hitCount = 0;
        _wobbleAmount = wobbleAmount;
        _enemyLayer = enemyLayer;
        Destroy(gameObject, bulletLifetime);
        _isNormalTurret = isNormal;

        _rb = GetComponent<Rigidbody>();
        if (_rb)
        {
            if (_wobbleAmount <= 0f || _isNormalTurret)
            {
                _rb.freezeRotation = true;
            }
            else
            {
                _rb.angularDamping = Mathf.Lerp(100f, 0f, wobbleAmount);
            }

            _rb.useGravity = false;
            _rb.isKinematic = true;
        }
    }

    public void SetWobble(float amount)
    {
        _wobbleAmount = amount;

        if (_rb)
        {
            if (_wobbleAmount <= 0f)
            {
                _rb.freezeRotation = true;
            }
            else
            {
                _rb.freezeRotation = false;
                _rb.angularDamping = Mathf.Lerp(100f, 0f, _wobbleAmount);
            }
        }
    }

    private void Update()
    {
        _hitTimer -= Time.deltaTime;

        if (_isNormalTurret)
        {
            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
            return;
        }

        FindClosestEnemy();
        Transform homingTarget = _target;

        // If no enemies, home back to latest target
        if (!homingTarget)
        {
            homingTarget = _lastKnownTarget;
        }

        // if the OG is dead too then just go straight
        if (!homingTarget)
        {
            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
            return;
        }

        Vector3 direction = (homingTarget.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            _rotateSpeed * Time.deltaTime
        );

        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
    }

    private void FindClosestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, enemySearchRadius, _enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy)
        {
            _target = closestEnemy;
            _lastKnownTarget = closestEnemy;
        }
        else
        {
            _target = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (_hitTimer > 0f) return;

        if (!other.IsOnLayer(_enemyLayer))
        {
            return;
        }

        if (other.TryGetComponent(out EnemyBase enemy))
        {
            Inflict(enemy);

            _hitCount++;
            _hitTimer = hitCooldown;

            if (_isNormalTurret || _hitCount >= _maxTargets)
            {
                Destroy(gameObject);
            }
        }
    }
    
    private void Inflict(EnemyBase enemy)
    {
        if (inflictor == null)
        {
            Debug.LogWarning("Inflector is null");
            return;
        }
        
        inflictor.Inflict(enemy);
        
        if (inflictor.CastVFX)
        {
            Instantiate(inflictor.CastVFX, transform.position, Quaternion.identity);
        }
        
        if (inflictor.AudioClip)
        {
            _audioPooler
                .New3DAudio(inflictor.AudioClip)
                .OnChannel(AudioType.Sfx)
                .AtPosition(transform.position)
                .Play();
        }
        
        if (inflictor.LingeringVFX)
        {
            Instantiate(inflictor.LingeringVFX, transform.position, Quaternion.identity);
        }
    }

    
}

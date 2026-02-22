using _Project.Scripts.Core.HealthManagement;
using Unity.VisualScripting;
using UnityEngine;

public class RecordDiscBullet : MonoBehaviour
{
    private float _rotateSpeed = 200f; //takes from disc shooter
    private float _wobbleAmount = 0f; //takes from disc shooter 
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

    public void Initialize(Transform target, float speed, int maxTargets, float rotateSpeed, float wobbleAmount, LayerMask enemyLayer)
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

        _rb = GetComponent<Rigidbody>();
        if (_rb)
        {
            if (wobbleAmount <= 0f)
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

        if ((_enemyLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        if (other.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(bulletDamage);

            _hitCount++;
            _hitTimer = hitCooldown;

            if (_hitCount >= _maxTargets)
            {
                Destroy(gameObject);
            }
        }
    }
}

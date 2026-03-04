using _Project.Scripts.Core.HealthManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RecordDiscBullet : MonoBehaviour
{
    private float _rotateSpeed = 200f; //takes from disc shooter
    private float _wobbleAmount = 0f; //takes from disc shooter 
    [SerializeField] private float hitCooldown = 0.2f; //cooldown before it starts detecting another trigger on hit
    [SerializeField] private float enemySearchRadius = 30f; //the radius that it searches the next closest enemy if theres none it will go back to previous enemy
    [SerializeField] private float bulletLifetime = 3.5f; // max seconds until bullet is destroyed regardless of how many targets it hit
    [SerializeField] private int bulletDamage = -35;
    [SerializeField] private float targetUpdateInterval = 0.2f; // frequencu of update destination
    [SerializeField] private float passThroughDistance = 2f; //distance to shoot through enemy so it doesn't linger around that enemy
    private Transform _target;
    private Transform _lastKnownTarget;
    private float _speed;
    private int _maxTargets;
    private int _hitCount;
    private float _hitTimer;
    private float _targetUpdateTimer;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private LayerMask _enemyLayer;
    private bool _isNormalTurret;
    private bool _useNavMesh;
    private Vector3 _lastMoveDirection;

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
        _agent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _lastMoveDirection = transform.forward;
        
        if (_agent != null && !_isNormalTurret)
        {
            SetupNavMeshAgent();
        }
        else if (_rb != null)
        {
            SetupRigidbody();
        }
        
    }
    
    private void SetupNavMeshAgent()
    {
        _useNavMesh = true;
        _agent.speed = _speed;
        _agent.angularSpeed = _rotateSpeed;
        _agent.acceleration = _speed * 2f; // Quick acceleration for responsive movement
        _agent.stoppingDistance = 0f; // dont stop at target
        _agent.autoBraking = false; // don't slow down when approaching destination
        _agent.updateRotation = true;
        _agent.updateUpAxis = true; // Keep bullet upright ig
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        _agent.radius = 0.01f;
        
        // Disable rigidbody physics if using NavMesh
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }

        // Set initial destination
        if (_target != null)
        {
            SetDestinationToTarget(_target);
        }
    }

    private void SetupRigidbody()
    {
        _useNavMesh = false;
        
        if (_wobbleAmount <= 0f || _isNormalTurret)
        {
            _rb.freezeRotation = true;
        }
        else
        {
            _rb.angularDamping = Mathf.Lerp(100f, 0f, _wobbleAmount);
        }

        _rb.useGravity = false;
        _rb.isKinematic = true;
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
        _targetUpdateTimer -= Time.deltaTime;

        if (_isNormalTurret)
        {
            HandleNormalTurretMovement();
            return;
        }
        
        if (_useNavMesh)
        {
            HandleNavMeshMovement();
        }
        else
        {
            HandleHomingMovement();
        }
        
        if (_agent != null && _agent.enabled && _agent.velocity.sqrMagnitude > 0.01f)
        {
            _lastMoveDirection = _agent.velocity.normalized;
        }
    }
    
    private void HandleNormalTurretMovement()
    {
        transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
    }

    private void HandleNavMeshMovement()
    {
        // Periodically update target and destination
        if (_targetUpdateTimer <= 0f)
        {
            _targetUpdateTimer = targetUpdateInterval;
            FindClosestEnemy();
            
            Transform currentTarget = _target ?? _lastKnownTarget;
            
            if (currentTarget != null)
            {
                SetDestinationToTarget(currentTarget);
            }
            else
            {
                // No target, set destination far ahead in current direction
                SetDestinationAhead();
            }
        }

        // If no target and agent has reached destination, move forward
        if (_target == null && _lastKnownTarget == null)
        {
            if (!_agent.hasPath || _agent.remainingDistance < 1f)
            {
                SetDestinationAhead();
            }
        }
        
        // Check if agent is stuck or path is invalid
        if (_agent.enabled && !_agent.hasPath && !_agent.pathPending)
        {
            // Try to find a new target or fall back to straight movement
            FindClosestEnemy();
            if (_target == null && _lastKnownTarget == null)
            {
                SetDestinationAhead();
            }
        }
    }

    private void SetDestinationToTarget(Transform target)
    {
        if (_agent == null || !_agent.enabled) return;
        
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 passThroughPoint = target.position + directionToTarget * passThroughDistance;
        
        // Check if pass-through point is on NavMesh
        if (NavMesh.SamplePosition(passThroughPoint, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
        else if (NavMesh.SamplePosition(target.position, out NavMeshHit targetHit, 5f, NavMesh.AllAreas))
        {
            // Fallback to target position if pass-through point is off NavMesh
            _agent.SetDestination(targetHit.position);
        }
        else
        {
            // Target not on NavMesh, move in general direction
            SetDestinationAhead();
        }
    }

    private void SetDestinationAhead()
    {
        if (_agent == null || !_agent.enabled) return;
        
        // Set destination far ahead in current movement direction
        Vector3 aheadPoint = transform.position + _lastMoveDirection * enemySearchRadius;
        
        if (NavMesh.SamplePosition(aheadPoint, out NavMeshHit hit, enemySearchRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
        
    }

    private void HandleHomingMovement()
    {
        FindClosestEnemy();
        Transform homingTarget = _target ?? _lastKnownTarget;

        if (homingTarget == null)
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
            // Skip if enemy is null or destroyed
            if (enemy == null) continue;
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            // For NavMesh, consider path distance instead of direct distance (optional)
            if (_useNavMesh && _agent != null && _agent.enabled)
            {
                NavMeshPath path = new NavMeshPath();
                if (_agent.CalculatePath(enemy.transform.position, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        float pathDistance = CalculatePathLength(path);
                        if (pathDistance < closestDistance)
                        {
                            closestDistance = pathDistance;
                            closestEnemy = enemy.transform;
                        }
                        continue;
                    }
                }
            }

            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            _target = closestEnemy;
            _lastKnownTarget = closestEnemy;
        }
        else
        {
            _target = null;
        }
    }

    private float CalculatePathLength(NavMeshPath path)
    {
        float length = 0f;
        
        if (path.corners.Length < 2) return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return length;
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

            if (_isNormalTurret || _hitCount >= _maxTargets)
            {
                Destroy(gameObject);
            }
            else if (_useNavMesh)
            {
                // Immediately find next target after hit
                _target = null; // Clear current target so it finds a new one
                _targetUpdateTimer = 0f;
                FindClosestEnemy();
                
                if (_target != null)
                {
                    SetDestinationToTarget(_target);
                }
                else
                {
                    // No more enemies, keep moving forward
                    SetDestinationAhead();
                }
            }
        }
    }
    
    private void OnDestroy()
    {
        // Clean up NavMeshAgent
        if (_agent != null && _agent.enabled)
        {
            _agent.enabled = false;
        }
    }
}

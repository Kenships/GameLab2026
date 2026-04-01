// using _Project.Scripts.Core.AudioPooling;
// using _Project.Scripts.Core.Enemies;
// using _Project.Scripts.Effects.Inflictors;
// using _Project.Scripts.Util.ExtensionMethods;
// using Sisus.Init;
// using UnityEngine;
// using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;
//
// namespace _Project.Scripts.Core.Modules
// {
//     [RequireComponent(typeof(Rigidbody))]
//     public class RecordDiscBullet : MonoBehaviour<AudioPooler>
//     {
//         [SerializeField] private float rotateSpeed = 200f;
//         [SerializeField] private float hitCooldown = 0.2f; //cooldown before it starts detecting another trigger on hit
//
//         [SerializeField] private float enemySearchRadius = 10f; 
//
//         [SerializeField, Range(0f, 1f)] private float wobbleAmount = 0.36f;
//
//         
//         private LayerMask _enemyLayer;
//         private Transform _target;
//         private Transform _lastKnownTarget;
//         private Rigidbody _rb;
//
//         private EnemyEffectInflictor _inflictor;
//
//         private float _speed;
//         private int _maxTargets;
//         private int _hitCount;
//         private float _hitTimer;
//
//         private AudioPooler _audioPooler;
//
//         protected override void Init(AudioPooler audioPooler)
//         {
//             _audioPooler = audioPooler;
//         }
//
//         public void Initialize(Transform target, LayerMask targetLayer, EnemyEffectInflictor inflictor, float speed, float projectileLifeTime, int maxTargets)
//         {
//             _inflictor = inflictor;
//             _target = target;
//             _lastKnownTarget = _target;
//             _enemyLayer = targetLayer;
//
//             _speed = speed;
//             _maxTargets = maxTargets;
//             _hitCount = 0;
//
//             Destroy(gameObject, projectileLifeTime);
//
//             _rb = GetComponent<Rigidbody>();
//             _rb.angularDamping = Mathf.Lerp(100f, 0f, wobbleAmount);
//             _rb.useGravity = false;
//             _rb.isKinematic = true;
//         }
//
//         public void SetWobble(float amount)
//         {
//             wobbleAmount = amount;
//
//             if (_rb)
//             {
//                 if (wobbleAmount <= 0f)
//                 {
//                     _rb.freezeRotation = true;
//                 }
//                 else
//                 {
//                     _rb.freezeRotation = false;
//                     _rb.angularDamping = Mathf.Lerp(100f, 0f, wobbleAmount);
//                 }
//             }
//         }
//
//         private void FixedUpdate()
//         {
//             _hitTimer -= Time.deltaTime;
//
//             FindClosestEnemy();
//             Transform homingTarget = _target;
//
//             // If no enemies, home back to latest target
//             if (!homingTarget)
//             {
//                 homingTarget = _lastKnownTarget;
//             }
//
//             // if the OG is dead too then just go straight
//             if (!homingTarget)
//             {
//                 transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
//                 return;
//             }
//
//             Vector3 direction = (homingTarget.position - transform.position).normalized;
//
//             Quaternion targetRotation = Quaternion.LookRotation(direction);
//             transform.rotation = Quaternion.RotateTowards(
//                 transform.rotation,
//                 targetRotation,
//                 rotateSpeed * Time.deltaTime
//             );
//
//             transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
//         }
//
//         private void FindClosestEnemy()
//         {
//             Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, enemySearchRadius, _enemyLayer);
//
//             float closestDistance = Mathf.Infinity;
//             Transform closestEnemy = null;
//
//             foreach (Collider enemy in enemiesInRange)
//             {
//                 float distance = Vector3.Distance(transform.position, enemy.transform.position);
//
//                 if (distance < closestDistance)
//                 {
//                     closestDistance = distance;
//                     closestEnemy = enemy.transform;
//                 }
//             }
//
//             if (closestEnemy)
//             {
//                 _target = closestEnemy;
//                 _lastKnownTarget = closestEnemy;
//             }
//             else
//             {
//                 _target = null;
//             }
//         }
//
//         private void OnTriggerEnter(Collider other)
//         {
//             if (_hitTimer > 0f)
//                 return;
//
//             if (!other.IsOnLayer(_enemyLayer))
//             {
//                 return;
//             }
//
//             if (other.TryGetComponent(out EnemyBase enemy))
//             {
//                 Inflict(enemy);
//
//                 _hitCount++;
//                 _hitTimer = hitCooldown;
//
//                 if (_hitCount >= _maxTargets)
//                 {
//                     Destroy(gameObject);
//                 }
//             }
//         }
//
//         private void Inflict(EnemyBase enemy)
//         {
//             if (_inflictor == null)
//             {
//                 Debug.LogWarning("Inflector is null");
//                 return;
//             }
//
//             _inflictor.Inflict(enemy);
//
//             if (_inflictor.CastVFX)
//             {
//                 Instantiate(_inflictor.CastVFX, enemy.transform.position, Quaternion.identity);
//             }
//
//             if (_inflictor.AudioClip)
//             {
//                 _audioPooler
//                     .New3DAudio(_inflictor.AudioClip)
//                     .OnChannel(AudioType.Sfx)
//                     .AtPosition(transform.position)
//                     .Play();
//             }
//         }
//     }
// }


using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Inflictors;
using _Project.Scripts.Util.ExtensionMethods;
using Sisus.Init;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules
{
    [RequireComponent(typeof(Rigidbody))]
    public class RecordDiscBullet : MonoBehaviour<AudioPooler>
    {
        [SerializeField] private float rotateSpeed = 200f;
        [SerializeField] private float hitCooldown = 0.2f;

        [SerializeField] private float enemySearchRadius = 10f;

        [SerializeField, Range(0f, 1f)] private float wobbleAmount = 0.36f;

        [Header("=== Height Constraints ===")]
        [SerializeField] private float targetHeightOffset = 1f;
        [SerializeField] private float minHeight = 0.5f;
        [SerializeField] private float maxDownwardAngle = 30f;

        private LayerMask _enemyLayer;
        private Transform _target;
        private Transform _lastKnownTarget;
        private Rigidbody _rb;

        private EnemyEffectInflictor _inflictor;

        private float _speed;
        private int _maxTargets;
        private int _hitCount;
        private float _hitTimer;

        private AudioPooler _audioPooler;

        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }

        public void Initialize(Transform target, LayerMask targetLayer, EnemyEffectInflictor inflictor, float speed, float projectileLifeTime, int maxTargets)
        {
            _inflictor = inflictor;
            _target = target;
            _lastKnownTarget = _target;
            _enemyLayer = targetLayer;

            _speed = speed;
            _maxTargets = maxTargets;
            _hitCount = 0;

            Destroy(gameObject, projectileLifeTime);

            _rb = GetComponent<Rigidbody>();
            _rb.angularDamping = Mathf.Lerp(100f, 0f, wobbleAmount);
            _rb.useGravity = false;
            _rb.isKinematic = true;
        }

        public void SetWobble(float amount)
        {
            wobbleAmount = amount;

            if (_rb)
            {
                if (wobbleAmount <= 0f)
                {
                    _rb.freezeRotation = true;
                }
                else
                {
                    _rb.freezeRotation = false;
                    _rb.angularDamping = Mathf.Lerp(100f, 0f, wobbleAmount);
                }
            }
        }

        private void FixedUpdate()
        {
            _hitTimer -= Time.deltaTime;

            FindClosestEnemy();
            Transform homingTarget = _target;

            if (!homingTarget)
            {
                homingTarget = _lastKnownTarget;
            }

            if (!homingTarget)
            {
                transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
                ClampHeight();
                return;
            }

            // Add height offset to target position
            Vector3 targetPosition = homingTarget.position + Vector3.up * targetHeightOffset;
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Clamp downward angle
            direction = ClampDownwardDirection(direction);

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );

            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));

            // Ensure bullet doesn't go below minimum height
            ClampHeight();
        }

        private Vector3 ClampDownwardDirection(Vector3 direction)
        {
            // Calculate the current pitch angle
            float pitchAngle = Vector3.SignedAngle(
                new Vector3(direction.x, 0f, direction.z).normalized,
                direction,
                Vector3.Cross(Vector3.up, direction)
            );

            // If aiming too far down, clamp it
            if (pitchAngle < -maxDownwardAngle)
            {
                Vector3 horizontalDir = new Vector3(direction.x, 0f, direction.z).normalized;
                direction = Quaternion.AngleAxis(-maxDownwardAngle, Vector3.Cross(Vector3.up, horizontalDir)) * horizontalDir;
            }

            return direction.normalized;
        }

        private void ClampHeight()
        {
            if (transform.position.y < minHeight)
            {
                Vector3 pos = transform.position;
                pos.y = minHeight;
                transform.position = pos;

                // Also correct rotation to not point downward
                Vector3 euler = transform.eulerAngles;
                if (euler.x > 180f) euler.x -= 360f;
                if (euler.x > 0f) euler.x = 0f;
                transform.eulerAngles = euler;
            }
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
            if (_hitTimer > 0f)
                return;

            if (!other.IsOnLayer(_enemyLayer))
            {
                return;
            }

            if (other.TryGetComponent(out EnemyBase enemy))
            {
                Inflict(enemy);

                _hitCount++;
                _hitTimer = hitCooldown;

                if (_hitCount >= _maxTargets)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void Inflict(EnemyBase enemy)
        {
            if (_inflictor == null)
            {
                Debug.LogWarning("Inflector is null");
                return;
            }

            _inflictor.Inflict(enemy);

            if (_inflictor.CastVFX)
            {
                Instantiate(_inflictor.CastVFX, enemy.transform.position, Quaternion.identity);
            }

            if (_inflictor.AudioClip)
            {
                _audioPooler
                    .New3DAudio(_inflictor.AudioClip)
                    .OnChannel(AudioType.Sfx)
                    .AtPosition(transform.position)
                    .Play();
            }
        }
    }
}
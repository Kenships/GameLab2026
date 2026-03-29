using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Modules.Factories;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Targeting.Strategies;
using System.Collections.Generic;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules
{
    [RequireComponent(typeof(RangeDetector))]
    public class Turret : PickupModuleBase, IDamageable
    {
        [Header("References")]
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject rotationBody;
        [SerializeField] private Animation turretAnimator;
        [SerializeField] private GameObject newModel;
        [SerializeField] private GameObject brokenModel;

        [Header("Shooting Settings")]
        [SerializeField] private EnemyTargetingStrategy targetingStrategy;
        [SerializeField] private BulletFactoryBase bulletFactory;
        [SerializeField] private float shotsPerSecond = 1f; //yk
        [SerializeField] private float attackSpeedMultiplier = 1.5f;
        [SerializeField] private float detectionRange = 15f; // range around shooter that detects enemies
        [SerializeField] private float headRotationSpeed = 10f;
        
        
        [Header("Audio")] [SerializeField] private AudioClip shootSound;
        [SerializeField] private float shootSoundVolume = 0.1f;

        [Header("Player Selection Visuals")]
        [SerializeField] private GameObject player1Visual;

        [SerializeField] private GameObject player2Visual;

        private float _shootTimer;
        private EnemyBase _currentTarget;
        private RangeDetector _rangeDetector;
        private float _shotsPerSecond;
        private Health _myHealth;

        private bool _canShoot = true;

        private readonly List<EnemyBase> _enemies = new();

        protected override void Start()
        {
            base.Start();
            _rangeDetector ??= GetComponent<RangeDetector>();
            _rangeDetector.radius = detectionRange;
            _shotsPerSecond = shotsPerSecond;
            _myHealth = gameObject.GetComponent<Health>();
        }

        private void ReevaluateTarget()
        {
            List<EnemyBase> targets = targetingStrategy.Evaluate(_enemies);
            
            _currentTarget = null;

            foreach (var target in targets)
            {
                if (target != null)
                {
                    _currentTarget = target;
                    break;
                }
            }
        }

        private void PerformAttack()
        {
            if (!_canShoot) return;

                _rangeDetector.GetObjectTypeInRangeNoAlloc(_enemies);
            ReevaluateTarget();

            if (!_currentTarget)
                return;

            RotateHeadTowardsTarget();

            _shootTimer -= Time.deltaTime;

            if (_shootTimer <= 0f)
            {
                Shoot();
                _shootTimer = _shotsPerSecond;
            }
        }

        private void RotateHeadTowardsTarget()
        {
            Vector3 direction = (_currentTarget.transform.position - rotationBody.transform.position).normalized;
            direction.y = 0;
            if (direction == Vector3.zero)
                return;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            rotationBody.transform.rotation = Quaternion.Slerp(rotationBody.transform.rotation, targetRotation,
                Time.deltaTime * headRotationSpeed);
        }

        private void Shoot()
        {
            if (!_currentTarget)
                return;

            Vector3 directionToEnemy = (_currentTarget.transform.position - spawnPoint.position).normalized;
            Quaternion rotationToEnemy = Quaternion.LookRotation(directionToEnemy);

            if (turretAnimator)
            {
                turretAnimator.Play();
            }
            
            _audioPooler.New2DAudio(shootSound).OnChannel(AudioType.Sfx).SetVolume(shootSoundVolume).Play();

            GameObject bullet = bulletFactory.CreateBullet(_currentTarget.transform, spawnPoint.position, rotationToEnemy);
        }

        protected override void LoadState()
        {
            if (_isFastForwarding)
            {
                state = ModuleState.Attack;
                return;
            }
            
            PerformAttack();
            base.LoadState();
        }

        protected override void AttackState()
        {
            if (!_isFastForwarding)
            {
                state = _health.CurrentHealth == 0 ? ModuleState.Used : ModuleState.Load;
                return;
            }
            
            PerformAttack();
            base.AttackState();
        }
        
        protected override void UsedState()
        {
            if (_isRewinding)
            {
                state = ModuleState.Load;
                return;
            }
            base.UsedState();
        }

        protected override void OnStateChanged(ModuleState prevState)
        {
            switch (state)
            {
                case ModuleState.Load:
                    newModel.SetActive(true);
                    brokenModel.SetActive(false);
                    _shotsPerSecond = 1f / shotsPerSecond;
                    _canShoot = true;
                    break;
                case ModuleState.Attack:
                    _shotsPerSecond = 1f / (shotsPerSecond * attackSpeedMultiplier);
                    break;
                case ModuleState.Used:
                    _canShoot = false;
                    brokenModel.SetActive(true);
                    newModel.SetActive(false);
                    break;
            }
        }

        public override void ShowVisual(PlayerData.PlayerID playerID)
        {
            if (!player1Visual || !player2Visual)
            {
                Debug.LogWarning("Player Selection Visuals not set");
                return;
            }

            if (playerID == PlayerData.PlayerID.Player1)
            {
                player1Visual.SetActive(true);
            }
            else
            {
                player2Visual.SetActive(true);
            }
        }

        public override void HideVisual(PlayerData.PlayerID playerID)
        {
            if (!player1Visual || !player2Visual)
            {
                Debug.LogWarning("Player Selection Visuals not set");
                return;
            }

            if (playerID == PlayerData.PlayerID.Player1)
            {
                player1Visual.SetActive(false);
            }
            else
            {
                player2Visual.SetActive(false);
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            _rangeDetector ??= GetComponent<RangeDetector>();
            _rangeDetector.radius = detectionRange;
#endif
        }

        public void Damage(float damage)
        {
            _myHealth.AddToHealth(-damage);
        }

        public void ApplyEffect(IEffect<IDamageable> effect)
        {
            
        }

        public void RemoveEffect(IEffect<IDamageable> effect)
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Targeting;
using _Project.Scripts.Targeting.Interface;
using _Project.Scripts.Targeting.Strategies;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules
{
    [RequireComponent(typeof(RangeDetector))]
    public class RecordDiscShooter : HpPickupModuleBase
    {
        [Header("References")]
        [SerializeField] RecordDiscBullet recordDiscPrefab;
        [SerializeField] private Transform spawnPoint;

        [Header("Shooting Settings")]
        [SerializeField] private EnemyTargetingStrategy targetingStrategy;
        [SerializeField] private float rotateSpeed = 200f; // homing rotation sharpness (higher the sharper)
        [SerializeField] private float defaultBulletSpeed = 10f;
        [SerializeField] private float fastBulletSpeed = 15f;
        [SerializeField] private float slowBulletSpeed = 2.5f;
        [SerializeField] private float timeBetweenShots = 1f; //yk
        [SerializeField] [Range(0f, 1f)] private float bulletWobble = 0f; // o = bullet shoots straght (like frisbee) | 1= bullet wobbles (me throwing a frisbee)
        [SerializeField] private int maxTargets = 3; // max number of hits until bullet is destroyed
        [SerializeField] private float detectionRange = 15f; // range around shooter that detects enemies
        [SerializeField] private LayerMask enemyLayer;

        [Header("Audio")]
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private float shootSoundVolume = 0.1f;
        
        [Header("Player Selection Visuals")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;

        [Header("Normal Turret")]
        [SerializeField] private bool isNormalTurret = false;
        [SerializeField] private GameObject head;
        [SerializeField] private float headRotateSpeed = 10f;
        [SerializeField] private Animation normalTurretAnimator;

        private float _shootTimer;
        private EnemyBase _currentTarget;
        private float _currentBulletSpeed;
        private RangeDetector _rangeDetector;
        private float _currentTimeBetweenShots;

        private readonly List<EnemyBase> _enemies = new();

        protected override void Start()
        {
            base.Start();
            _rangeDetector ??= GetComponent<RangeDetector>();
            _rangeDetector.radius = detectionRange;
            _currentBulletSpeed = defaultBulletSpeed;
            _currentTimeBetweenShots = timeBetweenShots;
            
            _rangeDetector.OnObjectEnter += ReevaluateTarget;
            _rangeDetector.OnObjectExit += ReevaluateTarget;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _rangeDetector.OnObjectEnter -= ReevaluateTarget;
            _rangeDetector.OnObjectExit -= ReevaluateTarget;
        }

        private void ReevaluateTarget(Collider enemy)
        {
            List<EnemyBase> targets = targetingStrategy.Evaluate(_enemies);
            _currentTarget = targets.Count > 0 ? targets[0] : null;
        }

        private void PerformAttack()
        {
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_enemies);

            if (isNormalTurret && _currentTarget)
            {
                RotateHeadTowardsTarget();
            }

            if (!_currentTarget)
                return;

            _shootTimer -= Time.deltaTime;

            if (_shootTimer <= 0f)
            {
                Shoot();
                _shootTimer = _currentTimeBetweenShots;
            }
        }

        private void RotateHeadTowardsTarget()
        {
            if (head == null || _currentTarget == null) return;

            Vector3 direction = (_currentTarget.transform.position - head.transform.position).normalized;
            direction.y = 0;
            if (direction == Vector3.zero) return; 
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            head.transform.rotation = Quaternion.Slerp(head.transform.rotation,targetRotation,Time.deltaTime * headRotateSpeed);
        }

        private void Shoot()
        {
            if (!_currentTarget) return;

            Vector3 directionToEnemy = (_currentTarget.transform.position - spawnPoint.position).normalized;
            Quaternion rotationToEnemy = Quaternion.LookRotation(directionToEnemy);

            _audioPooler.New2DAudio(shootSound).OnChannel(AudioType.Sfx).SetVolume(shootSoundVolume).Play();

            if (isNormalTurret)
            {
                normalTurretAnimator.Play();
            }

            RecordDiscBullet bullet = Instantiate(recordDiscPrefab, spawnPoint.position, rotationToEnemy);
            bullet.Initialize(_currentTarget.transform, _currentBulletSpeed, maxTargets, rotateSpeed, bulletWobble, enemyLayer, isNormalTurret);
        }

        protected override void LoadState()
        {
            PerformAttack();
            base.LoadState();
        }

        protected override void AttackState()
        {
            PerformAttack();
            base.AttackState();
        }

        protected override void UsedState()
        {
            PerformAttack();
            base.UsedState();
        }

        protected override void OnStateChanged(ModuleState prevState)
        {
            switch (prevState)
            {
                case ModuleState.Load:
                    _currentBulletSpeed = defaultBulletSpeed;
                    _currentTimeBetweenShots = timeBetweenShots;
                    break;
                case ModuleState.Attack:
                    _currentBulletSpeed = fastBulletSpeed;
                    _currentTimeBetweenShots = timeBetweenShots / (fastBulletSpeed / defaultBulletSpeed);
                    break;
                case ModuleState.Used:
                    _currentBulletSpeed = slowBulletSpeed;
                    _currentTimeBetweenShots = timeBetweenShots / (slowBulletSpeed / defaultBulletSpeed);
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
    }
}
    

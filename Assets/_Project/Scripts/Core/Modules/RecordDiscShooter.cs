using System.Collections.Generic;
using _Project.Scripts.Core.Modules.Base_Class;
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

        [Header("Turret Mode")]
        [SerializeField] private bool isNormalTurret = false;

        private float _shootTimer;
        private Transform _currentTarget;
        private float _currentBulletSpeed;
        private RangeDetector _rangeDetector;
        private float _currentTimeBetweenShots;

        private void Start()
        {
            _rangeDetector = GetComponent<RangeDetector>();
            _rangeDetector.radius = detectionRange;
            _currentBulletSpeed = defaultBulletSpeed;
            _currentTimeBetweenShots = timeBetweenShots;
        }

        private void PerformAttack()
        {
            _currentTarget = _rangeDetector.GetClosestObjectOfType<Transform>();
            
            if (_currentTarget == null)
                return;

            _shootTimer -= Time.deltaTime;

            if (_shootTimer <= 0f)
            {
                Shoot();
                _shootTimer = _currentTimeBetweenShots;
            }
        }

        private void Shoot()
        {
            if (!_currentTarget) return;

            Vector3 directionToEnemy = (_currentTarget.position - spawnPoint.position).normalized;
            Quaternion rotationToEnemy = Quaternion.LookRotation(directionToEnemy);

            _audioPooler.New2DAudio(shootSound).OnChannel(AudioType.Sfx).SetVolume(shootSoundVolume).Play();

            RecordDiscBullet bullet = Instantiate(recordDiscPrefab, spawnPoint.position, rotationToEnemy);
            bullet.Initialize(_currentTarget, _currentBulletSpeed, maxTargets, rotateSpeed, bulletWobble, enemyLayer, isNormalTurret);
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

        protected override void OnStateChanged(ModuleState newState)
        {
            switch (newState)
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
        
        public override void ShowVisual(int playerIndex)
        {
            if (!player1Visual || !player2Visual)
            {
                Debug.LogWarning("Player Selection Visuals not set");
                return;
            }
            
            if (playerIndex == 1)
            {
                player1Visual.SetActive(true);
            }
            else
            {
                player2Visual.SetActive(true);
            }
        }

        public override void HideVisual(int playerIndex)
        {
            if (!player1Visual || !player2Visual)
            {
                Debug.LogWarning("Player Selection Visuals not set");
                return;
            }
            
            if (playerIndex == 1)
            {
                player1Visual.SetActive(false);
            }
            else
            {
                player2Visual.SetActive(false);
            }
        }
    }
}
    

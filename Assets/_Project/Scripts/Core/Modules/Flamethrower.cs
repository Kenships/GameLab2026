using System.Collections.Generic;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Effects.Inflictors;
using _Project.Scripts.Effects.Interface;
using _Project.Scripts.Targeting.Interface;
using _Project.Scripts.Targeting.Strategies;
using _Project.Scripts.Util.Timer.Timers;
using UnityEngine;

namespace _Project.Scripts.Core.Modules
{
    public class Flamethrower : HpPickupModuleBase, IDamageable
    {
        [Header("Particle Settings")]
        
        [SerializeField] private ParticleSystem particle;
        /*
        [SerializeField] private float emissionRateToAngleRatio = 16f;
        // The relationship between rangeDetector.angle and particle angle can't be represented by a simple function, 
        // so everytime you change rangeDetector.angle, you have to also adjust angleMultiplier to have desired particle effect
        [SerializeField] private float angleMultiplier = 0.6f;
        // The relationship between rangeDetector.radius and particle radius can't be represented by a simple function, 
        // so everytime you change rangeDetector.angle, you have to also adjust angleMultiplier to have desired particle effect
        [SerializeField] private float radiusMultiplier = 3f;
        [SerializeField] private float fastEmissionRateToAngleRatio = 32f;
        [SerializeField] private float fastRadiusMultiplier = 3.75f;
        */

        [SerializeField] private ParticleSystem.MinMaxCurve newSpeed;
        [SerializeField] private ParticleSystem.MinMaxCurve newDistance;

        private ParticleSystem.MinMaxCurve normalSpeed;
        private ParticleSystem.MinMaxCurve normalDistance;

        [Header("References")]
        [SerializeField] private GameObject newModel;
        [SerializeField] private GameObject brokenModel;

        [Header("Flamethrower Settings")]
        [SerializeField] private EnemyEffectInflictor inflictor;

        [SerializeField] private EnemyTargetingStrategy targetingStrategy;
        [SerializeField] private float normalDps = 4f;
        [SerializeField] private float fastDps = 8f;
        [SerializeField] private float fastRadius = 10;

        [Header("Player Selection Visuals")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;


        private float _currentDamage;
        private float _currentDps;
        private float _normalRadius;
        private RangeDetector _rangeDetector; // rangeType is sector
        private List<EnemyBase> _enemies;
        private bool _isDamagingEnemies;
        private CountdownTimer _attackCooldownTimer;
        private Health _myHealth;

        protected override void Start()
        {
            base.Start();
            _currentDps = normalDps;
            _attackCooldownTimer = new CountdownTimer(1f/_currentDps);
            _enemies = new List<EnemyBase>();
            _myHealth = gameObject.GetComponent<Health>();

            _rangeDetector = GetComponent<RangeDetector>();
            if (!_rangeDetector)
            {
                Debug.Log("missing rangeDetector");
                return;
            }

            _normalRadius = _rangeDetector.radius;

            if (!particle)
            {
                Debug.Log("missing particle");
            }

            var main = particle.main;

            normalSpeed = new ParticleSystem.MinMaxCurve(main.startSpeed.constantMin, main.startSpeed.constantMax);

            normalDistance = new ParticleSystem.MinMaxCurve(main.startLifetime.constantMin, main.startLifetime.constantMax);

            /*
            UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio);
            UpdateDistance(_rangeDetector.radius * radiusMultiplier);
            */

            state = ModuleState.Load;
        }

        private void UpdateParticleBehaviour(ParticleSystem.MinMaxCurve S, ParticleSystem.MinMaxCurve L)
        {
            var main = particle.main;

            main.startLifetime = L;
            main.startSpeed = S;
        }

        private void UpdateParticleAngle(float angle, float emissionRateToAngleRatio)
        {
            var shape = particle.shape;
            shape.angle = angle;

            var emission = particle.emission;
            emission.rateOverTime = angle * emissionRateToAngleRatio;
        }

        private void UpdateDistance(float distance)
        {
            var main = particle.main;

            float currentLifetime = main.startLifetime.constant;

            main.startSpeed = distance / currentLifetime;
        }

        private void UpdateSpeed(float distance)
        {
            var main = particle.main;

            float currentLifetime = main.startLifetime.constant;

            main.startSpeed = distance / currentLifetime;
        }

        private void PerformAttack()
        {
            if (_attackCooldownTimer.IsRunning)
            {
                return;
            }
            _rangeDetector.GetObjectTypeInRangeNoAlloc(_enemies);

            if (_enemies.Count <= 0)
            {
                return;
            }

            foreach(IDamageable enemy in _enemies)
            {
                inflictor.Inflict(enemy);
            }

            _attackCooldownTimer.Reset(1f / _currentDps);
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

        #region State Methods
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

        protected override void OnStateChanged(ModuleState prevState)
        {
            switch (state)
            {
                case ModuleState.Load:
                    /*
                    UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, emissionRateToAngleRatio);
                    */
                    _rangeDetector.radius = _normalRadius;
                    /*
                    UpdateDistance(_rangeDetector.radius * radiusMultiplier);
                    */

                    newModel.SetActive(true);
                    brokenModel.SetActive(false);

                    UpdateParticleBehaviour(normalSpeed, normalDistance);

                    particle.Play();
                    _currentDps = normalDps;
                    break;
                case ModuleState.Attack:
                    _currentDps = fastDps;
                    /*
                    UpdateParticleAngle(_rangeDetector.angle * angleMultiplier, fastEmissionRateToAngleRatio);
                    */
                    UpdateParticleBehaviour(newSpeed, newDistance);
                    _rangeDetector.radius = fastRadius;
                    /*
                    UpdateDistance(_rangeDetector.radius * fastRadiusMultiplier);
                    */
                    break;
                case ModuleState.Used:
                    particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    brokenModel.SetActive(true);
                    newModel.SetActive(false);
                    break;
            }
        }
        #endregion

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

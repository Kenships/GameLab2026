using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.InputManagement.Interfaces;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.Util.ExtensionMethods;
using Sisus.Init;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core
{
    public class VHSModule : Module, IDamageable
    {
        [Header("References")]
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;
    
        [Header("VHS Settings")]
        [SerializeField] private float vhsMaxHealth = 300f;
        [SerializeField] private float defaultRewindSpeed = 1f;
        [SerializeField] private float fastForwardMultiplier = 1.2f;

        [Header("VHS Progression")]
        [SerializeField] private int amountofmilestones = 8;
        [SerializeField] private Stack<float> milestones = new Stack<float>();



        private Health _myHealth;
        private bool _isFastForwarding;
        private SceneLoader _sceneLoader;

        

        protected override void OnAwake()
        {
            _myHealth = gameObject.GetOrAdd<Health>();
            _myHealth.Initialize(vhsMaxHealth, 0);

            _sceneLoader = GetComponent<SceneLoader>();

            DetermineProgressMilestones();

            // TODO: Temporary please fix
            _myHealth.OnFullHp += () => GetComponent<SceneLoader>().LoadScene();
        }

        private void DetermineProgressMilestones()
        {
            int tempamountofmilestones = amountofmilestones + 1;
            float milestoneDiference = vhsMaxHealth / tempamountofmilestones;

            // right now i starts at -1 because no reward at the end of the game (compensated by adding 1 in prev step)

            for (int i = tempamountofmilestones - 1; i > 0; i--)
            {
                float temp = milestoneDiference * i;
                //Debug.Log(temp + "milestone");
                milestones.Push(temp);
            }
    
        }

        void Update()
        {
            if (milestones.Count > 0 && _myHealth.CurrentHealth >= milestones.Peek())
            {
                MilestoneReached();
            }
        }

        private void MilestoneReached()
        {
            milestones.Pop();
            _sceneLoader.LoadScene();
            Time.timeScale = 0f;
        }

        public void Damage(float damage)
        {
            _myHealth.AddToHealth(-damage);
        }

        protected override void LoadState()
        {
            float delta = _isFastForwarding ? defaultRewindSpeed * fastForwardMultiplier : defaultRewindSpeed;
            
            _myHealth.AddToHealth(delta * Time.deltaTime);
        }

        protected override void AttackState()
        {
            // NOP
        }

        protected override void UsedState()
        {
            // NOP
        }

        protected override void OnStateChanged(ModuleState newState)
        {
            // NOP
        }

        public override void Rewind()
        {
            // NOP
        }

        public override void CancelRewind()
        {
            // NOP
        }
        
        public override void FastForward()
        {
            _isFastForwarding = true;
        }

        public override void CancelFastForward()
        {   
            _isFastForwarding = false;
        }

        public override void ShowVisual(PlayerData.PlayerID playerIndex)
        {
            if (playerIndex == PlayerData.PlayerID.Player1)
            {
                player1Visual.SetActive(true);
            }
            else
            {
                player2Visual.SetActive(true);
            }
        }

        public override void HideVisual(PlayerData.PlayerID playerIndex)
        {
            if (playerIndex == PlayerData.PlayerID.Player1)
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

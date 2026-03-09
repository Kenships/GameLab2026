using _Project.Scripts.Core;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveTank : HpPickupModuleBase
{
    [Header("References")]
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private GameObject loadModel;
    [SerializeField] private GameObject usedModel;

    [Header("Explosive Tank Settings")]
    [SerializeField] private float damage = 90f;
    [Tooltip("Does damage twice, to fit the explosion VFX")]
    [SerializeField] private float timeGapBeforeSecondAttack = 0.75f;

    [Header("Player Selection Visuals")]
    [SerializeField] private GameObject player1Visual;
    [SerializeField] private GameObject player2Visual;


    private RangeDetector _rangeDetector; // rangeType is circle
    private List<IDamageable> _enemies;

    protected override void Start()
    {
        base.Start();
        _enemies = new List<IDamageable>();

        var main = explosionParticle.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        _rangeDetector = GetComponent<RangeDetector>();
        if (!_rangeDetector)
        {
            Debug.Log("missing rangeDetector");
            return;
        }

        if (!explosionParticle)
        {
            Debug.Log("missing particle");
        }

        state = ModuleState.Load;
    }

    private void PerformAttack()
    {
        _rangeDetector.GetObjectTypeInRangeNoAlloc(_enemies);

        if (_enemies.Count <= 0)
        {
            return;
        }

        foreach (IDamageable enemy in _enemies)
        {
            enemy?.Damage(damage);
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

    #region State Methods

    protected override void OnStateChanged(ModuleState prevState)
    {
        switch (state)
        {
            case ModuleState.Load:
                usedModel.SetActive(false);
                loadModel.SetActive(true);
                break;
            case ModuleState.Attack:
                break;
            case ModuleState.Used:
                PerformAttack();
                explosionParticle.Play();
                Invoke(nameof(PerformAttack), timeGapBeforeSecondAttack);
                loadModel.SetActive(false);
                usedModel.SetActive(true);
                break;
        }
    }

    #endregion
}

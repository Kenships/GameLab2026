using _Project.Scripts.Core;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class LazerCannon : HpPickupModuleBase
{
    [Header("References")]
    [SerializeField] private Transform lazerBeamStartPos;

    [Header("Lazer Beam Settings")]
    [SerializeField] private float damage = 90f;

    [Header("Player Selection Visuals")]
    [SerializeField] private GameObject player1Visual;
    [SerializeField] private GameObject player2Visual;


    private RangeDetector _rangeDetector; // rangeType is rectangle
    private Transform lazerBeam;
    private List<IDamageable> _enemies;

    private void Start()
    {
        _enemies = new List<IDamageable>();
        lazerBeam = lazerBeamStartPos.transform.GetChild(0);

        _rangeDetector = GetComponent<RangeDetector>();
        if (!_rangeDetector)
        {
            Debug.Log("missing rangeDetector");
            return;
        }

        state = ModuleState.Load;
    }

    private void PlayLazerBeamAnim()
    {
        float beamScale = _rangeDetector.length / (lazerBeam.localScale.y * 2);
        Sequence.Create()
            .Chain(Tween.Scale(lazerBeamStartPos.transform, startValue: 0f, endValue: beamScale, duration: 0.2f, ease: Ease.OutExpo))
            .ChainDelay(1f)
            .Chain(Tween.Scale(lazerBeamStartPos.transform, endValue: 0f, duration: 0.2f, ease: Ease.InExpo));
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
                break;
            case ModuleState.Attack:
                break;
            case ModuleState.Used:
                PerformAttack();
                PlayLazerBeamAnim();
                break;
        }
    }

    #endregion
}

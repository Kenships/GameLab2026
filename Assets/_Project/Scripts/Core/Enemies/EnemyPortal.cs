using System;
using System.Linq;
using _Project.Scripts.Enemies;
using UnityEngine;

namespace _Project.Scripts.Core.Enemies
{
    public class EnemyPortal : MonoBehaviour
    {
        [SerializeField] ScriptableEventWaveData waveData;

        private void Start()
        {
            waveData.OnRaised += WaveDataOnOnRaised;
        }

        private void WaveDataOnOnRaised(EnemyWaveSpawner.Wave obj)
        {
            foreach (var portalSpawn in obj.portalSpawns)
            {
                if (portalSpawn.spawnPosition == transform)
                {
                    gameObject.SetActive(true);
                    return;
                }
            }
            
            gameObject.SetActive(false);
        }
    }
}

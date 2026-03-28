using System;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Enemies.Factories;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Core.SceneLoading;
using _Project.Scripts.Enemies;
using _Project.Scripts.UI;
using Sisus.Init;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Core.Enemies
{
    public class EnemyWaveSpawner : MonoBehaviour<AudioPooler>
    {
        [System.Serializable]
        public struct BlinkSettings
        {
            public int blinkCount;
            public float fadeInDuration;
            public float holdDuration;
            public float fadeOutDuration;
            public float initialDelay;
        }

        [System.Serializable]
        public class EnemySpawnEntry
        {
            public EnemyFactoryBase enemyFactory;
            public int count = 1;
        }

        [System.Serializable]
        public class PortalWaveSpawn
        {
            public Transform spawnPosition;
            public EnemySpawnEntry[] enemies;

            public RawImage arrowImage;
            public BlinkSettings blinkSettings = new BlinkSettings
            {
                blinkCount = 3,
                fadeInDuration = 0.2f,
                holdDuration = 0.3f,
                fadeOutDuration = 0.2f,
                initialDelay = 0f
            };
        }

        [System.Serializable]
        public class Wave
        {
            public string waveName = "Wave";

            [Header("Portal-based spawns for this wave")]
            public PortalWaveSpawn[] portalSpawns;

            [Header("Spawn timing")]
            public float spawnDelayMin = 0.3f;
            public float spawnDelayMax = 2f;

            [Header("Break after this wave")]
            public float restAfterWave = 5f;
        }

        [Header("References")]
        [SerializeField] private Transform vhsLocation;
        [SerializeField] private ScriptableEventWaveData waveStartEvent;
        [SerializeField] private EnemyWaveUI waveUI;

        [Header("Wave Settings")]
        [SerializeField] private Wave[] waves;

        private List<EnemyBase> currentWaveEnemies = new List<EnemyBase>();
        private AudioPooler _audioPooler;
        private SceneLoader _sceneLoader;

        private void Start()
        {
            _sceneLoader = GetComponent<SceneLoader>();
            StartCoroutine(SpawnWaves());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }

        private IEnumerator SpawnWaves()
        {
            if (waves == null || waves.Length == 0)
            {
                Debug.LogWarning("No waves assigned in EnemySpawner.");
                yield break;
            }

            for (int waveIndex = 0; waveIndex < waves.Length; waveIndex++)
            {
                Wave currentWave = waves[waveIndex];

                if (currentWave.portalSpawns == null || currentWave.portalSpawns.Length == 0)
                {
                    Debug.LogWarning($"Wave {waveIndex + 1} has no portal spawns.");
                    continue;
                }

                // Clear previous wave's enemy list
                currentWaveEnemies.Clear();
                
                waveStartEvent?.Raise(currentWave);

                Debug.Log($"Starting {currentWave.waveName}");

                int activePortalRoutines = 0;

                for (int p = 0; p < currentWave.portalSpawns.Length; p++)
                {
                    PortalWaveSpawn portalSpawn = currentWave.portalSpawns[p];

                    if (portalSpawn.spawnPosition == null)
                    {
                        Debug.LogWarning($"Wave {waveIndex + 1} has a null spawn position.");
                        continue;
                    }

                    if (portalSpawn.enemies == null || portalSpawn.enemies.Length == 0)
                    {
                        Debug.LogWarning($"Wave {waveIndex + 1} portal spawn has no enemies.");
                        continue;
                    }

                    activePortalRoutines++;
                    StartCoroutine(SpawnPortalRoutine(currentWave, portalSpawn, () => activePortalRoutines--));
                }

                // Wait for all portal spawn routines to finish (all enemies spawned)
                yield return new WaitUntil(() => activePortalRoutines <= 0);

                // Wait until all enemies of this wave are dead (references become null)
                while (true)
                {
                    // Remove destroyed enemies (null references)
                    currentWaveEnemies.RemoveAll(enemy => enemy == null);
                    if (currentWaveEnemies.Count == 0)
                        break;
                    yield return null;
                }

                Debug.Log($"Finished {currentWave.waveName}");

                if (waveUI != null)
                {
                    yield return waveUI.ShowWaveCompleted();
                }
                
                _audioPooler.StopAllSFX();
                
                _sceneLoader.LoadScene();
                Time.timeScale = 0f;
                PlayerInteractionController.IsTimeFlowing = false;

                // Rest period after wave (all enemies are dead)
                if (waveIndex < waves.Length - 1)
                {
                    if (waveUI != null)
                    {
                        waveUI.StartCountdown(currentWave.restAfterWave);
                    }

                    yield return new WaitForSeconds(currentWave.restAfterWave);
                }
            }

            Debug.Log("All waves finished.");
        }

        private IEnumerator SpawnPortalRoutine(Wave wave, PortalWaveSpawn portalSpawn, System.Action onComplete)
        {
            if (portalSpawn.arrowImage != null && waveUI != null)
            {
                yield return StartCoroutine(waveUI.BlinkArrowSmooth(
                    portalSpawn.arrowImage,
                    portalSpawn.blinkSettings.blinkCount,
                    portalSpawn.blinkSettings.fadeInDuration,
                    portalSpawn.blinkSettings.holdDuration,
                    portalSpawn.blinkSettings.fadeOutDuration,
                    portalSpawn.blinkSettings.initialDelay
                ));
            }

            for (int e = 0; e < portalSpawn.enemies.Length; e++)
            {
                EnemySpawnEntry entry = portalSpawn.enemies[e];

                if (entry.enemyFactory == null)
                {
                    Debug.LogWarning($"Wave '{wave.waveName}' has a null enemy factory.");
                    continue;
                }

                for (int i = 0; i < entry.count; i++)
                {
                    SpawnEnemy(entry.enemyFactory, portalSpawn.spawnPosition);

                    float delay = Random.Range(wave.spawnDelayMin, wave.spawnDelayMax);
                    yield return new WaitForSeconds(delay);
                }
            }

            onComplete?.Invoke();
        }

        private EnemyBase SpawnEnemy(EnemyFactoryBase factory, Transform spawnPoint)
        {
            if (factory == null || spawnPoint == null)
            {
                Debug.LogWarning("Factory or spawn point is null.");
                return null;
            }

            EnemyBase enemy = factory.CreateEnemy();

            if (enemy.TryGetComponent<NavMeshAgent>(out var agent))
            {
                agent.Warp(spawnPoint.position);
            }
            else
            {
                enemy.transform.position = spawnPoint.position;
            }

            if (currentWaveEnemies != null)
            {
                currentWaveEnemies.Add(enemy);
            }

            return enemy;
        }

        private void OnDrawGizmosSelected()
        {
            if (waves == null) return;

            for (int i = 0; i < waves.Length; i++)
            {
                Wave wave = waves[i];
                if (wave == null || wave.portalSpawns == null) continue;

                foreach (PortalWaveSpawn portalSpawn in wave.portalSpawns)
                {
                    if (portalSpawn == null || portalSpawn.spawnPosition == null) continue;

                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(portalSpawn.spawnPosition.position, 0.5f);

                    if (vhsLocation != null)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(portalSpawn.spawnPosition.position, vhsLocation.position);
                    }
                }
            }
        }
    }
}

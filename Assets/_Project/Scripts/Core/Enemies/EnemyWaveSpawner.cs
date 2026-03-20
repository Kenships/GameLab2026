using System.Collections;
using _Project.Scripts.Core.Enemies.Factories;
using _Project.Scripts.Enemies;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace _Project.Scripts.Core.Enemies
{
    public class EnemyWaveSpawner : MonoBehaviour
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

        [Header("Wave Settings")]
        [SerializeField] private Wave[] waves;

        private void Start()
        {
            StartCoroutine(SpawnWaves());
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

                yield return new WaitUntil(() => activePortalRoutines <= 0);

                Debug.Log($"Finished {currentWave.waveName}");

                if (waveIndex < waves.Length - 1)
                {
                    yield return new WaitForSeconds(currentWave.restAfterWave);
                }
            }

            Debug.Log("All waves finished.");
        }

        private IEnumerator SpawnPortalRoutine(Wave wave, PortalWaveSpawn portalSpawn, System.Action onComplete)
        {
            if (portalSpawn.arrowImage != null)
            {
                yield return StartCoroutine(BlinkArrowSmooth(
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
        private void SpawnEnemy(EnemyFactoryBase factory, Transform spawnPoint)
        {
            if (factory == null || spawnPoint == null)
            {
                Debug.LogWarning("Factory or spawn point is null.");
                return;
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
        }

        private IEnumerator BlinkArrowSmooth(RawImage arrow,
                                      int blinkCount,
                                      float fadeIn,
                                      float hold,
                                      float fadeOut,
                                      float initialDelay)
        {
            if (arrow == null) yield break;

            if (initialDelay > 0f)
                yield return new WaitForSeconds(initialDelay);

            arrow.enabled = true;

            for (int i = 0; i < blinkCount; i++)
            {
                yield return StartCoroutine(FadeRawImage(arrow, 0f, 1f, fadeIn));

                yield return new WaitForSeconds(hold);

                yield return StartCoroutine(FadeRawImage(arrow, 1f, 0f, fadeOut));
            }

            SetRawImageAlpha(arrow, 0f);

            arrow.enabled = false;
        }

        private IEnumerator FadeRawImage(RawImage img, float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float alpha = Mathf.Lerp(from, to, t);
                SetRawImageAlpha(img, alpha);
                yield return null;
            }
            SetRawImageAlpha(img, to);
        }

        private void SetRawImageAlpha(RawImage img, float alpha)
        {
            Color color = img.color;
            color.a = alpha;
            img.color = color;
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
using Obvious.Soap;
using PrimeTween;
using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Core.Modules.Base_Class;
using UnityEngine.UI;
using _Project.Scripts.UI;

namespace _Project.Scripts.Core.Modules
{
    public class ModuleSpawner : MonoBehaviour
    {
        [SerializeField] private ScriptableEventGameObject spawnEvent;
        [System.Serializable]
        public class LandingInfo
        {
            public Transform start;
            public Transform end;
            public RawImage arrow;
            [HideInInspector] public List<GameObject> stackedModules = new List<GameObject>();
        }
        [SerializeField] private HintUI hintUI;
        [SerializeField] private ParticleSystem dropEffect;
        [SerializeField] private List<LandingInfo> landingInfos = new List<LandingInfo>();
        private float lastCleanTime;
        private float cleanInterval = 0.25f;
        private bool oneTimeHint = true;


        private void Start()
        {
            spawnEvent.OnRaised += SpawnEventOnRaised;
        }

        private void OnDestroy()
        {
            spawnEvent.OnRaised -= SpawnEventOnRaised;
        }

        private void FixedUpdate()
        {
            if (Time.time - lastCleanTime >= cleanInterval)
            {
                lastCleanTime = Time.time;

                for (int i = 0; i < landingInfos.Count; i++)
                {
                    CleanAndDropModules(landingInfos[i]);
                }
            }
        }

        private void SpawnEventOnRaised(GameObject obj)
        {
            int index = GetBestAvailableIndex();
            LandingInfo info = landingInfos[index];
            int height = info.stackedModules.Count;
            Vector3 targetPos = info.end.position + Vector3.up * height;

            GameObject module = Instantiate(obj, info.start.position, Quaternion.identity);
            info.stackedModules.Add(module);
            Tween.Position(module.transform, targetPos, 1.5f, Ease.InCubic)
                .OnComplete(() => {
                    if (oneTimeHint) 
                    {
                        info.arrow.enabled = true;
                        hintUI.PlayArrowBackAndForth(info.arrow); 
                    }
                    oneTimeHint = false;
                    dropEffect.transform.position = targetPos;
                    dropEffect.Play();
                });
        }

        private int GetBestAvailableIndex()
        {
            int minCount = int.MaxValue;
            List<int> bestIndices = new List<int>();

            for (int i = 0; i < landingInfos.Count; i++)
            {
                int count = landingInfos[i].stackedModules.Count;
                if (count < minCount)
                {
                    minCount = count;
                    bestIndices.Clear();
                    bestIndices.Add(i);
                }
                else if (count == minCount)
                {
                    bestIndices.Add(i);
                }
            }

            return bestIndices[Random.Range(0, bestIndices.Count)];
        }

        private void CleanAndDropModules(LandingInfo info)
        {
            for (int i = 0; i < info.stackedModules.Count; i++)
            {
                GameObject module = info.stackedModules[i];
                if (module != null && module.GetComponent<Module>().EnableModule != false)
                {
                    hintUI.StopArrowBackAndForth();
                    info.arrow.enabled = false;
                    info.stackedModules.RemoveAt(i);
                    i--;

                    for (int j = i + 1; j < info.stackedModules.Count; j++)
                    {
                        GameObject aboveModule = info.stackedModules[j];
                        Vector3 newPos = info.end.position + Vector3.up * j;
                        Tween.Position(aboveModule.transform, newPos, 0.5f, Ease.InCubic);
                    }
                }
            }
        }
    }
}
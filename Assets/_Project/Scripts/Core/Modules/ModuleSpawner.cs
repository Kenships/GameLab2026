using System.Collections;
using Obvious.Soap;
using PrimeTween;
using System.Collections.Generic;
using UnityEngine;
using _Project.Scripts.Core.Modules.Base_Class;
using UnityEngine.UI;
using _Project.Scripts.UI;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.InputManagement;
using _Project.Scripts.Util.Timer;
using Sisus.Init;
using UnityEngine.InputSystem;

namespace _Project.Scripts.Core.Modules
{
    public class ModuleSpawner : MonoBehaviour<AudioPooler>
    {
        [SerializeField] private FloatVariable hapticsIntensity;
        [SerializeField] private NESActionReader player1;
        [SerializeField] private NESActionReader player2;
        
        [SerializeField] private ScriptableEventGameObject spawnEvent;
        [SerializeField] private ScriptableEventGameObject moduleSpawnedEvent;
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

        [Header("Audio")][SerializeField] private AudioClip landingSound;
        [SerializeField] private float landingSoundVolume = 0.1f;

        private AudioPooler _audioPooler;
        private List<Gamepad> _gamePads = new();
        
        
        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }

        private void Start()
        {
            
            spawnEvent.OnRaised += SpawnEventOnRaised;

            if (!player1 || !player2)
            {
                return;
            }
            
            if (player1.TryGetGamePad(out Gamepad player1Pad))
            {
                _gamePads.Add(player1Pad);
                player1Pad.SetMotorSpeeds(0f, 0f);
            }

            if (player2.TryGetGamePad(out Gamepad player2Pad))
            {
                _gamePads.Add(player2Pad);
                player2Pad.SetMotorSpeeds(0f, 0f);
            }
            
            
            
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
            _audioPooler.New2DAudio(landingSound).OnChannel(AudioType.Sfx).SetVolume(landingSoundVolume).AddToScene(gameObject.scene.buildIndex).Play();
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
                        hintUI.PlayArrowBackAndForth(info.arrow); 
                    }
                    oneTimeHint = false;
                    dropEffect.transform.position = targetPos;
                    
                    StartCoroutine(PlayLandingSound());
                    
                    dropEffect.Play();
                });
            moduleSpawnedEvent?.Raise(module);
        }

        private IEnumerator PlayLandingSound()
        {
            foreach (Gamepad pad in _gamePads)
            {
                pad.SetMotorSpeeds(1f * hapticsIntensity.Value, .2f * hapticsIntensity.Value);
            }
            
            float timer = .5f;
            while (timer > 0)
            {
                timer -= Time.unscaledDeltaTime;
                yield return null;
            }
            
            foreach (Gamepad pad in _gamePads)
            {
                pad.SetMotorSpeeds(0f, 0f);
            }
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
                if (module != null && module.GetComponent<PickupModuleBase>().IsPickedUp)
                {
                    hintUI.StopArrowBackAndForth(info.arrow);
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

using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Core.AudioPooling.Implement;
using _Project.Scripts.Core.AudioPooling.Interface;
using _Project.Scripts.Util.CustomAttributes;
using AYellowpaper.SerializedCollections;
using Sisus.Init;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;
using ILogger = _Project.Scripts.Util.Logger.Interface.ILogger;
using Logger = _Project.Scripts.Util.Logger.Logger;

namespace _Project.Scripts.Core.AudioPooling
{
    public enum AudioOverridePolicy
    {
        DontPlayOnFull,
        OverrideFirst
    }
    [Service(typeof(AudioPooler), LoadScene = 0)]
    public partial class AudioPooler : MonoBehaviour<ILogger>
    {
        ILogger _logger;
        protected override void Init(ILogger playerReader)
        {
            _logger = playerReader;
        }

        [SerializeField] private bool createBuffer;
        [SerializeField, ShowIf(nameof(createBuffer))] private int bufferSize;

        [SerializeField] private AudioOverridePolicy audioOverridePolicy;

        [SerializeField] private SerializedDictionary<AudioType, int> maxAudioSources;
        [SerializeField] private SerializedDictionary<AudioType, AudioMixerGroup> audioMixerGroups;
        
        #region DebugProperties
        [Space]
        [SerializeField] private bool showDebug;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebug))] private int numberOfActiveSources;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebug))] private int numberOfInactiveSources;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebug))] private SerializedDictionary<AudioType, List<PooledAudioSource>> activeSourcesByAudioType;
        [SerializeField, ReadOnly, ShowIf(nameof(showDebug))] private SerializedDictionary<int, List<PooledAudioSource>> activeSourcesBySceneIndex;

        #endregion
        
        private readonly Stack<PooledAudioSource> _inactiveSources = new();

        private readonly HashSet<AudioType> _audioTypes = new();

        private void OnValidate()
        {
#if UNITY_EDITOR
            //Add new keys
            _audioTypes.Clear();
            _audioTypes.AddRange((AudioType[])Enum.GetValues(typeof(AudioType)));
            foreach (AudioType audioType in _audioTypes)
            {
                if (!maxAudioSources.ContainsKey(audioType))
                {
                    maxAudioSources.Add(audioType, 0);
                }

                if (!audioMixerGroups.ContainsKey(audioType))
                {
                    audioMixerGroups.Add(audioType, null);
                }
            }

            //Remove deleted Keys
            var keysToRemoveAudioSourceMax = new List<AudioType>();
            var keysToRemoveAudioMixerGroups = new List<AudioType>();

            foreach (var keyValue in maxAudioSources)
            {
                if (!_audioTypes.Contains(keyValue.Key))
                {
                    keysToRemoveAudioSourceMax.Add(keyValue.Key);
                }

                if (!audioMixerGroups.ContainsKey(keyValue.Key))
                {
                    keysToRemoveAudioMixerGroups.Add(keyValue.Key);
                }
            }

            foreach (var key in keysToRemoveAudioSourceMax)
            {
                maxAudioSources.Remove(key);
            }

            foreach (var key in keysToRemoveAudioMixerGroups)
            {
                audioMixerGroups.Remove(key);
            }
#endif
        }

        protected override void OnAwake()
        {
            for (int i = 0; i < bufferSize; i++)
            {
                var audioSource = CreateAudioSource();
                audioSource.gameObject.SetActive(false);
                _inactiveSources.Push(audioSource);
            }
        }

        public AudioConfig2D New2DAudio(AudioClip clip)
        {
            return new AudioConfig2D(this, clip);
        }

        public AudioConfig3D New3DAudio(AudioClip clip)
        {
            return new AudioConfig3D(this, clip);
        }

        public IAudioPlayer Play(IAudioConfig audioConfig)
        {
            // check for capacity availability
            PooledAudioSource audioSource = null;
            if (!activeSourcesByAudioType.TryGetValue(audioConfig.AudioType, out List<PooledAudioSource> list) ||
                list.Count < maxAudioSources[audioConfig.AudioType])
            {
                return GetNextAudioSource(audioConfig);
            }

            int currentPriority = audioConfig.Priority;
            int minPriority = Int32.MaxValue;
            foreach (PooledAudioSource activeAudio in activeSourcesByAudioType[audioConfig.AudioType]
                         .Where(activeAudio =>
                                    activeAudio.Priority < minPriority && activeAudio.Priority < currentPriority))
            {
                minPriority = activeAudio.Priority;
                audioSource = activeAudio;
            }

            if (audioSource != null)
            {
                _logger.LogWarning($"AudioPooler of AudioType: {audioConfig.AudioType} is Full. " +
                                   $"Replaced playing {audioSource.Clip.name} with " +
                                   $"{audioConfig.Clip.name}");
                audioSource.Stop();

                return GetNextAudioSource(audioConfig);
            }

            switch (audioOverridePolicy)
            {
                case AudioOverridePolicy.DontPlayOnFull:
                    _logger.LogWarning($"AudioPooler of AudioType: {audioConfig.AudioType} is Full. " +
                                       $"Skip playing {audioConfig.Clip.name}");
                    return new EmptyAudioPlayer();

                case AudioOverridePolicy.OverrideFirst:
                    PooledAudioSource first = activeSourcesByAudioType[audioConfig.AudioType]
                        .First(val => val.Priority == audioConfig.Priority);

                    _logger.LogWarning($"AudioPooler of AudioType: {audioConfig.AudioType} is Full. " +
                                       $"Replaced playing {first.Clip.name} with " +
                                       $"{audioConfig.Clip.name}");
                    first.Stop();
                    break;
            }

            return GetNextAudioSource(audioConfig);
        }

        public AudioMixerGroup GetMixerFor(AudioType audioType)
        {
            return audioMixerGroups[audioType];
        }

        private IAudioPlayer GetNextAudioSource(IAudioConfig audioConfig)
        {
            PooledAudioSource audioSource;
            if (_inactiveSources.Count == 0)
            {
                audioSource = CreateAudioSource();
            }
            else
            {
                audioSource = _inactiveSources.Pop();
            }

            audioSource.Initialize(audioConfig);
            audioSource.gameObject.SetActive(true);
            audioSource.Play();

            if (activeSourcesByAudioType.ContainsKey(audioConfig.AudioType))
            {
                activeSourcesByAudioType[audioConfig.AudioType].Add(audioSource);
            }
            else
            {
                activeSourcesByAudioType.TryAdd(audioConfig.AudioType, new List<PooledAudioSource> { audioSource });
            }

            if (activeSourcesBySceneIndex.ContainsKey(audioConfig.SceneBuildIndex))
            {
                activeSourcesBySceneIndex[audioConfig.SceneBuildIndex].Add(audioSource);
            }
            else
            {
                activeSourcesBySceneIndex.TryAdd(audioConfig.SceneBuildIndex,
                    new List<PooledAudioSource> { audioSource });
            }

            numberOfInactiveSources = _inactiveSources.Count;
            numberOfActiveSources++;
            return new AudioPlayer(audioSource);
        }

        public void ReturnToPool(PooledAudioSource pooledAudioSource)
        {
            activeSourcesByAudioType[pooledAudioSource.AudioType].Remove(pooledAudioSource);
            activeSourcesBySceneIndex[pooledAudioSource.SceneBuildIndex].Remove(pooledAudioSource);
            pooledAudioSource.gameObject.SetActive(false);
            _inactiveSources.Push(pooledAudioSource);
            numberOfActiveSources--;
            numberOfInactiveSources = _inactiveSources.Count;
        }

        private PooledAudioSource CreateAudioSource()
        {
            GameObject audioObject = new GameObject("AudioSource");
            audioObject.transform.SetParent(transform);
            PooledAudioSource audioComponent = audioObject.AddComponent<PooledAudioSource>();
            return audioComponent;
        }

        public void StopAllSFX()
        {
            var allActiveSources = activeSourcesByAudioType.Values
                .SelectMany(list => list)
                .ToList();

            foreach (var source in allActiveSources)
            {
                source.Stop();
                ReturnToPool(source);
            }
        }

    }
}

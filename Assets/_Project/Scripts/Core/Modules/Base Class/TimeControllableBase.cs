using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.AudioPooling.Interface;
using _Project.Scripts.Core.Modules.Interface;
using _Project.Scripts.Core.Player;
using Sisus.Init;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules.Base_Class
{
    public class TimeControllableBase : MonoBehaviour<AudioPooler>, ITimeControllable
    {
        [Header("Audio")]
        [SerializeField] protected AudioClip fastForwardSound;
        [SerializeField] protected float fastForwardSoundVolume = 1f;
        [SerializeField] protected AudioClip rewindSound;
        [SerializeField] protected float rewindSoundVolume = 1f;
        
        private enum TimeAction
        {
            FastForward,
            Rewind
        }
        protected bool _isRewinding;
        protected bool _isFastForwarding;
        protected IAudioPlayer _currentFastForwardSound;
        protected IAudioPlayer _currentRewindSound;
        
        protected AudioPooler _audioPooler;
        protected Action _fastForwardAction;
        protected Action _rewindAction;
        protected Action _cancelFastForwardAction;
        protected Action _cancelRewindAction;

        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }

        private readonly Dictionary<PlayerData.PlayerID, TimeAction> _interactingPlayers = new();

        private void HandleInteract(PlayerData.PlayerID playerID, TimeAction action)
        {
            if (_interactingPlayers.Count == 0)
            {
                _interactingPlayers.Add(playerID, action);
                PerformAction(action);
            }
            else if (_interactingPlayers.ContainsKey(playerID) && _interactingPlayers.Count == 1)
            {
                CancelAction(_interactingPlayers[playerID]);
                
                _interactingPlayers.Add(playerID, action);
                PerformAction(action);
            }
            else if (!_interactingPlayers.ContainsKey(playerID) && _interactingPlayers.Count == 1)
            {
                var existingAction = _interactingPlayers.First().Value;
                if (existingAction != action)
                {
                    // Opposite effect => nothing happens
                    CancelAction(existingAction);
                    _interactingPlayers.Clear();
                }
                else
                {
                    // Same effect => stronger power
                    _interactingPlayers.Add(playerID, action);
                    UpdateDoubleAudio();
                }
            }
        }

        private void UpdateDoubleAudio()
        {
            if (IsDoubleFastForward())
            {
                _currentFastForwardSound?.Stop();
                _currentFastForwardSound = _audioPooler.New2DAudio(fastForwardSound)
                    .OnChannel(AudioType.Sfx)
                    .SetVolume(fastForwardSoundVolume)
                    .SetPitch(1.25f)
                    .LoopAudio()
                    .Play();
            }
            else if (IsDoubleRewind())
            {
                _currentRewindSound?.Stop();
                _currentRewindSound = _audioPooler.New2DAudio(rewindSound)
                    .OnChannel(AudioType.Sfx)
                    .SetVolume(rewindSoundVolume)
                    .SetPitch(1.25f)
                    .LoopAudio()
                    .Play();
            }
        }

        private void PerformAction(TimeAction action)
        {
            switch (action)
            {
                case TimeAction.FastForward:
                    _isFastForwarding = true;
                    _fastForwardAction?.Invoke();
                    _currentFastForwardSound ??= _audioPooler.New2DAudio(fastForwardSound).OnChannel(AudioType.Sfx)
                        .SetVolume(fastForwardSoundVolume).LoopAudio().Play();
                    break;
                case TimeAction.Rewind:
                    _isRewinding = true;
                    _rewindAction?.Invoke();
                    _currentRewindSound ??= _audioPooler.New2DAudio(rewindSound).OnChannel(AudioType.Sfx)
                        .SetVolume(rewindSoundVolume).LoopAudio().Play();
                    break;
            }
        }

        private void CancelAction(TimeAction action)
        {
            switch (action)
            {
                case TimeAction.FastForward:
                    _isFastForwarding = false;
                    _cancelFastForwardAction?.Invoke();
                    _currentFastForwardSound?.Stop();
                    _currentFastForwardSound = null;
                    break;
                case TimeAction.Rewind:
                    _isRewinding = false;
                    _cancelRewindAction?.Invoke();
                    _currentRewindSound?.Stop();
                    _currentRewindSound = null;
                    break;
            }
        }
        
        public virtual void Rewind(PlayerData.PlayerID playerID)
        {
            HandleInteract(playerID, TimeAction.Rewind);
        }

        public virtual void CancelRewind(PlayerData.PlayerID playerID)
        {
            if (!_interactingPlayers.ContainsKey(playerID) || _interactingPlayers[playerID] != TimeAction.Rewind)
            {
                return;
            }
            
            CancelAction(_interactingPlayers[playerID]);
            _interactingPlayers.Remove(playerID);

            if (_interactingPlayers.Count == 1)
            {
                PerformAction(_interactingPlayers.First().Value);
            }
        }
    
        public virtual void FastForward(PlayerData.PlayerID playerID)
        { 
            HandleInteract(playerID, TimeAction.FastForward);
        }

        public virtual void CancelFastForward(PlayerData.PlayerID playerID)
        {
            if (!_interactingPlayers.ContainsKey(playerID) || _interactingPlayers[playerID] != TimeAction.FastForward)
            {
                return;
            }
            
            CancelAction(_interactingPlayers[playerID]);
            _interactingPlayers.Remove(playerID);
            
            if (_interactingPlayers.Count == 1)
            {
                PerformAction(_interactingPlayers.First().Value);
            }
        }

        protected void ClearInteractingPlayers()
        {
            _interactingPlayers.Clear();
        }

        protected bool IsDoubleFastForward()
        {
            return _interactingPlayers.Count == 2 && _interactingPlayers.Values.All(v => v == TimeAction.FastForward);
        }

        protected bool IsDoubleRewind()
        {
            return _interactingPlayers.Count == 2 && _interactingPlayers.Values.All(v => v == TimeAction.Rewind);
        }
    }
}

using System;
using System.Collections; // 记得引用这个
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using UnityEngine;

namespace _Project.Scripts.Tutorial
{
    public class TutCoopInteractListener : MonoBehaviour, ITutorialListener
    {
        [SerializeField] private TutPlayerInteractionController player1;
        [SerializeField] private TutPlayerInteractionController player2;

        private Action _callback;
        private bool _isResolved = false;

        public void Invoke(Action callback)
        {
            _callback = callback;
            _isResolved = false;

            player1.OnRewind += TimeControl;
            player2.OnRewind += TimeControl;
            player1.OnFastForward += TimeControl;
            player2.OnFastForward += TimeControl;
        }

        private void TimeControl(PlayerData.PlayerID playerID)
        {
            if (_isResolved) return;

            StartCoroutine(CheckStatusAtEndOfFrame());
        }

        private IEnumerator CheckStatusAtEndOfFrame()
        {
            yield return new WaitForEndOfFrame();

            if (_isResolved) yield break;

            if (CheckPlayerHolding(player1) || CheckPlayerHolding(player2))
            {
                CompleteStep();
            }
        }

        private bool CheckPlayerHolding(TutPlayerInteractionController player)
        {
            return player.HeldModule != null && player.HeldModule.TryGetComponent(out Module m) && m.IsTimeControlling;
        }

        private void CompleteStep()
        {
            _isResolved = true;

            player1.OnRewind -= TimeControl;
            player2.OnRewind -= TimeControl;
            player1.OnFastForward -= TimeControl;
            player2.OnFastForward -= TimeControl;

            _callback?.Invoke();
        }

        private void OnDestroy()
        {
            if (player1)
            {
                player1.OnRewind -= TimeControl;
                player1.OnFastForward -= TimeControl;
            }
            if (player2)
            {
                player2.OnRewind -= TimeControl;
                player2.OnFastForward -= TimeControl;
            }
        }
    }
}
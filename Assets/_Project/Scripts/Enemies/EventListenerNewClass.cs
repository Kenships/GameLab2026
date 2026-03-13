using _Project.Scripts.Core.Enemies;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Enemies
{
    [AddComponentMenu("Soap/EventListeners/EventListener"+nameof(EnemyWaveSpawner.Wave))]
    public class EventListenerWaveData : EventListenerGeneric<EnemyWaveSpawner.Wave>
    {
        [SerializeField] private EventResponse[] _eventResponses = null;
        protected override EventResponse<EnemyWaveSpawner.Wave>[] EventResponses => _eventResponses;
        [System.Serializable]
        public class EventResponse : EventResponse<EnemyWaveSpawner.Wave>
        {
            [SerializeField] private ScriptableEventWaveData _scriptableEvent = null;
            public override ScriptableEvent<EnemyWaveSpawner.Wave> ScriptableEvent => _scriptableEvent;
            [SerializeField] private NewClassUnityEvent _response = null;
            public override UnityEvent<EnemyWaveSpawner.Wave> Response => _response;
        }
        [System.Serializable]
        public class NewClassUnityEvent : UnityEvent<EnemyWaveSpawner.Wave>
        {
            
        }
    }
}

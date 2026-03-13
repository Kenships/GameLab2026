using _Project.Scripts.Core.Enemies;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Enemies
{
    [CreateAssetMenu(fileName = "ScriptableEvent" + nameof(EnemyWaveSpawner.Wave), menuName = "Soap/ScriptableEvents/"+ nameof(EnemyWaveSpawner.Wave))]
    public class ScriptableEventWaveData : ScriptableEvent<EnemyWaveSpawner.Wave>
    {
        
    }
}

using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Effects.Interface;
using UnityEngine;

namespace _Project.Scripts.Effects.Inflictors
{
    [CreateAssetMenu(fileName = nameof(EnemyEffectInflictor), menuName = "Inflictors/EnemyEffectInflictor")]
    public class EnemyEffectInflictor : ScriptableObject
    {
        [SerializeReference, SubclassSelector] private IEffectFactory<EnemyBase>[] enemyEffectFactories;
        
        [field: SerializeField] public GameObject CastVFX { get; private set;}
        [field: SerializeField] public AudioClip AudioClip { get; private set;}
        
        public void Inflict(EnemyBase enemy)
        {
            foreach (var effectFactory in enemyEffectFactories)
            {
                enemy.ApplyEffect(effectFactory.CreateEffect());
            }
        }
    }
}

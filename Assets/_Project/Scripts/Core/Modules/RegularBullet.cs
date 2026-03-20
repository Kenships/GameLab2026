using _Project.Scripts.Core.AudioPooling;
using _Project.Scripts.Core.Enemies;
using _Project.Scripts.Effects.Inflictors;
using _Project.Scripts.Util.ExtensionMethods;
using Sisus.Init;
using UnityEngine;
using AudioType = _Project.Scripts.Core.AudioPooling.Interface.AudioType;

namespace _Project.Scripts.Core.Modules
{
    [RequireComponent(typeof(Rigidbody))]
    public class RegularBullet : MonoBehaviour<AudioPooler>
    {
        private LayerMask _enemyLayer;
        
        private AudioPooler _audioPooler;
        private Rigidbody _rb;
        
        private EnemyEffectInflictor _inflictor;
        private float _speed;
        private int _pierce;

        private int _pierceCount;
        
        protected override void Init(AudioPooler audioPooler)
        {
            _audioPooler = audioPooler;
        }
        
        public void Initialize(LayerMask targetLayer, EnemyEffectInflictor inflictor, float speed, float projectileLifeTime, int pierce)
        {
            _inflictor = inflictor;
            _enemyLayer = targetLayer;
            _speed = speed;
            _pierce = pierce;
            _pierceCount = 0;
            
            Destroy(gameObject, projectileLifeTime);

            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.freezeRotation = true;
            _rb.isKinematic = true;
        }

        private void FixedUpdate()
        {
            transform.Translate(Vector3.forward * (_speed * Time.deltaTime));
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject || !other.IsOnLayer(_enemyLayer))
            {
                return;
            }

            if (other.TryGetComponent(out EnemyBase enemy))
            {
                Inflict(enemy);
                _pierceCount++;

                if (_pierceCount >= _pierce)
                {
                    Destroy(gameObject);
                }
            }
        }
        
        private void Inflict(EnemyBase enemy)
        {
            if (_inflictor == null)
            {
                Debug.LogWarning("Inflector is null");
                return;
            }

            _inflictor.Inflict(enemy);

            if (_inflictor.CastVFX)
            {
                Instantiate(_inflictor.CastVFX, transform.position, Quaternion.identity);
            }

            if (_inflictor.AudioClip)
            {
                _audioPooler
                    .New3DAudio(_inflictor.AudioClip)
                    .OnChannel(AudioType.Sfx)
                    .AtPosition(transform.position)
                    .Play();
            }

            if (_inflictor.LingeringVFX)
            {
                Instantiate(_inflictor.LingeringVFX, transform.position, Quaternion.identity);
            }
        }

        
    }
}

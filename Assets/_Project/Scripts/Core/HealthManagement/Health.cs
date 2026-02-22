using UnityEngine;

namespace _Project.Scripts.Core.HealthManagement
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] protected int currentHealth;
        protected int _maxHealth;
    

        public void Initialize(int health)
        {
            _maxHealth = health;
            currentHealth = _maxHealth;
        }

        public virtual void Kill()
        {
            Destroy(gameObject);
        }

        public virtual void Damage(int damage)
        {
            currentHealth = currentHealth + damage;

            if (currentHealth > _maxHealth)
            {
                currentHealth = _maxHealth;
            } else if (currentHealth < 0)
            {
                Kill();
            }
        }
    }
}


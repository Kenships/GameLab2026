using Sisus.Init;
using UnityEngine;


public interface IDamageable
{
    void EffectHealth(int healthChange);
    public void Initialize(int health);
}




public class Health : MonoBehaviour, IDamageable
{
    protected int maxHealth;
    [SerializeField] protected int currentHealth;

    public void Initialize(int health)
    {
        this.maxHealth = health;
        this.currentHealth = maxHealth;
    }

    //Damage is negative 
    //Healing is positive
    public virtual void EffectHealth(int healthChange)
    {
        currentHealth = currentHealth + healthChange;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        } else if (currentHealth < 0)
        {
            Kill();
        }
    }

    public virtual void Kill()
    {
        Destroy(gameObject);
    }
}


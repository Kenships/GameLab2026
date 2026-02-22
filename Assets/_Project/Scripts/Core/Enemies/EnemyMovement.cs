using _Project.Scripts.Core.HealthManagement;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform target; //takes from EnemySpawnManager
    private float moveSpeed; //takes from  EnemySpawnManager
    private float attackCooldown; //takes from EnemySpawnManager
    private int attackDamage; //takes from  EnemySpawnManager

    private bool atVHS = false; //true if arrived at VHS

    private float damageTimer;

    public void Initialize(Transform target, float moveSpeed, float attackCooldown, int attackDamage)
    {
        atVHS = false;
        this.target = target;
        this.moveSpeed = moveSpeed;
        this.attackCooldown = attackCooldown;
        this.attackDamage = attackDamage;

    }

    private void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        // Face the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 0.5f)
        {
            atVHS = true;
        }

        if (atVHS == false)
        {
                // Move towards the VHS location
                transform.position += direction * moveSpeed * Time.deltaTime;
        }

        if (atVHS)
        {
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0f)
            {
                DamageVHS(attackDamage);
                damageTimer = attackCooldown;
            }
        }
    }

    private void DamageVHS(int Damage)
    {
        if (target.GetComponent<IDamageable>() != null)
        {
            target.GetComponent<IDamageable>().Damage(-1 * Damage);
        }
    }
}

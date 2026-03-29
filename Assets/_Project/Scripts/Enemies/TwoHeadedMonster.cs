using _Project.Scripts.Core;
using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.Modules.Base_Class;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(RangeDetector))]
public class TwoHeadedMonster : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float walkTimeBeforeAttack = 5f;
    [SerializeField] private float attackDuration = 2f;
    [SerializeField] private float attackDamage = 30f;
    [SerializeField] private float attackMoment = 1.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private RangeDetector rangeDetector;

    private float walkTimer;
    private Transform attackTarget; 
    private Quaternion preAttackRotation;
    private float attackTimer;
    private bool hasDealtDamage;

    private enum EnemyState { Walking, Attacking }
    private EnemyState currentState = EnemyState.Walking;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rangeDetector = GetComponent<RangeDetector>();
        walkTimer = walkTimeBeforeAttack;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Walking:
                UpdateWalking();
                break;
            case EnemyState.Attacking:
                UpdateAttacking();
                break;
        }
    }

    private void UpdateWalking()
    {
        walkTimer -= Time.deltaTime;

        if (walkTimer <= 0f)
        {
            Transform nearestTower = FindNearestTower();
            if (nearestTower != null)
            {
                agent.isStopped = true;

                attackTarget = nearestTower;
                preAttackRotation = transform.rotation;

                currentState = EnemyState.Attacking;
                attackTimer = attackDuration;
                hasDealtDamage = false;

                if (animator != null)
                    animator.SetBool("isAttacking", true);
            }
            else
            {
                walkTimer = walkTimeBeforeAttack;
            }
        }
    }

    private void UpdateAttacking()
    {
        Vector3 direction = (attackTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        attackTimer -= Time.deltaTime;

        if (!hasDealtDamage && attackTimer <= attackDuration - attackMoment)
        {
            attackTarget.GetComponent<IDamageable>()?.Damage(attackDamage);
            if (attackTarget.CompareTag("TriggerTypeModule"))
                attackTarget.GetComponent<Module>().state = Module.ModuleState.Used;
            hasDealtDamage = true;
        }

        if (attackTimer <= 0f)
        {
            ExitAttackState();
        }
    }

    private void ExitAttackState()
    {
        transform.rotation = preAttackRotation;
        if (animator != null)
            animator.SetBool("isAttacking", false);
        agent.isStopped = false;
        walkTimer = walkTimeBeforeAttack;
        currentState = EnemyState.Walking;
    }
    private Transform FindNearestTower()
    {
        if (rangeDetector == null) return null;

        IDamageable nearestDamageable = rangeDetector.GetClosestObjectOfType<IDamageable>();
        if (nearestDamageable != null)
        {
            MonoBehaviour mb = nearestDamageable as MonoBehaviour;
            if (mb != null)
                return mb.transform;
        }
        return null;
    }
}
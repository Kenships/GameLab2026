using UnityEngine;

public class RecordDiscBullet : MonoBehaviour
{
    private float rotateSpeed = 200f; //takes from disc shooter
    private float wobbleAmount = 0f; //takes from disc shooter 
    [SerializeField] private float hitCooldown = 0.2f; //cooldown before it starts detecting another trigger on hit
    [SerializeField] private float enemySearchRadius = 30f; //the radius that it searches the next closest enemy if theres none it will go back to previous enemy
    [SerializeField] private float bulletLifetime = 3.5f; // max seconds until bullet is destroyed regardless of how many targets it hit
    private Transform target;
    private Transform lastKnownTarget;
    private float speed;
    private int maxTargets;
    private int hitCount;
    private float hitTimer;
    private Rigidbody rb;
    private LayerMask enemyLayer;

    public void Initialize(Transform target, float speed, int maxTargets, float rotateSpeed, float wobbleAmount, LayerMask enemyLayer)
    {
        this.rotateSpeed = rotateSpeed;
        this.target = target;
        this.lastKnownTarget = target;
        this.speed = speed;
        this.maxTargets = maxTargets;
        this.hitCount = 0;
        this.wobbleAmount = wobbleAmount;
        this.enemyLayer = enemyLayer;
        Destroy(gameObject, bulletLifetime); 

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (wobbleAmount <= 0f)
            {
                rb.freezeRotation = true;
            }
            else
            {
                rb.angularDamping = Mathf.Lerp(100f, 0f, wobbleAmount);
            }

            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    public void SetWobble(float amount)
    {
        wobbleAmount = amount;

        if (rb != null)
        {
            if (wobbleAmount <= 0f)
            {
                rb.freezeRotation = true;
            }
            else
            {
                rb.freezeRotation = false;
                rb.angularDamping = Mathf.Lerp(100f, 0f, wobbleAmount);
            }
        }
    }

    private void Update()
    {
        hitTimer -= Time.deltaTime;
        
        FindClosestEnemy();
        Transform homingTarget = target;

        // If no enemies, home back to latest target
        if (homingTarget == null)
        {
            homingTarget = lastKnownTarget;
        }

        // if the OG is dead too then just go straight
        if (homingTarget == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        Vector3 direction = (homingTarget.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void FindClosestEnemy()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, enemySearchRadius, enemyLayer);

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider enemy in enemiesInRange)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
            lastKnownTarget = closestEnemy;
        }
        else
        {
            target = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitTimer > 0f) return;

        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            hitCount++;
            hitTimer = hitCooldown;

            if (hitCount >= maxTargets)
            {
                Destroy(gameObject);
            }
        }
    }
}
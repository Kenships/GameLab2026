using System;
using UnityEngine;

public class RecordDiscBullet : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float wobbleAmount = 0f;
    [SerializeField] private float hitCooldown = 0.2f;

    private Transform target;
    private float speed;
    private int maxTargets;
    private int hitCount;
    private float hitTimer;
    private Rigidbody rb;

    public void Initialize(Transform target, float speed, int maxTargets, float rotateSpeed, float wobbleAmount)
    {
        this.rotateSpeed = rotateSpeed;
        this.target = target;
        this.speed = speed;
        this.maxTargets = maxTargets;
        this.hitCount = 0;
        this.wobbleAmount = wobbleAmount;

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

        if (target == null)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime
        );

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitTimer > 0f) return;

        if (((1 << other.gameObject.layer) & LayerMask.GetMask("Enemies")) != 0)
        {
            hitCount++;
            Debug.Log(hitCount);
            hitTimer = hitCooldown;

            if (hitCount >= maxTargets)
            {
                Debug.Log("destroy" + maxTargets);
                Destroy(gameObject);
            }
        }
    }
}
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Transform target; //takes from EnemySpawnManager
    private float moveSpeed; //takes from  EnemySpawnManager

    public void Initialize(Transform target, float moveSpeed)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
    }

    private void Update()
    {
        if (target == null) return;

        // Move towards the VHS location
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Face the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;

        // Optional: destroy enemy when it reaches the VHS location
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 0.5f)
        {
            // TODO: what happens when you hit the vhs idk
            Destroy(gameObject);
        }
    }
}
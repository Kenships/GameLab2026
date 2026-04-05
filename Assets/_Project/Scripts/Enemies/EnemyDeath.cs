using _Project.Scripts.Core.HealthManagement;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [SerializeField] private Vector2 deathAnimSize = new Vector2(100, 100);
    private void OnDestroy()
    {
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        StopMotionManager.Instance.SpawnAnimation(targetPos, deathAnimSize);
    }
}

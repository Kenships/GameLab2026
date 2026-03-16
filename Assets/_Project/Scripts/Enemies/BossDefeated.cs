using _Project.Scripts.Core.HealthManagement;
using _Project.Scripts.Core.SceneLoading;
using Obvious.Soap;
using UnityEngine;

public class BossDefeated : MonoBehaviour
{
    [SerializeField] private ScriptableEventNoParam bossDefeatedEvent;

    private void OnDestroy()
    {
        bossDefeatedEvent.Raise();
    }
    [ContextMenu("Test")]
    public void Test()
    {
        Destroy(this);
    }
}

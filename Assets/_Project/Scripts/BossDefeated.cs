using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts
{
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
}

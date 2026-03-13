using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Core.Modules
{
    public class ModuleSpawner : MonoBehaviour
    {
        [SerializeField] private ScriptableEventGameObject spawnEvent;
        [SerializeField] private Transform spawnLocation;

        private void Start()
        {
            spawnLocation ??= transform;
            spawnEvent.OnRaised += SpawnEventOnRaised;
        }

        private void OnDestroy()
        {
            spawnEvent.OnRaised -= SpawnEventOnRaised;
        }

        private void SpawnEventOnRaised(GameObject obj)
        {
            GameObject module = Instantiate(obj);
            module.transform.position = spawnLocation ? spawnLocation.position : transform.position;
        }
    }
}

using UnityEngine;

namespace _Project.Scripts.Core.Grid
{
    public interface IGridService
    {
        public Vector3 GetGridWorldPosition(Vector3 worldPos);
        public GameObject[] GetObjectsInRadius(Vector3 worldPos);
        public void PlaceObjectOnGrid(GameObject obj, Vector3 worldPos);
        public bool InstantiatePrefabOnGrid(GameObject prefab, Vector3 worldPos);
    }
}

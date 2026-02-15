using UnityEngine;

public interface IGridService
{
    public Vector3 GetGridWorldPosition(Vector3 worldPos);
    // Only object in the layer called ¡°Object On Grid¡± can be detected
    public GameObject GetObjectOnGrid(Vector3 worldPos);
    // Can only instantiate on empty grid
    public bool InstantiatePrefabOnGrid(GameObject prefab, Vector3 worldPos);
    public bool DestroyObjectOnGrid(Vector3 worldPos);
}

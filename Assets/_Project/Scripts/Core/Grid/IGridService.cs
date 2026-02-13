using UnityEngine;

public interface IGridService
{
    public Vector3 GetGridWorldPosition(Vector3 worldPos);
    public GameObject GetGridIndicator();
    public GameObject GetObjectOnGrid(Vector3 worldPos);
    // Can only instantiate on empty grid
    public bool InstantiatePrefabOnGrid(GameObject prefab, Vector3 worldPos);
    public bool DestroyObjectOnGrid(Vector3 worldPos);
}

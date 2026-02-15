using UnityEngine;

public interface IGridService
{
    public Vector3 GetGridWorldPosition(Vector3 worldPos);
    // Only object in the layer called ¡°Object On Grid¡± can be detected
    public GameObject GetObjectOnGrid(Vector3 worldPos);
    // Can only instantiate on empty grid
    public bool InstantiatePrefabOnGrid(GameObject prefab, Vector3 worldPos);
    public bool DestroyObjectOnGrid(Vector3 worldPos);
    // playerID is either 1 or 2
    public GameObject GetGridIndicator(int playerID);
    // playerID is either 1 or 2
    public Vector3 GetGridIndicatorWorldPosition(int playerID);
    // Only object in the layer called ¡°Object On Grid¡± can be detected; playerID is either 1 or 2
    public GameObject GetObjectOnGridIndicator(int playerID);
}

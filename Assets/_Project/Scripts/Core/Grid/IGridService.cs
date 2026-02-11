using UnityEngine;

public interface IGridService
{
    public Vector3 GetGridWorldPosition(Vector3 worldPos);
    public GameObject GetGridIndicator();
    public GameObject GetObjectOnGrid(Vector3 worldPos);
    public void InstantiatePrefabOnGrid(GameObject prefab, Vector3 worldPos);
    public void DestroyObjectOnGrid(Vector3 worldPos);
}

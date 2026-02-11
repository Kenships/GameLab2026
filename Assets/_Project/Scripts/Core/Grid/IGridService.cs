using UnityEngine;

public interface IGridService
{
    public Vector3 GetGridWorldPosition(Vector3 pos);
    public GameObject GetGridIndicator();
    public GameObject GetObjectOnGrid(Vector3 pos);
    public void InstantiatePrefabOnGrid(GameObject prefab, Vector3 pos);
    public void DestroyObjectOnGrid(Vector3 pos);
}

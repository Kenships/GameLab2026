using UnityEngine;
using Sisus.Init;

[Service(typeof(IGridService), FindFromScene = true)]
public class GridSystem : MonoBehaviour, IGridService
{
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridIndicator;
    [SerializeField] private float gridIndicatorYCoordinate = 0.05f;
    public Vector3 GetGridWorldPosition(Vector3 pos)
    {
        Vector3Int gridPosition = grid.WorldToCell(pos);
        Vector3 gridPositionWorld = grid.CellToWorld(gridPosition);
        return new Vector3(gridPositionWorld.x, gridIndicatorYCoordinate, gridPositionWorld.z);
    }
    public GameObject GetGridIndicator()
    {
        return gridIndicator;
    }
    public GameObject GetObjectOnGrid(Vector3 pos)
    {
        Collider[] colliders = Physics.OverlapSphere(GetGridWorldPosition(pos), grid.cellSize.x/2);

        if (colliders.Length > 0)
        {
            return colliders[0].gameObject;
        }
        return null;
    }
    public void InstantiatePrefabOnGrid(GameObject prefab, Vector3 pos)
    {
        if (prefab == null)
        {
            Debug.Log("InstantiatePrefabOnGrid: prefab can't be null");
            return;
        }
        Instantiate(prefab, GetGridWorldPosition(pos), Quaternion.identity);
    }
    public void DestroyObjectOnGrid(Vector3 pos)
    {
        Destroy(GetObjectOnGrid(pos));
    }
}
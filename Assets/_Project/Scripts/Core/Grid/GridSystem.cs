using UnityEngine;
using Sisus.Init;

[Service(typeof(IGridService), FindFromScene = true)]
public class GridSystem : MonoBehaviour, IGridService
{
    [SerializeField] private bool showGridIndicator = true;
    
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridIndicator;
    [SerializeField] private float gridPositionYCoordinate = 0.05f;
    [SerializeField] private LayerMask detectionLayerMask;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        gridIndicator?.SetActive(showGridIndicator);
        #endif
    }
    
    public Vector3 GetGridWorldPosition(Vector3 worldPos)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPos);
        Vector3 gridPositionWorld = grid.CellToWorld(gridPosition);
        return new Vector3(gridPositionWorld.x, gridPositionYCoordinate, gridPositionWorld.z);
    }
    public GameObject GetGridIndicator()
    {
        return gridIndicator;
    }
    public GameObject GetObjectOnGrid(Vector3 worldPos)
    {
        Collider[] colliders = Physics.OverlapSphere(GetGridWorldPosition(worldPos), grid.cellSize.x/2, detectionLayerMask);

        if (colliders.Length > 0)
        {
            return colliders[0].gameObject;
        }
        return null;
    }
    // Can only instantiate on empty grid
    public bool InstantiatePrefabOnGrid(GameObject prefab, Vector3 worldPos)
    {
        if (prefab == null)
        {
            Debug.Log("InstantiatePrefabOnGrid: prefab can't be null");
            return false;
        }
        if (GetObjectOnGrid(worldPos) == null)
        {
            Instantiate(prefab, GetGridWorldPosition(worldPos), Quaternion.identity);
            return true;
        }
        return false;
    }
    public bool DestroyObjectOnGrid(Vector3 worldPos)
    {
        GameObject obj = GetObjectOnGrid(worldPos);
        if (obj != null)
        {
            Destroy(obj);
            return true;
        }
        return false;
    }
}

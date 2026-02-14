using UnityEngine;
using Sisus.Init;

[Service(typeof(IGridService), FindFromScene = true)]
public class GridSystem : MonoBehaviour, IGridService
{
    [SerializeField] private bool showGridIndicator = true;
    
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridIndicator;
    [SerializeField] private float gridPositionYCoordinate = 0.05f;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        gridIndicator?.SetActive(showGridIndicator);
        #endif
    }
    
    public Vector3 GetGridWorldPosition(Vector3 worldPos)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPos);
        Vector3 gridPositionWorld = grid.GetCellCenterWorld(gridPosition);
        return new Vector3(gridPositionWorld.x, gridPositionYCoordinate, gridPositionWorld.z);
    }
    // Only object in the layer called ¡°Object On Grid¡± can be detected
    public GameObject GetObjectOnGrid(Vector3 worldPos)
    {
        Collider[] colliders = Physics.OverlapSphere(GetGridWorldPosition(worldPos), grid.cellSize.x/2, LayerMask.GetMask("Object On Grid"));

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
            GameObject obj = Instantiate(prefab, GetGridWorldPosition(worldPos), Quaternion.identity);
            obj.layer = LayerMask.NameToLayer("Object On Grid");
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
    public GameObject GetGridIndicator()
    {
        return gridIndicator;
    }
    public Vector3 GetGridIndicatorWorldPosition()
    {
        return GetGridWorldPosition(gridIndicator.transform.position);
    }
    // Only object in the layer called ¡°Object On Grid¡± can be detected
    public GameObject GetObjectOnGridIndicator()
    {
        return GetObjectOnGrid(gridIndicator.transform.position);
    }
}

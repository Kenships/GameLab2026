using UnityEngine;
using Sisus.Init;

[Service(typeof(IGridService), FindFromScene = true)]
public class GridSystem : MonoBehaviour, IGridService
{
    [SerializeField] private bool showGridIndicator = true;
    
    [SerializeField] private Grid grid;
    [SerializeField] private GameObject gridIndicator1, gridIndicator2;
    [SerializeField] private float gridPositionYCoordinate = 0.05f;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        gridIndicator1?.SetActive(showGridIndicator);
        gridIndicator2?.SetActive(showGridIndicator);
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
    // playerID is either 1 or 2
    public GameObject GetGridIndicator(int playerID)
    {
        if (playerID == 1)
        {
            return gridIndicator1;
        }
        else if (playerID == 2)
        {
            return gridIndicator2;
        }
        else
        {
            Debug.Log("GetGridIndicator: invalid playerID");
            return null;
        }
    }
    // playerID is either 1 or 2
    public Vector3 GetGridIndicatorWorldPosition(int playerID)
    {
        if(playerID == 1)
        {
            return GetGridWorldPosition(gridIndicator1.transform.position);
        }
        else if (playerID == 2)
        {
            return GetGridWorldPosition(gridIndicator2.transform.position);
        }
        else
        {
            Debug.Log("GetGridIndicatorWorldPosition: invalid playerID");
            return Vector3.zero;
        }
    }
    // Only object in the layer called ¡°Object On Grid¡± can be detected; playerID is either 1 or 2
    public GameObject GetObjectOnGridIndicator(int playerID)
    {
        if (playerID == 1)
        {
            return GetObjectOnGrid(gridIndicator1.transform.position);
        }
        else if (playerID == 2)
        {
            return GetObjectOnGrid(gridIndicator2.transform.position);
        }
        else
        {
            Debug.Log("GetObjectOnGridIndicator: invalid playerID");
            return null;
        }
    }
}

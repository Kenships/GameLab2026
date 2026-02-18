using _Project.Scripts.Core.Grid;
using _Project.Scripts.Util.CustomAttributes;
using UnityEngine;
using Sisus.Init;

[Service(typeof(IGridService), FindFromScene = true)]
public class GridSystem : MonoBehaviour, IGridService
{
    [SerializeField] private bool allowDiagonal = false;
    [SerializeField] private Grid grid;
    [SerializeField] private float gridPositionYCoordinate = 0.05f;
    [SerializeField] private int colliderBufferSize = 10;

    private Collider[] _buffer;

    private void Awake()
    {
        _buffer = new Collider[colliderBufferSize];
    }

    public Vector3 GetGridWorldPosition(Vector3 worldPos)
    {
        Vector3Int gridPosition = grid.WorldToCell(worldPos);
        Vector3 gridPositionWorld = grid.GetCellCenterWorld(gridPosition);
        return new Vector3(gridPositionWorld.x, gridPositionYCoordinate, gridPositionWorld.z);
    }
    // Only object in the layer called ��Object On Grid�� can be detected
    public GameObject[] GetObjectsInRadius(Vector3 worldPos)
    {
        var size = Physics.OverlapSphereNonAlloc(GetGridWorldPosition(worldPos), grid.cellSize.x/2, _buffer, LayerMask.GetMask("Object On Grid"));
        GameObject[] objects = new GameObject[size];
        
        for (int i = 0; i < size; i++)
        {
            objects[i] = _buffer[i].gameObject;
        }
        return objects;
    }

    public void PlaceObjectOnGrid(GameObject obj, Vector3 worldPos)
    {
        obj.transform.position = GetGridWorldPosition(worldPos);
        
        if (!allowDiagonal)
        {
            Vector3 currentRotation = obj.transform.eulerAngles;
            obj.transform.rotation = Quaternion.Euler(
                currentRotation.x,
                AdjustIfDiagonal(currentRotation.y),
                currentRotation.z
            );
        }
    }

    // Can only instantiate on empty grid
    public bool InstantiatePrefabOnGrid(GameObject prefab, Vector3 worldPos)
    {
        if (prefab == null)
        {
            Debug.Log("InstantiatePrefabOnGrid: prefab can't be null");
            return false;
        }

        if (GetObjectsInRadius(worldPos) == null)
        {
            GameObject obj = Instantiate(prefab, GetGridWorldPosition(worldPos), Quaternion.identity);
            obj.layer = LayerMask.NameToLayer("Object On Grid");
            return true;
        }

        return false;
    }
    
    private float AdjustIfDiagonal(float angle)
    {
        float tolerance = 1f;

        angle = (angle % 360f + 360f) % 360f;

        // Check if within tolerance of axes (0��, 90��, 180��, 270��, 360��)
        float axisRemainder = angle % 90f;
        bool isNearAxis = Mathf.Min(axisRemainder, 90f - axisRemainder) <= tolerance;

        // If not near an axis, rotate counter-clockwise to next axis
        if (!isNearAxis)
        {
            // Calculate next axis (counter-clockwise direction)
            float nextAxis = Mathf.Ceil(angle / 90f) * 90f;
            return nextAxis >= 360f ? 0f : nextAxis;
        }

        return angle;
    }

    [Header("Grid Debug")]
    [SerializeField] private bool turnOnGizmos;

    [SerializeField, ShowIf(nameof(turnOnGizmos))] private int gridSize = 10;
    [SerializeField, ShowIf(nameof(turnOnGizmos))] private Vector3 rotationOffset;
    [SerializeField, ShowIf(nameof(turnOnGizmos))] private Vector3 positionOffset;
    [SerializeField, ShowIf(nameof(turnOnGizmos))] private Vector3 renderSize;
    [SerializeField, ShowIf(nameof(turnOnGizmos))] private Mesh renderMesh;

    private void OnDrawGizmos()
    {
        if (!turnOnGizmos)
            return;
        for (int row = -gridSize; row < gridSize; row++)
        {
            for (int col = -gridSize + row % 2; col < gridSize; col += 2)
            {
                Vector3 gridPos = GetGridWorldPosition(new Vector3(row, 0, col) + positionOffset + transform.position);
                Gizmos.color = Color.aquamarine;
                Gizmos.DrawMesh(renderMesh, gridPos, Quaternion.Euler(rotationOffset), renderSize);
            }
        }
    }
}

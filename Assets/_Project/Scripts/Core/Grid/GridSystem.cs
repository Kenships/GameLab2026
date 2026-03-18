using _Project.Scripts.Util;
using _Project.Scripts.Util.CustomAttributes;
using Sisus.Init;
using UnityEngine;

namespace _Project.Scripts.Core.Grid
{
    [Service(typeof(IGridService), FindFromScene = true)]
    public class GridSystem : MonoBehaviour, IGridService
    {
        [SerializeField] private bool allowDiagonal = false;
        [SerializeField] private UnityEngine.Grid grid;
        [SerializeField] private float gridPositionYCoordinate = 0.05f;

        [SerializeField] private LayerMask objectOnGridLayer;
        public static LayerMask ObjectOnGridLayer { get; private set; }

        private void Awake()
        {
            ObjectOnGridLayer = objectOnGridLayer;
        }

        public Vector3 GetGridWorldPosition(Vector3 worldPos)
        {
            Vector3Int gridPosition = grid.WorldToCell(worldPos);
            Vector3 gridPositionWorld = grid.GetCellCenterWorld(gridPosition);
            return new Vector3(gridPositionWorld.x, gridPositionYCoordinate, gridPositionWorld.z);
        }
        // Only object in the layer called "Object On Grid" can be detected
        public GameObject GetObjectOnGrid(Vector3 worldPos)
        {
            Vector3 gridPos = GetGridWorldPosition(worldPos);
            Collider[] colliders = Physics.OverlapSphere(gridPos, grid.cellSize.x/4, objectOnGridLayer);

            if(colliders.Length <= 0)
            {
                return null;
            }
            else if(colliders.Length == 1)
            {
                return colliders[0].gameObject;
            }
            else // Get the closest one in case there are more than one collider
            {
                Collider result = colliders[0];
                foreach (Collider collider in colliders)
                {
                    if (Vector3.Distance(collider.transform.position, gridPos)
                        < Vector3.Distance(result.transform.position, gridPos))
                    {
                        result = collider;
                    }
                }
                return result.gameObject;
            }
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

            if (GetObjectOnGrid(worldPos) == null)
            {
                GameObject obj = Instantiate(prefab, GetGridWorldPosition(worldPos), Quaternion.identity);
                obj.layer = ObjectOnGridLayer.ToLayerIndex();
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
}

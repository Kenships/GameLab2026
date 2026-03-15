using System.Collections.Generic;
using _Project.Scripts.Targeting;
using _Project.Scripts.Util.CustomAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.Core
{
    public class RangeDetector : MonoBehaviour
    {
        public enum RangeType
        {
            Circle,
            Sector,
            Rectangle
        }

        public event UnityAction<Collider> OnObjectEnter;
        public event UnityAction<Collider> OnObjectExit;

        [Header("Range Type")] public RangeType rangeType = RangeType.Circle;

        private bool IsCircle => rangeType is RangeType.Circle or RangeType.Sector;
        private bool IsSector => rangeType == RangeType.Sector;
        private bool IsRectangle => rangeType == RangeType.Rectangle;
        
        [ShowIf(nameof(IsCircle))] public float radius = 5f;

        
        [ShowIf(nameof(IsSector))] 
        [Range(0, 360)]
        public float angle = 60f;
        
        [ShowIf(nameof(IsRectangle))]public float width = 5f;
        [ShowIf(nameof(IsRectangle))] public float length = 10f;

        [Header("Target Filter")] 
        public LayerMask targetLayers;
        public LayerMask obsticalLayers;

        [Header("Start Point")] public Transform startingTransform; // If not assigned, this.transform will be used

        [Header("Other")]
        public bool ignoreYAxis = true; // Default true: only XZ plane is considered (ignores height difference)

        [SerializeField] private int colliderBufferSize = 100;

        private Collider _myCollider;
        private Collider[] _colliderBuffer;
        private HashSet<Collider> _currentlyInRange = new();
        private HashSet<Collider> _previouslyInRange = new();

        private void Awake()
        {
            _colliderBuffer = new Collider[colliderBufferSize];
        }

        public void GetObjectTypeInRangeNoAlloc<T>(List<T> objectList, bool checkLineOfSight = true)
        {
            // Initialize Current and Previous in range sets
            _previouslyInRange.Clear();
            foreach (Collider obj in _currentlyInRange)
            {
                _previouslyInRange.Add(obj);
            }
            _currentlyInRange.Clear();

            // Initialize Detection Perams
            Transform start = GetStartingTransform();
            float maxRange = GetMaxRange();

            int count = Physics.OverlapSphereNonAlloc(start.position, maxRange, _colliderBuffer, targetLayers);

            for (int i = 0; i < count; i++)
            {
                if (!IsInRange(_colliderBuffer[i].transform, start) ||
                    (checkLineOfSight && !IsLineOfSight(_colliderBuffer[i].transform, start))) continue;

                if (!_previouslyInRange.Contains(_colliderBuffer[i]) && _colliderBuffer[i] != null)
                {
                   OnObjectEnter?.Invoke(_colliderBuffer[i]);
                }
                
                _currentlyInRange.Add(_colliderBuffer[i]);
            }

            foreach (Collider obj in _previouslyInRange)
            {
                if (!_currentlyInRange.Contains(obj) && obj != null)
                {
                   OnObjectExit?.Invoke(obj);
                }
            }
            
            objectList.Clear();

            foreach (Collider obj in _currentlyInRange)
            {
                if (obj.TryGetComponent(out T objOfType))
                {
                    objectList.Add(objOfType);
                }
            }
        }

        private bool IsLineOfSight(Transform a,
            Transform b)
        {
            Vector3 origin = a.position;
            Vector3 center = b.position;

            Vector3 right = b.right;

            Vector3[] points =
            {
                center,            // center
                center + right,    // right side
                center - right     // left side
            };

            foreach (var p in points)
            {
                Vector3 dir = p - origin;
                float dist = dir.magnitude;

                if (!Physics.Raycast(origin, dir.normalized, dist, obsticalLayers, QueryTriggerInteraction.Ignore))
                {
                    return true; // at least one ray reached target
                }
            }

            return false;
        }

        public T GetClosestObjectOfType<T>()
        {
            List<Transform> transforms = GetTransformsInRange();
            Transform closest = null;
            float best = float.MaxValue;

            var startPos = GetStartingTransform().position;

            foreach (var t in transforms)
            {
                float d = GetDistance(startPos, t.position);
                if (d < best)
                {
                    best = d;
                    closest = t;
                }
            }

            return closest ? closest.GetComponent<T>() : default;
        }

        public List<T> GetObjectTypeInRange<T>()
        {
            List<T> result = new List<T>();
            GetObjectTypeInRangeNoAlloc(result);
            return result;
        }

        public List<Transform> GetTransformsInRange()
        {
            return GetObjectTypeInRange<Transform>();
        }

        private Transform GetStartingTransform()
        {
            return startingTransform ? startingTransform : transform;
        }

        private float GetDistance(Vector3 from, Vector3 to)
        {
            if (ignoreYAxis)
            {
                Vector3 fromXZ = Vector3.ProjectOnPlane(from, Vector3.up);
                Vector3 toXZ = Vector3.ProjectOnPlane(to, Vector3.up);
                return Vector3.Distance(fromXZ, toXZ);
            }

            return Vector3.Distance(from, to);
        }

        private bool IsInRange(Transform target, Transform start)
        {
            Vector3 targetPos = target.position;
            Vector3 startPos = start.position;

            switch (rangeType)
            {
                case RangeType.Circle:
                    return true;

                case RangeType.Sector:
                    Vector3 toTarget = targetPos - startPos;
                    Vector3 forward = start.forward;

                    if (ignoreYAxis)
                    {
                        toTarget.y = 0;
                        forward.y = 0;
                    }

                    float angleBetween = Vector3.Angle(forward, toTarget);
                    return angleBetween <= angle * 0.5f;

                case RangeType.Rectangle:
                    // Fixed lazer cannon's bug (caused by scale)
                    Matrix4x4 m = Matrix4x4.TRS(start.position, start.rotation, Vector3.one);
                    Vector3 targetLocalPos = m.inverse.MultiplyPoint3x4(targetPos);

                    if (ignoreYAxis)
                        targetLocalPos.y = 0;

                    float halfWidth = width * 0.5f;
                    return targetLocalPos.z >= 0 && targetLocalPos.z <= length &&
                           targetLocalPos.x >= -halfWidth && targetLocalPos.x <= halfWidth;
                default:
                    return false;
            }
        }

        private float GetMaxRange()
        {
            switch (rangeType)
            {
                case RangeType.Circle:
                case RangeType.Sector:
                    return radius;
                case RangeType.Rectangle:
                    float halfWidth = width * 0.5f;
                    return Mathf.Sqrt(halfWidth * halfWidth + length * length);
                default:
                    return 0;
            }
        }

        #region Gizmos Visualization

        // ---------- Gizmos Visualization ----------
        private void OnDrawGizmosSelected()
        {
            Transform start = GetStartingTransform();
            Gizmos.color = Color.green;

            switch (rangeType)
            {
                case RangeType.Circle:
                    DrawCircleGizmo(start.position, radius);
                    break;
                case RangeType.Sector:
                    DrawSectorGizmo(start.position, start.forward, radius, angle);
                    break;
                case RangeType.Rectangle:
                    DrawRectangleGizmo(start.position, start.rotation, width, length);
                    break;
            }
        }

        private void DrawCircleGizmo(Vector3 startPos, float r)
        {
            int segments = 40;
            float deltaTheta = 2f * Mathf.PI / segments;
            Vector3 prevPoint = startPos + new Vector3(r, 0, 0);
            for (int i = 1; i <= segments; i++)
            {
                float theta = i * deltaTheta;
                Vector3 point = startPos + new Vector3(r * Mathf.Cos(theta), 0, r * Mathf.Sin(theta));
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
        }

        private void DrawSectorGizmo(Vector3 startPos, Vector3 forward, float r, float ang)
        {
            forward.y = 0;
            forward.Normalize();

            Vector3 leftDir = Quaternion.Euler(0, -ang * 0.5f, 0) * forward;
            Vector3 rightDir = Quaternion.Euler(0, ang * 0.5f, 0) * forward;

            Gizmos.DrawLine(startPos, startPos + leftDir * r);
            Gizmos.DrawLine(startPos, startPos + rightDir * r);

            int segments = 20;
            float stepAngle = ang / segments;
            Vector3 prevPoint = startPos + leftDir * r;
            for (int i = 1; i <= segments; i++)
            {
                float currentAngle = -ang * 0.5f + i * stepAngle;
                Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * forward;
                Vector3 point = startPos + dir * r;
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
        }

        private void DrawRectangleGizmo(Vector3 startPos, Quaternion rotation, float w, float l)
        {
            float halfW = w * 0.5f;
            Vector3[] localCorners =
            {
                new(-halfW, 0, 0),
                new(halfW, 0, 0),
                new(halfW, 0, l),
                new(-halfW, 0, l)
            };

            Vector3[] worldCorners = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                worldCorners[i] = startPos + rotation * localCorners[i];
            }

            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(worldCorners[i], worldCorners[(i + 1) % 4]);
            }
        }

        #endregion

        public void ResetRangeDetection()
        {
            _previouslyInRange.Clear();
            _currentlyInRange.Clear();
        }
    }
}

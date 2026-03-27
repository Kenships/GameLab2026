using _Project.Scripts.Core.Grid;
using Sisus.Init;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Core
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RangeVisualizer : MonoBehaviour<IGridService>
    {
        [Header("References")]
        [SerializeField] private RangeDetector detector;

        [Header("Visual Settings")]
        [SerializeField] private Color fillColor = new Color(0.2f, 1.0f, 0.6f, 0.2f);
        [SerializeField] private Color borderColor = new Color(0.4f, 1.0f, 0.8f, 0.9f);
        [SerializeField] private float borderWidth = 0.2f;
        [SerializeField] private int segments = 60;
        [SerializeField] private float yOffset = 0.5f;
        [SerializeField] private float hideDelay = 0.05f;
        
        private Transform gridIndicator1;
        private Transform gridIndicator2;
        private IGridService _gridSystem;
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _vfxMaterial;

        public bool isVisible = false;
        private float _hideTimer = 0f;

        private void Start()
        {
            gridIndicator1 = GameObject.Find("Grid Indicator 1").transform;
            gridIndicator2 = GameObject.Find("Grid Indicator 2").transform;
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            
            _vfxMaterial = new Material(Shader.Find("Sprites/Default"));
            _meshRenderer.material = _vfxMaterial;
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            _mesh = new Mesh { name = "RangeVFX_Mesh" };
            _meshFilter.mesh = _mesh;
        }

        protected override void Init(IGridService gridService)
        {
            _gridSystem = gridService;
        }

        private void Update()
        {
            CheckGridFocus();
        }

        private void CheckGridFocus()
        {
            Vector3 myGridPos = _gridSystem.GetGridWorldPosition(transform.position);

            bool isAnyNearby = IsIndicatorNearby(gridIndicator1, myGridPos) || IsIndicatorNearby(gridIndicator2, myGridPos);

            if (isAnyNearby)
            {
                isVisible = true;
                _hideTimer = hideDelay;
            }
            else
            {
                _hideTimer -= Time.deltaTime;
                if (_hideTimer <= 0) isVisible = false;
            }
        }

        private bool IsIndicatorNearby(Transform indicator, Vector3 centerPos)
        {
            if (indicator == null) return false;

            Vector3 indicatorPos = indicator.position;
            float distance = Vector2.Distance(new Vector2(centerPos.x, centerPos.z), new Vector2(indicatorPos.x, indicatorPos.z));

            return distance < 0.1f;
        }

        private void LateUpdate()
        {
            if (detector == null) return;

            if (!isVisible)
            {
                if (_meshRenderer.enabled) _meshRenderer.enabled = false;
                return;
            }
            if (!_meshRenderer.enabled) _meshRenderer.enabled = true;

            UpdateVFXMesh();
        }

        private void UpdateVFXMesh()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Color> colors = new List<Color>();

            switch (detector.rangeType)
            {
                case RangeDetector.RangeType.Circle:
                    GenerateCircleData(vertices, triangles, colors, detector.radius, 360f);
                    break;
                case RangeDetector.RangeType.Sector:
                    GenerateCircleData(vertices, triangles, colors, detector.radius, detector.angle);
                    break;
                case RangeDetector.RangeType.Rectangle:
                    GenerateRectangleData(vertices, triangles, colors, detector.width, detector.length);
                    break;
            }

            _mesh.Clear();
            _mesh.SetVertices(vertices);
            _mesh.SetTriangles(triangles, 0);
            _mesh.SetColors(colors);
            _mesh.RecalculateNormals();

            transform.position = (detector.startingTransform ? detector.startingTransform.position : detector.transform.position) + Vector3.up * yOffset;
            transform.rotation = (detector.startingTransform ? detector.startingTransform.rotation : detector.transform.rotation);
        }

        private void GenerateCircleData(List<Vector3> verts, List<int> tris, List<Color> cols, float radius, float angle)
        {
            int startIdx = verts.Count;
            float angleStep = angle / segments;
            float halfAngle = angle * 0.5f;

            // Fill
            verts.Add(Vector3.zero);
            cols.Add(fillColor);

            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = -halfAngle + (i * angleStep);
                Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
                verts.Add(dir * (radius - borderWidth));
                cols.Add(fillColor);

                if (i > 0)
                {
                    tris.Add(startIdx);
                    tris.Add(startIdx + i);
                    tris.Add(startIdx + i + 1);
                }
            }

            // Border
            int borderStartIdx = verts.Count;
            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = -halfAngle + (i * angleStep);
                Vector3 dir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;

                verts.Add(dir * (radius - borderWidth));
                cols.Add(borderColor);
                verts.Add(dir * radius);
                cols.Add(borderColor);

                if (i < segments)
                {
                    int n = borderStartIdx + i * 2;
                    tris.Add(n); tris.Add(n + 1); tris.Add(n + 2);
                    tris.Add(n + 1); tris.Add(n + 3); tris.Add(n + 2);
                }
            }

            if (angle < 360f)
            {
                AddLineBorder(verts, tris, cols, Vector3.zero,
                    Quaternion.Euler(0, -halfAngle, 0) * Vector3.forward * radius);
                AddLineBorder(verts, tris, cols, Vector3.zero,
                    Quaternion.Euler(0, halfAngle, 0) * Vector3.forward * radius);
            }
        }

        private void GenerateRectangleData(List<Vector3> verts, List<int> tris, List<Color> cols, float width, float length)
        {
            float hw = width * 0.5f;

            float left = -hw + borderWidth;
            float right = hw - borderWidth;
            float bottom = 0f;
            float top = length - borderWidth;

            // -Fill
            int startIdx = verts.Count;
            verts.Add(new Vector3(left, 0, bottom));
            verts.Add(new Vector3(right, 0, bottom));
            verts.Add(new Vector3(right, 0, top));
            verts.Add(new Vector3(left, 0, top));
            for (int i = 0; i < 4; i++) cols.Add(fillColor);
            tris.Add(startIdx); tris.Add(startIdx + 2); tris.Add(startIdx + 1);
            tris.Add(startIdx); tris.Add(startIdx + 3); tris.Add(startIdx + 2);

            // Left border
            AddLineBorder(verts, tris, cols, new Vector3(left, 0, bottom), new Vector3(left, 0, top));
            // Right border
            AddLineBorder(verts, tris, cols, new Vector3(right, 0, bottom), new Vector3(right, 0, top));
            // End border
            AddLineBorder(verts, tris, cols, new Vector3(left, 0, top), new Vector3(right, 0, top));
        }

        private void AddLineBorder(List<Vector3> verts, List<int> tris, List<Color> cols, Vector3 start, Vector3 end)
        {
            int startIdx = verts.Count;
            Vector3 dir = (end - start).normalized;
            Vector3 normal = Vector3.Cross(dir, Vector3.up).normalized;
            float halfWidth = borderWidth * 0.5f;

            verts.Add(start - normal * halfWidth);
            verts.Add(start + normal * halfWidth);
            verts.Add(end + normal * halfWidth);
            verts.Add(end - normal * halfWidth);

            for (int i = 0; i < 4; i++) cols.Add(borderColor);

            tris.Add(startIdx); tris.Add(startIdx + 1); tris.Add(startIdx + 2);
            tris.Add(startIdx); tris.Add(startIdx + 2); tris.Add(startIdx + 3);
        }
    }
}

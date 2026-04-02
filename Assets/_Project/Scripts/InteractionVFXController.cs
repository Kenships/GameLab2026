using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Modules;
using _Project.Scripts.Core.Modules.Base_Class;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class InteractionVFXController : MonoBehaviour
    {
        public enum AbilityMode
        {
            FastForward,
            Rewind
        }

        private AbilityMode currentMode = AbilityMode.FastForward;
        
        [SerializeField] private IconManager iconManager;
        [SerializeField] private ModuleInfoManager moduleInfoManager; 

        [Header("Ring Settings")]
        [SerializeField] private float radius = 5f;
        [SerializeField] private float ringWidth = 0.15f;
        [SerializeField] private int segments = 80;

        [Header("Appearance")]
        [SerializeField] private Color ringColor = new Color(0.4f, 0.8f, 1f, 0.85f);
        [SerializeField] private float fadeSpeed = 4f;
        [SerializeField] private float yOffset = 0.05f;

        [Header("Pulse Settings")]
        [SerializeField] private bool enablePulse = true;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseIntensity = 0.15f;

        [Header("Ripple Settings")]
        [SerializeField] private bool enableRipple = false;
        [SerializeField] private float rippleInterval = 0.6f;
        [SerializeField] private int maxRipples = 3;
        
        [Header("ModuleIcons")]
        [SerializeField] private Texture2D turretIcon;
        [SerializeField] private Texture2D flameIcon;
        [SerializeField] private Texture2D lazerIcon;
        [SerializeField] private Texture2D carIcon;
        [SerializeField] private Texture2D discIcon;
        [SerializeField] private Texture2D explosiveIcon;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Material ringMaterial;

        private float targetAlpha = 0f;
        private float currentAlpha = 0f;
        private bool isVisible = false;

        private Color activeColor;

        private List<RippleRing> activeRipples = new List<RippleRing>();
        private float rippleTimer = 0f;

        private string currentModuleName; 
        
        private void Awake()
        {
            meshFilter   = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            activeColor = ringColor;

            SetupMaterial();
            GenerateRingMesh(radius);

            transform.localPosition = new Vector3(0f, yOffset, 0f);
        }

        private void Update()
        {
            HandleFade();
            HandlePulse();

            if (enableRipple && isVisible)
                HandleRipples();

            UpdateRipples();
        }

        public void ShowHeldObject(GameObject heldObject)
        {
            Debug.Log(heldObject);
            if (heldObject.GetComponent<Flamethrower>()!= null)
            {
                moduleInfoManager.ShowInfo(flameIcon,"Flame Thrower");
            }
            else if (heldObject.GetComponent<LazerCannon>()!= null)
            {
                moduleInfoManager.ShowInfo(lazerIcon,"Lazer Cannon");
            }
            else if (heldObject.GetComponent<ExplosiveTank>() != null)
            {
                moduleInfoManager.ShowInfo(explosiveIcon,"Explosive Tank");
            }
            // else if (heldObject.GetComponent<Car>()!= null)
            // {
            //     moduleInfoManager.ShowInfo(carIcon,"Car");
            // }
            else if (heldObject.GetComponent<Turret>() != null)
            {
                if (heldObject.GetComponent<Turret>().turretType == "disc")
                {
                    moduleInfoManager.ShowInfo(discIcon,"Saw Shooter");
                }
                else if (heldObject.GetComponent<Turret>().turretType == "normal")
                {
                    moduleInfoManager.ShowInfo(turretIcon,"Turret");
                }
            }
        }

        public void HideHeldObject()
        {
            moduleInfoManager.Hide();
        }


        public void ShowWind(AbilityMode mode = AbilityMode.FastForward)
        {
            currentMode = mode;
            activeColor = ringColor;

            if (mode == AbilityMode.FastForward && iconManager != null)
            {
                iconManager.ShowFastForward();
            }
            else if (mode == AbilityMode.Rewind  && iconManager != null)
            {
                iconManager.ShowRewind();
            }

            ringMaterial.color = new Color(activeColor.r, activeColor.g, activeColor.b, currentAlpha);

            isVisible   = true;
            targetAlpha = activeColor.a;
        }

        public void HideWind()
        {
            isVisible   = false;
            targetAlpha = 0f;
            if (iconManager != null)
            {
                iconManager.Hide();
            }
            
            ClearRipples();
        }

        public void Toggle(AbilityMode mode = AbilityMode.FastForward)
        {
            if (isVisible) HideWind();
            else           ShowWind(mode);
        }

 

        private void HandleFade()
        {
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            Color c = ringMaterial.color;
            ringMaterial.color = new Color(c.r, c.g, c.b, currentAlpha);
        }

        private void HandlePulse()
        {
            if (!enablePulse || !isVisible) return;

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            ringMaterial.SetColor(
                "_Color",
                new Color(
                    activeColor.r * pulse,
                    activeColor.g * pulse,
                    activeColor.b * pulse,
                    currentAlpha
                )
            );
        }
    

        private void HandleRipples()
        {
            rippleTimer += Time.deltaTime;
            if (rippleTimer >= rippleInterval && activeRipples.Count < maxRipples)
            {
                rippleTimer = 0f;
                SpawnRipple(currentMode);
            }
        }

        private void SpawnRipple(AbilityMode mode)
        {
            GameObject rippleGO = new GameObject("Ripple");
            rippleGO.transform.SetParent(transform.parent);
            rippleGO.transform.localPosition = transform.localPosition;
            rippleGO.transform.rotation      = transform.rotation;

            MeshFilter   mf = rippleGO.AddComponent<MeshFilter>();
            MeshRenderer mr = rippleGO.AddComponent<MeshRenderer>();

            Material rippleMat = new Material(ringMaterial) { color = activeColor };
            mr.material = rippleMat;

            // FastForward: starts at center, moves out to ring edge
            // Rewind:      starts at ring edge, moves in to center
            float startRadius = (mode == AbilityMode.Rewind)
                ? radius
                : 0.01f;

            mf.mesh = GenerateMesh(startRadius);

            activeRipples.Add(new RippleRing
            {
                go       = rippleGO,
                mat      = rippleMat,
                mf       = mf,
                progress = 0f,
                mode     = mode,
                color    = activeColor
            });
        }

        private void UpdateRipples()
        {
            for (int i = activeRipples.Count - 1; i >= 0; i--)
            {
                RippleRing r = activeRipples[i];
                r.progress += Time.deltaTime * (1f / rippleInterval);

                float currentRippleRadius;
                float alpha;

                if (r.mode == AbilityMode.Rewind)
                {
                    // contracts from ring edge → center
                    currentRippleRadius = Mathf.Lerp(radius, 0.01f, r.progress);

                    // fades in, peaks in the middle of travel, fades out on arrival
                    alpha = Mathf.Lerp(0f, r.color.a * 0.7f, Mathf.Sin(r.progress * Mathf.PI));
                }
                else
                {
                    // expands from center → ring edge
                    currentRippleRadius = Mathf.Lerp(0.01f, radius, r.progress);

                    // fades in, peaks in the middle of travel, fades out on arrival
                    alpha = Mathf.Lerp(0f, r.color.a * 0.7f, Mathf.Sin(r.progress * Mathf.PI));
                }

                r.mf.mesh   = GenerateMesh(currentRippleRadius, ringWidth * 0.6f);
                r.mat.color = new Color(r.color.r, r.color.g, r.color.b, alpha);

                if (r.progress >= 1f)
                {
                    Destroy(r.go);
                    activeRipples.RemoveAt(i);
                }
            }
        }

        private void ClearRipples()
        {
            foreach (RippleRing r in activeRipples)
                Destroy(r.go);

            activeRipples.Clear();
            rippleTimer = 0f;
        }

        private class RippleRing
        {
            public GameObject  go;
            public Material    mat;
            public MeshFilter  mf;
            public float       progress;
            public AbilityMode mode;
            public Color       color;
        }
    

        private void GenerateRingMesh(float r)
        {
            meshFilter.mesh = GenerateMesh(r);
        }

        private Mesh GenerateMesh(float outerRadius, float width = -1f)
        {
            if (width < 0f) width = ringWidth;

            float innerRadius = outerRadius - width;

            Mesh mesh = new Mesh();
            mesh.name = "RingMesh";

            Vector3[] vertices  = new Vector3[segments * 2];
            Vector2[] uvs       = new Vector2[segments * 2];
            int[]     triangles = new int[segments * 6];

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float cos   = Mathf.Cos(angle);
                float sin   = Mathf.Sin(angle);

                vertices[i]            = new Vector3(cos * outerRadius, 0f, sin * outerRadius);
                vertices[i + segments] = new Vector3(cos * innerRadius,  0f, sin * innerRadius);

                uvs[i]            = new Vector2(1f, (float)i / segments);
                uvs[i + segments] = new Vector2(0f, (float)i / segments);
            }

            for (int i = 0; i < segments; i++)
            {
                int next     = (i + 1) % segments;
                int triIndex = i * 6;

                triangles[triIndex]     = i;
                triangles[triIndex + 1] = next;
                triangles[triIndex + 2] = i + segments;

                triangles[triIndex + 3] = next;
                triangles[triIndex + 4] = next + segments;
                triangles[triIndex + 5] = i + segments;
            }

            mesh.vertices  = vertices;
            mesh.triangles = triangles;
            mesh.uv        = uvs;
            mesh.RecalculateNormals();

            return mesh;
        }
    

        private void SetupMaterial()
        {
            ringMaterial = new Material(Shader.Find("Sprites/Default"))
            {
                color = new Color(activeColor.r, activeColor.g, activeColor.b, 0f)
            };

            ringMaterial.renderQueue            = 3001;
            meshRenderer.material               = ringMaterial;
            meshRenderer.shadowCastingMode      = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows         = false;
        }
    
    }
}

using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Depth_Based_Pixelator.RenderFeatures
{
    public class PixelationFeature : ScriptableRendererFeature
    {
        public RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;

        [Header("Material")] public Material pixelationMaterial;

        [HideInInspector][SerializeField] private Shader m_UnlitWhite;
        [HideInInspector][SerializeField] private Shader m_MergeShader;
        [HideInInspector][SerializeField] private Shader m_PixelationShader;
        [HideInInspector][SerializeField] private Shader m_DepthShader;

        private Material m_UnlitWhiteMaterial;

        private PixelationPass m_CopyColorPass;

        public const int MaxdepthlevelCount = 8;

        private static readonly int DepthLevelCountID = Shader.PropertyToID("_Depth_Level_Count");

        private int depthLevelCount;
        private int[] thresholds = new int[MaxdepthlevelCount - 1];

        private Material[] pixelationMaterials = new Material[MaxdepthlevelCount];
        private Material[] mergeMaterials = new Material[MaxdepthlevelCount];

        private void GetSettings()
        {
            depthLevelCount = pixelationMaterial.GetInt(DepthLevelCountID);

            for (int i = 0; i < depthLevelCount - 1; i++)
            {
                if (i >= thresholds.Length - 1) return;
                thresholds[i] = pixelationMaterial.GetInt($"_Threshold_{i + 1}");
            }
        }

        public override void Create()
        {
            #region Material Setup
            if (m_UnlitWhite == null)
            {
                m_UnlitWhite = Shader.Find("Hidden/UnlitWhite");
                if (m_UnlitWhite == null)
                {
                    return;
                }
            }

            if (m_MergeShader == null)
            {
                m_MergeShader = Shader.Find($"Shader Graphs/Merge");
                if (m_MergeShader == null)
                {
                    return;
                }
            }

            if (m_PixelationShader == null)
            {
                m_PixelationShader = Shader.Find($"Shader Graphs/Pixelation");
                if (m_PixelationShader == null)
                {
                    return;
                }
            }
            #endregion

            if (!pixelationMaterial) return;

            // Get settings from material.
            GetSettings();

            // Setup pixelation.
            for (int i = 0; i < MaxdepthlevelCount; i++)
            {
                // Create material.
                if (i > pixelationMaterials.Length - 1) return;
                pixelationMaterials[i] = CoreUtils.CreateEngineMaterial(m_PixelationShader);
            }

            // Setup merge.
            for (int i = 0; i < MaxdepthlevelCount; i++)
            {
                // Create material.
                mergeMaterials[i] = CoreUtils.CreateEngineMaterial(m_MergeShader);
            }

            m_UnlitWhiteMaterial = CoreUtils.CreateEngineMaterial(m_UnlitWhite);
            
            m_CopyColorPass = new PixelationPass(passEvent, null, pixelationMaterials, mergeMaterials, pixelationMaterial);
        }

#if !UNITY_6000_3_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (m_CopyColorPass != null)
            {
                m_CopyColorPass.ConfigureInput(ScriptableRenderPassInput.Depth);
            }
        }
#endif

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!pixelationMaterial) return;
            if (renderingData.cameraData.renderType == CameraRenderType.Overlay) return;
            if (renderingData.cameraData.cameraType != CameraType.Game) return;
            renderer.EnqueuePass(m_CopyColorPass);
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var mat in pixelationMaterials) CoreUtils.Destroy(mat);
            foreach (var mat in mergeMaterials) CoreUtils.Destroy(mat);

            m_CopyColorPass = null;

            CoreUtils.Destroy(m_UnlitWhiteMaterial);
        }
    }
}
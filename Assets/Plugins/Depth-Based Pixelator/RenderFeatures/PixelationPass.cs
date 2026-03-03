using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Depth_Based_Pixelator.RenderFeatures
{
    public class PixelationPass : ScriptableRenderPass
    {
        private Material m_CopyColorMaterial;
        private Material[] m_PixelationMaterials;
        private Material[] m_MergeLayerMaterials;
        private Material m_FinalMaterial;

        private static readonly int PixelationColorTex = Shader.PropertyToID("_PixelationColorTex");
        private static readonly int FrontTexID = Shader.PropertyToID("_FrontTex");
        private static readonly int BackTexID = Shader.PropertyToID("_BackTex");

        private static readonly int DepthBasedID = Shader.PropertyToID("_Depth_Based");
        private static readonly int ResolutionID = Shader.PropertyToID("_Resolution");
        private static readonly int MinThresholdID = Shader.PropertyToID("_Min_Threshold");
        private static readonly int MaxThresholdID = Shader.PropertyToID("_Max_Threshold");
        private static readonly int DepthLevelCountID = Shader.PropertyToID("_Depth_Level_Count");
        private static readonly int ResolutionMultiplier = Shader.PropertyToID("_Resolution_Multiplier");
        private static readonly int DownscaleFactor = Shader.PropertyToID("_Downscale_Factor");

        private static readonly int ShowDepthID = Shader.PropertyToID("_Show_Depth");

        class PixelatePassData
        {
            public TextureHandle source;
            public TextureHandle depth;
            public Material material;
            public Material controllerMat;
            public int id;
        }

        class MergePassData
        {
            public TextureHandle frontTex;
            public TextureHandle backTex;
            public Material material;
        }

        class FinalPassData
        {
            public TextureHandle source;
            public Material material;
        }

        public PixelationPass(RenderPassEvent passEvent, Material copyColorMaterial, Material[] pixelationMaterials, Material[] mergeLayerMaterials, Material finalMaterial)
        {
            renderPassEvent = passEvent;
            m_CopyColorMaterial = copyColorMaterial;
            m_PixelationMaterials = pixelationMaterials;
            m_MergeLayerMaterials = mergeLayerMaterials;
            m_FinalMaterial = finalMaterial;
        }

        void ExecutePixelatePass(PixelatePassData data, RasterGraphContext context)
        {
            var depthLevelCount = data.controllerMat.GetInt(DepthLevelCountID);
            data.material.SetFloat(DepthBasedID, data.controllerMat.GetFloat(DepthBasedID));

            data.material.SetFloat(ResolutionMultiplier, data.controllerMat.GetFloat(ResolutionMultiplier));
            data.material.SetFloat(DownscaleFactor, data.controllerMat.GetFloat(DownscaleFactor));
            data.material.SetInt(ResolutionID, data.controllerMat.GetInt($"_Resolution_{Mathf.Min(data.id + 1, depthLevelCount)}"));
            data.material.SetInt(MinThresholdID, data.id == 0 ? 0 : data.controllerMat.GetInt($"_Threshold_{data.id}"));
            data.material.SetInt(MaxThresholdID, data.id >= depthLevelCount - 1 ? 1000000 + data.id * 10000 : data.controllerMat.GetInt($"_Threshold_{data.id + 1}"));
            data.material.SetTexture(Shader.PropertyToID("_CameraDepthTexture"), data.depth); // <-- bind depth handle

            data.material.SetTexture(PixelationColorTex, data.source);
            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
        }

        void ExecuteMergePass(MergePassData data, RasterGraphContext context)
        {
            data.material.SetTexture(FrontTexID, data.frontTex);
            data.material.SetTexture(BackTexID, data.backTex);
            Blitter.BlitTexture(context.cmd, data.backTex, new Vector4(1, 1, 0, 0), data.material, 0);
        }

        void ExecuteFinalPass(FinalPassData data, RasterGraphContext context)
        {
            data.material.SetTexture(PixelationColorTex, data.source);
            Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
        {
            var cameraData = frameContext.Get<UniversalCameraData>();
            if (cameraData.renderType == CameraRenderType.Overlay) return;
            if (cameraData.cameraType != CameraType.Game) return;

            //-- Setup
            var depthLevelCount = m_FinalMaterial.GetInt(DepthLevelCountID);
            depthLevelCount = Mathf.Min(depthLevelCount, PixelationFeature.MaxdepthlevelCount);
            
            var maxResolution = 0;
            for (int i = 0; i < depthLevelCount; i++) 
            {
                maxResolution = Mathf.Max(maxResolution, m_FinalMaterial.GetInt($"_Resolution_{i + 1}"));
            }

            var resourceData = frameContext.Get<UniversalResourceData>();

            var cameraTexture = resourceData.activeColorTexture;

            var fullResDescriptor = cameraTexture.GetDescriptor(renderGraph);
            fullResDescriptor.depthBufferBits = 0;
            fullResDescriptor.msaaSamples = MSAASamples.None;
            fullResDescriptor.format = GraphicsFormat.R8G8B8A8_UNorm;
            fullResDescriptor.filterMode = FilterMode.Point;
            fullResDescriptor.wrapMode = TextureWrapMode.Clamp;
            fullResDescriptor.name = "Pixelation_CopyColor_FullRes";

            var lowResDescriptor = cameraTexture.GetDescriptor(renderGraph);

            var flag = lowResDescriptor.width > Screen.width / 3 & lowResDescriptor.height > Screen.height / 3;
            if (flag)
            {
                lowResDescriptor.width = Screen.width / 3;
                lowResDescriptor.height = Screen.height / 3;
            }

            lowResDescriptor.depthBufferBits = 0;
            lowResDescriptor.msaaSamples = MSAASamples.None;
            lowResDescriptor.format = GraphicsFormat.R8G8B8A8_UNorm;
            lowResDescriptor.filterMode = FilterMode.Point;
            lowResDescriptor.wrapMode = TextureWrapMode.Clamp;

            // Setup textures.
            var copyColorTexture = renderGraph.CreateTexture(fullResDescriptor);

            var pixelateTextures = new TextureHandle[depthLevelCount];
            for (int i = 0; i < depthLevelCount - 1; i++)
            {
                lowResDescriptor.name = "Pixelation_PixelationLayer_" + i;
                pixelateTextures[i] = renderGraph.CreateTexture(lowResDescriptor); 
            }

            if (maxResolution >= Screen.height)
            {
                fullResDescriptor.name = "Pixelation_PixelationLayer_" + (depthLevelCount - 1);
                pixelateTextures[depthLevelCount - 1] = renderGraph.CreateTexture(fullResDescriptor);
            }
            else
            {
                lowResDescriptor.name = "Pixelation_PixelationLayer_" + (depthLevelCount - 1);
                pixelateTextures[depthLevelCount - 1] = renderGraph.CreateTexture(lowResDescriptor);
            }

            var mergeTextures = new TextureHandle[depthLevelCount];
            for (int i = 0; i < depthLevelCount; i++)
            {
                fullResDescriptor.name = "Pixelation_MergeLayer_" + i;
                mergeTextures[i] = renderGraph.CreateTexture(fullResDescriptor);
            }

            //-- Copy color

            if (m_CopyColorMaterial)
            {
                var blitParams = new RenderGraphUtils.BlitMaterialParameters(cameraTexture, copyColorTexture, m_CopyColorMaterial, 0);
                renderGraph.AddBlitPass(blitParams, "PixelationPass_CopyColor");
            }
            else
            {
                renderGraph.AddCopyPass(cameraTexture, copyColorTexture);
            }

            //-- Pixelate layers

            for (int i = 0; i < depthLevelCount; i++)
            {
                using (var builder = renderGraph.AddRasterRenderPass<PixelatePassData>("PixelationPass_Pixelate_" + i, out var passData))
                {
                    passData.material = m_PixelationMaterials[i];
                    passData.source = copyColorTexture;
                    passData.controllerMat = m_FinalMaterial;
                    passData.depth = resourceData.activeDepthTexture;          // <-- bind depth

                    passData.id = i;

                    builder.UseTexture(passData.source);
                    builder.UseTexture(passData.depth);
                    builder.SetRenderAttachment(pixelateTextures[i], 0, AccessFlags.Write);
                    builder.SetRenderFunc<PixelatePassData>(ExecutePixelatePass);
                }
            }

            //-- Merge layers

            for (int i = 0; i < depthLevelCount; i++)
            {
                using (var builder = renderGraph.AddRasterRenderPass<MergePassData>("PixelationPass_Merge_" + i, out var passData))
                {
                    passData.material = m_MergeLayerMaterials[i];

                    passData.backTex = (i == 0) ? copyColorTexture : mergeTextures[i - 1];
                    passData.frontTex = pixelateTextures[depthLevelCount - i - 1];

                    builder.UseTexture(passData.frontTex); // Read previous composite
                    builder.UseTexture(passData.backTex);  // Read new layer

                    // Write the new composite to the next texture in the chain
                    builder.SetRenderAttachment(mergeTextures[i], 0, AccessFlags.Write);
                    builder.SetRenderFunc<MergePassData>(ExecuteMergePass);
                }
            }

            //-- Final

            var showDepth = m_FinalMaterial.GetInt(ShowDepthID) == 1;
            var depthBased = true;// m_FinalMaterial.GetInt(DepthBasedID) == 1;
            if (!depthBased || showDepth)
            {
                renderGraph.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(mergeTextures[depthLevelCount - 1], cameraTexture, m_FinalMaterial, 0), "PixelationPass_Final");
            }
            else
            {
                resourceData.cameraColor = mergeTextures[depthLevelCount - 1];
            }
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static INab.ToonDetailer.URP.ToonDetailer;


namespace INab.ToonDetailer.URP
{
    public class ToonDetailerPass : ScriptableRenderPass
    {
        private ToonDetailerSettings m_Settings;
        private Material m_Material;
        private static MaterialPropertyBlock s_SharedPropertyBlock = new MaterialPropertyBlock();

        private static readonly int kDepthMaskTexture = Shader.PropertyToID("_DepthMaskRT");

        private static readonly int kBlitTexturePropertyId = Shader.PropertyToID("_BlitTexture");
        private static readonly int kBlitScaleBiasPropertyId = Shader.PropertyToID("_BlitScaleBias");

        // NORMALS with deferred
        // Should be fixed in 6000.0.22f
        //private static readonly int s_CameraNormalsTextureID = Shader.PropertyToID("_CameraNormalsTexture");


        private class PassData
        {
            public Material material;
            public TextureHandle source;
            public bool UseMask;
            public TextureHandle depthMask;
            public int shaderPass;
            public bool ControlViaVolumes;

            ////////////////////////////////////
            // NORMALS with deferred
            // Should be fixed in 6000.0.22f
            ////////////////////////////////////
            //public TextureHandle cameraNormalsTexture;
            ////////////////////////////////////

        }

        public ToonDetailerPass(string passName)
        {
            profilingSampler = new ProfilingSampler(passName);
        }

        public void Setup(ref Material material,ref ToonDetailerSettings settings)
        {
            m_Material = material;
            m_Settings = settings;
        }

        private static void ExecuteMainPass(PassData data, RasterGraphContext context)
        {
            // Both of these work. TODO: what is the difference?
            //Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), data.material, 0);
            //ExecuteMainPass(context.cmd, data.source.IsValid() ? data.source : null, data.material, data.depthMask);

            s_SharedPropertyBlock.Clear();
            if (data.source.IsValid()) s_SharedPropertyBlock.SetTexture(kBlitTexturePropertyId, data.source);

            if(data.UseMask)
            {
                if(data.depthMask.IsValid()) s_SharedPropertyBlock.SetTexture(kDepthMaskTexture, data.depthMask);
            }

            s_SharedPropertyBlock.SetVector(kBlitScaleBiasPropertyId, new Vector4(1, 1, 0, 0));

            if(data.ControlViaVolumes)
            {
                ToonDetailerVolumeComponent myVolume = VolumeManager.instance.stack?.GetComponent<ToonDetailerVolumeComponent>();
                if (myVolume != null)
                {
                    // Adjustments

                    s_SharedPropertyBlock.SetColor("_ColorHue", myVolume._ColorHue.value);
                    s_SharedPropertyBlock.SetFloat("_FadeStart", myVolume._FadeStart.value);
                    s_SharedPropertyBlock.SetFloat("_FadeEnd", myVolume._FadeEnd.value);
                    s_SharedPropertyBlock.SetFloat("_BlackOffset", myVolume._BlackOffset.value);

                    // Countour

                    s_SharedPropertyBlock.SetFloat("_ContoursIntensity", myVolume._ContoursIntensity.value);
                    s_SharedPropertyBlock.SetFloat("_ContoursThickness", myVolume._ContoursThickness.value);
                    s_SharedPropertyBlock.SetFloat("_ContoursElevationStrength", 3.0f * (myVolume._ContoursElevationStrength.value * (0.7f / (1.0f - myVolume._ContoursElevationSmoothness.value))));
                    s_SharedPropertyBlock.SetFloat("_ContoursElevationSmoothness", 1 - myVolume._ContoursElevationSmoothness.value);
                    s_SharedPropertyBlock.SetFloat("_ContoursDepressionStrength", 2.0f * (myVolume._ContoursDepressionStrength.value * (0.7f / (1.0f - myVolume._ContoursDepressionSmoothness.value))));
                    s_SharedPropertyBlock.SetFloat("_ContoursDepressionSmoothness", 1 - myVolume._ContoursDepressionSmoothness.value);


                    // Cavity 
                    s_SharedPropertyBlock.SetFloat("_CavityIntensity", myVolume._CavityIntensity.value);
                    s_SharedPropertyBlock.SetFloat("_CavityRadius", myVolume._CavityRadius.value);
                    s_SharedPropertyBlock.SetFloat("_CavityStrength", myVolume._CavityStrength.value);
                    s_SharedPropertyBlock.SetInt("_CavitySamples", myVolume._CavitySamples.value);
                }
            }

            ////////////////////////////////////
            // NORMALS with deferred
            //Should be fixed in 6000.0.22f
            ////////////////////////////////////
            //if (data.cameraNormalsTexture.IsValid())
            //    data.material.SetTexture(s_CameraNormalsTextureID, data.cameraNormalsTexture);
            ////////////////////////////////////

            context.cmd.DrawProcedural(Matrix4x4.identity, data.material, data.shaderPass, MeshTopology.Triangles, 3, 1, s_SharedPropertyBlock);
        }

        private void UpdateMaterialProperties(bool orthographic)
        {
            CoreUtils.SetKeyword(m_Material, ToonDetailer.k_Orthographic, orthographic);

            switch (m_Settings._DetailerType)
            {
                case ToonDetailerSettings.DetailerType.Both:
                    m_Material.EnableKeyword(ToonDetailer.k_UseContours);
                    m_Material.EnableKeyword(ToonDetailer.k_UseCavity);
                    break;
                case ToonDetailerSettings.DetailerType.Contours:
                    m_Material.EnableKeyword(ToonDetailer.k_UseContours);
                    m_Material.DisableKeyword(ToonDetailer.k_UseCavity);
                    break;
                case ToonDetailerSettings.DetailerType.Cavity:
                    m_Material.DisableKeyword(ToonDetailer.k_UseContours);
                    m_Material.EnableKeyword(ToonDetailer.k_UseCavity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #region materialProperties

            // General

            // Adjustments

            m_Material.SetColor("_ColorHue", m_Settings._ColorHue);
            m_Material.SetFloat("_FadeStart", m_Settings._FadeStart);
            m_Material.SetFloat("_FadeEnd", m_Settings._FadeEnd);
            m_Material.SetFloat("_BlackOffset", m_Settings._BlackOffset);

            if (m_Settings._UseFade)
            {
                m_Material.EnableKeyword(ToonDetailer.k_FadeOn);
            }
            else
            {
                m_Material.DisableKeyword(ToonDetailer.k_FadeOn);
                m_Material.DisableKeyword(ToonDetailer.k_FadeContoursOnly);
            }


            if (m_Settings._FadeAffectsOnlyContours && m_Settings._UseFade)
            {
                m_Material.EnableKeyword(ToonDetailer.k_FadeContoursOnly);
                m_Material.DisableKeyword(ToonDetailer.k_FadeOn);
            }
            else
            {
                m_Material.DisableKeyword(ToonDetailer.k_FadeContoursOnly);
            }

            // Countour

            m_Material.SetFloat("_ContoursIntensity", m_Settings._ContoursIntensity);
            m_Material.SetFloat("_ContoursThickness", m_Settings._ContoursThickness);
            m_Material.SetFloat("_ContoursElevationStrength", 3.0f * (m_Settings._ContoursElevationStrength * (0.7f / (1.0f - m_Settings._ContoursElevationSmoothness))));
            m_Material.SetFloat("_ContoursElevationSmoothness", 1 - m_Settings._ContoursElevationSmoothness);
            m_Material.SetFloat("_ContoursDepressionStrength", 2.0f * (m_Settings._ContoursDepressionStrength * (0.7f / (1.0f - m_Settings._ContoursDepressionSmoothness))));
            m_Material.SetFloat("_ContoursDepressionSmoothness", 1 - m_Settings._ContoursDepressionSmoothness);

            // Cavity 
            m_Material.SetFloat("_CavityIntensity", m_Settings._CavityIntensity);
            m_Material.SetFloat("_CavityRadius", m_Settings._CavityRadius);
            m_Material.SetFloat("_CavityStrength", m_Settings._CavityStrength);
            m_Material.SetInt("_CavitySamples", m_Settings._CavitySamples);

            #endregion
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            UpdateMaterialProperties(cameraData.camera.orthographic);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(passName, out var passData, profilingSampler))
            {
                var resourceData = frameData.Get<UniversalResourceData>();

                ////////////////////////////////////
                // NORMALS with deferred
                //Should be fixed in 6000.0.22f
                ////////////////////////////////////
                //UniversalRenderer universalRenderer = cameraData.renderer as UniversalRenderer;

                // It seems like we do not need that
                //bool isDeferred = universalRenderer != null && universalRenderer.renderingModeActual == RenderingMode.Deferred;
                //TextureHandle cameraNormalsTexture = resourceData.cameraNormalsTexture;

                //builder.UseTexture(cameraNormalsTexture, AccessFlags.Read);
                //passData.cameraNormalsTexture = cameraNormalsTexture;
                ///////////////////////

                if (frameData.Contains<TextureRefData>())
                {
                    var texRef = frameData.Get<TextureRefData>();
                    passData.depthMask = texRef.depthMaskTexture;
                    builder.UseTexture(passData.depthMask);
                }
                passData.UseMask = m_Settings.UseMask;

                passData.shaderPass = ((int)m_Settings._MaskUse);
                passData.material = m_Material;
                passData.source = resourcesData.cameraColor;
                passData.ControlViaVolumes = m_Settings._ControlViaVolumes;

                TextureHandle destination;

                var cameraColorDesc = renderGraph.GetTextureDesc(resourcesData.cameraColor);
                cameraColorDesc.name = "_CameraColorToonDetailer";
                cameraColorDesc.clearBuffer = true;

                destination = renderGraph.CreateTexture(cameraColorDesc);

                builder.UseTexture(passData.source, AccessFlags.Read);

                /*
                // Experiments

                //We do not need that either.Not sure whether we should need tho.
                //Debug.Assert(resourcesData.cameraNormalsTexture.IsValid());
                //builder.UseTexture(resourcesData.cameraNormalsTexture);
                //Debug.Assert(resourcesData.cameraDepthTexture.IsValid());
                //builder.UseTexture(resourcesData.cameraDepthTexture);

                // We do not need that either. Not sure whether we should need tho.
                //builder.SetRenderAttachmentDepth(resourcesData.activeDepthTexture, AccessFlags.Read);

                // This we could use if we DO copy to ALL pixels (in most effects we do actually) / for now we do not use this performance booster
                //TODO: test this out instead of the stuff above
                //destination = resourcesData.cameraColor;
                //passData.inputTexture = TextureHandle.nullHandle;

                */

                builder.SetRenderAttachment(destination, 0, AccessFlags.Write);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecuteMainPass(data, context));

                resourcesData.cameraColor = destination;
            }
        }

        public void Dispose()
        {
            // Nothing here
            //m_SSAOTextures[0]?.Release();
            //m_SSAOParamsPrev = default;
        }

    }
}
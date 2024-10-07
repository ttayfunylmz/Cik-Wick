using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace INab.ToonDetailer.URP
{
    [Serializable]
    public class ToonDetailerSettings
    {
        public enum DetailerType { Both = 0, Contours = 1, Cavity = 2 }
        public enum MaskUse { None = 0, NotEqual = 1, Equal = 2 }

        public bool UseMask
        {
            get
            {
                if (_MaskUse != MaskUse.None)
                    return true;

                return false;
            }
        }

        // General

        [SerializeField] public DetailerType _DetailerType = DetailerType.Both;
        [SerializeField] public MaskUse _MaskUse = MaskUse.None;
        [SerializeField] public LayerMask _MaskLayer;
        [SerializeField] public bool _ControlViaVolumes = false;

        // Adjustments

        [SerializeField] public Color _ColorHue = Color.black;
        [SerializeField] public bool _UseFade = false;
        [SerializeField] public bool _FadeAffectsOnlyContours = false;
        [SerializeField] public float _FadeStart = 40;
        [SerializeField] public float _FadeEnd = 60;
        [SerializeField, Range(0, 1)] public float _BlackOffset = .5f;

        // Contours

        [SerializeField, Range(0, 1)] public float _ContoursIntensity = 0.5f;
        [SerializeField, Range(0, 3)] public float _ContoursThickness = 1f;
        [SerializeField, Range(0, 3)] public float _ContoursElevationStrength = 1;
        [SerializeField, Range(0, 0.9f)] public float _ContoursElevationSmoothness = 0;
        [SerializeField, Range(0, 3)] public float _ContoursDepressionStrength = 2;
        [SerializeField, Range(0, 0.9f)] public float _ContoursDepressionSmoothness = 0;

        // Cavity

        [SerializeField, Range(0, 1)] public float _CavityIntensity = 1.0f;
        [SerializeField, Range(0, 1)] public float _CavityRadius = 0.5f;
        [SerializeField, Range(0, 5)] public float _CavityStrength = 1.25f;
        [SerializeField, Range(1, 16)] public int _CavitySamples = 12;
    }

    /// <summary>
    /// The class for the Toon Detailer renderer feature.
    /// </summary>
    [SupportedOnRenderer(typeof(UniversalRendererData))]
    [DisallowMultipleRendererFeature("Toon Detailer")]
    public class ToonDetailer : ScriptableRendererFeature
    {
        public class TextureRefData : ContextItem
        {
            public TextureHandle depthMaskTexture = TextureHandle.nullHandle;

            public override void Reset()
            {
                depthMaskTexture = TextureHandle.nullHandle;
            }
        }

        [SerializeField] private ToonDetailerSettings m_Settings = new ToonDetailerSettings();

        [SerializeField][HideInInspector] private Shader m_Shader;
        [SerializeField][HideInInspector] private Shader m_DepthShader;

        private Material m_ToonDetailerMaterial;
        private Material m_DepthMaterial;

        private ToonDetailerPass m_ToonDetailerPass = null;
        private DepthMaskPass m_DepthMaskPass = null;

        public const string k_UseContours = "_USE_CONTOURS";
        public const string k_UseCavity = "_USE_CAVITY";
        public const string k_Orthographic = "_ORTHOGRAPHIC";
        public const string k_FadeContoursOnly = "_FADE_COUNTOURS_ONLY";
        public const string k_FadeOn = "_FADE_ON";


        public override void Create()
        {
            m_ToonDetailerPass = new ToonDetailerPass("Toon Detailer");
            if(m_Settings.UseMask) m_DepthMaskPass = new DepthMaskPass("Toon Detailer Depth Mask");
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_ToonDetailerPass == null)
                return;

            if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection)
                return;

            if(m_Settings._ControlViaVolumes)
            {
                ToonDetailerVolumeComponent myVolume = VolumeManager.instance.stack?.GetComponent<ToonDetailerVolumeComponent>();
                if (myVolume == null || !myVolume.IsActive())
                    return;
            }

            // TODO: do i need that here or in the pass?
            if (m_Settings.UseMask) m_DepthMaskPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            m_ToonDetailerPass.renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;

            // Required if depth priming or depth texture in urp settings is off. Normals work only with Forward and Forward+. (TODO: for now)
            m_ToonDetailerPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);

            m_Shader = Shader.Find("Hidden/INabStudio/ToonDetailer");
            m_DepthShader = Shader.Find("Hidden/INabStudio/ToonDetailer/DepthMask");

            if (m_Shader == null || m_DepthShader == null) 
                return;
            m_ToonDetailerMaterial = new Material(m_Shader);
            if (m_Settings.UseMask) m_DepthMaterial = new Material(m_DepthShader);
            //OR?    m_Material = CoreUtils.CreateEngineMaterial(m_Shader);


            if (m_Settings.UseMask) m_DepthMaskPass.Setup(ref m_DepthMaterial, ref m_Settings._MaskLayer);
            m_ToonDetailerPass.Setup(ref m_ToonDetailerMaterial, ref m_Settings);
            if (m_Settings.UseMask) renderer.EnqueuePass(m_DepthMaskPass);
            renderer.EnqueuePass(m_ToonDetailerPass);
        }


        protected override void Dispose(bool disposing)
        {
            if(m_DepthMaskPass != null) m_DepthMaskPass.Dispose();
            m_DepthMaskPass = null;

            if(m_ToonDetailerPass != null) m_ToonDetailerPass.Dispose();
            m_ToonDetailerPass = null;

#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                if(m_DepthMaterial) Destroy(m_DepthMaterial);
                Destroy(m_ToonDetailerMaterial);
            }
            else
            {
                if (m_DepthMaterial) DestroyImmediate(m_DepthMaterial);
                DestroyImmediate(m_ToonDetailerMaterial);
            }
#else
                if(m_DepthMaterial)Destroy(m_DepthMaterial);
                Destroy(m_ToonDetailerMaterial);
#endif

            // If  CoreUtils.CreateEngineMaterial(m_Shader); used
            //CoreUtils.Destroy(m_Material);
        }


    }
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace INab.ToonDetailer.URP
{
    [VolumeComponentMenu("INab Studio/Toon Detailer")]
    [VolumeRequiresRendererFeatures(typeof(ToonDetailer))]
    [SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
    public sealed class ToonDetailerVolumeComponent : VolumeComponent, IPostProcessComponent
    {
        public ToonDetailerVolumeComponent()
        {
            displayName = "Toon Detailer";
        }

        // Adjustments

        [SerializeField] public ColorParameter _ColorHue = new ColorParameter(Color.black);
        [SerializeField] public BoolParameter _FadeAffectsOnlyContours = new BoolParameter(false);
        [SerializeField] public FloatParameter _FadeStart = new FloatParameter(40);
        [SerializeField] public FloatParameter _FadeEnd = new FloatParameter(40);
        public ClampedFloatParameter _BlackOffset = new ClampedFloatParameter(0.5f, 0f, 1f);

        // Contours

        public ClampedFloatParameter _ContoursIntensity = new ClampedFloatParameter(0.5f, 0f, 1f);
        public ClampedFloatParameter _ContoursThickness = new ClampedFloatParameter(1, 0f, 3f);
        public ClampedFloatParameter _ContoursElevationStrength = new ClampedFloatParameter(1, 0f, 3f);
        public ClampedFloatParameter _ContoursElevationSmoothness = new ClampedFloatParameter(0, 0f, 0.9f);
        public ClampedFloatParameter _ContoursDepressionStrength = new ClampedFloatParameter(2, 0f, 3f);
        public ClampedFloatParameter _ContoursDepressionSmoothness = new ClampedFloatParameter(0, 0f, 0.9f);

        // Cavity

        public ClampedFloatParameter _CavityIntensity = new ClampedFloatParameter(1, 0f, 1f);
        public ClampedFloatParameter _CavityRadius = new ClampedFloatParameter(0.5f, 0f, 1f);
        public ClampedFloatParameter _CavityStrength = new ClampedFloatParameter(1.25f, 0f, 5f);
        public ClampedIntParameter _CavitySamples = new ClampedIntParameter(12, 1, 16);

        public bool IsActive()
        {
            if (_ContoursIntensity.GetValue<float>() == 0.0f && _CavityIntensity.GetValue<float>() == 0.0f)
                return false;
            
            return true;
        }
    }
}
Shader "Hidden/INabStudio/ToonDetailer"
{
    HLSLINCLUDE
    
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        // Do we need that?
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"

        TEXTURE2D_X(_DepthMaskRT);
        SAMPLER(sampler_DepthMaskRT);

        // Adjustments
        
        float4 _ColorHue;
        float _FadeStart;
        float _FadeEnd;
        float _BlackOffset;
        
        // Countours
        
        float _ContoursIntensity;
        float _ContoursThickness;
        float _ContoursElevationStrength;
        float _ContoursElevationSmoothness;
        float _ContoursDepressionStrength;
        float _ContoursDepressionSmoothness;
        
        // Cavity
        
        float _CavityIntensity;
        float _CavityRadius;
        float _CavityStrength;
        int _CavitySamples;
        
        // Static
       
        static const float kContrast = 0.6;
        #define ATTENUATION      0.015625
        static const float kBeta = 0.002;

        #define SCREEN_PARAMS               GetScaledScreenParams()

        static const float SSAORandomUV[40] =
        {
            0.00000000,  // 00
            0.33984375,  // 01
            0.75390625,  // 02
            0.56640625,  // 03
            0.98437500,  // 04
            0.07421875,  // 05
            0.23828125,  // 06
            0.64062500,  // 07
            0.35937500,  // 08
            0.50781250,  // 09
            0.38281250,  // 10
            0.98437500,  // 11
            0.17578125,  // 12
            0.53906250,  // 13
            0.28515625,  // 14
            0.23137260,  // 15
            0.45882360,  // 16
            0.54117650,  // 17
            0.12941180,  // 18
            0.64313730,  // 19

            0.92968750,  // 20
            0.76171875,  // 21
            0.13333330,  // 22
            0.01562500,  // 23
            0.00000000,  // 24
            0.10546875,  // 25
            0.64062500,  // 26
            0.74609375,  // 27
            0.67968750,  // 28
            0.35156250,  // 29
            0.49218750,  // 30
            0.12500000,  // 31
            0.26562500,  // 32
            0.62500000,  // 33
            0.44531250,  // 34
            0.17647060,  // 35
            0.44705890,  // 36
            0.93333340,  // 37
            0.87058830,  // 38
            0.56862750,  // 39
        };

        float2 GetScreenSpacePosition(float2 uv)
        {
            return uv * SCREEN_PARAMS.xy;
        }

        float2 CosSin(float theta)
        {
            float sn, cs;
            sincos(theta, sn, cs);
            return float2(cs, sn);
        }

        float3 PickSamplePoint(float2 uv, float randAddon, int index)
        {
            float2 positionSS = GetScreenSpacePosition(uv);
            float gn = InterleavedGradientNoise(positionSS, index);

            float u = frac(gn) * 2.0 - 1.0;
            float theta = gn * TWO_PI;
            return float3(CosSin(theta) * sqrt(1.0 - u * u), u);
        }

        float RawToLinearDepth(float rawDepth)
        {
            #if defined(_ORTHOGRAPHIC)
                #if UNITY_REVERSED_Z
                    return ((_ProjectionParams.z - _ProjectionParams.y) * (1.0 - rawDepth) + _ProjectionParams.y);
                #else
                    return ((_ProjectionParams.z - _ProjectionParams.y) * (rawDepth) + _ProjectionParams.y);
                #endif
            #else
                return LinearEyeDepth(rawDepth, _ZBufferParams);
            #endif
        }

        float GetLinearDepth(float2 uv)
        {
            float rawDepth = SampleSceneDepth(uv.xy).r;
            return RawToLinearDepth(rawDepth);
        }

        float3 ReconstructViewPos(float2 uv, float depth, float2 p11_22, float2 p13_31)
        {
            #if defined(_ORTHOGRAPHIC)
                float3 viewPos = float3(((uv.xy * 2.0 - 1.0 - p13_31) * p11_22), depth);
            #else
                float3 viewPos = float3(depth * ((uv.xy * 2.0 - 1.0 - p13_31) * p11_22), depth);
            #endif
            return viewPos;
        }

        void GetDepthNormalView(float2 uv, float2 p11_22, float2 p13_31, out float depth, out float3 normal, out float3 vpos)
        {
            depth  = GetLinearDepth(uv);
            vpos = ReconstructViewPos(uv, depth, p11_22, p13_31);
            normal = SampleSceneNormals(uv);
        }

        float3x3 GetCoordinateConversionParameters(out float2 p11_22, out float2 p13_31)
        {
            float3x3 camProj = (float3x3)unity_CameraProjection;

            p11_22 = rcp(float2(camProj._11, camProj._22));
            p13_31 = float2(camProj._13, camProj._23);

            return camProj;
        }

  
        float ContoursAdjust(float value, float strength, float smoothness)
        {
            value = clamp(value,0,smoothness);
        
            return value * strength;
        }

               float Contours(float2 uv)
        {
           float3 contourOffset = float3(_BlitTexture_TexelSize.xy, 0.0) * _ContoursThickness;

           if(uv.y - contourOffset.y < 0.001f || uv.x - contourOffset.x < 0.001f)
           {
               return 0.0f; 
           }

           float upperNormal = SampleSceneNormals(uv + contourOffset.zy).g;
           float lowerNormal = SampleSceneNormals(uv - contourOffset.zy).g;

           float rightNormal = SampleSceneNormals(uv + contourOffset.xz).r;
           float leftNormal  = SampleSceneNormals(uv - contourOffset.xz).r;

           float greenDifference = upperNormal - lowerNormal;

           float redDifference = rightNormal - leftNormal;

           float combinedValue = greenDifference + redDifference;

           if (combinedValue >= 0.0f)
           {
               return ContoursAdjust(combinedValue, _ContoursElevationStrength, _ContoursElevationSmoothness);
           }
           else
           {
               return -ContoursAdjust(-combinedValue, _ContoursDepressionStrength, _ContoursDepressionSmoothness);
           }
        }

        float Cavity(float2 uv)
        {
            float darkValue = 0.0;
            float brightValue = 0.0;

            float2 coefsA, coefsB;
            float3x3 transformMatrix = GetCoordinateConversionParameters(coefsA, coefsB);

            float depthValue;
            float3 normalVector;
            float3 viewCoord;
            GetDepthNormalView(uv, coefsA, coefsB, depthValue, normalVector, viewCoord);

            float noiseAddition = uv.x * 1e-10;
            float inverseSamples = rcp(_CavitySamples);

            UNITY_LOOP
            for (int iter = 0; iter < _CavitySamples; iter++)
            {
                float3 sampleVec = PickSamplePoint(uv, noiseAddition, iter);
                sampleVec *= sqrt((iter + 1.0) * inverseSamples) * _CavityRadius * 0.5;
                float3 shiftedViewCoord = viewCoord + sampleVec;

                float3 screenCoord = mul(transformMatrix, shiftedViewCoord);
                float2 uvShifted;
                #if defined(_ORTHOGRAPHIC)
                    uvShifted = clamp((screenCoord.xy + 1.0) * 0.5, 0.0, 1.0);
                #else
                    uvShifted = clamp((screenCoord.xy / shiftedViewCoord.z + 1.0) * 0.5, 0.0, 1.0);
                #endif

                float shiftedDepth = GetLinearDepth(uvShifted);

                float3 reconstructedView = ReconstructViewPos(uvShifted, shiftedDepth, coefsA, coefsB);
                float3 dirVector = reconstructedView - viewCoord;
                float distance = length(dirVector);
                float dotProduct = dot(dirVector, normalVector);

                float negativeDot = dotProduct - kBeta * depthValue;
                float positiveDot = -dotProduct - kBeta * depthValue;

                float biasOffset = 0.05 * distance + 0.0001;
                float attenuationValue = 1.0 / (distance * (1.0 + distance * distance * ATTENUATION));

                if (negativeDot > -biasOffset)
                {
                    darkValue += negativeDot * attenuationValue;
                }

                if (positiveDot > biasOffset)
                {
                    brightValue += positiveDot * attenuationValue;
                }
            }

            darkValue *= 1.0 / _CavitySamples;
            brightValue *= 1.0 / _CavitySamples;

            darkValue = pow(darkValue * inverseSamples, kContrast);
            brightValue = pow(brightValue * inverseSamples, kContrast);

            darkValue = saturate(darkValue * _CavityStrength);
            brightValue *= _CavityStrength;

            return (1.0 - darkValue) * (1.0 + brightValue);
        }

         float DepthFade(float2 uv)
        {
            float rawDepth = SampleSceneDepth(uv.xy).r;
            float fadeValue = max(RawToLinearDepth(rawDepth)-_FadeStart,0) / _FadeEnd;
            fadeValue = pow(fadeValue,2);
            return saturate(smoothstep(0,1,fadeValue));
        }

        float CombineDetails(float2 uv)
        {
            float cavity = 1.0;
            float contours = 0.0;
        
            #ifdef _USE_CONTOURS
            contours =  Contours(uv);
            #endif
            
            #ifdef _USE_CAVITY
            cavity = Cavity(uv);
            #endif
                   
            #ifdef _FADE_COUNTOURS_ONLY
            contours = lerp (contours,0,DepthFade(uv));
            #endif

            cavity = lerp(1, cavity, _CavityIntensity);
        
            contours = lerp(0, contours, _ContoursIntensity);
            
            float value = clamp(cavity * (1.0 + contours),0,4);
        
            return value;
        }

        float4 DetailerOutput(float4 color, float2 texcoord)
        {
            float value = CombineDetails(texcoord);

            #ifdef _FADE_ON
            value = lerp (value,1,DepthFade(texcoord));
            #endif
    
            // Dark offset
            value = clamp(value,_BlackOffset,4);

            // add hue to it // sunset/moonlight etc
            color = lerp(color* _ColorHue,color,saturate(value));

            return float4(color.rgb * value, 1);
        }


        /////////////////////////////////////////////////////////////

        float4 Detailer_PostProcess_NoMask(Varyings input) : SV_Target
        {
           UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
           //return SampleSceneNormals(input.texcoord).rgbr;

           float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

           return DetailerOutput(color, input.texcoord);
        }

        float4 Detailer_PostProcess_NotEqual(Varyings input) : SV_Target
        {
           UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
           float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
           float mask = SAMPLE_TEXTURE2D_X(_DepthMaskRT, sampler_DepthMaskRT, input.texcoord).r;
           float rawDepth = SampleSceneDepth(input.texcoord.xy).r;
           float4 output = DetailerOutput(color,input.texcoord);
           
           if(rawDepth < .0000001) return output;
           
           if(rawDepth - mask > 0.001)
           {
               return output;
           }
           return color;

        }

        float4 Detailer_PostProcess_Equal(Varyings input) : SV_Target
        {
           UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
           
           float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
           float mask = SAMPLE_TEXTURE2D_X(_DepthMaskRT, sampler_DepthMaskRT, input.texcoord).r;
           float rawDepth = SampleSceneDepth(input.texcoord.xy).r;
           float4 output = DetailerOutput(color,input.texcoord);
           
           if(rawDepth < .0000001) return color;
           
           if(rawDepth - mask > 0.001)
           {
               return color;
           }
           return output;
        }

    
    ENDHLSL
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "Detailer_PostProcess_NoMask"

            HLSLPROGRAM
            #pragma multi_compile_local _ _USE_CONTOURS
            #pragma multi_compile_local _ _USE_CAVITY
            #pragma multi_compile_local _ _ORTHOGRAPHIC
            #pragma multi_compile_local _ _FADE_COUNTOURS_ONLY
            #pragma multi_compile_local _ _FADE_ON

            #pragma vertex Vert
            #pragma fragment Detailer_PostProcess_NoMask
            
            ENDHLSL
        }

         Pass
        {
            Name "Detailer_PostProcess_NotEqual"

            HLSLPROGRAM
            #pragma multi_compile_local _ _USE_CONTOURS
            #pragma multi_compile_local _ _USE_CAVITY
            #pragma multi_compile_local _ _ORTHOGRAPHIC
            #pragma multi_compile_local _ _FADE_COUNTOURS_ONLY
            #pragma multi_compile_local _ _FADE_ON

            #pragma vertex Vert
            #pragma fragment Detailer_PostProcess_NotEqual
            
            ENDHLSL
        }

         Pass
        {
            Name "Detailer_PostProcess_Equal"

            HLSLPROGRAM
            #pragma multi_compile_local _ _USE_CONTOURS
            #pragma multi_compile_local _ _USE_CAVITY
            #pragma multi_compile_local _ _ORTHOGRAPHIC
            #pragma multi_compile_local _ _FADE_COUNTOURS_ONLY
            #pragma multi_compile_local _ _FADE_ON

            #pragma vertex Vert
            #pragma fragment Detailer_PostProcess_Equal
            
            ENDHLSL
        }
        
       
    }
}
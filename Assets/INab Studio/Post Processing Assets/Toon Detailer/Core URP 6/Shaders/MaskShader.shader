Shader "Hidden/INabStudio/ToonDetailer/DepthMask"
{
    HLSLINCLUDE

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
   
		struct Attributes
        {
            float4 position     : POSITION;
            float3 normal        : NORMAL;
            float2 texcoord     : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float2 uv           : TEXCOORD0;
            float4 positionCS   : SV_POSITION;
            float3 positionVS   : TEXCOORD1;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Varyings DepthOnlyVertex(Attributes input)
        {
            Varyings output = (Varyings)0;
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            float3 pos = input.position.xyz - input.normal * 0.0000;
            VertexPositionInputs positionInputs = GetVertexPositionInputs(pos);
			output.positionCS = positionInputs.positionCS;
			output.positionVS = positionInputs.positionVS;

            return output;
        }

        float DepthOnlyFragment(Varyings input) : SV_TARGET
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            return input.positionCS.z; 
        }

	ENDHLSL

	SubShader
	{
		Tags { "RenderPipeline" = "UniversalPipeline" }

		Cull Off
		ZWrite On
		ZTest LEqual

		Pass
		{
			Name "SceneDepthPass"

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma vertex DepthOnlyVertex
			#pragma fragment DepthOnlyFragment

			ENDHLSL
		}
	}
}

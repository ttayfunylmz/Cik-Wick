#ifndef OS_OUTLINE_OBJECTS_INCLUDE
#define OS_OUTLINE_OBJECTS_INCLUDE


///////////////////////////////////////////////////////////////////////////////
//                      Includes                                             //
///////////////////////////////////////////////////////////////////////////////

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
// See ShaderVariablesFunctions.hlsl in com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl



///////////////////////////////////////////////////////////////////////////////
//                      Properties                                           //
///////////////////////////////////////////////////////////////////////////////

CBUFFER_START(UnityPerMaterial)
    float4 _OutlineColor;
    float _OutlineThickness;
    float _CompleteFalloffDistance;
    Texture2D _NoiseTexture;
    SAMPLER(sampler_linear_repeat);
    float _NoiseFrequency;
    float _NoiseFramerate;
    int _USE_VERTEX_COLOR_ENABLED;
    int _ATTENUATE_BY_DISTANCE_ENABLED;
    int _RANDOM_OFFSETS_ENABLED;
    int _USE_SMOOTHED_NORMALS_ENABLED;
CBUFFER_END


///////////////////////////////////////////////////////////////////////////////
//                      Utilities                                            //
///////////////////////////////////////////////////////////////////////////////

float nrand(float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
}


/////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
///                                                                           ///
///                     SHADER BODY                                           ///
///                                                                           ///
/////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////
//                      Structs                                              //
///////////////////////////////////////////////////////////////////////////////

struct Attributes
{
    half4 position : POSITION;
    half3 normal : NORMAL;
    half4 color : COLOR;
    half3 normalSmooth : TEXCOORD3;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    half4 positionHCS : SV_POSITION;
	half3 normalWS : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};



///////////////////////////////////////////////////////////////////////////////
//                      Vertex                                               //
///////////////////////////////////////////////////////////////////////////////

Varyings Vertex(Attributes IN)
{
	Varyings OUT = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(IN);
	UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
    
    ///////////////////////////////
    //   Typical Vertex          //
    ///////////////////////////////
    
    half3 normalWS = normalize(GetVertexNormalInputs(IN.normal).normalWS);
    if(_USE_SMOOTHED_NORMALS_ENABLED == 1)
    {
        normalWS = TransformObjectToWorldNormal(IN.normalSmooth);
    }
                
    float3 positionWS = TransformObjectToWorld(IN.position.xyz);
    
    
    
    ///////////////////////////////
    //   SETUP                   //
    ///////////////////////////////
    _OutlineThickness = max(_OutlineThickness, 0);
    
	if (_USE_VERTEX_COLOR_ENABLED == 1)
	{
		_OutlineThickness *= IN.color.r;
	}
    
    if(_ATTENUATE_BY_DISTANCE_ENABLED == 1)
    {
        _OutlineThickness *= 1.0 - saturate(distance(_WorldSpaceCameraPos, positionWS) / _CompleteFalloffDistance);
    }
    
    half dist = length(IN.position.xyz);

    half r = _Time.y * _NoiseFramerate;
    if(_RANDOM_OFFSETS_ENABLED == 1)
    {
        r = nrand(floor(r));
    }
    
    half value = _NoiseTexture.SampleLevel(sampler_linear_repeat, half2(r + (dist * _NoiseFrequency), 0), 0).r;
    _OutlineThickness *= value;
    
    
    ///////////////////////////////
    //   Hull and Set            //
    ///////////////////////////////
    
    positionWS += (normalWS * _OutlineThickness);
    OUT.positionHCS = TransformWorldToHClip(positionWS);
	OUT.normalWS = normalWS;
    return OUT;
}


///////////////////////////////////////////////////////////////////////////////
//                      Fragment                                             //
///////////////////////////////////////////////////////////////////////////////

half4 Fragment(Varyings IN) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
    
    return _OutlineColor;
}

half FragmentDepthOnly(Varyings IN) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
    
    return 0;
}

half4 FragmentDepthNormalsOnly(Varyings IN) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
    
    return float4(normalize(IN.normalWS), 0);
}
#endif
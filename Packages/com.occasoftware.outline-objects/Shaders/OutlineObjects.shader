Shader "OccaSoftware/Outline Objects"
{
    Properties
    {
        [HDR]_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Float) = 0.1
        _CompleteFalloffDistance ("Complete Falloff Distance", Float) = 30.0
        _NoiseTexture ("Noise Texture", 2D) = "white" {} 
        _NoiseFrequency ("Noise Frequency", Float) = 5.0
        _NoiseFramerate ("Noise Framerate", Float) = 12.0
        
        [Toggle(_USE_VERTEX_COLOR_ENABLED)] _USE_VERTEX_COLOR_ENABLED ("Use Vertex Color (R) for Outline Thickness?", Float) = 0
        [Toggle(_ATTENUATE_BY_DISTANCE_ENABLED)] _ATTENUATE_BY_DISTANCE_ENABLED("Attenuate Outline Thickness by Camera Distance?", Float) = 0
        [Toggle(_RANDOM_OFFSETS_ENABLED)] _RANDOM_OFFSETS_ENABLED("Randomly offset the sample position", Float) = 0
        [Toggle(_USE_SMOOTHED_NORMALS_ENABLED)] _USE_SMOOTHED_NORMALS_ENABLED("Use Smoothed Normals (UV3)", Float) = 0

        _Surface("__surface", Float) = 0.0
        _QueueOffset("Queue offset", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0

        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
    }
    
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite [_ZWrite]
        ZTest LEqual
        Cull Front
        Blend [_SrcBlend][_DstBlend]
        
        Pass
        {
            Name "DrawOutlines"
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #pragma multi_compile_instancing

            // Material Keywords
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAMODULATE_ON
            
            #include "OutlineObjectsPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags {"LightMode" = "DepthOnly"}

            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentDepthOnly
            
            #pragma multi_compile_instancing
            
            #include "OutlineObjectsPass.hlsl"
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthNormalsOnly"
            Tags {"LightMode" = "DepthNormalsOnly"}

            ZWrite On

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment FragmentDepthNormalsOnly
            
            #pragma multi_compile_instancing
            
            #include "OutlineObjectsPass.hlsl"
            ENDHLSL
        }
    }
    
    CustomEditor "OccaSoftware.OutlineObjects.Editor.OutlineObjectsShaderGUI"
}

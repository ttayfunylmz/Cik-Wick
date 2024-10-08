// Made with Amplify Shader Editor v1.9.0.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Archanor VFX/Epic Toon FX/ETFX_PowerboxUnlit"
{
	Properties
	{
		_Icon("Icon", 2D) = "white" {}
		_Background("Background", 2D) = "white" {}
		_IconTint("Icon Tint", Color) = (0,0,0,0)
		_IconContrast("Icon Contrast", Range( 0.01 , 5)) = 1
		_IconBrightness("Icon Brightness", Range( 0 , 3)) = 1
		_BackgroundTint("Background Tint", Color) = (1,0,0.05511808,0)
		_BackgroundContrast("Background Contrast", Range( 0 , 5)) = 1
		_BackgroundBrightness("Background Brightness", Range( 0 , 3)) = 1
		_IconScale("Icon Scale", Float) = 2
		_IconOffset("Icon Offset", Vector) = (0,-1,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _BackgroundBrightness;
		uniform sampler2D _Background;
		uniform float4 _Background_ST;
		uniform float _BackgroundContrast;
		uniform float4 _BackgroundTint;
		uniform sampler2D _Icon;
		uniform float _IconScale;
		uniform float3 _IconOffset;
		uniform float4 _IconTint;
		uniform float _IconContrast;
		uniform float _IconBrightness;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_Background = i.uv_texcoord * _Background_ST.xy + _Background_ST.zw;
			float4 temp_cast_0 = (_BackgroundContrast).xxxx;
			float2 temp_cast_1 = (_IconScale).xx;
			float2 uv_TexCoord45 = i.uv_texcoord * temp_cast_1 + _IconOffset.xy;
			float4 tex2DNode2 = tex2D( _Icon, uv_TexCoord45 );
			float4 temp_cast_3 = (_IconContrast).xxxx;
			float4 lerpResult38 = lerp( ( _BackgroundBrightness * ( pow( tex2D( _Background, uv_Background ) , temp_cast_0 ) * _BackgroundTint ) * i.vertexColor ) , ( pow( ( tex2DNode2 + _IconTint ) , temp_cast_3 ) * _IconBrightness ) , tex2DNode2.a);
			o.Emission = lerpResult38.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19002
91;338;1504;720;2509.251;849.3961;2.274973;True;False
Node;AmplifyShaderEditor.Vector3Node;52;-1591.329,136.8914;Inherit;False;Property;_IconOffset;Icon Offset;9;0;Create;True;0;0;0;False;0;False;0,-1,0;0,-1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;50;-1604.805,27.40174;Inherit;False;Property;_IconScale;Icon Scale;8;0;Create;True;0;0;0;False;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;45;-1327.3,47.80945;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;54;-983.4308,307.6082;Inherit;False;Property;_IconTint;Icon Tint;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-1042.502,20.61381;Inherit;True;Property;_Icon;Icon;0;0;Create;True;0;0;0;False;0;False;-1;a418ccab11aad674783413a05d3779ee;fe892915b0ecc7041a4e1fa5ce9b6d15;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1177.422,-543.4243;Inherit;True;Property;_Background;Background;1;0;Create;True;0;0;0;False;0;False;-1;6a5e2aff47ffbc44ebe6ab296846e509;c701ebbdfe2ea60438c5ab736af04bfb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;76;-1159.07,-344.7339;Inherit;False;Property;_BackgroundContrast;Background Contrast;6;0;Create;True;0;0;0;False;0;False;1;0.2;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;6;-870.3491,-241.268;Inherit;False;Property;_BackgroundTint;Background Tint;5;0;Create;True;0;0;0;False;0;False;1,0,0.05511808,0;0.5647059,0.4471444,0.2470588,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;75;-814.532,-415.6688;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-742.6393,517.5684;Inherit;False;Property;_IconContrast;Icon Contrast;3;0;Create;True;0;0;0;False;0;False;1;1;0.01;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;72;-714.4584,223.8855;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-577.1462,-260.0124;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;82;-448.8102,-31.15087;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;80;-486.0616,636.2398;Inherit;False;Property;_IconBrightness;Icon Brightness;4;0;Create;True;0;0;0;False;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;73;-440.9699,315.338;Inherit;True;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-583.1085,-398.4962;Inherit;False;Property;_BackgroundBrightness;Background Brightness;7;0;Create;True;0;0;0;False;0;False;1;1;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-243.4128,-208.8986;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;81;-142.416,314.3189;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;38;36.7254,72.31671;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;330.2302,21.70766;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Archanor VFX/Epic Toon FX/ETFX_PowerboxUnlit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;45;0;50;0
WireConnection;45;1;52;0
WireConnection;2;1;45;0
WireConnection;75;0;3;0
WireConnection;75;1;76;0
WireConnection;72;0;2;0
WireConnection;72;1;54;0
WireConnection;7;0;75;0
WireConnection;7;1;6;0
WireConnection;73;0;72;0
WireConnection;73;1;74;0
WireConnection;79;0;78;0
WireConnection;79;1;7;0
WireConnection;79;2;82;0
WireConnection;81;0;73;0
WireConnection;81;1;80;0
WireConnection;38;0;79;0
WireConnection;38;1;81;0
WireConnection;38;2;2;4
WireConnection;0;2;38;0
ASEEND*/
//CHKSM=70E26878BF5F28EA291E13D7FF661D9AAEE9E407
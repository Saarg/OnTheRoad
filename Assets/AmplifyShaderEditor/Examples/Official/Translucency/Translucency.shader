// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Translucency"
{
	Properties
	{
		_Specular("Specular", Range( 0 , 1)) = 0
		[HideInInspector] __dirty( "", Int ) = 1
		_Color0("Color 0", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Depth("Depth", 2D) = "white" {}
		_Tint("Tint", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecularCustom keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		struct SurfaceOutputStandardSpecularCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			fixed3 Specular;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			fixed3 Translucency;
		};

		uniform sampler2D _Normal;
		uniform float4 _Albedo_ST;
		uniform fixed4 _Tint;
		uniform sampler2D _Albedo;
		uniform fixed _Specular;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform sampler2D _Depth;
		uniform float4 _Depth_ST;
		uniform fixed4 _Color0;

		inline half4 LightingStandardSpecularCustom(SurfaceOutputStandardSpecularCustom s, half3 viewDir, UnityGI gi )
		{
			#if !DIRECTIONAL
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandardSpecular r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Specular = s.Specular;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandardSpecular (r, viewDir, gi) + c;
		}

		inline void LightingStandardSpecularCustom_GI(SurfaceOutputStandardSpecularCustom s, UnityGIInput data, inout UnityGI gi )
		{
			UNITY_GI(gi, s, data);
		}

		void surf( Input i , inout SurfaceOutputStandardSpecularCustom o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal,uv_Albedo) );
			o.Albedo = ( _Tint * tex2D( _Albedo,uv_Albedo) ).xyz;
			fixed3 temp_cast_2 = _Specular;
			o.Specular = temp_cast_2;
			float2 uv_Depth = i.uv_texcoord * _Depth_ST.xy + _Depth_ST.zw;
			o.Translucency = ( tex2D( _Depth,uv_Depth) * _Color0 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5002
339;92;1190;648;796.6179;88.28336;1.1;True;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-93.79053,185.3298;Float;FLOAT4;0.0,0,0,0;COLOR;0,0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-119.7905,-259.6702;Float;COLOR;0,0,0,0;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.ColorNode;6;-528.7905,296.3298;Float;Property;_Color0;Color 0;-1;0;0,0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;16;-169.3906,29.62976;Float;Property;_Specular;Specular;-1;0;0;0;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;17;-1016.736,-72.31985;Float;0;9;2;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;186.6,-73.70001;Fixed;True;2;Fixed;ASEMaterialInspector;StandardSpecular;ASESampleShaders/Translucency;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;9;-560.7905,-261.6702;Float;Property;_Albedo;Albedo;1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;12;-597.7905,-81.67023;Float;Property;_Normal;Normal;2;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;7;-570.7905,119.3298;Float;Property;_Depth;Depth;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.ColorNode;10;-505.058,-441.5712;Float;Property;_Tint;Tint;4;0;0,0,0,0
WireConnection;8;0;7;0
WireConnection;8;1;6;0
WireConnection;11;0;10;0
WireConnection;11;1;9;0
WireConnection;0;0;11;0
WireConnection;0;1;12;0
WireConnection;0;3;16;0
WireConnection;0;7;8;0
WireConnection;9;1;17;0
WireConnection;12;1;17;0
ASEEND*/
//CHKSM=1116A2E36496DCE7A208AB4B8667135A132729E2
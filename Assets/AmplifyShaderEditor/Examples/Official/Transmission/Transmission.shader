// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/Transmission"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Transmission("Transmission", Color) = (0,0,0,0)
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Color("Color", Color) = (0,0,0,0)
		_BaseColor("Base Color", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Gloss("Gloss", Range( 0 , 1)) = 0.8
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_NormalMap;
			float2 uv_BaseColor;
		};

		struct SurfaceOutputStandardCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			fixed3 Transmission;
		};

		uniform sampler2D _NormalMap;
		uniform sampler2D _BaseColor;
		uniform fixed4 _Color;
		uniform fixed _Metallic;
		uniform fixed _Gloss;
		uniform fixed4 _Transmission;

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			half3 transmission = max(0 , -dot(s.Normal, gi.light.dir)) * gi.light.color * s.Transmission;
			half4 d = half4(s.Albedo * transmission , 0);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + d;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			UNITY_GI(gi, s, data);
		}

		void surf( Input input , inout SurfaceOutputStandardCustom output )
		{
			output.Normal = UnpackNormal( tex2D( _NormalMap,input.uv_NormalMap) );
			output.Albedo = ( tex2D( _BaseColor,input.uv_BaseColor) * _Color ).rgb;
			output.Metallic = _Metallic;
			output.Smoothness = _Gloss;
			output.Transmission = _Transmission.rgb;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;932.0179;77.84949;1;True;False
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-422.3381,-5.544991;Float;FLOAT4;0.0,0,0,0;COLOR;0.0,0,0,0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;80.91907,-11.16125;Fixed;True;2;Fixed;ASEMaterialInspector;Standard;ASESampleShaders/Transmission;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SamplerNode;1;-823.954,-122.8771;Float;Property;_BaseColor;Base Color;3;None;True;0;False;white;Auto;False;Object;-1;Auto;FLOAT2;0,0;FLOAT2;1.0,0;FLOAT;0.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RangedFloatNode;4;-430.4556,117.6564;Float;Property;_Metallic;Metallic;4;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;5;-429.0605,215.0723;Float;Property;_Gloss;Gloss;5;0.8;0;1
Node;AmplifyShaderEditor.ColorNode;7;-342.3347,467.6476;Float;Property;_Transmission;Transmission;-1;0,0,0,0
Node;AmplifyShaderEditor.ColorNode;2;-716.0739,66.4277;Float;Property;_Color;Color;2;0,0,0,0
Node;AmplifyShaderEditor.SamplerNode;6;-513.9389,303.0193;Float;Property;_NormalMap;Normal Map;1;None;True;0;True;bump;Auto;True;Object;-1;Auto;FLOAT2;0,0;FLOAT2;1.0,0;FLOAT;0.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;0;0;3;0
WireConnection;0;1;6;0
WireConnection;0;3;4;0
WireConnection;0;4;5;0
WireConnection;0;6;7;0
ASEEND*/
//CHKSM=53A5E429E7C95B0726DD5A4A9AA0BB20FA280758

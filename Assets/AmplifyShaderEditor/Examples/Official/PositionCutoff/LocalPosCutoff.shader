// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/LocalPosCutoff"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Distribution("Distribution", Range( 0.1 , 10)) = 1
		_SmoothnessFactor("SmoothnessFactor", Range( 0 , 1)) = 0
		_StartPoint("StartPoint", Range( -10 , 10)) = 0.75
		_UnderwaterInfluence("UnderwaterInfluence", Range( 0 , 1)) = 0
		_Tint("Tint", Color) = (0.5294118,0.4264289,0,0)
		_Normals("Normals", 2D) = "bump" {}
		_Albedo("Albedo", 2D) = "white" {}
		_Occlusion("Occlusion", 2D) = "white" {}
		_Metallic("Metallic", 2D) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_Normals;
			float2 uv_Albedo;
			float3 localVertexPos;
			float2 uv_Metallic;
			float2 uv_Occlusion;
		};

		uniform sampler2D _Normals;
		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform float _StartPoint;
		uniform float _Distribution;
		uniform float _UnderwaterInfluence;
		uniform sampler2D _Metallic;
		uniform float _SmoothnessFactor;
		uniform sampler2D _Occlusion;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.localVertexPos = vertexData.vertex.xyz ;
		}

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Normal = UnpackNormal( tex2D( _Normals,input.uv_Normals) );
			float temp_output_15_0 = saturate( ( ( input.localVertexPos.y + _StartPoint ) / _Distribution ) );
			output.Albedo = lerp( _Tint , tex2D( _Albedo,input.uv_Albedo) , clamp( temp_output_15_0 , _UnderwaterInfluence , 1.0 ) ).xyz;
			float4 temp_cast_1 = ( 1.0 - temp_output_15_0 );
			float4 temp_output_49_0 = ( tex2D( _Metallic,input.uv_Metallic) + temp_cast_1 );
			output.Metallic = temp_output_49_0.x;
			float4 temp_cast_3 = ( 1.0 - temp_output_15_0 );
			output.Smoothness = ( temp_output_49_0 * _SmoothnessFactor ).x;
			output.Occlusion = tex2D( _Occlusion,input.uv_Occlusion).x;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;1164.707;409.5154;1.3;True;False
Node;AmplifyShaderEditor.CommentaryNode;27;-1268.119,489.6995;Float;1032.899;469.7996;Cutoff;9;15;2;16;5;14;30;54;47;50
Node;AmplifyShaderEditor.CommentaryNode;21;-914.6158,-533.7644;Float;719.1993;440.2003;Color Stuff;4;20;19;18;17
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;432.1003,45.80003;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/LocalPosCutoff;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleDivideOpNode;16;-876.0185,742.7026;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1254.918,841.5022;Float;Property;_Distribution;Distribution;0;1;0.1;10
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;182.0511,215.8345;Float;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;34;-108.9506,481.5498;Float;Property;_SmoothnessFactor;SmoothnessFactor;1;0;0;1
Node;AmplifyShaderEditor.LerpOp;52;-233.6482,-23.06393;Float;COLOR;0.0,0,0,0;FLOAT4;0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.ClampOpNode;30;-492.2693,530.6258;Float;FLOAT;0.0;FLOAT;0.0;FLOAT;1.0
Node;AmplifyShaderEditor.SimpleAddOpNode;49;0.6511464,115.0344;Float;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.LocalVertexPosNode;47;-1234.361,543.9348;Float
Node;AmplifyShaderEditor.SimpleAddOpNode;14;-997.0165,635.501;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SaturateNode;15;-712.8203,722.0024;Float;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;2;-1247.918,722.3019;Float;Property;_StartPoint;StartPoint;2;0.75;-10;10
Node;AmplifyShaderEditor.RangedFloatNode;54;-851.6987,535.1356;Float;Property;_UnderwaterInfluence;UnderwaterInfluence;3;0;0;1
Node;AmplifyShaderEditor.OneMinusNode;50;-422.1988,688.3354;Float;FLOAT;0.0
Node;AmplifyShaderEditor.ColorNode;53;-519.748,-93.96416;Float;Property;_Tint;Tint;4;0.5294118,0.4264289,0,0
Node;AmplifyShaderEditor.WorldPosInputsNode;59;-1229.238,316.9132;Float
Node;AmplifyShaderEditor.SamplerNode;19;-864.6157,-466.2642;Float;Property;_Normals;Normals;5;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;17;-512.4171,-483.7644;Float;Property;_Albedo;Albedo;6;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;20;-838.4163,-314.6636;Float;Property;_Occlusion;Occlusion;7;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;18;-526.317,-278.5635;Float;Property;_Metallic;Metallic;8;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;0;0;52;0
WireConnection;0;1;19;0
WireConnection;0;3;49;0
WireConnection;0;4;51;0
WireConnection;0;5;20;0
WireConnection;16;0;14;0
WireConnection;16;1;5;0
WireConnection;51;0;49;0
WireConnection;51;1;34;0
WireConnection;52;0;53;0
WireConnection;52;1;17;0
WireConnection;52;2;30;0
WireConnection;30;0;15;0
WireConnection;30;1;54;0
WireConnection;49;0;18;0
WireConnection;49;1;50;0
WireConnection;14;0;47;2
WireConnection;14;1;2;0
WireConnection;15;0;16;0
WireConnection;50;0;15;0
ASEEND*/
//CHKSM=0F564558C98BDEFDA6600BCCDE179EBEC4EA9A09

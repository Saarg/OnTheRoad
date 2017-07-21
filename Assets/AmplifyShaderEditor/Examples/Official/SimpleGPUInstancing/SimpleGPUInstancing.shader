// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SimpleGPUInstancing"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		[HideInInspector] __dirty( "", Int ) = 1
		_Checkers("Checkers", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Checkers;
		uniform float4 _Checkers_ST;

		UNITY_INSTANCING_CBUFFER_START(ASESampleShadersSimpleGPUInstancing)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
		UNITY_INSTANCING_CBUFFER_END

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Checkers = i.uv_texcoord * _Checkers_ST.xy + _Checkers_ST.zw;
			o.Albedo = ( tex2D( _Checkers,uv_Checkers) * UNITY_ACCESS_INSTANCED_PROP(_Color) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=6102
251;92;854;650;770.5;199.5;1;True;False
Node;AmplifyShaderEditor.SamplerNode;1;-518.5,-163.5;Float;True;Property;_Checkers;Checkers;-1;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;2;-464.5,65.5;Float;False;InstancedProperty;_Color;Color;-1;0;1,1,1,1;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-153.5,-50.5;Float;False;0;FLOAT4;0.0,0,0,0;False;1;COLOR;0.0,0,0,0;False;COLOR
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;23,-97;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;ASESampleShaders/SimpleGPUInstancing;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0.0;False;7;FLOAT3;0.0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0.0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;0;0;3;0
ASEEND*/
//CHKSM=E7C51AC20EB5FCB99B9213C7379F683AF8434486
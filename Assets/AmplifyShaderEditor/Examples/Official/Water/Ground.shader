// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/LocalGradient"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Top("Top", Color) = (0,0,0,0)
		_Bottom("Bottom", Color) = (0,0,0,0)
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
			float3 localVertexPos;
		};

		uniform fixed4 _Bottom;
		uniform fixed4 _Top;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.localVertexPos = vertexData.vertex.xyz ;
		}

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Albedo = lerp( _Bottom , _Top , saturate( input.localVertexPos.z ) ).rgb;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;1515.199;641.1003;2;True;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Fixed;True;2;Fixed;ASEMaterialInspector;Standard;ASESampleShaders/LocalGradient;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0.0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.LerpOp;3;-220.5998,44.29988;Float;COLOR;0.0,0,0,0;COLOR;0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.ColorNode;4;-542.8,-201.1001;Float;Property;_Bottom;Bottom;-1;0,0,0,0
Node;AmplifyShaderEditor.ColorNode;1;-533.7999,-30.2;Float;Property;_Top;Top;-1;0,0,0,0
Node;AmplifyShaderEditor.LocalVertexPosNode;2;-550.1002,167.7;Float
Node;AmplifyShaderEditor.SaturateNode;11;-369.3995,167.6996;Float;FLOAT;0.0
WireConnection;0;0;3;0
WireConnection;3;0;4;0
WireConnection;3;1;1;0
WireConnection;3;2;11;0
WireConnection;11;0;2;3
ASEEND*/
//CHKSM=944F606AE34C3D70E84C2A0E3E530D5227D8E0BB

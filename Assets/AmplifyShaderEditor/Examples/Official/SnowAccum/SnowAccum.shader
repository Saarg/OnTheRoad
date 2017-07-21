// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/SnowAccum"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_RockAlbedo("Rock Albedo", 2D) = "white" {}
		_RockNormal("Rock Normal", 2D) = "bump" {}
		_RockSpecular("Rock Specular", 2D) = "white" {}
		_SnowAlbedo("Snow Albedo", 2D) = "white" {}
		_SnowNormal("Snow Normal", 2D) = "bump" {}
		_SnowSpecular("Snow Specular", 2D) = "white" {}
		_SnowAmount("SnowAmount", Range( 0 , 2)) = 0.13
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_RockNormal;
			float2 uv_SnowNormal;
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_RockAlbedo;
			float2 uv_SnowAlbedo;
			float2 uv_RockSpecular;
			float2 uv_SnowSpecular;
		};

		uniform sampler2D _RockNormal;
		uniform sampler2D _SnowNormal;
		uniform float _SnowAmount;
		uniform sampler2D _RockAlbedo;
		uniform sampler2D _SnowAlbedo;
		uniform sampler2D _RockSpecular;
		uniform sampler2D _SnowSpecular;

		void surf( Input input , inout SurfaceOutputStandardSpecular output )
		{
			output.Normal = lerp( UnpackNormal( tex2D( _RockNormal,input.uv_RockNormal) ) , UnpackNormal( tex2D( _SnowNormal,input.uv_SnowNormal) ) , saturate( ( WorldNormalVector( input , float3( 0,0,1 )).y * _SnowAmount ) ) );
			output.Albedo = lerp( tex2D( _RockAlbedo,input.uv_RockAlbedo) , tex2D( _SnowAlbedo,input.uv_SnowAlbedo) , saturate( ( WorldNormalVector( input , lerp( UnpackNormal( tex2D( _RockNormal,input.uv_RockNormal) ) , UnpackNormal( tex2D( _SnowNormal,input.uv_SnowNormal) ) , saturate( ( WorldNormalVector( input , float3( 0,0,1 )).y * _SnowAmount ) ) ) ).y * _SnowAmount ) ) ).xyz;
			output.Specular = lerp( tex2D( _RockSpecular,input.uv_RockSpecular) , tex2D( _SnowSpecular,input.uv_SnowSpecular) , saturate( ( WorldNormalVector( input , lerp( UnpackNormal( tex2D( _RockNormal,input.uv_RockNormal) ) , UnpackNormal( tex2D( _SnowNormal,input.uv_SnowNormal) ) , saturate( ( WorldNormalVector( input , float3( 0,0,1 )).y * _SnowAmount ) ) ) ).y * _SnowAmount ) ) ).xyz;
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;2105.999;660.7953;1.9;True;False
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;146.9001,42.89999;Float;True;2;Float;ASEMaterialInspector;StandardSpecular;ASESampleShaders/SnowAccum;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1773.599,-115.1001;Float;Property;_SnowAmount;SnowAmount;6;0.13;0;2
Node;AmplifyShaderEditor.LerpOp;10;-127.1,-308.6001;Float;FLOAT4;0.0,0,0,0;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.WorldNormalInputsNode;20;-1685.906,-319.7951;Float;True
Node;AmplifyShaderEditor.LerpOp;15;-1067.3,73.59992;Float;FLOAT3;0.0,0,0;FLOAT3;0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1444.505,-272.8951;Float;FLOAT;0.0;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;22;-1283.806,-264.4951;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WorldNormalVector;19;-865.5047,-194.2967;Float;FLOAT3;0,0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-651.7996,-116.3;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SaturateNode;18;-496.7049,-110.1974;Float;FLOAT;0.0
Node;AmplifyShaderEditor.LerpOp;17;-223.4019,317.8009;Float;FLOAT4;0.0,0,0,0;FLOAT4;0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SamplerNode;16;-704.8019,470.7009;Float;Property;_SnowSpecular;Snow Specular;5;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;2;-693.0005,253.9001;Float;Property;_RockSpecular;Rock Specular;2;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;9;-824.2003,-500.8003;Float;Property;_SnowAlbedo;Snow Albedo;3;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;1;-811.4,-722.1001;Float;Property;_RockAlbedo;Rock Albedo;0;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;4;-1548.101,72.7999;Float;Property;_RockNormal;Rock Normal;1;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;14;-1541.701,238.3998;Float;Property;_SnowNormal;Snow Normal;4;None;True;0;True;bump;Auto;True;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
WireConnection;0;0;10;0
WireConnection;0;1;15;0
WireConnection;0;3;17;0
WireConnection;10;0;1;0
WireConnection;10;1;9;0
WireConnection;10;2;18;0
WireConnection;15;0;4;0
WireConnection;15;1;14;0
WireConnection;15;2;22;0
WireConnection;21;0;20;2
WireConnection;21;1;12;0
WireConnection;22;0;21;0
WireConnection;19;0;15;0
WireConnection;11;0;19;2
WireConnection;11;1;12;0
WireConnection;18;0;11;0
WireConnection;17;0;2;0
WireConnection;17;1;16;0
WireConnection;17;2;18;0
ASEEND*/
//CHKSM=EE72F9BAFCCFAD299777CDB34FA337924A597040

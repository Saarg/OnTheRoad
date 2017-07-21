// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "defaultMat"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Burnt_normals("Burnt_normals", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Burnt_normals;
		uniform float4 _Burnt_normals_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Burnt_normals = i.uv_texcoord * _Burnt_normals_ST.xy + _Burnt_normals_ST.zw;
			o.Normal = ( tex2D( _Burnt_normals, uv_Burnt_normals ) * 0.2 ).xyz;
			o.Metallic = 0.0;
			float temp_output_2_0 = 0.2;
			o.Smoothness = temp_output_2_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
960;114;960;1034;577;268;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;2;-228,279;Float;False;Constant;_Float0;Float 0;1;0;0.2;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-409,-48;Float;True;Property;_Burnt_normals;Burnt_normals;0;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Misc/Burnt_normals.tga;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;0.0;False;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;3;-227,201;Float;False;Constant;_Float1;Float 1;1;0;0;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-96,-17;Float;False;2;0;FLOAT4;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;81,-19;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;defaultMat;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;0
WireConnection;4;1;2;0
WireConnection;0;1;4;0
WireConnection;0;3;3;0
WireConnection;0;4;2;0
ASEEND*/
//CHKSM=4141510B5BF14D430E6C3C24116F0865B54D64EE
// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/ReadFromAtlasTiled"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Min("Min", Vector) = (0,0,0,0)
		_Max("Max", Vector) = (0,0,0,0)
		_Atlas("Atlas", 2D) = "white" {}
		_TileSize("TileSize", Vector) = (2,2,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float2 _Max;
		uniform float4 _Atlas_TexelSize;
		uniform float2 _Min;
		uniform float4 _Atlas_ST;
		uniform float2 _TileSize;
		uniform sampler2D _Atlas;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult21 = float2( _Atlas_TexelSize.x , _Atlas_TexelSize.y );
			float2 ScaledMax = ( _Max * appendResult21 );
			float2 ScaledMin = ( _Min * appendResult21 );
			float2 Size = ( ScaledMax - ScaledMin );
			float2 uv_Atlas = i.uv_texcoord * _Atlas_ST.xy + _Atlas_ST.zw;
			float2 TiledVar = ( fmod( uv_Atlas , ( float2( 1,1 ) / _TileSize ) ) * Size );
			o.Emission = tex2D( _Atlas,( ScaledMin + ( TiledVar * _TileSize ) )).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5002
339;92;1190;648;1095.558;538.1483;1.6;False;False
Node;AmplifyShaderEditor.Vector2Node;13;-1241.596,248.2998;Float;Property;_Min;Min;0;0;0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-976.4965,291.9005;Float;ScaledMin;-1;False;FLOAT2;0.0,0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;-789.0002,171.3005;Float;FLOAT2;0.0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.TexelSizeNode;19;-1576.995,257.2996;Float;30
Node;AmplifyShaderEditor.AppendNode;21;-1420.795,302.0998;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1115.595,361.0998;Float;FLOAT2;0.0,0;FLOAT2;0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-664.9017,127.0011;Float;Size;0;False;FLOAT2;0.0,0
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1025.2,-266.6001;Float;0;30;2;FLOAT2;2,2;FLOAT2;0,0
Node;AmplifyShaderEditor.SimpleDivideOpNode;54;-866.0472,-100.3501;Float;FLOAT2;0.0,0;FLOAT2;0.0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1222.595,86.69989;Float;FLOAT2;0.0,0;FLOAT2;0.0,0
Node;AmplifyShaderEditor.Vector2Node;14;-1399.996,27.2001;Float;Property;_Max;Max;1;0;0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-1026.897,141.3005;Float;ScaledMax;-1;False;FLOAT2;0.0,0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;251.5002,-363.0998;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/ReadFromAtlasTiled;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-402.9997,-218.998;Float;27
Node;AmplifyShaderEditor.SamplerNode;30;-58.50118,-225.9996;Float;Property;_Atlas;Atlas;3;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-191.198,-148.7983;Float;FLOAT2;0.0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-534.299,-93.79826;Float;FLOAT2;0.0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-224.9486,51.95014;Float;FLOAT2;0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;-381.1538,-36.34888;Float;TiledVar;1;False;FLOAT2;0.0,0
Node;AmplifyShaderEditor.Vector2Node;50;-1123.448,-14.44986;Float;Property;_TileSize;TileSize;3;0;2,2
Node;AmplifyShaderEditor.Vector2Node;56;-1137.555,-154.6484;Float;Constant;_Vector0;Vector 0;4;0;1,1
Node;AmplifyShaderEditor.FmodOpNode;49;-683.4481,-184.4499;Float;FLOAT2;0.0,0;FLOAT2;0.0,0
WireConnection;27;0;22;0
WireConnection;38;0;26;0
WireConnection;38;1;27;0
WireConnection;21;0;19;1
WireConnection;21;1;19;2
WireConnection;22;0;13;0
WireConnection;22;1;21;0
WireConnection;39;0;38;0
WireConnection;54;0;56;0
WireConnection;54;1;50;0
WireConnection;23;0;14;0
WireConnection;23;1;21;0
WireConnection;26;0;23;0
WireConnection;0;2;30;0
WireConnection;30;1;40;0
WireConnection;40;0;41;0
WireConnection;40;1;53;0
WireConnection;42;0;49;0
WireConnection;42;1;39;0
WireConnection;53;0;55;0
WireConnection;53;1;50;0
WireConnection;55;0;42;0
WireConnection;49;0;12;0
WireConnection;49;1;54;0
ASEEND*/
//CHKSM=E632CBE29F0D81F9601AFE291E2EE588F8F4F7CA
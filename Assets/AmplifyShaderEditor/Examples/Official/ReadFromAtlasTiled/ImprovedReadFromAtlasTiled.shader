// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/ImprovedReadFromAtlasTiled"
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

		uniform float2 _Min;
		uniform float4 _Atlas_TexelSize;
		uniform float2 _Max;
		uniform float2 _TileSize;
		uniform float4 _Atlas_ST;
		uniform sampler2D _Atlas;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult21 = float2( _Atlas_TexelSize.x , _Atlas_TexelSize.y );
			float2 ScaledMin = ( _Min * appendResult21 );
			float2 ScaledMax = ( _Max * appendResult21 );
			float2 invTileSize = ( float2( 1,1 ) / _TileSize );
			float2 Size = ( ScaledMax - ScaledMin );
			float2 uv_Atlas = i.uv_texcoord * _Atlas_ST.xy + _Atlas_ST.zw;
			float2 A = uv_Atlas;
			float2 B = invTileSize;
			float2 TiledVar = ( frac(A/B)*B * Size );
			float2 finalUV = ( ScaledMin + ( TiledVar * _TileSize ) );
			o.Emission = tex2Dlod( _Atlas,float4( finalUV, 0, 0.0)).xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=5002
339;92;1190;648;207.5582;508.8461;1;True;False
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;-789.0002,171.3005;Float;FLOAT2;0.0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.TexelSizeNode;19;-1576.995,257.2996;Float;30
Node;AmplifyShaderEditor.AppendNode;21;-1420.795,302.0998;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;-1115.595,361.0998;Float;FLOAT2;0.0,0;FLOAT2;0,0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-1222.595,86.69989;Float;FLOAT2;0.0,0;FLOAT2;0.0,0
Node;AmplifyShaderEditor.Vector2Node;14;-1399.996,27.2001;Float;Property;_Max;Max;1;0;0,0
Node;AmplifyShaderEditor.TextureCoordinatesNode;12;-1040.2,-383.6001;Float;0;30;2;FLOAT2;2,2;FLOAT2;0,0
Node;AmplifyShaderEditor.RangedFloatNode;58;-155.7451,163.2524;Float;Constant;_Float0;Float 0;4;0;0;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-664.9017,127.0011;Float;Size;0;True;FLOAT2;0.0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-979.0965,291.9005;Float;ScaledMin;-1;True;FLOAT2;0.0,0
Node;AmplifyShaderEditor.SimpleDivideOpNode;54;-993.446,-178.3502;Float;FLOAT2;0.0,0;FLOAT2;0.0,0
Node;AmplifyShaderEditor.Vector2Node;56;-1146.655,-243.0486;Float;Constant;_Vector0;Vector 0;4;0;1,1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-549.8989,-113.2983;Float;FLOAT2;0.0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.SimpleAddOpNode;40;-226.2981,-166.9983;Float;FLOAT2;0.0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;546.5997,-353.9999;Float;True;2;Float;ASEMaterialInspector;Standard;ASESampleShaders/ImprovedReadFromAtlasTiled;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;OBJECT;0.0;FLOAT3;0.0,0,0;FLOAT3;0.0,0,0;OBJECT;0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;44.74492,-287.3459;Float;finalUV;2;True;FLOAT2;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;53;-236.6483,29.85015;Float;FLOAT2;0,0;FLOAT2;0.0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-465.3993,-270.9979;Float;27
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;-442.2534,-71.44888;Float;TiledVar;1;True;FLOAT2;0.0,0
Node;AmplifyShaderEditor.CustomExpressionNode;61;-750.8585,-416.0444;Float;frac(A/B)*B;2;2;True;A;FLOAT2;0,0;True;B;FLOAT2;0,0;FLOAT2;0,0;FLOAT2;0,0
Node;AmplifyShaderEditor.SamplerNode;30;232.9987,-154.6996;Float;Property;_Atlas;Atlas;3;0;None;True;0;False;white;Auto;False;Object;-1;MipLevel;Texture2D;SAMPLER2D;;FLOAT2;0,0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-1026.897,141.3005;Float;ScaledMax;-1;True;FLOAT2;0.0,0
Node;AmplifyShaderEditor.Vector2Node;13;-1233.596,213.2998;Float;Property;_Min;Min;0;0;0,0
Node;AmplifyShaderEditor.Vector2Node;50;-1149.448,-45.44986;Float;Property;_TileSize;TileSize;3;0;2,2
Node;AmplifyShaderEditor.RegisterLocalVarNode;59;-852.9485,-65.94765;Float;invTileSize;-1;True;FLOAT2;0.0
WireConnection;38;0;26;0
WireConnection;38;1;27;0
WireConnection;21;0;19;1
WireConnection;21;1;19;2
WireConnection;22;0;13;0
WireConnection;22;1;21;0
WireConnection;23;0;14;0
WireConnection;23;1;21;0
WireConnection;39;0;38;0
WireConnection;27;0;22;0
WireConnection;54;0;56;0
WireConnection;54;1;50;0
WireConnection;42;0;61;0
WireConnection;42;1;39;0
WireConnection;40;0;41;0
WireConnection;40;1;53;0
WireConnection;0;2;30;0
WireConnection;60;0;40;0
WireConnection;53;0;55;0
WireConnection;53;1;50;0
WireConnection;55;0;42;0
WireConnection;61;0;12;0
WireConnection;61;1;59;0
WireConnection;30;1;60;0
WireConnection;30;2;58;0
WireConnection;26;0;23;0
WireConnection;59;0;54;0
ASEEND*/
//CHKSM=EA4D5A26ADAFEE9F10675C6B458F7958DC5BFE42
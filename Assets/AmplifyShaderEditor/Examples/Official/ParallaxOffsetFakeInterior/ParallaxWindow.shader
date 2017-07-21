// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UserSamples/ParallaxWindow"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		_Back("Back", 2D) = "white" {}
		_BackDark("Back Dark", Float) = 0.7
		_BackDepthScale("Back Depth Scale", Range( 0 , 1)) = 0
		_Mid("Mid", 2D) = "white" {}
		_MidDark("Mid Dark", Float) = 0.3
		_MidDepthScale("Mid Depth Scale", Range( 0 , 1)) = 0.3
		_Front("Front", 2D) = "white" {}
		_Mask("Mask", 2D) = "white" {}
		_Specular("Specular", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#pragma target 2.5
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 texcoord_0;
			float3 viewDir;
		};

		uniform fixed _BackDepthScale;
		uniform fixed _MidDepthScale;
		uniform sampler2D _Back;
		uniform fixed _BackDark;
		uniform sampler2D _Mid;
		uniform fixed _MidDark;
		uniform sampler2D _Mask;
		uniform sampler2D _Front;
		uniform fixed _Specular;
		uniform fixed _Smoothness;

		void vertexDataFunc( inout appdata_full vertexData, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.texcoord_0.xy = vertexData.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
		}

		void surf( Input input , inout SurfaceOutputStandardSpecular output )
		{
			float2 Offset75 = ( ( 0.0 - 1.0 ) * input.viewDir.xy * _BackDepthScale ) + input.texcoord_0;
			fixed2 OffsetBack = Offset75;
			float2 Offset76 = ( ( 0.0 - 1.0 ) * input.viewDir.xy * _MidDepthScale ) + input.texcoord_0;
			fixed2 OffsetMid = Offset76;
			output.Normal = fixed3(0,0,1);
			fixed4 tex2DNode62 = tex2D( _Mask,input.texcoord_0);
			output.Albedo = lerp( lerp( ( tex2D( _Back,OffsetBack) * _BackDark ) , ( tex2D( _Mid,OffsetMid) * _MidDark ) , tex2D( _Mask,OffsetMid).g ) , tex2D( _Front,input.texcoord_0) , tex2DNode62.r ).xyz;
			float temp_output_87_0 = ( 1.0 - tex2DNode62.r );
			fixed3 temp_cast_4 = ( temp_output_87_0 * _Specular );
			output.Specular = temp_cast_4;
			output.Smoothness = ( temp_output_87_0 * _Smoothness );
			output.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3001
393;92;1091;695;1697.759;-31.65363;1;True;False
Node;AmplifyShaderEditor.LerpOp;48;316.0967,430.2044;Float;FLOAT4;0.0,0,0,0;FLOAT4;0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;112.1981,277.5027;Float;FLOAT4;0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;115.598,454.4031;Float;FLOAT4;0.0,0,0,0;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-326.5037,181.3044;Float;Property;_Back;Back;1;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;46;-323.2037,449.3041;Float;Property;_Mid;Mid;4;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.Vector3Node;50;703.1962,561.9033;Float;Constant;_Vector0;Vector 0;-1;0,0,1
Node;AmplifyShaderEditor.RangedFloatNode;49;-1354.041,412.4043;Float;Property;_MidDepthScale;Mid Depth Scale;6;0.3;0;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-619.4551,390.4736;Float;OffsetMid;2;FLOAT2;0.0,0
Node;AmplifyShaderEditor.RegisterLocalVarNode;70;-627.5119,138.6462;Float;OffsetBack;1;FLOAT2;0.0,0
Node;AmplifyShaderEditor.RangedFloatNode;74;383.9784,1165.016;Float;Property;_Smoothness;Smoothness;10;0;0;1
Node;AmplifyShaderEditor.RangedFloatNode;54;-1339.354,167.9353;Float;Property;_BackDepthScale;Back Depth Scale;3;0;0;1
Node;AmplifyShaderEditor.WireNode;85;-1416.174,174.5851;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.TextureCoordinatesNode;9;-1716.362,402.5095;Float;0;-1;FLOAT2;1,1;FLOAT2;0,0
Node;AmplifyShaderEditor.WireNode;86;-1398.606,375.6921;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.RangedFloatNode;61;-163.8694,364.756;Float;Property;_BackDark;Back Dark;2;0.7;0;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-166.3069,629.3103;Float;Property;_MidDark;Mid Dark;5;0.3;0;0
Node;AmplifyShaderEditor.SamplerNode;47;-318.1801,725.5137;Float;Property;_Mask;Mask;8;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;57;-329.4955,980.0483;Float;Property;_Front;Front;7;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;62;-343.6602,1229.647;Float;Property;_TextureSample0;Texture Sample 0;-1;None;True;0;False;white;Auto;False;Instance;47;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;88;711.759,725.038;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;89;720.4633,883.5364;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.RangedFloatNode;73;376.3207,972.8581;Float;Property;_Specular;Specular;9;0;0;1
Node;AmplifyShaderEditor.WireNode;92;-1331.782,534.7811;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.WireNode;94;-656.5728,703.5836;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.WireNode;93;-607.8077,594.7998;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.WireNode;98;-7.037296,568.9813;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;96;78.49758,648.8132;Float;FLOAT;0.0
Node;AmplifyShaderEditor.OneMinusNode;87;411.4357,739.0054;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;80;220.0073,788.4324;Float;FLOAT;0.0
Node;AmplifyShaderEditor.WireNode;101;175.4373,700.1351;Float;FLOAT4;0.0,0,0,0
Node;AmplifyShaderEditor.ParallaxMappingNode;75;-953.2018,119.736;Float;Normal;FLOAT2;0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.ParallaxMappingNode;76;-946.2023,332.5014;Float;Normal;FLOAT2;0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.LerpOp;58;689.1907,430.6974;Float;FLOAT4;0.0,0,0,0;FLOAT4;0.0,0,0,0;FLOAT;0.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1026.3,428.7931;Fixed;True;1;Fixed;ASEMaterialInspector;StandardSpecular;UserSamples/ParallaxWindow;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0,0,0;OBJECT;0.0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.WireNode;95;-1362.383,609.8043;Float;FLOAT2;0.0,0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;51;-1104.096,239.0735;Float;Tangent
WireConnection;48;0;59;0
WireConnection;48;1;63;0
WireConnection;48;2;96;0
WireConnection;59;0;45;0
WireConnection;59;1;61;0
WireConnection;63;0;46;0
WireConnection;63;1;98;0
WireConnection;45;1;70;0
WireConnection;46;1;15;0
WireConnection;15;0;76;0
WireConnection;70;0;75;0
WireConnection;85;0;9;0
WireConnection;86;0;9;0
WireConnection;47;1;15;0
WireConnection;57;1;93;0
WireConnection;62;1;94;0
WireConnection;88;0;87;0
WireConnection;88;1;73;0
WireConnection;89;0;87;0
WireConnection;89;1;74;0
WireConnection;92;0;9;0
WireConnection;94;0;95;0
WireConnection;93;0;92;0
WireConnection;98;0;64;0
WireConnection;96;0;47;2
WireConnection;87;0;62;1
WireConnection;80;0;62;1
WireConnection;101;0;57;0
WireConnection;75;0;85;0
WireConnection;75;2;54;0
WireConnection;75;3;51;0
WireConnection;76;0;86;0
WireConnection;76;2;49;0
WireConnection;76;3;51;0
WireConnection;58;0;48;0
WireConnection;58;1;101;0
WireConnection;58;2;80;0
WireConnection;0;0;58;0
WireConnection;0;1;50;0
WireConnection;0;3;88;0
WireConnection;0;4;89;0
WireConnection;95;0;9;0
ASEEND*/
//CHKSM=3F777A2F30127F40A1DD86EA009FDF9EB9183A70
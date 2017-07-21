// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UserSamples/DissolveBurn"
{
	Properties
	{
		_MaskClipValue( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] __dirty( "", Int ) = 1
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_DisolveGuide("Disolve Guide", 2D) = "white" {}
		_BurnRamp("Burn Ramp", 2D) = "white" {}
		_DissolveAmount("Dissolve Amount", Range( 0 , 1)) = 0
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		ZTest LEqual
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_Normal;
			float2 uv_Albedo;
			float2 uv_DisolveGuide;
		};

		uniform sampler2D _Normal;
		uniform sampler2D _Albedo;
		uniform float _DissolveAmount;
		uniform sampler2D _DisolveGuide;
		uniform sampler2D _BurnRamp;
		uniform float _MaskClipValue = 0.5;

		void surf( Input input , inout SurfaceOutputStandard output )
		{
			output.Normal = tex2D( _Normal,input.uv_Normal).xyz;
			output.Albedo = tex2D( _Albedo,input.uv_Albedo).xyz;
			float temp_output_73_0 = ( (-0.6 + (( 1.0 - _DissolveAmount ) - 0.0) * (0.6 - -0.6) / (1.0 - 0.0)) + tex2D( _DisolveGuide,input.uv_DisolveGuide).r );
			float temp_output_130_0 = ( 1.0 - clamp( (-4.0 + (temp_output_73_0 - 0.0) * (4.0 - -4.0) / (1.0 - 0.0)) , 0.0 , 1.0 ) );
			output.Emission = ( temp_output_130_0 * tex2D( _BurnRamp,float2( temp_output_130_0 , 0 )) ).xyz;
			clip( temp_output_73_0 - _MaskClipValue );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=3002
288;92;1035;737;1264.822;-382.2362;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;132;144.1929,26.72195;Float;765.1592;493.9802;Created by The Four Headed Cat @fourheadedcat - www.twitter.com/fourheadedcat;1;0
Node;AmplifyShaderEditor.CommentaryNode;129;-892.9326,49.09825;Float;814.5701;432.0292;Burn Effect - Emission;6;113;126;115;114;112;130
Node;AmplifyShaderEditor.CommentaryNode;128;-967.3727,510.0833;Float;908.2314;498.3652;Dissolve - Opacity Mask;5;4;71;2;73;111
Node;AmplifyShaderEditor.OneMinusNode;71;-655.2471,583.1434;Float;FLOAT;0.0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-202.3633,125.7657;Float;FLOAT;0.0;FLOAT4;0,0,0,0
Node;AmplifyShaderEditor.SamplerNode;2;-557.5587,798.9492;Float;Property;_DisolveGuide;Disolve Guide;2;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.SamplerNode;78;-323.0742,-278.0451;Float;Property;_Albedo;Albedo;0;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.ClampOpNode;113;-797.634,90.31517;Float;FLOAT;0.0;FLOAT;0.0;FLOAT;1.0
Node;AmplifyShaderEditor.OneMinusNode;130;-627.5982,83.10277;Float;FLOAT;0.0
Node;AmplifyShaderEditor.AppendNode;115;-549.438,307.1016;Float;FLOAT2;0;0;0;0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SamplerNode;114;-422.1431,295.0128;Float;Property;_BurnRamp;Burn Ramp;3;None;True;0;False;white;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.TFHCRemap;111;-526.4305,583.9279;Float;FLOAT;0.0;FLOAT;0.0;FLOAT;1.0;FLOAT;-0.6;FLOAT;0.6
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-319.6845,566.4299;Float;FLOAT;0.0;FLOAT;0.0
Node;AmplifyShaderEditor.SamplerNode;131;-325.7014,-110.6792;Float;Property;_Normal;Normal;1;None;True;0;True;bump;Auto;False;Object;-1;Auto;SAMPLER2D;0,0;FLOAT2;1.0;FLOAT;1.0;FLOAT2;0,0;FLOAT2;0,0;FLOAT;1.0
Node;AmplifyShaderEditor.TFHCRemap;112;-878.1525,280.8961;Float;FLOAT;0.0;FLOAT;0.0;FLOAT;1.0;FLOAT;-4.0;FLOAT;4.0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;435.9929,109.222;Float;True;2;Float;ASEMaterialInspector;Standard;UserSamples/DissolveBurn;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;3;False;0;0;Masked;0.5;False;True;0;False;TransparentCutout;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT3;0,0,0;FLOAT;0.0;FLOAT;0.0;FLOAT;0.0;FLOAT3;0.0;FLOAT3;0.0;FLOAT;0.0;OBJECT;0.0;OBJECT;0.0;OBJECT;0,0,0;OBJECT;0.0;FLOAT4;0,0,0,0;FLOAT3;0,0,0
Node;AmplifyShaderEditor.RangedFloatNode;4;-919.0424,582.2975;Float;Property;_DissolveAmount;Dissolve Amount;4;0;0;1
WireConnection;71;0;4;0
WireConnection;126;0;130;0
WireConnection;126;1;114;0
WireConnection;113;0;112;0
WireConnection;130;0;113;0
WireConnection;115;0;130;0
WireConnection;114;1;115;0
WireConnection;111;0;71;0
WireConnection;73;0;111;0
WireConnection;73;1;2;1
WireConnection;112;0;73;0
WireConnection;0;0;78;0
WireConnection;0;1;131;0
WireConnection;0;2;126;0
WireConnection;0;9;73;0
ASEEND*/
//CHKSM=FEF79207B68709647908AA3B0C7625AACCB07DCC

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New AmplifyShader"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ "GrabScreen0" }
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float4 screenPos;
		};

		uniform sampler2D GrabScreen0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = float4(0.1005623,0.1646909,0.2279412,0).rgb;
			float4 screenPos3 = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
			float scale3 = -1.0;
			#else
			float scale3 = 1.0;
			#endif
			float halfPosW3 = screenPos3.w * 0.5;
			screenPos3.y = ( screenPos3.y - halfPosW3 ) * _ProjectionParams.x* scale3 + halfPosW3;
			screenPos3.w += 0.00000000001;
			screenPos3.xyzw /= screenPos3.w;
			float4 temp_cast_1 = (1.0).xxxx;
			o.Emission = lerp( tex2Dproj( GrabScreen0, UNITY_PROJ_COORD( screenPos3 ) ) , temp_cast_1 , 0.23085 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=10001
960;114;960;1034;728.9998;191.9;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;5;-440,326.2;Float;False;Constant;_Float0;Float 0;0;0;1;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;3;-439,154.2;Float;False;Global;GrabScreen0;Grab Screen 0;0;0;Object;-1;1;0;FLOAT4;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;6;-439,404.2;Float;False;Constant;_Blending;Blending;0;0;0.23085;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.LerpOp;4;-196,222.2;Float;False;3;0;COLOR;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;2;FLOAT;0.0;False;1;COLOR
Node;AmplifyShaderEditor.ColorNode;1;-452,-33;Float;False;Constant;_Color0;Color 0;0;0;0.1005623,0.1646909,0.2279412,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;Standard;New AmplifyShader;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Transparent;0.5;True;True;0;False;Transparent;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;Relative;0;;-1;-1;-1;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;3;0
WireConnection;4;1;5;0
WireConnection;4;2;6;0
WireConnection;0;0;1;0
WireConnection;0;2;4;0
ASEEND*/
//CHKSM=7400E149C39FA95332F8EC4F947CECC4A6090550
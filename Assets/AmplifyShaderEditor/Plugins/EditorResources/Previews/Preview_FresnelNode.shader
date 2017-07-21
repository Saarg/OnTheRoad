Shader "Hidden/FresnelNode"
{
	Properties
	{
		_A ("_Normal", 2D) = "white" {}
		_B ("_Bias", 2D) = "white" {}
		_C ("_Scale", 2D) = "white" {}
		_D ("_Power", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _A;
			sampler2D _B;
			sampler2D _C;
			sampler2D _D;
			sampler2D _E;

			float _ProjectInLinear;
			float _Connected;

			float4 frag(v2f_img i) : SV_Target
			{
				float4 n = tex2D( _A, i.uv );
				float b = tex2D( _B, i.uv ).r;
				float s = tex2D( _C, i.uv ).r;
				float pw = tex2D( _D, i.uv ).r;

				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );

				float3 viewDir = 0;
				float3 normal = 0;
				//float a = 0;
				r = saturate( r );
				//if ( r < 1 )
				//{
					float2 uvs;
					float f = ( 1 - sqrt( 1 - r ) ) / r;
					uvs.x = p.x;
					uvs.y = p.y;

					float3 vertexPos = float3( uvs, (f-1)*2);
					viewDir = normalize(float3(0,0,-5) - vertexPos);
					
					uvs.x = p.x * f;
					uvs.y = p.y * f;
					normal = normalize(vertexPos);
					
					if ( _Connected == 1 )
						normal = n.rgb;

					//a = 1;
				//}
				float fresnel = (b + s*pow(s - dot( normal, viewDir ) , pw));
				return fresnel;// * a;
			}
			ENDCG
		}
	}
}

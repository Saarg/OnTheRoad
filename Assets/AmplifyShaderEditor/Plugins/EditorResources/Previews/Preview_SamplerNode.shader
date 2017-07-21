Shader "Hidden/SamplerNode"
{
	Properties
	{
		_B ("_UVs", 2D) = "white" {}
		_C ("_Level", 2D) = "white" {}
		_F ("_NormalScale", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityStandardUtils.cginc"

			sampler2D _Sampler;
			sampler2D _B;
			sampler2D _C;
			sampler2D _F;
			float _CustomUVs;
			float _Unpack;
			float _LodType;

			float4 frag( v2f_img i ) : SV_Target
			{
				float2 uvs = i.uv;
				if ( _CustomUVs == 1 )
					uvs = tex2D( _B, i.uv ).xy;

				float n = tex2D( _F, i.uv ).r;

				float4 c = 0;
				if ( _LodType == 1 ) {
					float lod = tex2D( _C, i.uv ).r;
					c = tex2Dlod( _Sampler, float4(uvs,0,lod) );
				}
				else if ( _LodType == 2 ) {
					float bias = tex2D( _C, i.uv ).r;
					c = tex2Dbias( _Sampler, float4(uvs,0,bias) );
				}
				else {
					c = tex2D( _Sampler, uvs );
				}

				if ( _Unpack == 1 ) {
					c.rgb = UnpackScaleNormal( c, n );
				} 

				return c;
			}
			ENDCG
		}
	}
}

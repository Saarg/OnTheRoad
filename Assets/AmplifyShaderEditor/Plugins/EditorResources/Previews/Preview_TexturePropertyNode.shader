Shader "Hidden/TexturePropertyNode"
{
	//Properties
	//{
	//	_A ("_Tilling", 2D) = "white" {}
	//	_B ("_Offset", 2D) = "white" {}
	//}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _Sampler;
			float _NotLinear;

			float4 frag( v2f_img i ) : SV_Target
			{
				float4 c = tex2D( _Sampler, i.uv);
				if ( _NotLinear ) {
					c.rgb = LinearToGammaSpace( c );
				}
				return c;
			}
			ENDCG
		}
	}
}

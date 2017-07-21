Shader "Hidden/WorldPosInputsNode"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			float _TangentSpace;

			float4 frag(v2f_img i) : SV_Target
			{
				float2 p = 2 * i.uv - 1;
				float r = sqrt( dot(p,p) );
				float3 vertexPos;
				float3 worldViewDir;
				float3 normal;
				r = saturate( r );
				//if ( r < 1 )
				//{
					float2 uvs;
					float f = ( 1 - sqrt( 1 - r ) ) / r;
					uvs.x = p.x;
					uvs.y = p.y;// *f;
					
					vertexPos = float3( uvs, (f-1)*2);
					worldViewDir = normalize(float3(0,0,-5) - vertexPos);
					normal = normalize(vertexPos);
					
				//}
				//else {
				//	vertexPos = 0;
				//	worldViewDir = 0;
				//	normal = 0;
				//}


				if ( _TangentSpace == 1 ) {
					float2 tp = 2 * i.uv - 1;
					float tr = sqrt( dot(tp,tp) );
					tr = saturate( tr );
					//if ( tr < 1 ) {
						float2 tuvs;
						float tf = ( 1 - sqrt( 1 - tr ) ) / tr;

						float3 tangent = normalize(float3( 1-tf, tp.y*0.01, tp.x ));
						float3 worldPos = mul(unity_ObjectToWorld, vertexPos).xyz;
						fixed3 worldNormal = UnityObjectToWorldNormal(normal);
						fixed3 worldTangent = UnityObjectToWorldDir(tangent);
						fixed tangentSign = -1;
						fixed3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
						float4 tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
						float4 tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
						float4 tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

						fixed3 viewDirTan = tSpace0.xyz * worldViewDir.x + tSpace1.xyz * worldViewDir.y + tSpace2.xyz * worldViewDir.z;
						return float4(normalize(viewDirTan), 1);
					//}
					//else {
					//	return 0;
					//}
				}

				return float4((worldViewDir), 1);
			}
			ENDCG
		}
	}
}

// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Tangent", "Vertex Data", "Vertex tangent vector in object space, can be used in both local vertex offset and fragment outputs" )]
	public sealed class TangentVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "tangent";
			ChangeOutputProperties( 0, "XYZ", WirePortDataType.FLOAT3 );
			m_outputPorts[ 4 ].Visible = false;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "0a44bb521d06d6143a4acbc3602037f8";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			}
			else
			{
				dataCollector.ForceNormal = true;

				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				dataCollector.AddToInput( UniqueId, Constants.InternalData, false );

				//dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "worldTangent", "WorldNormalVector( " + Constants.InputVarStr + ", float3(1,0,0) )" );
				GeneratorUtils.GenerateWorldTangent( ref dataCollector, UniqueId );
				dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
#if UNITY_5_4_OR_NEWER
				string matrix = "unity_WorldToObject";
#else
				string matrix = "_World2Object";
#endif

				dataCollector.AddToLocalVariables( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "vertexTangent", "mul( " + matrix + ", float4( "+ GeneratorUtils.WorldTangentStr + ", 0 ) )" );
				return GetOutputVectorItem( 0, outputId, "vertexTangent" );
			}
		}
	}
}

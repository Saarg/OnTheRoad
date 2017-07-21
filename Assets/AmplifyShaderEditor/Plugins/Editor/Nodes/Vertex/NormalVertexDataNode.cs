// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Vertex Normal", "Vertex Data", "Vertex normal vector in object space, can be used in both local vertex offset and fragment outputs" )]
	public sealed class NormalVertexDataNode : VertexDataNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentVertexData = "normal";
			ChangeOutputProperties( 0, "XYZ", WirePortDataType.FLOAT3 );
			m_outputPorts[ 4 ].Visible = false;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "6b24b06c33f9fe84c8a2393f13ab5406";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				return base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			}
			else
			{
				//dataCollector.ForceNormal = true;

				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_NORMAL ), true );
				dataCollector.AddToInput( UniqueId, Constants.InternalData, false );

				//dataCollector.AddToLocalVariables( m_uniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "worldNormal", "WorldNormalVector( " + Constants.InputVarStr + ", float3(0,0,1) )" );
				GeneratorUtils.GenerateWorldNormal( ref dataCollector, UniqueId );
				dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
#if UNITY_5_4_OR_NEWER
				string matrix = "unity_WorldToObject";
#else
				string matrix = "_World2Object";
#endif
				dataCollector.AddToLocalVariables( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT3, "vertexNormal", "mul( " + matrix + ", float4( "+ GeneratorUtils.WorldNormalStr + ", 0 ) )" );
				return GetOutputVectorItem( 0, outputId, "vertexNormal" );
			}
		}
	}
}

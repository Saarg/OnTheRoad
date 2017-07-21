// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Dot", "Vector", "Scalar dot product of two vectors" )]
	public sealed class DotProductOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT, false );
			m_dynamicOutputType = false;
			m_useInternalPortData = true;
			m_previewShaderGUID = "85f11fd5cb9bb954c8615a45c57a3784";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );

			switch ( m_mainDataType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				{
					return "dot( " + m_inputA + " , " + m_inputB + " )";
				}
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}
			return UIUtils.InvalidParameter( this );
		}
	}
}

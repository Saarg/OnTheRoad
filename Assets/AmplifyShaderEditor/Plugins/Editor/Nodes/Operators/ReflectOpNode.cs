// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Reflect", "Vector", "Reflection vector given an incidence vector and a normal vector" )]
	public sealed class ReflectOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "Incident";
			m_inputPorts[ 1 ].Name = "Normal";
			m_textLabelWidth = 55;
			m_previewShaderGUID = "fb520f2145c0fa0409320a9e6d720758";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			switch ( m_outputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.INT:
				case WirePortDataType.COLOR:
				case WirePortDataType.OBJECT:
				{
					return "reflect( " + m_inputA + " , " + m_inputB + " )";
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

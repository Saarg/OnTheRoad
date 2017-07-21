// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Refract", "Vector", "Computes a refraction vector" )]
	public sealed class RefractOpVec : DynamicTypeNode
	{
		override protected void AddPorts()
		{
			base.AddPorts();
			m_inputPorts[ 0 ].Name = "Incident";
			m_inputPorts[ 1 ].Name = "Normal";
			AddInputPort( WirePortDataType.FLOAT, true, "Eta" );
			m_textLabelWidth = 55;
			m_previewShaderGUID = "5ab44ca484bed8b4884b03b1c00fdc3d";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string interp = m_inputPorts[ 2 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 2 ].DataType, ignoreLocalvar );

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
					return "refract( " + m_inputA + " , " + m_inputB + " , " + interp + " )";
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

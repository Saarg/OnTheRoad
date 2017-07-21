// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Ext]Add", "Operators", "Simple scalar addition or vector per component addition", null, KeyCode.None, false )]
	class AddOpNode : ExtensibleInputPortNode
	{
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			if ( m_inputPorts.Count == 0 )
			{
				return UIUtils.NoConnection( this );
			}
			else if ( m_inputPorts.Count == 1 && m_inputPorts[ 0 ].IsConnected )
				return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );

			string result = "( ";
			switch ( m_selectedType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					bool firstOp = true;
					for ( int portId = 0; portId < m_inputPorts.Count; portId++ )
					{
						if ( m_inputPorts[ portId ].IsConnected )
						{
							if ( firstOp )
							{
								firstOp = false;
								result += m_inputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ portId ].DataType, ignoreLocalvar );
							}
							else
							{
								result += " + " + m_inputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ portId ].DataType, ignoreLocalvar );
							}
						}
					}
				}
				break;
			}

			result += " )";
			return result;
		}

	}
}

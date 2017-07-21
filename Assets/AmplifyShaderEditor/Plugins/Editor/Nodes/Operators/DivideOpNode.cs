// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "[Ext]Divide", "Operators", "Simple scalar division or vector per component division", null, KeyCode.None, false )]
	class DivideOpNode : ExtensibleInputPortNode
	{
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_inputPorts.Count == 0 )
			{
				return UIUtils.NoConnection( this );
			}
			else if ( m_inputPorts.Count == 1 && m_inputPorts[ 0 ].IsConnected )
				return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalVar );

			string result = "( ";
			switch ( m_selectedType )
			{
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					if ( m_inputPorts.Count == 1 && m_inputPorts[ 0 ].IsConnected )
					{
						result += m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalVar );
					}
					else
					{
						bool firstOp = true;
						for ( int portId = 0; portId < m_inputPorts.Count; portId++ )
						{
							if ( m_inputPorts[ portId ].IsConnected )
							{
								if ( firstOp )
								{
									firstOp = false;
									result += m_inputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ portId ].DataType, ignoreLocalVar );
								}
								else
								{
									result += " / " + m_inputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ portId ].DataType, ignoreLocalVar );
								}
							}
						}
					}
					break;
				}
			}
			result += " )";
			return result;
		}

	}
}

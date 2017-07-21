// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "[Ext]Multiply", "Operators", "Simple scalar multiplication or per component vector multiplication or matrix multiplication", null, KeyCode.None, false )]
	class MultiplyOpNode : ExtensibleInputPortNode
	{
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_inputPorts.Count == 0 )
			{
				return UIUtils.NoConnection( this );
			}
			else if ( m_inputPorts.Count == 1 && m_inputPorts[ 0 ].IsConnected )
				return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );

			string result = string.Empty;
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
					result = "( ";
					if ( m_inputPorts.Count == 1 && m_inputPorts[ 0 ].IsConnected )
					{
						result += m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );
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
									result += m_inputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ portId ].DataType, ignoreLocalvar );
								}
								else
								{
									result += " * " + m_inputPorts[ portId ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ portId ].DataType, ignoreLocalvar );
								}
							}
						}
					}
					result += " )";
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{

					result = "mul( " +
							m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].GetConnection().DataType, ignoreLocalvar ) + " , " +
							m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 1 ].GetConnection().DataType, ignoreLocalvar ) + " )";
				}
				break;
			}

			if ( String.IsNullOrEmpty( result ) )
			{
				return UIUtils.InvalidParameter( this );
			}
			return result;
		}

		protected override void OnTypeChange()
		{
			m_freeInputCountNb = !( m_selectedType == WirePortDataType.FLOAT3x3 || m_selectedType == WirePortDataType.FLOAT4x4 );

			if ( !m_freeInputCountNb )
				m_inputCount = 2;
		}

	}
}

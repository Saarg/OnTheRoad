// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Divide", "Operators", "Simple divide of two variables", null, KeyCode.D )]
	public sealed class SimpleDivideOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_previewShaderGUID = "409f06d00d1094849b0834c52791fa72";
		}

		public override string BuildResults( int outputId,  ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );
			if ( m_inputPorts[ 0 ].DataType != m_mainDataType )
			{
				m_inputA = UIUtils.CastPortType( dataCollector.PortCategory, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, m_mainDataType, m_inputA );
			}
			m_inputB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 1 ].DataType, ignoreLocalvar );
			if ( m_inputPorts[ 1 ].DataType != m_mainDataType && m_inputPorts[ 1 ].DataType != WirePortDataType.FLOAT )
			{
				m_inputB = UIUtils.CastPortType( dataCollector.PortCategory, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, m_mainDataType, m_inputB );
			}

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
					return "( " + m_inputA + " / " + m_inputB + " )";
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

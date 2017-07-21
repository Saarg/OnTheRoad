// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	public class DynamicTypeNode : ParentNode
	{
		protected string m_inputA = string.Empty;
		protected string m_inputB = string.Empty;
		protected bool m_dynamicOutputType = true;

		[UnityEngine.SerializeField]
		protected WirePortDataType m_mainDataType = WirePortDataType.FLOAT;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_useInternalPortData = true;
			m_textLabelWidth = 35;
			AddPorts();
		}

		protected virtual void AddPorts()
		{
			AddInputPort( WirePortDataType.FLOAT, false, "A" );
			AddInputPort( WirePortDataType.FLOAT, false, "B" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_inputPorts[0].CreatePortRestrictions( WirePortDataType.OBJECT,
														WirePortDataType.FLOAT,
														WirePortDataType.FLOAT2,
														WirePortDataType.FLOAT3,
														WirePortDataType.FLOAT4,
														WirePortDataType.COLOR,
														WirePortDataType.FLOAT3x3,
														WirePortDataType.FLOAT4x4,
														WirePortDataType.INT );
			m_inputPorts[ 1 ].CreatePortRestrictions( WirePortDataType.OBJECT,
														WirePortDataType.FLOAT,
														WirePortDataType.FLOAT2,
														WirePortDataType.FLOAT3,
														WirePortDataType.FLOAT4,
														WirePortDataType.COLOR,
														WirePortDataType.FLOAT3x3,
														WirePortDataType.FLOAT4x4,
														WirePortDataType.INT );
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			UpdateConnection( inputPortId );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnection( portId );
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			UpdateDisconnectedConnection( portId );
		}

		void UpdateDisconnectedConnection( int portId )
		{
			if ( m_inputPorts[ 0 ].DataType != m_inputPorts[ 1 ].DataType )
			{
				int otherPortId = ( portId + 1 ) % 2;
				if ( m_inputPorts[ otherPortId ].IsConnected )
				{
					m_mainDataType = m_inputPorts[ otherPortId ].DataType;
					m_inputPorts[ portId ].ChangeType( m_mainDataType, false );
					if ( m_dynamicOutputType )
						m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
				}
				else
				{
					if (UIUtils.GetPriority( m_inputPorts[ 0 ].DataType ) > UIUtils.GetPriority( m_inputPorts[ 1 ].DataType ) )
					{
						m_mainDataType = m_inputPorts[ 0 ].DataType;
						m_inputPorts[ 1 ].ChangeType( m_mainDataType, false );
					}
					else
					{
						m_mainDataType = m_inputPorts[ 1 ].DataType;
						m_inputPorts[ 0 ].ChangeType( m_mainDataType, false );
					}

					if ( m_dynamicOutputType )
					{
						if ( m_mainDataType != m_outputPorts[ 0 ].DataType )
						{
							m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
						}
					}
				}
			}
		}

		void UpdateConnection( int portId )
		{
			m_inputPorts[ portId ].MatchPortToConnection();
			int otherPortId = ( portId + 1 ) % 2;
			if ( !m_inputPorts[ otherPortId ].IsConnected )
			{
				m_inputPorts[ otherPortId ].ChangeType( m_inputPorts[ portId ].DataType, false );
			}

			if ( m_inputPorts[ 0 ].DataType == m_inputPorts[ 1 ].DataType )
			{
				m_mainDataType = m_inputPorts[ 0 ].DataType;
				if ( m_dynamicOutputType )
					m_outputPorts[ 0 ].ChangeType( InputPorts[ 0 ].DataType, false );
			}
			else
			{
				if ( UIUtils.GetPriority( m_inputPorts[ 0 ].DataType ) > UIUtils.GetPriority( m_inputPorts[ 1 ].DataType ) )
				{
					m_mainDataType = m_inputPorts[ 0 ].DataType;
				}
				else
				{
					m_mainDataType = m_inputPorts[ 1 ].DataType;
				}

				if ( m_dynamicOutputType )
				{
					if ( m_mainDataType != m_outputPorts[ 0 ].DataType )
					{
						m_outputPorts[ 0 ].ChangeType( m_mainDataType, false );
					}
				}
			}
		}

		public virtual string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			SetInputData( outputId, ref dataCollector, ignoreLocalvar );
			return string.Empty;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string result = BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		protected void SetInputData( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			m_inputA = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			if ( m_inputPorts[ 0 ].DataType != m_mainDataType )
			{
				m_inputA = UIUtils.CastPortType( dataCollector.PortCategory, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputA, m_inputPorts[ 0 ].DataType, m_mainDataType, m_inputA );
			}
			m_inputB = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			if ( m_inputPorts[ 1 ].DataType != m_mainDataType )
			{
				m_inputB = UIUtils.CastPortType( dataCollector.PortCategory, m_currentPrecisionType, new NodeCastInfo( UniqueId, outputId ), m_inputB, m_inputPorts[ 1 ].DataType, m_mainDataType, m_inputB );
			}
		}

	}
}

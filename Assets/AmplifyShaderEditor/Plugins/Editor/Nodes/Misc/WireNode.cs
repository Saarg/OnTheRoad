using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Wire Node", "Misc", "Wire Node", null, KeyCode.None, false )]
	public sealed class WireNode : ParentNode
	{
		private bool m_markedToDelete = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.OBJECT, false, string.Empty );
			AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
			m_drawPreview = false;
			m_drawPreviewExpander = false;
			m_canExpand = false;
			m_previewShaderGUID = "fa1e3e404e6b3c243b5527b82739d682";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnOutputPortConnected( int portId, int otherNodeId, int otherPortId )
		{
			base.OnOutputPortConnected( portId, otherNodeId, otherPortId );
			if ( m_outputPorts[ portId ].ConnectionCount > 1 )
			{
				for ( int i = 0; i < m_outputPorts[ portId ].ExternalReferences.Count; i++ )
				{
					if ( m_outputPorts[ portId ].ExternalReferences[ i ].PortId != otherPortId )
					{
						UIUtils.DeleteConnection( true, m_outputPorts[ portId ].ExternalReferences[ i ].NodeId, m_outputPorts[ portId ].ExternalReferences[ i ].PortId, false, true );
					}
				}
			}
		}


		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			TestIfValid();
		}

		public override void OnOutputPortDisconnected( int portId )
		{
			base.OnOutputPortDisconnected( portId );
			TestIfValid();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			return m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
		}

		public override void DrawProperties()
		{
			if ( m_markedToDelete )
				return;

			base.DrawProperties();
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if ( m_markedToDelete )
				return;

			if ( m_repopulateDictionaries )
			{
				m_repopulateDictionaries = false;

				m_inputPortsDict.Clear();
				int inputCount = m_inputPorts.Count;
				for ( int i = 0; i < inputCount; i++ )
				{
					m_inputPortsDict.Add( m_inputPorts[ i ].PortId, m_inputPorts[ i ] );
				}

				m_outputPortsDict.Clear();
				int outputCount = m_outputPorts.Count;
				for ( int i = 0; i < outputCount; i++ )
				{
					m_outputPortsDict.Add( m_outputPorts[ i ].PortId, m_outputPorts[ i ] );
				}
			}

			if ( m_initialized )
			{
				if ( m_sizeIsDirty )
				{
					m_sizeIsDirty = false;

					m_extraSize.Set( 20f, 20f );
					m_position.width = m_extraSize.x + UIUtils.PortsSize.x;
					m_position.height = m_extraSize.y + UIUtils.PortsSize.y;

					Vec2Position -= Position.size * 0.5f;
					//m_cachedPos = m_position;
					if ( OnNodeChangeSizeEvent != null )
					{
						OnNodeChangeSizeEvent( this );
					}

					ChangeSizeFinished();
				}

				CalculatePositionAndVisibility( drawInfo );
				Color colorBuffer = GUI.color;

				if ( m_anchorAdjust < 0 )
				{
					m_anchorAdjust = UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon ).normal.background.width;
				}

				//GUI.Box(m_globalPosition, string.Empty, UIUtils.CurrentWindow.CustomStylesInstance.Box);

				//Input ports
				{
					Rect currInputPortPos = m_globalPosition;
					currInputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
					currInputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;

					currInputPortPos.x += m_position.width * 0.5f * drawInfo.InvertedZoom - currInputPortPos.width * 0.5f;
					currInputPortPos.y += m_position.height * 0.5f * drawInfo.InvertedZoom - currInputPortPos.height * 0.5f;

					int inputCount = m_inputPorts.Count;
					for ( int i = 0; i < inputCount; i++ )
					{
						if ( m_inputPorts[ i ].Visible )
						{
							// Button
							m_inputPorts[ i ].Position = currInputPortPos;

							GUIStyle style = m_inputPorts[ i ].IsConnected ? UIUtils.GetCustomStyle( CustomStyle.PortFullIcon ) : UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon );

							Rect portPos = currInputPortPos;
							portPos.x -= Constants.PORT_X_ADJUST;
							m_inputPorts[ i ].ActivePortArea = portPos;
							if ( m_isVisible )
							{
								if ( ContainerGraph.ParentWindow.Options.ColoredPorts )
									GUI.color = Selected ? UIUtils.GetColorFromWireStatus( WireStatus.Highlighted ) : UIUtils.GetColorForDataType( m_inputPorts[ i ].DataType, false, false );
								else
									GUI.color = Selected ? UIUtils.GetColorFromWireStatus( WireStatus.Highlighted ) : UIUtils.GetColorForDataType( m_inputPorts[ i ].DataType, true, true );
								GUI.Box( currInputPortPos, string.Empty, style );
							}

							GUI.color = colorBuffer;
						}
					}
				}

				//Output Ports
				{
					Rect currOutputPortPos = m_globalPosition;
					currOutputPortPos.width = drawInfo.InvertedZoom * UIUtils.PortsSize.x;
					currOutputPortPos.height = drawInfo.InvertedZoom * UIUtils.PortsSize.y;

					currOutputPortPos.x += m_position.width * 0.5f * drawInfo.InvertedZoom - currOutputPortPos.width * 0.5f;
					currOutputPortPos.y += m_position.height * 0.5f * drawInfo.InvertedZoom - currOutputPortPos.height * 0.5f;

					int outputCount = m_outputPorts.Count;
					for ( int i = 0; i < outputCount; i++ )
					{
						if ( m_outputPorts[ i ].Visible )
						{
							//Button
							m_outputPorts[ i ].Position = currOutputPortPos;

							GUIStyle style = m_isVisible ? ( m_outputPorts[ i ].IsConnected ? UIUtils.GetCustomStyle( CustomStyle.PortFullIcon ) : UIUtils.GetCustomStyle( CustomStyle.PortEmptyIcon ) ) : null;

							Rect portPos = currOutputPortPos;
							m_outputPorts[ i ].ActivePortArea = portPos;
							if ( m_isVisible )
							{
								if ( ContainerGraph.ParentWindow.Options.ColoredPorts )
									GUI.color = Selected ? UIUtils.GetColorFromWireStatus( WireStatus.Highlighted ) : UIUtils.GetColorForDataType( m_outputPorts[ i ].DataType, false, false );
								else
									GUI.color = Selected ? UIUtils.GetColorFromWireStatus( WireStatus.Highlighted ) : UIUtils.GetColorForDataType( m_outputPorts[ i ].DataType, true, false );
								GUI.Box( currOutputPortPos, string.Empty, style );
							}

							GUI.color = colorBuffer;
						}
					}
				}
				GUI.color = colorBuffer;
				TestIfValid();
			}
		}

		void TestIfValid()
		{
			if ( !m_inputPorts[ 0 ].IsConnected )
			{
				if ( !UIUtils.InputPortReference.IsValid || UIUtils.InputPortReference.IsValid && UIUtils.InputPortReference.NodeId != UniqueId )
					ContainerGraph.MarkWireNodeSequence( this, true );
			}

			if ( !m_outputPorts[ 0 ].IsConnected )
			{
				if ( !UIUtils.OutputPortReference.IsValid || UIUtils.OutputPortReference.IsValid && UIUtils.OutputPortReference.NodeId != UniqueId )
					ContainerGraph.MarkWireNodeSequence( this, false );
			}
		}

		public Vector3 TangentDirection
		{
			get
			{
				ParentNode otherInputNode = null;
				ParentNode otherOutputNode = null;

				//defaults to itself so it can still calculate tangents
				WirePort otherInputPort = m_outputPorts[ 0 ];
				WirePort otherOutputPort = m_inputPorts[ 0 ];

				if ( m_outputPorts[ 0 ].ConnectionCount > 0 )
				{
					otherInputNode = UIUtils.GetNode( m_outputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );
					otherInputPort = otherInputNode.GetInputPortByUniqueId( m_outputPorts[ 0 ].ExternalReferences[ 0 ].PortId );
				}

				if ( m_inputPorts[ 0 ].ConnectionCount > 0 )
				{
					otherOutputNode = UIUtils.GetNode( m_inputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );
					otherOutputPort = otherOutputNode.GetOutputPortByUniqueId( m_inputPorts[ 0 ].ExternalReferences[ 0 ].PortId );
				}

				//TODO: it still generates crooked lines if wire nodes get too close to non-wire nodes (the fix would be to calculate the non-wire nodes magnitude properly)
				float mag = Constants.HORIZONTAL_TANGENT_SIZE * ContainerGraph.ParentWindow.CameraDrawInfo.InvertedZoom;

				Vector2 outPos;
				if ( otherOutputNode != null && otherOutputNode.GetType() != typeof( WireNode ) )
					outPos = otherOutputPort.Position.position + Vector2.right * mag;
				else
					outPos = otherOutputPort.Position.position;

				Vector2 inPos;
				if ( otherInputNode != null && otherInputNode.GetType() != typeof( WireNode ) )
					inPos = otherInputPort.Position.position - Vector2.right * mag;
				else
					inPos = otherInputPort.Position.position;

				Vector2 tangent = ( outPos - inPos ).normalized;
				return new Vector3( tangent.x, tangent.y );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );

			m_extraSize.Set( 20f, 20f );
			m_position.width = m_extraSize.x + UIUtils.PortsSize.x;
			m_position.height = m_extraSize.y + UIUtils.PortsSize.y;

			Vec2Position += Position.size * 0.5f;
		}

		public WireReference FindNewValidInputNode( WireNode current )
		{
			if ( current.InputPorts[ 0 ].IsConnected )
			{
				ParentNode node = UIUtils.GetNode( current.InputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );
				if ( node != null )
				{
					WireNode wireNode = node as WireNode;
					if ( wireNode != null && wireNode.MarkToDelete )
					{
						return FindNewValidInputNode( wireNode );
					}
					else
					{
						return current.InputPorts[ 0 ].ExternalReferences[ 0 ];
					}
				}
			}
			return null;
		}

		public WireReference FindNewValidOutputNode( WireNode current )
		{
			if ( current.OutputPorts[ 0 ].IsConnected )
			{
				ParentNode node = UIUtils.GetNode( current.OutputPorts[ 0 ].ExternalReferences[ 0 ].NodeId );

				if ( node != null )
				{
					WireNode wireNode = node as WireNode;
					if ( wireNode != null && wireNode.MarkToDelete )
					{
						return FindNewValidOutputNode( wireNode );
					}
					else
					{
						return current.OutputPorts[ 0 ].ExternalReferences[ 0 ];
					}
				}
			}
			return null;
		}

		public override void Rewire()
		{
			//if ( m_inputPorts[ 0 ].ExternalReferences != null && m_inputPorts[ 0 ].ExternalReferences.Count > 0 )
			//{
			//WireReference backPort = m_inputPorts[ 0 ].ExternalReferences[ 0 ];
			//for ( int i = 0; i < m_outputPorts[ 0 ].ExternalReferences.Count; i++ )
			//{
			//	UIUtils.CurrentWindow.ConnectInputToOutput( m_outputPorts[ 0 ].ExternalReferences[ i ].NodeId, m_outputPorts[ 0 ].ExternalReferences[ i ].PortId, backPort.NodeId, backPort.PortId );
			//}
			//}
			MarkToDelete = true;
			WireReference outputReference = FindNewValidInputNode( this );
			WireReference inputReference = FindNewValidOutputNode( this );
			if ( outputReference != null && inputReference != null )
			{
				ContainerGraph.ParentWindow.ConnectInputToOutput( inputReference.NodeId, inputReference.PortId, outputReference.NodeId, outputReference.PortId );
			}
		}

		public bool MarkToDelete
		{
			get { return m_markedToDelete; }
			set { m_markedToDelete = value; }
		}
	}
}

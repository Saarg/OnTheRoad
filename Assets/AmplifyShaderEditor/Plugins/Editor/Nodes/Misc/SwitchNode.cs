using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Debug Switch", "Misc", "Hard Switch between any of its input ports" )]
	public class SwitchNode : ParentNode
	{
		private const string InputPortName = "In ";
	
		private const string CurrSelectedStr = "Current";
		private const string MaxAmountStr = "Max Amount";
		private const int MaxAllowedAmount = 8;
		private readonly string[] MaxAllowedLabels = { "2", "3", "4", "5", "6", "7", "8" };
		private readonly int[] MaxAllowedValues = { 2, 3, 4, 5, 6, 7, 8 };

		[SerializeField]
		private string[] AvailableInputsLabels = { "0", "1" };

		[SerializeField]
		private int[] AvailableInputsValues = { 0, 1 };

		[SerializeField]
		private int m_currentSelectedInput = 0;

		[SerializeField]
		private int m_maxAmountInputs = 2;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			for ( int i = 0; i < MaxAllowedAmount; i++ )
			{
				AddInputPort( WirePortDataType.FLOAT, false, InputPortName + i );
				//m_inputPorts[ i ].IsEditable = true;
				m_inputPorts[ i ].Visible = ( i < 2 );
			}
			AddOutputPort( WirePortDataType.FLOAT, m_inputPorts[0].Name );
			m_insideSize.Set( 50, 25 );
			m_autoWrapProperties = true;
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			m_inputPorts[ inputPortId ].MatchPortToConnection();
			if ( inputPortId == m_currentSelectedInput )
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ inputPortId ].DataType, false );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ portId ].MatchPortToConnection();
			if ( portId == m_currentSelectedInput )
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ portId ].DataType, false );
		}

		public void UpdateLabels()
		{
			AvailableInputsLabels = new string[ m_maxAmountInputs ];
			AvailableInputsValues = new int[ m_maxAmountInputs ];

			for ( int i = 0; i < m_maxAmountInputs; i++ )
			{
				AvailableInputsLabels[ i ] = i.ToString();
				AvailableInputsValues[ i ] = i;
			}
		}

		void UpdateOutput()
		{
			m_outputPorts[ 0 ].ChangeProperties( m_inputPorts[ m_currentSelectedInput ].Name , m_inputPorts[ m_currentSelectedInput ].DataType, false );
			m_sizeIsDirty = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			Rect newPos = m_remainingBox;
			newPos.width = 40 * drawInfo.InvertedZoom;
			newPos.height = 25 * drawInfo.InvertedZoom;
			newPos.x += m_remainingBox.width * 0.5f - newPos.width * 0.5f;

			EditorGUI.BeginChangeCheck();
			m_currentSelectedInput = EditorGUIIntPopup( newPos, m_currentSelectedInput, AvailableInputsLabels, AvailableInputsValues, UIUtils.SwitchNodePopUp );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdateOutput();
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_currentSelectedInput = EditorGUILayoutIntPopup( CurrSelectedStr, m_currentSelectedInput, AvailableInputsLabels, AvailableInputsValues );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdateOutput();
			}

			EditorGUI.BeginChangeCheck();
			m_maxAmountInputs = EditorGUILayoutIntPopup( MaxAmountStr, m_maxAmountInputs, MaxAllowedLabels, MaxAllowedValues );
			if ( EditorGUI.EndChangeCheck() )
			{
				for ( int i = 0; i < MaxAllowedAmount; i++ )
				{
					m_inputPorts[ i ].Visible = ( i < m_maxAmountInputs );
				}

				if ( m_currentSelectedInput >= m_maxAmountInputs )
				{
					m_currentSelectedInput = m_maxAmountInputs - 1;
					UpdateOutput();
				}

				UpdateLabels();
				m_sizeIsDirty = true;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			return m_inputPorts[ m_currentSelectedInput ].GeneratePortInstructions( ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentSelectedInput = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_maxAmountInputs = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( m_maxAmountInputs > 2 )
			{
				for ( int i = 2; i < m_maxAmountInputs; i++ )
				{
					m_inputPorts[ i ].Visible = true;
				}
			}
			m_sizeIsDirty = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentSelectedInput );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_maxAmountInputs );
		}
	}
}

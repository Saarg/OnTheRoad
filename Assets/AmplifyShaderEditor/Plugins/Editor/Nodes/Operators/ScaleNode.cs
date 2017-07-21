// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Scale", "Operators", "Scales input by a float factor" )]
	public sealed class ScaleNode : ParentNode
	{
		private const string ScaleFactorStr = "Scale";

		[SerializeField]
		private float m_scaleFactor = 1;

		private int m_cachedPropertyId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, " " );
			AddOutputPort( WirePortDataType.FLOAT, " " );
			m_insideSize.Set( 50, 10 );
			m_autoWrapProperties = true;
			m_previewShaderGUID = "6d8ec9d9dab62c44aa2dcc0e3987760d";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_ScaleFloat" );

			PreviewMaterial.SetFloat( m_cachedPropertyId, m_scaleFactor );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_scaleFactor = EditorGUILayoutFloatField( ScaleFactorStr, m_scaleFactor );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
		}

		public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
			m_inputPorts[ 0 ].MatchPortToConnection();
			m_outputPorts[ 0 ].ChangeType( InputPorts[ 0 ].DataType, false );
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_isVisible )
			{
				m_propertyDrawPos.x = m_remainingBox.x + Constants.FLOAT_WIDTH_SPACING * drawInfo.InvertedZoom * 0.5f;
				m_propertyDrawPos.y = m_remainingBox.y + Constants.INPUT_PORT_DELTA_Y * drawInfo.InvertedZoom * 0.5f;
				m_propertyDrawPos.width = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_WIDTH_FIELD_SIZE;
				m_propertyDrawPos.height = drawInfo.InvertedZoom * Constants.FLOAT_DRAW_HEIGHT_FIELD_SIZE;
				UIUtils.DrawFloat( this, ref m_propertyDrawPos, ref m_scaleFactor, 1 );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string portResult = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar );
			string result = "( " + portResult + " * " + m_scaleFactor + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_scaleFactor );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_scaleFactor = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
		}
	}
}

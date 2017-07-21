// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class ExtensibleInputPortNode : ParentNode
	{
		private const string _inputAmountStr = "Input amount";
		private const string _inputTypeStr = "Input type";

		[SerializeField]
		protected int m_inputCount = 2;

		[SerializeField]
		private int m_lastInputCount = 2;

		[SerializeField]
		protected WirePortDataType m_selectedType;

		[SerializeField]
		private WirePortDataType m_lastSelectedType;

		[SerializeField]
		protected bool m_freeInputCountNb;

		public ExtensibleInputPortNode() : base() { }
		public ExtensibleInputPortNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }


		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_freeInputCountNb = true;
			m_selectedType = m_lastSelectedType = WirePortDataType.OBJECT;
			AddInputPort( m_selectedType, false, "0" );
			AddInputPort( m_selectedType, false, "1" );
			AddOutputPort( m_selectedType, string.Empty );
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			{
				if ( m_freeInputCountNb )
				{
					EditorGUILayout.LabelField( _inputAmountStr );
					m_inputCount = EditorGUILayoutIntField( m_inputCount );
				}

				EditorGUILayout.LabelField( _inputTypeStr );
				m_selectedType = ( WirePortDataType ) EditorGUILayoutEnumPopup( m_selectedType );
			}
			EditorGUILayout.EndVertical();
			if ( m_inputCount != m_lastInputCount )
			{
				if ( m_inputCount > m_lastInputCount )
				{
					int newPortsAmount = ( m_inputCount - m_lastInputCount );
					for ( int i = 0; i < newPortsAmount; i++ )
					{
						AddInputPort( m_selectedType, false, ( i + m_lastInputCount ).ToString() );
					}
				}
				else
				{
					int newPortsAmount = ( m_lastInputCount - m_inputCount );
					for ( int i = 0; i < newPortsAmount; i++ )
					{
						DeleteInputPortByArrayIdx( m_inputPorts.Count - 1 );
					}
				}

				m_lastInputCount = m_inputCount;
				m_sizeIsDirty = true;
				m_isDirty = true;
				SetSaveIsDirty();
				OnInputChange();
			}

			if ( m_selectedType != m_lastSelectedType )
			{
				if ( UIUtils.CanCast( m_lastSelectedType, m_selectedType ) )
				{
					ChangeInputType( m_selectedType, false );
					ChangeOutputType( m_selectedType, false );

				}
				else
				{
					DeleteAllInputConnections( false );
					DeleteAllOutputConnections( false );
					ChangeInputType( m_selectedType, true );
					ChangeOutputType( m_selectedType, true );
				}
				SetSaveIsDirty();
				m_isDirty = true;
				m_lastSelectedType = m_selectedType;
				OnTypeChange();
			}
		}

		void UpdatePorts()
		{
			m_lastInputCount = Mathf.Max( m_inputCount, 1 );
			m_lastSelectedType = m_selectedType;

			DeleteAllInputConnections( true );
			DeleteAllOutputConnections( true );

			for ( int i = 0; i < m_inputCount; i++ )
			{
				AddInputPort( m_selectedType, false, i.ToString() );
			}
			AddOutputPort( m_selectedType, "out" );
			m_sizeIsDirty = true;
			SetSaveIsDirty();
		}

		protected virtual void OnInputChange() { }

		protected virtual void OnTypeChange() { }

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			m_lastSelectedType = m_selectedType;


			m_freeInputCountNb = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			m_inputCount = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

			DeleteAllInputConnections( true );
			DeleteAllOutputConnections( true );
			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_freeInputCountNb );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_inputCount );

		}
	}
}

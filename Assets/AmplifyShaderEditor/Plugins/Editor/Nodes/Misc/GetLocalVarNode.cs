// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Get Local Var", "Misc", "Use a registered local variable" )]
	public class GetLocalVarNode : ParentNode
	{
		[SerializeField]
		private int m_referenceId = -1;

		[SerializeField]
		private float m_referenceWidth = -1;

		[SerializeField]
		private int m_nodeId = -1;

		[SerializeField]
		private RegisterLocalVarNode m_currentSelected = null;

		private bool m_forceNodeUpdate = false;

		private int m_cachedPropertyId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
			m_textLabelWidth = 80;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "f21a6e44c7d7b8543afacd19751d24c6";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_currentSelected != null )
			{
				m_drawPreviewAsSphere = m_currentSelected.SpherePreview;
				CheckSpherePreview();

				if ( m_cachedPropertyId == -1 )
					m_cachedPropertyId = Shader.PropertyToID( "_A" );

				PreviewMaterial.SetTexture( m_cachedPropertyId, m_currentSelected.OutputPorts[ 0 ].OutputPreviewTexture );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_referenceId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceId, UIUtils.LocalVarNodeArr() );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_currentSelected = UIUtils.GetLocalVarNode( m_referenceId );
				if ( m_currentSelected != null )
				{
					m_nodeId = m_currentSelected.UniqueId;
					m_outputPorts[ 0 ].ChangeType( m_currentSelected.OutputPorts[ 0 ].DataType, false );
					m_drawPreviewAsSphere = m_currentSelected.SpherePreview;
					CheckSpherePreview();
				}

				m_sizeIsDirty = true;
				m_isDirty = true;
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			m_currentSelected = null;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_forceNodeUpdate )
			{
				m_forceNodeUpdate = false;
				if ( UIUtils.CurrentShaderVersion() > 15 )
				{
					m_currentSelected = UIUtils.GetNode( m_nodeId ) as RegisterLocalVarNode;
					m_referenceId = UIUtils.GetLocalVarNodeRegisterId( m_nodeId );
				}
				else
				{
					m_currentSelected = UIUtils.GetLocalVarNode( m_referenceId );
					if ( m_currentSelected != null )
					{
						m_nodeId = m_currentSelected.UniqueId;
					}
				}

				if ( m_currentSelected != null )
				{
					m_outputPorts[ 0 ].ChangeType( m_currentSelected.OutputPorts[ 0 ].DataType, false );
				}
			}

			UpdateLocalVar();
		}

		void UpdateLocalVar()
		{
			if ( m_referenceId > -1 )
			{
				m_currentSelected = UIUtils.GetLocalVarNode( m_referenceId );
				if ( m_currentSelected != null )
				{
					if ( m_currentSelected.OutputPorts[ 0 ].DataType != m_outputPorts[ 0 ].DataType )
					{
						m_outputPorts[ 0 ].ChangeType( m_currentSelected.OutputPorts[ 0 ].DataType, false );
					}

					m_additionalContent.text = string.Format( Constants.PropertyValueLabel, m_currentSelected.DataToArray );
					if ( m_referenceWidth != m_currentSelected.Position.width )
					{
						m_referenceWidth = m_currentSelected.Position.width;
						m_sizeIsDirty = true;
					}
				}
				else
				{
					m_referenceId = -1;
					m_referenceWidth = -1;
					m_additionalContent.text = string.Empty;
				}
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_currentSelected != null )
			{
				return m_currentSelected.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			}
			else
			{
				Debug.LogError( "Attempting to access inexistant local variable" );
				return "0";
			}
		}

		public override void PropagateNodeData( NodeData nodeData )
		{
			base.PropagateNodeData( nodeData );
			if ( m_currentSelected != null )
			{
				m_currentSelected.PropagateNodeData( nodeData );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 15 )
			{
				m_nodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}
			else
			{
				m_referenceId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}
			m_forceNodeUpdate = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_currentSelected != null ? m_currentSelected.UniqueId : -1 ) );
		}
		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if ( m_currentSelected != null )
			{
				UIUtils.FocusOnNode( m_currentSelected, 0, true );
			}
		}
	}
}

// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{

	[Serializable]
	[NodeAttributes( "Grab Screen Color", "Surface Standard Inputs", "Grabed pixel color value from screen" )]
	public sealed class ScreenColorNode : PropertyNode
	{
		private readonly Color ReferenceHeaderColor = new Color( 2.67f, 1.0f, 0.5f, 1.0f );
		
		private const string SamplerType = "tex2D";
		private const string GrabTextureDefault = "_GrabTexture";
		//private const string GrabVarStr = "grabScreenPos";

		private const string ScreenPosStr = "screenPos";
		private readonly string ScreenPosOnFragStr = Constants.InputVarStr + "." + ScreenPosStr;
		private readonly string ProjectionInstruction = "{0}.w += 0.00000000001;\n\t\t\t{0}.xyzw /= {0}.w;";
		private readonly string[] HackInstruction = {   "#if UNITY_UV_STARTS_AT_TOP",
														"float scale{0} = -1.0;",
														"#else",
														"float scale{0} = 1.0;",
														"#endif",
														"float halfPosW{1} = {0}.w * 0.5;",
														"{0}.y = ( {0}.y - halfPosW{1} ) * _ProjectionParams.x* scale{1} + halfPosW{1};"};


		[ SerializeField]
		private bool m_isTextureFetched;

		[SerializeField]
		private string m_textureFetchedValue;
		
		/////////////////////////////////////////////////////////

		[SerializeField]
		private TexReferenceType m_referenceType = TexReferenceType.Object;

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		[SerializeField]
		private GUIStyle m_referenceIconStyle = null;
		
		private ScreenColorNode m_referenceNode = null;

		[SerializeField]
		private float m_referenceWidth = -1;

		private bool m_forceNodeUpdate = false;

		public ScreenColorNode() : base() { }
		public ScreenColorNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { }

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );

			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddOutputColorPorts( "RGBA" );

			m_currentParameterType = PropertyType.Global;
			//m_useCustomPrefix = true;
			m_customPrefix = "Grab Screen ";
			m_freeType = false;
		}
		
		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if( m_referenceType == TexReferenceType.Object )
				UIUtils.RegisterScreenColorNode( this );
		}

		void UpdateHeaderColor()
		{
			m_headerColorModifier = ( m_referenceType == TexReferenceType.Object ) ? Color.white : ReferenceHeaderColor;
		}

		protected override void ChangeSizeFinished()
		{
			if ( m_referenceType == TexReferenceType.Instance )
			{
				m_position.width += 20;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );

			if ( m_forceNodeUpdate )
			{
				m_forceNodeUpdate = false;
				if ( UIUtils.CurrentShaderVersion() > 22 )
				{
					m_referenceNode = UIUtils.GetNode( m_referenceNodeId ) as ScreenColorNode;
					m_referenceArrayId = UIUtils.GetScreenColorNodeRegisterId( m_referenceNodeId );
				}
				else
				{
					m_referenceNode = UIUtils.GetScreenColorNode( m_referenceArrayId );
					if ( m_referenceNode != null )
					{
						m_referenceNodeId = m_referenceNode.UniqueId;
					}
				}
			}
			
			if ( SoftValidReference )
			{
				m_content.text = m_referenceNode.TitleContent.text + Constants.InstancePostfixStr;
				m_additionalContent.text = m_referenceNode.AdditonalTitleContent.text;
				
				if ( m_referenceIconStyle == null )
				{
					m_referenceIconStyle = UIUtils.GetCustomStyle( CustomStyle.SamplerTextureIcon );
				}

				Rect iconPos = m_globalPosition;
				iconPos.width = m_referenceIconStyle.normal.background.width * drawInfo.InvertedZoom;
				iconPos.height = m_referenceIconStyle.normal.background.height * drawInfo.InvertedZoom;

				iconPos.y += 6 * drawInfo.InvertedZoom;
				iconPos.x += m_globalPosition.width - iconPos.width - 7 * drawInfo.InvertedZoom;

				if ( GUI.Button( iconPos, string.Empty, m_referenceIconStyle ))
				{
					UIUtils.FocusOnNode( m_referenceNode, 1, true );
				}
			}
		}

		public override void DrawMainPropertyBlock()
		{
			EditorGUI.BeginChangeCheck();
			//m_referenceType = ( TexReferenceType ) EditorGUILayout.EnumPopup( Constants.ReferenceTypeStr, m_referenceType );
			m_referenceType = ( TexReferenceType ) EditorGUILayoutPopup( Constants.ReferenceTypeStr, ( int ) m_referenceType, Constants.ReferenceArrayLabels );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_sizeIsDirty = true;
				if ( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterScreenColorNode( this );
					m_content.text = m_propertyInspectorName;
					m_additionalContent.text = string.Format( Constants.PropertyValueLabel, GetPropertyValStr() );
				}
				else
				{
					UIUtils.UnregisterScreenColorNode( this );
					if ( SoftValidReference )
					{
						m_content.text = m_referenceNode.TitleContent.text + Constants.InstancePostfixStr;
						m_additionalContent.text = m_referenceNode.AdditonalTitleContent.text;
					}
				}
					UpdateHeaderColor();
			}
			
			if ( m_referenceType == TexReferenceType.Object )
			{
				EditorGUI.BeginChangeCheck();
				base.DrawMainPropertyBlock();
				if ( EditorGUI.EndChangeCheck() )
				{
					OnPropertyNameChanged();
					if ( string.IsNullOrEmpty( m_propertyInspectorName ) )
					{
						m_propertyName = GrabTextureDefault;
					}
				}
			}
			else
			{
				string[] arr = UIUtils.ScreenColorNodeArr();
				bool guiEnabledBuffer = GUI.enabled;
				if ( arr != null && arr.Length > 0 )
				{
					GUI.enabled = true;
				}
				else
				{
					m_referenceArrayId = -1;
					GUI.enabled = false;
				}

				m_referenceArrayId = EditorGUILayoutPopup(  Constants.AvailableReferenceStr, m_referenceArrayId, arr );
				GUI.enabled = guiEnabledBuffer;
			}
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdatePort();
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdatePort();
		}

		private void UpdatePort()
		{
			WirePortDataType otherType = m_inputPorts[ 0 ].ExternalReferences[ 0 ].DataType;
			if( otherType == WirePortDataType.FLOAT2 || otherType == WirePortDataType.FLOAT4 )
				m_inputPorts[ 0 ].MatchPortToConnection();

		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string propertyName = CurrentPropertyReference;

			OnPropertyNameChanged();

			bool emptyName = string.IsNullOrEmpty( m_propertyInspectorName );

			dataCollector.AddGrabPass( emptyName?string.Empty: propertyName );

			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );
			//if ( !m_inputPorts[ 0 ].IsConnected )
			//{
			//	string uvChannelDeclaration = IOUtils.GetUVChannelDeclaration( propertyName, -1, 0 );
			//	dataCollector.AddToInput( m_uniqueId, uvChannelDeclaration, true );
			//}
			string valueName = SetFetchedData( ref dataCollector, ignoreLocalVar);
			return GetOutputColorItem( 0, outputId, valueName );
		}

		public string SetFetchedData( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			string propertyName = CurrentPropertyReference;

			bool isProjecting = false;
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.FLOAT4 )
				isProjecting = true;

			if(!m_inputPorts[0].IsConnected)
				isProjecting = true;

			if ( ignoreLocalVar )
			{
				string samplerValue = SamplerType + ( isProjecting ? "proj" : "" ) +"( " + propertyName + ", " + GetUVCoords( ref dataCollector, ignoreLocalVar, isProjecting ) + " )";
				return samplerValue;
			}

			if ( m_isTextureFetched )
				return m_textureFetchedValue;

			string samplerOp = SamplerType + ( isProjecting ? "proj" : "" ) + "( " + propertyName + ", " + GetUVCoords( ref dataCollector, ignoreLocalVar, isProjecting ) + " )";

			int connectedPorts = 0;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					connectedPorts += 1;
					if ( connectedPorts > 1 || m_outputPorts[ i ].ConnectionCount > 1  )
					{
						// Create common local var and mark as fetched
						m_textureFetchedValue = SamplerType + "Node" + UniqueId;
						m_isTextureFetched = true;

						dataCollector.AddToLocalVariables( UniqueId, ( "float4 " ) + m_textureFetchedValue + " = " + samplerOp + ";" );
						m_outputPorts[ 0 ].SetLocalValue( m_textureFetchedValue );
						m_outputPorts[ 1 ].SetLocalValue( m_textureFetchedValue + ".r" );
						m_outputPorts[ 2 ].SetLocalValue( m_textureFetchedValue + ".g" );
						m_outputPorts[ 3 ].SetLocalValue( m_textureFetchedValue + ".b" );
						m_outputPorts[ 4 ].SetLocalValue( m_textureFetchedValue + ".a" );
						return m_textureFetchedValue;
					}
				}
			}
			return samplerOp;
		}

		public override void ResetOutputLocals()
		{
			base.ResetOutputLocals();
			m_isTextureFetched = false;
			m_textureFetchedValue = string.Empty;
		}

		public string GetUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, bool isProjecting )
		{
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				string result = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, ( isProjecting ? WirePortDataType.FLOAT4 : WirePortDataType.FLOAT2 ), ignoreLocalVar, true );
				if ( isProjecting )
					return "UNITY_PROJ_COORD( " + result + " )";
				else
					return result;
			}
			else
			{
				dataCollector.AddToInput( UniqueId, "float4 " + ScreenPosStr, true );

				string localVarName = ScreenPosStr + UniqueId;
				string value = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + localVarName + " = " + ScreenPosOnFragStr + ";";
				dataCollector.AddInstructions( value, true, true );

				dataCollector.AddInstructions( HackInstruction[ 0 ], true, true );
				dataCollector.AddInstructions( string.Format( HackInstruction[ 1 ], UniqueId ), true, true );
				dataCollector.AddInstructions( HackInstruction[ 2 ], true, true );
				dataCollector.AddInstructions( string.Format( HackInstruction[ 3 ], UniqueId ), true, true );
				dataCollector.AddInstructions( HackInstruction[ 4 ], true, true );
				dataCollector.AddInstructions( string.Format( HackInstruction[ 5 ], localVarName, UniqueId ), true, true );
				dataCollector.AddInstructions( string.Format( HackInstruction[ 6 ], localVarName, UniqueId ), true, true );
				dataCollector.AddInstructions( string.Format( ProjectionInstruction, localVarName ), true, true );
				return "UNITY_PROJ_COORD( " + localVarName + " )";

				//dataCollector.AddToInput( m_uniqueId, "float4 " + GrabVarStr, true );
				//dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + GrabVarStr + " = ComputeGrabScreenPos( UnityObjectToClipPos( " + Constants.VertexShaderInputStr + ".vertex ) )", m_uniqueId, true );
				//return "UNITY_PROJ_COORD( " + Constants.InputVarStr + "." + GrabVarStr + " )";
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			if ( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.UnregisterScreenColorNode( this );
			}
		}
		
		public bool SoftValidReference
		{
			get
			{
				if ( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					m_referenceNode = UIUtils.GetScreenColorNode( m_referenceArrayId );
					if ( m_referenceNode == null )
					{
						m_referenceArrayId = -1;
						m_referenceWidth = -1;
					}
					else if( m_referenceWidth != m_referenceNode.Position.width )
					{
						m_referenceWidth = m_referenceNode.Position.width;
						m_sizeIsDirty = true;
					}
					return m_referenceNode != null;
				}
				return false;
			}
		}

		public string CurrentPropertyReference
		{
			get
			{
				string propertyName = string.Empty;
				if ( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
				{
					ScreenColorNode node = UIUtils.GetScreenColorNode( m_referenceArrayId );
					propertyName = ( node != null ) ? node.PropertyName : m_propertyName;
				}
				else
				{
					propertyName = m_propertyName;
				}
				return propertyName;
			}
		}


		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 12 )
			{
				m_referenceType = ( TexReferenceType ) Enum.Parse( typeof( TexReferenceType ), GetCurrentParam( ref nodeParams ) );
				if ( UIUtils.CurrentShaderVersion() > 22 )
				{
					m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
				else
				{
					m_referenceArrayId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
				
				if ( m_referenceType == TexReferenceType.Instance )
				{
					UIUtils.UnregisterScreenColorNode( this );
					m_forceNodeUpdate = true;
				}

				UpdateHeaderColor();
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceType );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceNode != null ) ? m_referenceNode.UniqueId : -1 ) );
		}

		public override string GetPropertyValStr()
		{
			return m_propertyName;
		}

		public override string DataToArray { get { return m_propertyName; } }

		public override string GetUniformValue()
		{
			if ( SoftValidReference )
			{
				if ( m_referenceNode.IsConnected )
					return string.Empty;

				return m_referenceNode.GetUniformValue();
			}

			return "uniform sampler2D " + m_propertyName + ";";
		}

		public override void GetUniformData( out string dataType, out string dataName )
		{
			if ( SoftValidReference )
			{
				//if ( m_referenceNode.IsConnected )
				//{
				//	dataType = string.Empty;
				//	dataName = string.Empty;
				//}

				m_referenceNode.GetUniformData( out dataType, out  dataName );
				return;
			}

			dataType = "sampler2D";
			dataName  = m_propertyName;
		}
	}
}

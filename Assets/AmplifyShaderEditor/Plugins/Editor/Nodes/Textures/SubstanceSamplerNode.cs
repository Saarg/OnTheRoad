// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Substance Sample", "Textures", "Samples a procedural material", KeyCode.None, true, 0, int.MaxValue, typeof( SubstanceArchive ), typeof( ProceduralMaterial ) )]
	public sealed class SubstanceSamplerNode : PropertyNode
	{
		private const string GlobalVarDecStr = "uniform sampler2D {0};";
		private const string PropertyDecStr = "{0}(\"{0}\", 2D) = \"white\"";

		private const string AutoNormalStr = "Auto-Normal";
		private const string SubstanceStr = "Substance";

		private float TexturePreviewSizeX = 128;
		private float TexturePreviewSizeY = 128;


		private float PickerPreviewSizeX = 128;
		private float PickerPreviewSizeY = 17;
		private float PickerPreviewWidthAdjust = 18;

		private CacheNodeConnections m_cacheNodeConnections;

		[SerializeField]
		private int m_firstOutputConnected = 0;

		[SerializeField]
		private ProceduralMaterial m_proceduralMaterial;

		[SerializeField]
		private int m_textureCoordSet = 0;

		[SerializeField]
		private ProceduralOutputType[] m_textureTypes;

		[SerializeField]
		private bool m_autoNormal = true;

		private Type m_type;

		private List<int> m_outputConns = new List<int>();
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddOutputPort( WirePortDataType.COLOR, Constants.EmptyPortValue );
			m_insideSize.Set( TexturePreviewSizeX + PickerPreviewWidthAdjust, TexturePreviewSizeY + PickerPreviewSizeY + 5 );
			m_type = typeof( ProceduralMaterial );
			m_currentParameterType = PropertyType.Property;
			m_freeType = false;
			m_freeName = false;
			m_autoWrapProperties = true;
			m_customPrefix = "Substance Sample ";
			m_drawPrecisionUI = false;
			m_showPreview = true;
			m_selectedLocation = PreviewLocation.TopCenter;
			m_cacheNodeConnections = new CacheNodeConnections();
		}

		public override void OnOutputPortConnected( int portId, int otherNodeId, int otherPortId )
		{
			base.OnOutputPortConnected( portId, otherNodeId, otherPortId );
			m_firstOutputConnected = -1;
		}

		public override void OnOutputPortDisconnected( int portId )
		{
			base.OnOutputPortDisconnected( portId );
			m_firstOutputConnected = -1;
		}

		void CalculateFirstOutputConnected()
		{
			m_outputConns.Clear();
			int count = m_outputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					if ( m_firstOutputConnected < 0 )
						m_firstOutputConnected = i;

					m_outputConns.Add( i );
				}
			}

			if ( m_firstOutputConnected < 0 )
				m_firstOutputConnected = 0;

		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );


			Rect previewArea = m_remainingBox;
			previewArea.width = TexturePreviewSizeX * drawInfo.InvertedZoom;
			previewArea.height = TexturePreviewSizeY * drawInfo.InvertedZoom;
			previewArea.x += 0.5f * m_remainingBox.width - 0.5f * previewArea.width;
			GUI.Box( previewArea, string.Empty, UIUtils.ObjectFieldThumb );

			Rect pickerArea = previewArea;
			pickerArea.width = PickerPreviewSizeX * drawInfo.InvertedZoom;
			pickerArea.height = PickerPreviewSizeY * drawInfo.InvertedZoom;
			pickerArea.y += previewArea.height;

			Texture[] textures = m_proceduralMaterial != null ? m_proceduralMaterial.GetGeneratedTextures() : null;
			if ( textures != null )
			{
				
				if ( m_firstOutputConnected < 0  )
				{
					CalculateFirstOutputConnected();
				}
				else if ( textures.Length != m_textureTypes.Length )
				{
					OnNewSubstanceSelected( textures );
				}

				int texCount = m_outputConns.Count;
				previewArea.height /= texCount;

				for ( int i = 0; i < texCount; i++ )
				{
					EditorGUI.DrawPreviewTexture( previewArea, textures[ m_outputConns[ i ] ], null, ScaleMode.ScaleAndCrop );
					previewArea.y += previewArea.height;
				}
			}

			EditorGUI.BeginChangeCheck();
			m_proceduralMaterial = EditorGUIObjectField( pickerArea, m_proceduralMaterial, m_type, false ) as ProceduralMaterial;
			if ( EditorGUI.EndChangeCheck() )
			{
				textures = m_proceduralMaterial != null ? m_proceduralMaterial.GetGeneratedTextures() : null;
				OnNewSubstanceSelected( textures );
			}
			
		}

		void OnNewSubstanceSelected( Texture[] textures )
		{
			CacheCurrentSettings();
			ConfigPortsFromMaterial( true, textures );
			ConnectFromCache();
			m_requireMaterialUpdate = true;
			CalculateFirstOutputConnected();
			ContainerGraph.ParentWindow.RequestRepaint();
		}

		public override void DrawProperties()
		{
			base.DrawProperties();

			EditorGUI.BeginChangeCheck();
			m_proceduralMaterial = EditorGUILayoutObjectField( SubstanceStr, m_proceduralMaterial, m_type, false ) as ProceduralMaterial ;
			if ( EditorGUI.EndChangeCheck() )
			{
				Texture[] textures = m_proceduralMaterial != null ? m_proceduralMaterial.GetGeneratedTextures() : null;
				if ( textures != null )
				{
					OnNewSubstanceSelected( textures );
				}
			}

			m_textureCoordSet = EditorGUILayoutIntPopup( Constants.AvailableUVSetsLabel, m_textureCoordSet, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );
			EditorGUI.BeginChangeCheck();
			m_autoNormal = EditorGUILayoutToggle( AutoNormalStr, m_autoNormal );
			if ( EditorGUI.EndChangeCheck() )
			{
				for ( int i = 0; i < m_textureTypes.Length; i++ )
				{
					WirePortDataType portType = ( m_autoNormal && m_textureTypes[ i ] == ProceduralOutputType.Normal ) ? WirePortDataType.FLOAT3 : WirePortDataType.COLOR;
					if ( m_outputPorts[ i ].DataType != portType )
					{
						m_outputPorts[ i ].ChangeType( portType, false );
					}
				}
			}
		}

		private void CacheCurrentSettings()
		{
			m_cacheNodeConnections.Clear();
			for ( int portId = 0; portId < m_outputPorts.Count; portId++ )
			{
				if ( m_outputPorts[ portId ].IsConnected )
				{
					int connCount = m_outputPorts[ portId ].ConnectionCount;
					for ( int connIdx = 0; connIdx < connCount; connIdx++ )
					{
						WireReference connection = m_outputPorts[ portId ].GetConnection( connIdx );
						m_cacheNodeConnections.Add( m_outputPorts[ portId ].Name, new NodeCache( connection.NodeId, connection.PortId ) );
					}
				}
			}
		}

		private void ConnectFromCache()
		{
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				List<NodeCache> connections = m_cacheNodeConnections.GetList( m_outputPorts[ i ].Name );
				if ( connections != null )
				{
					int count = connections.Count;
					for ( int connIdx = 0; connIdx < count; connIdx++ )
					{
						UIUtils.SetConnection( connections[ connIdx ].TargetNodeId, connections[ connIdx ].TargetPortId, UniqueId, i );
					}
				}
			}
		}

		private void ConfigPortsFromMaterial( bool invalidateConnections = false, Texture[] newTextures = null )
		{
			//PreviewSizeX = ( m_proceduralMaterial != null ) ? UIUtils.ObjectField.CalcSize( new GUIContent( m_proceduralMaterial.name ) ).x + 15 : 110;
			m_insideSize.x = TexturePreviewSizeX + 5;
			SetAdditonalTitleText( ( m_proceduralMaterial != null ) ? string.Format( Constants.PropertyValueLabel, m_proceduralMaterial.name ) : string.Empty );

			Texture[] textures = newTextures != null ? newTextures : ( ( m_proceduralMaterial != null ) ? m_proceduralMaterial.GetGeneratedTextures() : null );
			if ( textures != null )
			{
				string nameToRemove = m_proceduralMaterial.name + "_";
				m_textureTypes = new ProceduralOutputType[ textures.Length ];
				for ( int i = 0; i < textures.Length; i++ )
				{
					ProceduralTexture procTex = textures[ i ] as ProceduralTexture;
					m_textureTypes[ i ] = procTex.GetProceduralOutputType();

					WirePortDataType portType = ( m_autoNormal && m_textureTypes[ i ] == ProceduralOutputType.Normal ) ? WirePortDataType.FLOAT3 : WirePortDataType.COLOR;
					string newName = textures[ i ].name.Replace( nameToRemove, string.Empty );
					char firstLetter = Char.ToUpper( newName[ 0 ] );
					newName = firstLetter.ToString() + newName.Substring( 1 );
					if ( i < m_outputPorts.Count )
					{
						m_outputPorts[ i ].ChangeProperties( newName, portType, false );
						if ( invalidateConnections )
						{
							m_outputPorts[ i ].FullDeleteConnections();
						}
					}
					else
					{
						AddOutputPort( portType, newName );
					}
				}

				if ( textures.Length < m_outputPorts.Count )
				{
					int itemsToRemove = m_outputPorts.Count - textures.Length;
					for ( int i = 0; i < itemsToRemove; i++ )
					{
						int idx = m_outputPorts.Count - 1;
						if ( m_outputPorts[ idx ].IsConnected )
						{
							m_outputPorts[ idx ].ForceClearConnection();
						}
						RemoveOutputPort( idx );
					}
				}
			}
			else
			{
				int itemsToRemove = m_outputPorts.Count - 1;
				m_outputPorts[ 0 ].ChangeProperties( Constants.EmptyPortValue, WirePortDataType.COLOR, false );
				m_outputPorts[ 0 ].ForceClearConnection();

				for ( int i = 0; i < itemsToRemove; i++ )
				{
					int idx = m_outputPorts.Count - 1;
					if ( m_outputPorts[ idx ].IsConnected )
					{
						m_outputPorts[ idx ].ForceClearConnection();
					}
					RemoveOutputPort( idx );
				}
			}

			m_sizeIsDirty = true;
			m_isDirty = true;
			Event.current.Use();
		}

		private void ConfigFromObject( UnityEngine.Object obj )
		{
			ProceduralMaterial newMat = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>( AssetDatabase.GetAssetPath( obj ) );
			if ( newMat != null )
			{
				m_proceduralMaterial = newMat;
				ConfigPortsFromMaterial();
			}
		}

		public override void OnObjectDropped( UnityEngine.Object obj )
		{
			ConfigFromObject( obj );
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			ConfigFromObject( obj );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_proceduralMaterial == null )
			{
				return "(0).xxxx";
			}

			if ( m_outputPorts[ outputId ].IsLocalValue )
			{
				return m_outputPorts[ outputId ].LocalValue;
			}

			Texture[] textures = m_proceduralMaterial.GetGeneratedTextures();

			string uvPropertyName = string.Empty;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				if ( m_outputPorts[ i ].IsConnected )
				{
					uvPropertyName = textures[ i ].name;
					break;
				}
			}

			string name = textures[ outputId ].name + OutputId;
			dataCollector.AddToUniforms( UniqueId, string.Format( GlobalVarDecStr, textures[ outputId ].name ) );
			dataCollector.AddToProperties( UniqueId, string.Format( PropertyDecStr, textures[ outputId ].name ) + "{}", -1 );
			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation );
			string value = string.Format( "tex2D{0}( {1}, {2})", ( isVertex ? "lod" : string.Empty ), textures[ outputId ].name, GetUVCoords( ref dataCollector, ignoreLocalvar, uvPropertyName ) );
			if ( m_autoNormal && m_textureTypes[ outputId ] == ProceduralOutputType.Normal )
			{
				value = string.Format( Constants.UnpackNormal, value );
			}

			dataCollector.AddPropertyNode( this );
			RegisterLocalVariable( outputId, value, ref dataCollector, name );

			return m_outputPorts[ outputId ].LocalValue;
		}

		public string GetUVCoords( ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar, string propertyName )
		{
			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation );
			if ( m_inputPorts[ 0 ].IsConnected )
			{
				return m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, isVertex ? WirePortDataType.FLOAT4 : WirePortDataType.FLOAT2, ignoreLocalVar, true );
			}
			else
			{
				string vertexCoords = Constants.VertexShaderInputStr + ".texcoord";
				if ( m_textureCoordSet > 0 )
				{
					vertexCoords += m_textureCoordSet.ToString();
				}
				string uvChannelName = IOUtils.GetUVChannelName( propertyName, m_textureCoordSet );

				string dummyPropUV = "_texcoord" + ( m_textureCoordSet > 0 ? ( m_textureCoordSet + 1 ).ToString() : "" );
				string dummyUV = "uv" + ( m_textureCoordSet > 0 ? ( m_textureCoordSet + 1 ).ToString() : "" ) + dummyPropUV;

				dataCollector.AddToUniforms( UniqueId, "uniform float4 " + propertyName + "_ST;" );
				dataCollector.AddToProperties( UniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 100 );
				dataCollector.AddToInput( UniqueId, "float2 " + dummyUV, true );

				if ( isVertex )
				{
					dataCollector.AddToVertexLocalVariables( UniqueId, "float4 " + uvChannelName + " = float4(" + vertexCoords + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw, 0 ,0);" );
					return uvChannelName;
				}
				else
					dataCollector.AddToLocalVariables( UniqueId, PrecisionType.Float, WirePortDataType.FLOAT2, uvChannelName, Constants.InputVarStr + "." + dummyUV + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );

				return uvChannelName;
			}
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( m_proceduralMaterial != null )
			{
				Texture[] textures = m_proceduralMaterial.GetGeneratedTextures();
				for ( int i = 0; i < textures.Length; i++ )
				{
					if ( mat.HasProperty( textures[ i ].name ) )
					{
						mat.SetTexture( textures[ i ].name, textures[ i ] );
					}
				}
			}
		}

		public override bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol )
		{
			if ( m_proceduralMaterial != null )
			{
				Texture[] textures = m_proceduralMaterial.GetGeneratedTextures();
				for ( int i = 0; i < textures.Length; i++ )
				{
					defaultCol.AddValue( textures[ i ].name, textures[ i ] );
				}
			}
			return true;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_proceduralMaterial = null;
			m_cacheNodeConnections.Clear();
			m_cacheNodeConnections = null;
			m_outputConns.Clear();
			m_outputConns = null;
		}

		public override string GetPropertyValStr()
		{
			return m_proceduralMaterial ? m_proceduralMaterial.name : string.Empty;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string guid = GetCurrentParam( ref nodeParams );
			m_textureCoordSet = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_autoNormal = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
			if ( guid.Length > 1 )
			{
				m_proceduralMaterial = AssetDatabase.LoadAssetAtPath<ProceduralMaterial>( AssetDatabase.GUIDToAssetPath( guid ) );
				if ( m_proceduralMaterial != null )
				{
					ConfigPortsFromMaterial();
				}
				else
				{
					UIUtils.ShowMessage( "Substance not found ", MessageSeverity.Error );
				}
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			string guid = ( m_proceduralMaterial != null ) ? AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_proceduralMaterial ) ) : "0";
			IOUtils.AddFieldValueToString( ref nodeInfo, guid );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_textureCoordSet );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoNormal );
		}

	}
}

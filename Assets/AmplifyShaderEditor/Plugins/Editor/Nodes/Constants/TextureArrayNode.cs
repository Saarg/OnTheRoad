// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Texture Array", "Textures", "Texture Array", KeyCode.None, true, 0, int.MaxValue, typeof( Texture2DArray ) )]
	public class TextureArrayNode : PropertyNode
	{
		[SerializeField]
		private Texture2DArray m_defaultTextureArray;

		[SerializeField]
		private Texture2DArray m_materialTextureArray;

		[SerializeField]
		private TexReferenceType m_referenceType = TexReferenceType.Object;

		[SerializeField]
		private int m_uvSet = 0;

		[SerializeField]
		private MipType m_mipMode = MipType.Auto;

		private readonly string[] m_mipOptions = { "Auto", "Mip Level" };

		private TextureArrayNode m_referenceSampler = null;

		[SerializeField]
		private int m_referenceArrayId = -1;

		[SerializeField]
		private int m_referenceNodeId = -1;

		[SerializeField]
		private bool m_autoUnpackNormals = false;

		private InputPort m_uvPort;
		private InputPort m_indexPort;
		private InputPort m_lodPort;
		private InputPort m_normalPort;

		private OutputPort m_colorPort;

		private const string AutoUnpackNormalsStr = "Normal";
		private const string NormalScaleStr = "Scale";

		private readonly Color ReferenceHeaderColor = new Color( 2.67f, 1.0f, 0.5f, 1.0f );
		private bool m_forceSamplerUpdate = false;

		private GUIStyle m_titleOverlay;
		private int m_tittleOverlayIndex = -1;

		private int m_cachedUvsId = -1;
		private int m_cachedSamplerId = -1;
		private int m_cachedUnpackId = -1;
		private int m_cachedLodId = -1;

		private bool m_linearTexture;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputColorPorts( "RGBA" );
			m_colorPort = m_outputPorts[ 0 ];
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" );
			AddInputPort( WirePortDataType.FLOAT, false, "Index" );
			AddInputPort( WirePortDataType.FLOAT, false, "Level" );
			AddInputPort( WirePortDataType.FLOAT, false, NormalScaleStr );
			m_inputPorts[ 2 ].Visible = false;
			m_uvPort = m_inputPorts[ 0 ];
			m_indexPort = m_inputPorts[ 1 ];
			m_lodPort = m_inputPorts[ 2 ];
			m_normalPort = m_inputPorts[ 3 ];
			m_normalPort.Visible = m_autoUnpackNormals;
			m_normalPort.FloatInternalData = 1.0f;
			m_insideSize.Set( 110, 110 + 5 );
			m_drawPrecisionUI = false;
			m_currentParameterType = PropertyType.Property;
			m_freeType = false;
			m_showPreview = true;
			m_drawPreviewExpander = false;
			m_drawPreview = false;
			m_customPrefix = "Texture Array ";
			m_selectedLocation = PreviewLocation.TopCenter;
			//m_useCustomPrefix = true;
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );
			m_previewShaderGUID = "2e6d093df2d289f47b827b36efb31a81";
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedUvsId == -1 )
				m_cachedUvsId = Shader.PropertyToID( "_CustomUVs" );

			if ( m_cachedSamplerId == -1 )
				m_cachedSamplerId = Shader.PropertyToID( "_Sampler" );

			if ( m_cachedUnpackId == -1 )
				m_cachedUnpackId = Shader.PropertyToID( "_Unpack" );

			if ( m_cachedLodId == -1 )
				m_cachedLodId = Shader.PropertyToID( "_LodType" );

			PreviewMaterial.SetFloat( m_cachedLodId, ( m_mipMode == MipType.MipLevel ? 1 : 0 ) );
			PreviewMaterial.SetFloat( m_cachedUnpackId, m_autoUnpackNormals ? 1 : 0 );
			if ( m_referenceType == TexReferenceType.Instance && m_referenceSampler != null )
			{
				PreviewMaterial.SetTexture( m_cachedSamplerId, m_referenceSampler.TextureArray );
			}
			else
			{
				PreviewMaterial.SetTexture( m_cachedSamplerId, TextureArray );
			}
			PreviewMaterial.SetFloat( m_cachedUvsId, ( m_uvPort.IsConnected ? 1 : 0 ) );
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();
			if ( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.RegisterTextureArrayNode( this );
				UIUtils.RegisterPropertyNode( this );
			}
		}

		new void ShowDefaults()
		{
			m_uvSet = EditorGUILayoutIntPopup( Constants.AvailableUVSetsLabel, m_uvSet, Constants.AvailableUVSetsStr, Constants.AvailableUVSets );

			MipType newMipMode = ( MipType ) EditorGUILayoutPopup( "Mip Mode", ( int ) m_mipMode, m_mipOptions );
			if ( newMipMode != m_mipMode )
			{
				m_mipMode = newMipMode;
			}

			if ( m_mipMode == MipType.MipLevel )
			{
				m_lodPort.Visible = true;
			}
			else
			{
				m_lodPort.Visible = false;
			}

			if ( !m_lodPort.IsConnected && m_lodPort.Visible )
			{
				m_lodPort.FloatInternalData = EditorGUILayoutFloatField( "Mip Level", m_lodPort.FloatInternalData );
			}

			if ( !m_indexPort.IsConnected )
			{
				m_indexPort.FloatInternalData = EditorGUILayoutFloatField( "Index", m_indexPort.FloatInternalData );
			}
		}

		public override void DrawMainPropertyBlock()
		{
			EditorGUI.BeginChangeCheck();
			//m_referenceType = ( TexReferenceType ) EditorGUILayout.EnumPopup( Constants.ReferenceTypeStr, m_referenceType );
			m_referenceType = ( TexReferenceType ) EditorGUILayoutPopup( Constants.ReferenceTypeStr, ( int ) m_referenceType, Constants.ReferenceArrayLabels );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_referenceType == TexReferenceType.Object )
				{
					UIUtils.RegisterTextureArrayNode( this );
					UIUtils.RegisterPropertyNode( this );

					SetTitleText( m_propertyInspectorName );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					m_referenceArrayId = -1;
					m_referenceNodeId = -1;
					m_referenceSampler = null;
				}
				else
				{
					UIUtils.UnregisterTextureArrayNode( this );
					UIUtils.UnregisterPropertyNode( this );
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
				}
			}
			else
			{
				string[] arr = UIUtils.TextureArrayNodeArr();
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

				m_referenceArrayId = EditorGUILayoutPopup( Constants.AvailableReferenceStr, m_referenceArrayId, arr );
				GUI.enabled = guiEnabledBuffer;
				ShowDefaults();

				DrawSamplerOptions();
			}
		}

		public override void OnPropertyNameChanged()
		{
			base.OnPropertyNameChanged();
			UIUtils.UpdateTextureArrayDataNode( UniqueId, PropertyInspectorName );
		}

		public override void DrawSubProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			m_defaultTextureArray = EditorGUILayoutObjectField( Constants.DefaultValueLabel, m_defaultTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
			}
		}

		public override void DrawMaterialProperties()
		{
			ShowDefaults();

			DrawSamplerOptions();

			EditorGUI.BeginChangeCheck();
			m_materialTextureArray = EditorGUILayoutObjectField( Constants.MaterialValueLabel, m_materialTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
			if ( EditorGUI.EndChangeCheck() )
			{
				CheckTextureImporter( true );
				SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
				m_requireMaterialUpdate = true;
			}
		}

		public void DrawSamplerOptions()
		{
			EditorGUI.BeginChangeCheck();
			bool autoUnpackNormals = EditorGUILayoutToggle( "Normal Map", m_autoUnpackNormals );
			if ( EditorGUI.EndChangeCheck() )
			{
				if ( m_autoUnpackNormals != autoUnpackNormals )
				{
					AutoUnpackNormals = autoUnpackNormals;

					ConfigureInputPorts();
					ConfigureOutputPorts();
				}
			}

			if ( m_autoUnpackNormals && !m_normalPort.IsConnected )
			{
				m_normalPort.FloatInternalData = EditorGUILayoutFloatField( NormalScaleStr, m_normalPort.FloatInternalData );
			}
		}

		public void ConfigureInputPorts()
		{
			m_normalPort.Visible = AutoUnpackNormals;

			//switch ( m_mipMode )
			//{
			//	case MipType.Auto:
			//	m_lodPort.Visible = false;
			//	break;
			//	case MipType.MipLevel:
			//	m_lodPort.Visible = true;
			//	break;
			//}

			m_sizeIsDirty = true;
		}

		public void ConfigureOutputPorts()
		{
			m_outputPorts[ m_colorPort.PortId + 4 ].Visible = !AutoUnpackNormals;

			if ( !AutoUnpackNormals )
			{
				m_colorPort.ChangeProperties( "RGBA", WirePortDataType.FLOAT4, false );
				m_outputPorts[ m_colorPort.PortId + 1 ].ChangeProperties( "R", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 2 ].ChangeProperties( "G", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 3 ].ChangeProperties( "B", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 4 ].ChangeProperties( "A", WirePortDataType.FLOAT, false );

			}
			else
			{
				m_colorPort.ChangeProperties( "XYZ", WirePortDataType.FLOAT3, false );
				m_outputPorts[ m_colorPort.PortId + 1 ].ChangeProperties( "X", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 2 ].ChangeProperties( "Y", WirePortDataType.FLOAT, false );
				m_outputPorts[ m_colorPort.PortId + 3 ].ChangeProperties( "Z", WirePortDataType.FLOAT, false );
			}

			m_sizeIsDirty = true;
		}

		public virtual void CheckTextureImporter( bool additionalCheck )
		{
			m_requireMaterialUpdate = true;
			Texture2DArray texture = m_materialMode ? m_materialTextureArray : m_defaultTextureArray;

			UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath( AssetDatabase.GetAssetPath( texture ), typeof( UnityEngine.Object ) );
			SerializedObject serializedObject = new UnityEditor.SerializedObject( obj );

			if ( serializedObject != null )
			{
				SerializedProperty colorSpace = serializedObject.FindProperty( "m_ColorSpace" );
				m_linearTexture = ( colorSpace.intValue == 0 );
			}
		}

		void UpdateHeaderColor()
		{
			m_headerColorModifier = ( m_referenceType == TexReferenceType.Object ) ? Color.white : ReferenceHeaderColor;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			EditorGUI.BeginChangeCheck();
			base.Draw( drawInfo );
			if ( m_forceSamplerUpdate )
			{
				m_forceSamplerUpdate = false;
				m_referenceSampler = UIUtils.GetNode( m_referenceNodeId ) as TextureArrayNode;
				m_referenceArrayId = UIUtils.GetTextureArrayNodeRegisterId( m_referenceNodeId );
			}
			if ( EditorGUI.EndChangeCheck() )
			{
				OnPropertyNameChanged();
			}

			bool instanced = CheckReference();

			if ( m_referenceType == TexReferenceType.Instance && m_referenceSampler != null )
			{
				SetTitleText( m_referenceSampler.PropertyInspectorName + Constants.InstancePostfixStr );
				SetAdditonalTitleText( m_referenceSampler.AdditonalTitleContent.text );
			}
			else
			{
				SetTitleText( PropertyInspectorName );
				SetAdditonalTitleText( AdditonalTitleContent.text );
			}

			if ( m_tittleOverlayIndex == -1 )
				m_tittleOverlayIndex = Array.IndexOf<GUIStyle>( GUI.skin.customStyles, GUI.skin.GetStyle( "ObjectFieldThumbOverlay" ) );

			m_titleOverlay = GUI.skin.customStyles[ m_tittleOverlayIndex ];

			int fontSizeUpper = m_titleOverlay.fontSize;

			Rect newRect = m_globalPosition;
			newRect.width = ( 128 ) * drawInfo.InvertedZoom;
			newRect.height = ( 128 ) * drawInfo.InvertedZoom;
			newRect.x = m_previewRect.x;
			newRect.y = m_previewRect.y;

			m_titleOverlay.fontSize = ( int ) ( 9 * drawInfo.InvertedZoom );

			Rect smallButton = newRect;
			smallButton.height = 14 * drawInfo.InvertedZoom;
			smallButton.y = newRect.yMax - smallButton.height - 2;
			smallButton.width = 40 * drawInfo.InvertedZoom;
			smallButton.x = newRect.xMax - smallButton.width - 2;

			m_showPreview = true;

			if ( instanced )
			{
				DrawPreview( drawInfo, m_previewRect );

				if ( GUI.Button( newRect, string.Empty, GUIStyle.none ) )
				{
					UIUtils.FocusOnNode( m_referenceSampler, 1, true );
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();

				if ( m_materialMode )
				{
					if ( m_materialTextureArray == null )
					{
						GUI.Box( newRect, "", UIUtils.ObjectFieldThumb );

						Color temp = GUI.color;
						GUI.color = Color.clear;
						m_materialTextureArray = EditorGUIObjectField( newRect, m_materialTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
						GUI.color = temp;

						GUI.Button( smallButton, "Select", UIUtils.GetCustomStyle( CustomStyle.SamplerButton ) );
						if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
							GUI.Label( newRect, "None (Texture2DArray)", UIUtils.ObjectFieldThumbOverlay );
					}
					else
					{
						Rect butRect = m_previewRect;
						butRect.y -= 1;
						butRect.x += 1;

						Rect hitRect = butRect;
						hitRect.height = 14 * drawInfo.InvertedZoom;
						hitRect.y = butRect.yMax - hitRect.height;
						hitRect.width = 4 * 14 * drawInfo.InvertedZoom;

						Color temp = GUI.color;
						GUI.color = Color.clear;
						bool restoreMouse = false;
						if ( Event.current.type == EventType.mouseDown && hitRect.Contains( Event.current.mousePosition ) )
						{
							restoreMouse = true;
							Event.current.type = EventType.ignore;
						}

						m_materialTextureArray = EditorGUIObjectField( newRect, m_materialTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
						if ( restoreMouse )
						{
							Event.current.type = EventType.mouseDown;
						}

						GUI.color = temp;

						DrawPreview( drawInfo, m_previewRect );
						DrawPreviewMaskButtons( drawInfo, butRect );

						GUI.Box( newRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
						GUI.Box( smallButton, "Select", UIUtils.GetCustomStyle( CustomStyle.SamplerButton ) );
					}
				}
				else
				{
					if ( m_defaultTextureArray == null )
					{
						GUI.Box( newRect, "", UIUtils.ObjectFieldThumb );

						Color temp = GUI.color;
						GUI.color = Color.clear;
						m_defaultTextureArray = EditorGUIObjectField( newRect, m_defaultTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
						GUI.color = temp;

						GUI.Button( smallButton, "Select", UIUtils.GetCustomStyle( CustomStyle.SamplerButton ) );
						if ( ContainerGraph.LodLevel <= ParentGraph.NodeLOD.LOD2 )
							GUI.Label( newRect, "None (Texture2DArray)", UIUtils.ObjectFieldThumbOverlay );
					}
					else
					{
						Rect butRect = m_previewRect;
						butRect.y -= 1;
						butRect.x += 1;

						Rect hitRect = butRect;
						hitRect.height = 14 * drawInfo.InvertedZoom;
						hitRect.y = butRect.yMax - hitRect.height;
						hitRect.width = 4 * 14 * drawInfo.InvertedZoom;

						Color temp = GUI.color;
						GUI.color = Color.clear;
						bool restoreMouse = false;
						if ( Event.current.type == EventType.mouseDown && hitRect.Contains( Event.current.mousePosition ) )
						{
							restoreMouse = true;
							Event.current.type = EventType.ignore;
						}

						m_defaultTextureArray = EditorGUIObjectField( newRect, m_defaultTextureArray, typeof( Texture2DArray ), false ) as Texture2DArray;
						if ( restoreMouse )
						{
							Event.current.type = EventType.mouseDown;
						}

						GUI.color = temp;

						DrawPreview( drawInfo, m_previewRect );
						DrawPreviewMaskButtons( drawInfo, butRect );

						GUI.Box( newRect, string.Empty, UIUtils.GetCustomStyle( CustomStyle.SamplerFrame ) );
						GUI.Box( smallButton, "Select", UIUtils.GetCustomStyle( CustomStyle.SamplerButton ) );
					}
				}

				//if ( m_materialMode )
				//	m_materialTextureArray = ( Texture2DArray ) EditorGUI.ObjectField( newRect, m_materialTextureArray, typeof( Texture2DArray ), false );
				//else
				//	m_defaultTextureArray = ( Texture2DArray ) EditorGUI.ObjectField( newRect, m_defaultTextureArray, typeof( Texture2DArray ), false );

				if ( EditorGUI.EndChangeCheck() )
				{
					CheckTextureImporter( true );
					SetAdditonalTitleText( string.Format( Constants.PropertyValueLabel, GetPropertyValStr() ) );
					BeginDelayedDirtyProperty();
					m_requireMaterialUpdate = true;
				}

				m_titleOverlay.fontSize = fontSizeUpper;
			}

		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );


			OnPropertyNameChanged();

			CheckReference();

			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation );

			bool instanced = false;
			if ( m_referenceType == TexReferenceType.Instance && m_referenceSampler != null )
				instanced = true;

			if ( !instanced )
				base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );

			string level = string.Empty;
			if ( m_lodPort.Visible )
			{
				level = m_lodPort.GeneratePortInstructions( ref dataCollector );
			}

			if ( isVertex && !m_lodPort.Visible )
				level = "0";


			string uvs = string.Empty;
			if ( m_uvPort.IsConnected )
			{
				uvs = m_uvPort.GeneratePortInstructions( ref dataCollector );
			}
			else
			{
				if ( isVertex )
					uvs = TexCoordVertexDataNode.GenerateVertexUVs( ref dataCollector, UniqueId, m_uvSet, ( instanced ? m_referenceSampler.PropertyName : PropertyName ) );
				else
					uvs = TexCoordVertexDataNode.GenerateFragUVs( ref dataCollector, UniqueId, m_uvSet, ( instanced ? m_referenceSampler.PropertyName : PropertyName ) );
			}
			string index = m_indexPort.GeneratePortInstructions( ref dataCollector );

			int connectionNumber = 0;
			for ( int i = 0; i < m_outputPorts.Count; i++ )
			{
				connectionNumber += m_outputPorts[ i ].ConnectionCount;
			}

			string propertyName = string.Empty;
			if ( !instanced )
				propertyName = PropertyName;
			else
				propertyName = m_referenceSampler.PropertyName;


			string m_normalMapUnpackMode = "";
			if ( m_autoUnpackNormals )
			{
				bool isScaledNormal = false;
				if ( m_normalPort.IsConnected )
				{
					isScaledNormal = true;
				}
				else
				{
					if ( m_normalPort.FloatInternalData != 1 )
					{
						isScaledNormal = true;
					}
				}
				if ( isScaledNormal )
				{
					string scaleValue = m_normalPort.GeneratePortInstructions( ref dataCollector );
					dataCollector.AddToIncludes( UniqueId, Constants.UnityStandardUtilsLibFuncs );
					m_normalMapUnpackMode = "UnpackScaleNormal( {0} ," + scaleValue + " )";
				}
				else
				{
					m_normalMapUnpackMode = "UnpackNormal( {0} )";
				}
			}

			string result = "UNITY_SAMPLE_TEX2DARRAY" + ( m_lodPort.Visible || isVertex ? "_LOD" : "" ) + "(" + propertyName + ", float3(" + uvs + ", " + index + ") " + ( m_lodPort.Visible || isVertex ? ", " + level : "" ) + " )";
			if ( m_autoUnpackNormals )
				result = string.Format( m_normalMapUnpackMode, result );

			//if ( connectionNumber > 1 )
			//{
			RegisterLocalVariable( 0, result, ref dataCollector, "texArray" + OutputId );
			//dataCollector.AddToLocalVariables( UniqueId, "float" + ( m_autoUnpackNormals ? "3" : "4" ) + " texArray" + m_uniqueId + " = " + result + ";" );
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			//}
			//else
			//{

			//	return GetOutputVectorItem( 0, outputId, result );
			//}
		}

		public override string GetPropertyValue()
		{
			return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", 2DArray ) = \"\" {}";
		}

		public override void GetUniformData( out string dataType, out string dataName )
		{
			dataType = "UNITY_DECLARE_TEX2DARRAY(";
			dataName = m_propertyName + " )";
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			string textureName = GetCurrentParam( ref nodeParams );
			m_defaultTextureArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>( textureName );
			m_uvSet = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_referenceType = ( TexReferenceType ) Enum.Parse( typeof( TexReferenceType ), GetCurrentParam( ref nodeParams ) );
			m_referenceNodeId = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( UIUtils.CurrentShaderVersion() > 3202 )
				m_mipMode = ( MipType ) Enum.Parse( typeof( MipType ), GetCurrentParam( ref nodeParams ) );
			if ( UIUtils.CurrentShaderVersion() > 5105 )
				m_autoUnpackNormals = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );

			if ( m_referenceType == TexReferenceType.Instance )
			{
				UIUtils.UnregisterTextureArrayNode( this );
				UIUtils.UnregisterPropertyNode( this );
				m_forceSamplerUpdate = true;
			}

			ConfigureInputPorts();
			ConfigureOutputPorts();

			m_lodPort.Visible = ( m_mipMode == MipType.MipLevel );

			UpdateHeaderColor();

			if ( m_defaultTextureArray )
			{
				m_materialTextureArray = m_defaultTextureArray;
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_defaultTextureArray != null ) ? AssetDatabase.GetAssetPath( m_defaultTextureArray ) : Constants.NoStringValue );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_uvSet.ToString() );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_referenceType );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( ( m_referenceSampler != null ) ? m_referenceSampler.UniqueId : -1 ) );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_mipMode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_autoUnpackNormals );
		}

		public override void ReadAdditionalClipboardData( ref string[] nodeParams )
		{
			base.ReadAdditionalClipboardData( ref nodeParams );
			string textureName = GetCurrentParam( ref nodeParams );
			m_materialTextureArray = AssetDatabase.LoadAssetAtPath<Texture2DArray>( textureName );
		}

		public override void WriteAdditionalClipboardData( ref string nodeInfo )
		{
			base.WriteAdditionalClipboardData( ref nodeInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, ( m_materialTextureArray != null ) ? AssetDatabase.GetAssetPath( m_materialTextureArray ) : Constants.NoStringValue );
		}


		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				OnPropertyNameChanged();
				if ( mat.HasProperty( PropertyName ) )
				{
					mat.SetTexture( PropertyName, m_materialTextureArray );
				}
			}
		}

		public override void SetMaterialMode( Material mat , bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat , fetchMaterialValues );
			if ( fetchMaterialValues && m_materialMode && UIUtils.IsProperty( m_currentParameterType ) )
			{
				if ( mat.HasProperty( PropertyName ) )
				{
					m_materialTextureArray = ( Texture2DArray ) mat.GetTexture( PropertyName );
					if ( m_materialTextureArray == null )
						m_materialTextureArray = m_defaultTextureArray;
				}
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( PropertyName ) )
			{
				m_materialTextureArray = ( Texture2DArray ) material.GetTexture( PropertyName );
				if ( m_materialTextureArray == null )
					m_materialTextureArray = m_defaultTextureArray;
			}
		}

		public override bool UpdateShaderDefaults( ref Shader shader, ref TextureDefaultsDataColector defaultCol )
		{
			if ( m_defaultTextureArray != null )
			{
				defaultCol.AddValue( PropertyName, m_defaultTextureArray );
			}

			return true;
		}

		public override string GetPropertyValStr()
		{
			return m_materialMode ? ( m_materialTextureArray != null ? m_materialTextureArray.name : IOUtils.NO_TEXTURES ) : ( m_defaultTextureArray != null ? m_defaultTextureArray.name : IOUtils.NO_TEXTURES );
		}

		public bool CheckReference()
		{
			if ( m_referenceType == TexReferenceType.Instance && m_referenceArrayId > -1 )
			{
				m_referenceSampler = UIUtils.GetTextureArrayNode( m_referenceArrayId );

				if ( m_referenceSampler == null )
					m_referenceArrayId = -1;
			}

			return m_referenceSampler != null;
		}

		public override void SetupFromCastObject( UnityEngine.Object obj )
		{
			base.SetupFromCastObject( obj );
			SetupFromObject( obj );
		}

		public override void OnObjectDropped( UnityEngine.Object obj )
		{
			SetupFromObject( obj );
		}

		void SetupFromObject( UnityEngine.Object obj )
		{
			if ( m_materialMode )
			{
				m_materialTextureArray = obj as Texture2DArray;
			}
			else
			{
				m_defaultTextureArray = obj as Texture2DArray;
			}
		}

		public Texture2DArray TextureArray
		{
			get { return ( m_materialMode ? m_materialTextureArray : m_defaultTextureArray ); }
		}

		public bool IsLinearTexture
		{
			get
			{
				return m_linearTexture;
			}
		}

		public bool AutoUnpackNormals
		{
			get { return m_autoUnpackNormals; }
			set
			{
				m_autoUnpackNormals = value;
			}
		}

		public override string DataToArray { get { return PropertyInspectorName; } }

		public override void Destroy()
		{
			base.Destroy();
			m_defaultTextureArray = null;
			m_materialTextureArray = null;

			if ( m_referenceType == TexReferenceType.Object )
			{
				UIUtils.UnregisterTextureArrayNode( this );
				UIUtils.UnregisterPropertyNode( this );
			}
		}
	}
}

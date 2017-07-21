// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum BuiltInFogAndAmbientColors
	{
		UNITY_LIGHTMODEL_AMBIENT = 0,
		unity_AmbientSky,
		unity_AmbientEquator,
		unity_AmbientGround,
		unity_FogColor
	}

	[Serializable]
	[NodeAttributes( "Fog And Ambient Colors", "Fog And Ambient", "Fog and Ambient colors" )]
	public sealed class FogAndAmbientColorsNode : ShaderVariablesNode
	{
		[SerializeField]
		private BuiltInFogAndAmbientColors m_selectedType = BuiltInFogAndAmbientColors.UNITY_LIGHTMODEL_AMBIENT;
		[SerializeField]
		private BuiltInFogAndAmbientColors m_oldVarType = BuiltInFogAndAmbientColors.UNITY_LIGHTMODEL_AMBIENT;

		private const string ColorLabelStr = "Color";
		private readonly string[] ColorValuesStr = {
														"Ambient light ( Legacy )",
														"Sky ambient light",
														"Equator ambient light",
														"Ground ambient light",
														"Fog"
													};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, ColorValuesStr[ ( int ) m_selectedType ], WirePortDataType.COLOR );
			m_textLabelWidth = 50;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_selectedType = ( BuiltInFogAndAmbientColors ) EditorGUILayoutPopup( ColorLabelStr, ( int ) m_selectedType, ColorValuesStr );

			if ( m_selectedType != m_oldVarType )
			{
				m_oldVarType = m_selectedType;
				ChangeOutputName( 0, ColorValuesStr[ ( int ) m_selectedType ] );
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			return m_selectedType.ToString();
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedType = ( BuiltInFogAndAmbientColors ) Enum.Parse( typeof( BuiltInFogAndAmbientColors ), GetCurrentParam( ref nodeParams ) );
			ChangeOutputName( 0, ColorValuesStr[ ( int ) m_selectedType ] );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedType );
		}
	}
}

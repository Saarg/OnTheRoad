// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum BuiltInShaderClipPlanesTypes
	{
		Left = 0,
		Right,
		Bottom,
		Top,
		Near,
		Far
	}

	[Serializable]
	[NodeAttributes( "Clip Planes", "Camera And Screen", "Camera World Clip Planes" )]
	public sealed class CameraWorldClipPlanes : ShaderVariablesNode
	{
		[SerializeField]
		private BuiltInShaderClipPlanesTypes m_selectedType = BuiltInShaderClipPlanesTypes.Left;
		[SerializeField]
		private BuiltInShaderClipPlanesTypes m_oldVarType = BuiltInShaderClipPlanesTypes.Left;

		private const string LabelStr = "Plane";
		private const string ValueStr = "unity_CameraWorldClipPlanes";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "ABCD", WirePortDataType.FLOAT4 );
			m_textLabelWidth = 55;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			m_selectedType = ( BuiltInShaderClipPlanesTypes ) EditorGUILayoutEnumPopup( LabelStr, m_selectedType );
			EditorGUILayout.EndVertical();

			if ( m_selectedType != m_oldVarType )
			{
				m_oldVarType = m_selectedType;
				SetSaveIsDirty();
			}
		}
		
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			return ValueStr + "[" + ( int ) m_selectedType + "]";
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_selectedType = ( BuiltInShaderClipPlanesTypes ) Enum.Parse( typeof( BuiltInShaderClipPlanesTypes ), GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_selectedType );
		}
	}
}

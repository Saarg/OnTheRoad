// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	public class RenderingOptionsOpHelper
	{
		private const string RenderingOptionsStr = " Rendering Options";

		[SerializeField]
		private List<CodeGenerationData> m_codeGenerationDataList;
		public RenderingOptionsOpHelper()
		{
			m_codeGenerationDataList = new List<CodeGenerationData>();
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Deferred", "exclude_path:deferred" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Forward", "exclude_path:forward" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " Exclude Legacy Deferred", "exclude_path:prepass" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Shadows", "noshadow" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Ambient Light", "noambient" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Per Vertex Light", "novertexlights" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Lightmaps", "nolightmap " ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Dynamic Global GI", "nodynlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Directional lightmaps", "nodirlightmap" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Built-in Fog", "nofog" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Meta", "nometa" ) );
			m_codeGenerationDataList.Add( new CodeGenerationData( " No Add Pass", "noforwardadd" ) );
		}

		public void Draw( UndoParentNode owner )
		{
			bool value = EditorVariablesManager.ExpandedRenderingOptions.Value;
			NodeUtils.DrawPropertyGroup( ref value, RenderingOptionsStr, () =>
			  {
				  int codeGenCount = m_codeGenerationDataList.Count;
				 // Starting from index 4 because other options are already contemplated with m_renderPath and add/receive shadows
				 for ( int i = 4; i < codeGenCount; i++ )
				  {
					  m_codeGenerationDataList[ i ].IsActive = owner.EditorGUILayoutToggleLeft( m_codeGenerationDataList[ i ].Name, m_codeGenerationDataList[ i ].IsActive );
				  }
			  } );
			EditorVariablesManager.ExpandedRenderingOptions.Value = value;
		}

		public void Build( ref string OptionalParameters )
		{
			int codeGenCount = m_codeGenerationDataList.Count;

			for ( int i = 0; i < codeGenCount; i++ )
			{
				if ( m_codeGenerationDataList[ i ].IsActive )
				{
					OptionalParameters += m_codeGenerationDataList[ i ].Value + Constants.OptionalParametersSep;
				}
			}
		}

		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				m_codeGenerationDataList[ i ].IsActive = Convert.ToBoolean( nodeParams[ index++ ] );
			}
		}

		public void WriteToString( ref string nodeInfo )
		{
			for ( int i = 0; i < m_codeGenerationDataList.Count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_codeGenerationDataList[ i ].IsActive );
			}
		}

		public void Destroy()
		{
			m_codeGenerationDataList.Clear();
			m_codeGenerationDataList = null;
		}
	}
}

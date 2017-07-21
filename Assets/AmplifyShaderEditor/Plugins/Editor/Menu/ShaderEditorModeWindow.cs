// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public sealed class ShaderEditorModeWindow : MenuParent
	{
		private static readonly Color OverallColorOn = new Color( 1f, 1f, 1f, 0.9f );
		private static readonly Color OverallColorOff = new Color( 1f, 1f, 1f, 0.3f );
		private static readonly Color FontColorOff = new Color( 1f, 1f, 1f, 0.4f );
		private const float DeltaY = 15;
		private const float DeltaX = 10;

		private const float CollSizeX = 180;
		private const float CollSizeY = 70;

		private const string CurrMatStr = "MATERIAL";
		private const string CurrShaderStr = "SHADER";

		private const string NoMaterialStr = "No Material";
		private const string NoShaderStr = "No Shader";

		private bool m_init = true;
		private GUIStyle m_materialLabelStyle;
		private GUIStyle m_shaderLabelStyle;

		//private GUIStyle m_rightStyle;
		//private Texture2D m_shaderOnTexture;
		//private Texture2D m_materialOnTexture;

		private GUIContent m_materialContent;
		private GUIContent m_shaderContent;

		private Vector2 m_auxVector2;
		private GUIContent m_auxContent;
		private Rect m_buttonPos;

		public ShaderEditorModeWindow( AmplifyShaderEditorWindow parentWindow ) : base( parentWindow, 0, 0, 0, 0, "ShaderEditorModeWindow", MenuAnchor.BOTTOM_CENTER, MenuAutoSize.NONE ) { }

		public void ConfigStyle( GUIStyle style )
		{
			style.normal.textColor = FontColorOff;
			style.hover.textColor = FontColorOff;
			style.active.textColor = FontColorOff;
			style.focused.textColor = FontColorOff;

			style.onNormal.textColor = FontColorOff;
			style.onHover.textColor = FontColorOff;
			style.onActive.textColor = FontColorOff;
			style.onFocused.textColor = FontColorOff;
		}


		public void Draw( Rect _graphArea, Vector2 mousePos, Shader currentShader, Material currentMaterial, float usableArea, float leftPos, float rightPos /*, bool showLastSelection*/ )
		{
			if ( m_init )
			{
				m_init = false;
				GUIStyle shaderModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderModeTitle );
				GUIStyle shaderModeNoShader = UIUtils.GetCustomStyle( CustomStyle.ShaderModeNoShader );
				GUIStyle materialModeTitle = UIUtils.GetCustomStyle( CustomStyle.MaterialModeTitle );
				GUIStyle shaderNoMaterialModeTitle = UIUtils.GetCustomStyle( CustomStyle.ShaderNoMaterialModeTitle );

				ConfigStyle( shaderModeTitle );
				ConfigStyle( shaderModeNoShader );
				ConfigStyle( materialModeTitle );
				ConfigStyle( shaderNoMaterialModeTitle );

				m_materialLabelStyle = new GUIStyle( shaderNoMaterialModeTitle );
				m_materialLabelStyle.contentOffset = new Vector2( m_materialLabelStyle.contentOffset.x, -m_materialLabelStyle.contentOffset.y );
				m_materialLabelStyle.fontSize += 6;

				//m_rightStyle = new GUIStyle( UIUtils.GetCustomStyle( CustomStyle.RightShaderMode ) );
				//m_shaderOnTexture = UIUtils.GetCustomStyle( CustomStyle.ShaderModeTitle ).normal.background;
				//m_materialOnTexture = UIUtils.GetCustomStyle( CustomStyle.MaterialModeTitle ).normal.background;

				m_shaderLabelStyle = new GUIStyle( shaderModeTitle );
				m_shaderLabelStyle.contentOffset = new Vector2( m_shaderLabelStyle.contentOffset.x, -m_shaderLabelStyle.contentOffset.y );
				m_shaderLabelStyle.fontSize += 6;

				m_materialContent = new GUIContent( CurrMatStr, "Select current material on Project view" );
				m_shaderContent = new GUIContent( CurrShaderStr, "Select current shader on Project view" );
			}
			Color buffereredColor = GUI.color;
			//Shader Mode
			if ( ParentWindow.CurrentGraph.CurrentMasterNode != null )
			{
				GUIStyle style = UIUtils.GetCustomStyle( currentShader == null ? CustomStyle.ShaderModeNoShader : CustomStyle.ShaderModeTitle );
				Texture2D shaderTex = style.normal.background;
				Rect shaderPos = _graphArea;
				float deltaMat = DeltaX + leftPos;
				shaderPos.x = deltaMat;
				shaderPos.y += shaderPos.height - shaderTex.height - DeltaY;
				shaderPos.width = shaderTex.width;
				shaderPos.height = shaderTex.height;

				Rect collArea = _graphArea;
				collArea.x = leftPos;
				collArea.y += collArea.height - CollSizeY;
				collArea.width = CollSizeX;
				collArea.height = CollSizeY;
				
				string shaderName = ( currentShader != null ) ? ( currentShader.name ) : NoShaderStr;
				GUI.color = collArea.Contains( mousePos ) ? OverallColorOn : OverallColorOff;
				GUI.Box( shaderPos, m_shaderContent, m_shaderLabelStyle );
				GUI.Box( shaderPos, shaderName, style );
				if ( GUI.Button( collArea, string.Empty, m_empty ) && currentShader != null )
				{
					Selection.activeObject = currentShader;
					EditorGUIUtility.PingObject( Selection.activeObject );
				}
			}

			// Material Mode
			if ( ParentWindow.CurrentGraph.CurrentMasterNode != null )
			{
				GUIStyle style = UIUtils.GetCustomStyle( currentMaterial == null ? CustomStyle.ShaderNoMaterialModeTitle : CustomStyle.MaterialModeTitle );
				Texture2D shaderTex = style.normal.background;

				Rect materialPos = _graphArea;
				float deltaShader = DeltaX + rightPos;
				materialPos.x += materialPos.width - shaderTex.width - deltaShader;
				materialPos.y += materialPos.height - shaderTex.height - DeltaY;
				materialPos.width = shaderTex.width;
				materialPos.height = shaderTex.height;
				
				Rect collArea = _graphArea;
				collArea.x += collArea.width - rightPos - CollSizeX;
				collArea.y += collArea.height - CollSizeY;
				collArea.width = CollSizeX;
				collArea.height = CollSizeY;

				GUI.color = collArea.Contains( mousePos ) ? OverallColorOn : OverallColorOff;
				
				string matName = ( currentMaterial != null ) ? ( currentMaterial.name ) : NoMaterialStr;
				GUI.Box( materialPos, m_materialContent, m_materialLabelStyle );
				GUI.Box( materialPos, matName, style );
				
				if ( GUI.Button( collArea, string.Empty, m_empty ) && currentMaterial != null )
				{
					Selection.activeObject = currentMaterial;
					EditorGUIUtility.PingObject( Selection.activeObject );
				}
			}

			// Shader Function
			if ( ParentWindow.CurrentGraph.CurrentMasterNode == null && ParentWindow.CurrentGraph.CurrentOutputNode != null)
			{
				GUIStyle style = UIUtils.GetCustomStyle( CustomStyle.ShaderFunctionMode );
				m_buttonPos = _graphArea;
				m_buttonPos.x = 10 + leftPos;
				m_buttonPos.y += m_buttonPos.height - 38 - 15;
				string functionName = ( ParentWindow.CurrentGraph.CurrentShaderFunction != null ) ? ( ParentWindow.CurrentGraph.CurrentShaderFunction.name ) : "No Shader Function";
				m_auxContent = new GUIContent( "<size=20>SHADER FUNCTION</size>\n"+ functionName );
				m_auxVector2 = style.CalcSize( m_auxContent );
				m_buttonPos.width = m_auxVector2.x + 30 + 4;
				m_buttonPos.height = 38;

				GUI.color = m_buttonPos.Contains( mousePos ) ? OverallColorOn : OverallColorOff;

				if ( GUI.Button( m_buttonPos, m_auxContent, style ) && ParentWindow.CurrentGraph.CurrentShaderFunction != null)
				{
					Selection.activeObject = ParentWindow.CurrentGraph.CurrentShaderFunction;
					EditorGUIUtility.PingObject( Selection.activeObject );
				}

				//if ( showLastSelection )
				//{
				//	string path = EditorPrefs.GetString( IOUtils.LAST_OPENED_OBJ_ID, "" );
				//	string objname = path.Remove( 0, path.LastIndexOf( "/" ) + 1 );
				//	string ext = objname.Remove( 0, objname.LastIndexOf( "." ) );

				//	if ( ext.Equals( ".mat" ) || ext.Equals( ".shader" ) )
				//	{
				//		if ( ext.Equals( ".shader" ) )
				//		{
				//			m_rightStyle.normal.background = m_shaderOnTexture;
				//		}
				//		else
				//		{
				//			m_rightStyle.normal.background = m_materialOnTexture;
				//		}

				//		m_auxContent = new GUIContent( "<size=20>RETURN</size>\n" + objname.Replace( ext, "" ) );
				//		m_auxVector2 = m_rightStyle.CalcSize( m_auxContent );

				//		Rect newpos = m_buttonPos;
				//		newpos.width = m_auxVector2.x;
				//		newpos.height = m_auxVector2.y;
				//		newpos.x = _graphArea.xMax - newpos.width - rightPos - 10;
				//		newpos.y = _graphArea.yMax - newpos.height - 15;//m_buttonPos.width;//rightPos;//_graphArea.width - rightPos - m_buttonPos.width;

				//		GUI.color = newpos.Contains( mousePos ) ? OverallColorOn : OverallColorOff;

				//		if ( GUI.Button( newpos, m_auxContent, m_rightStyle ) )
				//		{
				//			Object loadedObj = AssetDatabase.LoadAssetAtPath<Object>( path );
				//			Shader selectedShader = loadedObj as Shader;
				//			if ( selectedShader != null )
				//			{
				//				if ( IOUtils.IsASEShader( selectedShader ) )
				//				{
				//					AmplifyShaderEditorWindow.ConvertShaderToASE( selectedShader );
				//				}
				//			}
				//			else
				//			{
				//				Material mat = Selection.activeObject as Material;
				//				if ( mat != null )
				//				{
				//					if ( IOUtils.IsASEShader( mat.shader ) )
				//					{
				//						AmplifyShaderEditorWindow.LoadMaterialToASE( mat );
				//					}
				//				}
				//			}
				//		}
				//	}
				//}
			}

			GUI.color = buffereredColor;
		}

		public override void Destroy()
		{
			base.Destroy();
			m_materialLabelStyle = null;
			m_shaderLabelStyle = null;
		}
	}
}

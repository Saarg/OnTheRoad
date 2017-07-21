// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public class PaletteFilterData
	{
		public bool Visible;
		public bool HasCommunityData;
		public List<ContextMenuItem> Contents;
		public PaletteFilterData( bool visible )
		{
			Visible = visible;
			Contents = new List<ContextMenuItem>();
		}
	}

	public class PaletteParent : MenuParent
	{
		private const float ItemSize = 18;
		public delegate void OnPaletteNodeCreate( Type type, string name, AmplifyShaderFunction function );
		public event OnPaletteNodeCreate OnPaletteNodeCreateEvt;

		private string m_searchFilterStr = "Search";
		protected string m_searchFilterControl = "SHADERNAMETEXTFIELDCONTROLNAME";
		protected bool m_focusOnSearch = false;
		protected bool m_defaultCategoryVisible = false;

		//protected List<ContextMenuItem> m_allItems;
		protected List<ContextMenuItem> m_currentItems;
		protected Dictionary<string, PaletteFilterData> m_currentCategories;
		private bool m_forceUpdate = true;


		protected string m_searchFilter;
		
		private float m_searchLabelSize = -1;
		private GUIStyle m_buttonStyle;
		private GUIStyle m_foldoutStyle;

		protected bool m_previousWindowIsFunction = false;

		protected int m_validButtonId = 0;
		protected int m_initialSeparatorAmount = 1;

		private Vector2 _currScrollBarDims = new Vector2( 1, 1 );
		
		public PaletteParent( AmplifyShaderEditorWindow parentWindow, float x, float y, float width, float height, string name, MenuAnchor anchor = MenuAnchor.NONE, MenuAutoSize autoSize = MenuAutoSize.NONE ) : base( parentWindow, x, y, width, height, name, anchor, autoSize )
		{
			m_searchFilter = string.Empty;
			m_currentCategories = new Dictionary<string, PaletteFilterData>();
			//m_allItems = items;
			m_currentItems = new List<ContextMenuItem>();
		}

		public virtual void OnEnterPressed() { }
		public virtual void OnEscapePressed() { }
		
		public void FireNodeCreateEvent( Type type, string name, AmplifyShaderFunction function )
		{
			OnPaletteNodeCreateEvt( type, name, function );
		}

		public override void Draw( Rect parentPosition, Vector2 mousePosition, int mouseButtonId, bool hasKeyboadFocus )
		{
			base.Draw( parentPosition, mousePosition, mouseButtonId, hasKeyboadFocus );
			if ( m_previousWindowIsFunction != ParentWindow.IsShaderFunctionWindow )
			{
				m_forceUpdate = true;
			}

			m_previousWindowIsFunction = ParentWindow.IsShaderFunctionWindow;

			List<ContextMenuItem> allItems = ParentWindow.ContextMenuInstance.MenuItems;

			if ( m_searchLabelSize < 0 )
			{
				m_searchLabelSize = GUI.skin.label.CalcSize( new GUIContent( m_searchFilterStr ) ).x;
			}

			if ( m_foldoutStyle == null )
			{
				m_foldoutStyle = new GUIStyle( GUI.skin.GetStyle( "foldout" ) );
				m_foldoutStyle.fontStyle = FontStyle.Bold;
			}

			if ( m_buttonStyle == null )
			{
				m_buttonStyle = UIUtils.Label;
			}
			
			GUILayout.BeginArea( m_transformedArea, m_content, m_style );
			{

				for ( int i = 0; i < m_initialSeparatorAmount; i++ )
				{
					EditorGUILayout.Separator();
				}

				if ( Event.current.type == EventType.keyDown )
				{
					KeyCode key = Event.current.keyCode;
					if ( key == KeyCode.Return || key == KeyCode.KeypadEnter )
						OnEnterPressed();

					if ( key == KeyCode.Escape )
						OnEscapePressed();

					if ( m_isMouseInside || hasKeyboadFocus )
					{
						if ( key == ShortcutsManager.ScrollUpKey )
						{
							m_currentScrollPos.y -= 10;
							if ( m_currentScrollPos.y < 0 )
							{
								m_currentScrollPos.y = 0;
							}
							Event.current.Use();
						}

						if ( key == ShortcutsManager.ScrollDownKey )
						{
							m_currentScrollPos.y += 10;
							Event.current.Use();
						}
					}

				}

				float width = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = m_searchLabelSize;
				EditorGUI.BeginChangeCheck();
				{
					GUI.SetNextControlName( m_searchFilterControl );
					m_searchFilter = EditorGUILayout.TextField( m_searchFilterStr, m_searchFilter );
					if ( m_focusOnSearch )
					{
						m_focusOnSearch = false;
						EditorGUI.FocusTextInControl( m_searchFilterControl );
					}
				}
				if ( EditorGUI.EndChangeCheck() )
					m_forceUpdate = true;

				EditorGUIUtility.labelWidth = width;
				bool usingSearchFilter = ( m_searchFilter.Length == 0 );
				_currScrollBarDims.x = m_transformedArea.width;
				_currScrollBarDims.y = m_transformedArea.height;
				m_currentScrollPos = EditorGUILayout.BeginScrollView( m_currentScrollPos, GUILayout.Width( 0 ), GUILayout.Height( 0 ) );
				{
					if ( m_forceUpdate )
					{
						m_forceUpdate = false;

						m_currentItems.Clear();
						m_currentCategories.Clear();

						if ( usingSearchFilter )
						{
							for ( int i = 0; i < allItems.Count; i++ )
							{
								m_currentItems.Add( allItems[ i ] );
								if ( !m_currentCategories.ContainsKey( allItems[ i ].Category ) )
								{
									m_currentCategories.Add( allItems[ i ].Category, new PaletteFilterData( m_defaultCategoryVisible ) );
									m_currentCategories[ allItems[ i ].Category ].HasCommunityData = allItems[ i ].NodeAttributes.FromCommunity || m_currentCategories[ allItems[ i ].Category ].HasCommunityData;
								}
								m_currentCategories[ allItems[ i ].Category ].Contents.Add( allItems[ i ] );
							}
						}
						else
						{
							for ( int i = 0; i < allItems.Count; i++ )
							{
								if ( allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 ||
										allItems[ i ].Category.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0
									)
								{
									m_currentItems.Add( allItems[ i ] );
									if ( !m_currentCategories.ContainsKey( allItems[ i ].Category ) )
									{
										m_currentCategories.Add( allItems[ i ].Category, new PaletteFilterData( m_defaultCategoryVisible ) );
										m_currentCategories[ allItems[ i ].Category ].HasCommunityData = allItems[ i ].NodeAttributes.FromCommunity || m_currentCategories[ allItems[ i ].Category ].HasCommunityData;
									}
									m_currentCategories[ allItems[ i ].Category ].Contents.Add( allItems[ i ] );
								}
							}
						}
						var categoryEnumerator = m_currentCategories.GetEnumerator();
						while ( categoryEnumerator.MoveNext() )
						{
							categoryEnumerator.Current.Value.Contents.Sort( ( x, y ) => x.CompareTo( y, usingSearchFilter ) );
						}
					}

					float currPos = 0;
					var enumerator = m_currentCategories.GetEnumerator();
					while ( enumerator.MoveNext() )
					{
						var current = enumerator.Current;
						bool visible = GUILayout.Toggle( current.Value.Visible, current.Key, m_foldoutStyle );
						if ( m_validButtonId == mouseButtonId )
						{
							current.Value.Visible = visible;
						}

						currPos += ItemSize;
						if ( m_searchFilter.Length > 0 || current.Value.Visible )
						{
							for ( int i = 0; i < current.Value.Contents.Count; i++ )
							{
								if ( !IsItemVisible( currPos ) )
								{
									// Invisible
									GUILayout.Space( ItemSize );
								}
								else
								{
									// Visible
									EditorGUILayout.BeginHorizontal();
									GUILayout.Space( 16 );
									if ( m_isMouseInside )
									{
										if ( CheckButton( current.Value.Contents[ i ].ItemUIContent, m_buttonStyle, mouseButtonId ) )
										{
											int controlID = GUIUtility.GetControlID( FocusType.Passive );
											GUIUtility.hotControl = controlID;
											OnPaletteNodeCreateEvt( current.Value.Contents[ i ].NodeType, current.Value.Contents[ i ].Name, current.Value.Contents[ i ].Function );
										}
									}
									else
									{
										GUILayout.Label( current.Value.Contents[ i ].ItemUIContent, m_buttonStyle );
									}
									EditorGUILayout.EndHorizontal();
								}
								currPos += ItemSize;
							}
						}
					}
				}
				EditorGUILayout.EndScrollView();
			}
			GUILayout.EndArea();
			
		}

		public void DumpAvailableNodes( bool fromCommunity, string pathname )
		{
			string noTOCHeader = "__NOTOC__\n";
			string nodesHeader = "== Available Node Categories ==\n";
			string InitialCategoriesFormat = "[[#{0}|{0}]]<br>\n";
			string InitialCategories = string.Empty;
			string CurrentCategoryFormat = "\n== {0} ==\n\n";
			string NodesFootFormat = "[[Unity Products:Amplify Shader Editor/{0} | Learn More]] -\n[[#Top|Back to Categories]]\n";
			string NodesFootFormatSep = "----\n";
			string OverallFoot = "[[Category:Nodes]]";

			string NodeInfoBeginFormat = "{| cellpadding=\"10\"\n";
			string nodeInfoBodyFormat = "|- style=\"vertical-align:top;\"\n| http://amplify.pt/Nodes/{0}.jpg\n| [[Unity Products:Amplify Shader Editor/{1} | <span style=\"font-size: 120%;\"><span id=\"{2}\"></span>'''{2}'''<span> ]] <br> {3}\n";
			string NodeInfoEndFormat = "|}\n";

			string nodesInfo = string.Empty;

			var enumerator = m_currentCategories.GetEnumerator();
			while ( enumerator.MoveNext() )
			{
				var current = enumerator.Current;
				if ( fromCommunity && current.Value.HasCommunityData || !fromCommunity )
				{
					InitialCategories += string.Format( InitialCategoriesFormat, current.Key );
					nodesInfo += string.Format( CurrentCategoryFormat, current.Key );
					int count = current.Value.Contents.Count;
					for ( int i = 0; i < count; i++ )
					{
						if ( ( fromCommunity && current.Value.Contents[ i ].NodeAttributes.FromCommunity ) ||
							( !fromCommunity && !current.Value.Contents[ i ].NodeAttributes.FromCommunity ) )
						{
							string nodeFullName = current.Value.Contents[ i ].Name;
							string pictureFilename = UIUtils.ReplaceInvalidStrings( nodeFullName );

							string pageFilename = UIUtils.RemoveWikiInvalidCharacters( pictureFilename );

							pictureFilename = UIUtils.RemoveInvalidCharacters( pictureFilename );

							string nodeDescription = current.Value.Contents[ i ].ItemUIContent.tooltip;

							string nodeInfoBody = string.Format( nodeInfoBodyFormat, pictureFilename, pageFilename, nodeFullName, nodeDescription );
							string nodeInfoFoot = string.Format( NodesFootFormat, pageFilename );

							nodesInfo += ( NodeInfoBeginFormat + nodeInfoBody + NodeInfoEndFormat + nodeInfoFoot );
							if ( i != ( count - 1 ) )
							{
								nodesInfo += NodesFootFormatSep;
							}
						}
					}
				}
			}

			string finalText = noTOCHeader + nodesHeader + InitialCategories + nodesInfo + OverallFoot;

			if ( !System.IO.Directory.Exists( pathname ) )
			{
				System.IO.Directory.CreateDirectory( pathname );
			}
			// Save file
			string nodesPathname = pathname + ( fromCommunity ? "AvailableNodesFromCommunity.txt" : "AvailableNodes.txt" );
			Debug.Log( " Creating nodes file at " + nodesPathname );
			IOUtils.SaveTextfileToDisk( finalText, nodesPathname, false );
		}

		public virtual bool CheckButton( GUIContent content, GUIStyle style, int buttonId )
		{
			if ( buttonId != m_validButtonId )
			{
				GUILayout.Label( content, style );
				return false;
			}

			return GUILayout.RepeatButton( content, style );
		}

		public void FillList( ref List<ContextMenuItem> list )
		{
			List<ContextMenuItem> allList = ParentWindow.ContextMenuInstance.MenuItems;

			list.Clear();
			int count = allList.Count;
			for ( int i = 0; i < count; i++ )
			{
				list.Add( allList[ i ] );
			}
		}
		
		public Dictionary<string,PaletteFilterData> BuildFullList()
		{
			//Only need to build if search filter is active and list is set according to it
			if ( m_searchFilter.Length > 0 || !m_isActive || m_currentCategories.Count == 0  )
			{
				m_currentItems.Clear();
				m_currentCategories.Clear();

				List<ContextMenuItem> allItems = ParentWindow.ContextMenuInstance.MenuItems;

				for ( int i = 0; i < allItems.Count; i++ )
				{
					if ( allItems[ i ].Name.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0 ||
							allItems[ i ].Category.IndexOf( m_searchFilter, StringComparison.InvariantCultureIgnoreCase ) >= 0
						)
					{
						m_currentItems.Add( allItems[ i ] );
						if ( !m_currentCategories.ContainsKey( allItems[ i ].Category ) )
						{
							m_currentCategories.Add( allItems[ i ].Category, new PaletteFilterData( m_defaultCategoryVisible ) );
							m_currentCategories[ allItems[ i ].Category ].HasCommunityData = allItems[ i ].NodeAttributes.FromCommunity || m_currentCategories[ allItems[ i ].Category ].HasCommunityData;
						}
						m_currentCategories[ allItems[ i ].Category ].Contents.Add( allItems[ i ] );
					}
				}

				var categoryEnumerator = m_currentCategories.GetEnumerator();
				while ( categoryEnumerator.MoveNext() )
				{
					categoryEnumerator.Current.Value.Contents.Sort( ( x, y ) => x.CompareTo( y, false ) );
				}

				//mark to force update and take search filter into account
				m_forceUpdate = true;
			}
			return m_currentCategories;
		}

		private bool IsItemVisible( float currPos )
		{
			if ( ( currPos < m_currentScrollPos.y && ( currPos + ItemSize ) < m_currentScrollPos.y ) ||
									( currPos > ( m_currentScrollPos.y + _currScrollBarDims.y ) &&
									( currPos + ItemSize ) > ( m_currentScrollPos.y + _currScrollBarDims.y ) ) )
			{
				return false;
			}
			return true;
		}

		public override void Destroy()
		{
			base.Destroy();

			//m_allItems = null;

			m_currentItems.Clear();
			m_currentItems = null;

			m_currentCategories.Clear();
			m_currentCategories = null;

			OnPaletteNodeCreateEvt = null;
			m_buttonStyle = null;
			m_foldoutStyle = null;
		}

		//public void Clear() {
		//	m_allItems.Clear();
		//	m_allItems = new List<ContextMenuItem>();
		//}

		public bool ForceUpdate { get { return m_forceUpdate; } set { m_forceUpdate = value; } }
	}
}

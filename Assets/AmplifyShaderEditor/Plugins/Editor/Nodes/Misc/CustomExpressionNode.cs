// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Custom Expression", "Misc", "Creates Custom Expression" )]
	public sealed class CustomExpressionNode : ParentNode
	{
		private const string CustomExpressionInfo = "Creates a custom expression or function according to how code is written on text area.\n\n" +
													" - If a return function is detected on Code text area then a function will be created.\n" +
													"Also in function mode a ; is expected on the end of each instruction line.\n\n" +
													"- If no return function is detected them an expression will be generated and used directly on the vertex/frag body.\n" +
													"On Expression mode a ; is not required on the end of an instruction line.";
		private const char LineFeedSeparator = '$';
		private const char SemiColonSeparator = '@';
		private const string ReturnHelper = "return";
		private const double MaxTimestamp = 1;
		private const string DefaultExpressionName = "My Custom Expression";
		private const string DefaultInputName = "In";
		private const string CodeTitleStr = "Code";
		private const string OutputTypeStr = "Output Type";
		private const string InputsStr = "Inputs";
		private const string InputNameStr = "Name";
		private const string InputTypeStr = "Type";
		private const string InputValueStr = "Value";
		private const string ExpressionNameLabel = "Name";

		private readonly string[] AvailableWireTypesStr =   {   "Int",
																"Float",
																"Float2",
																"Float3",
																"Float4",
																"Color",
																"Float3x3",
																"Float4x4"};

		private readonly WirePortDataType[] AvailableWireTypes = {  WirePortDataType.INT,
																	WirePortDataType.FLOAT,
																	WirePortDataType.FLOAT2,
																	WirePortDataType.FLOAT3,
																	WirePortDataType.FLOAT4,
																	WirePortDataType.COLOR,
																	WirePortDataType.FLOAT3x3,
																	WirePortDataType.FLOAT4x4};

		private readonly Dictionary<WirePortDataType, int> WireToIdx = new Dictionary<WirePortDataType, int> {  { WirePortDataType.INT,     0},
																												{ WirePortDataType.FLOAT,   1},
																												{ WirePortDataType.FLOAT2,  2},
																												{ WirePortDataType.FLOAT3,  3},
																												{ WirePortDataType.FLOAT4,  4},
																												{ WirePortDataType.COLOR,   5},
																												{ WirePortDataType.FLOAT3x3,6},
																												{ WirePortDataType.FLOAT4x4,7}};
		[SerializeField]
		private string m_customExpressionName = string.Empty;

		[SerializeField]
		private List<bool> m_foldoutValuesFlags = new List<bool>();

		[SerializeField]
		private List<string> m_foldoutValuesLabels = new List<string>();

		[SerializeField]
		private string m_code = " ";

		[SerializeField]
		private int m_outputTypeIdx = 1;

		[SerializeField]
		private bool m_visibleInputsFoldout = true;

		[SerializeField]
		private string m_uniqueName;

		private int m_markedToDelete = -1;
		private const float ButtonLayoutWidth = 15;

		private bool m_repopulateNameDictionary = true;
		private Dictionary<string, int> m_usedNames = new Dictionary<string, int>();

		private double m_lastTimeModified = 0;
		private bool m_nameModified = false;
		private bool m_editPropertyNameMode = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "In0" );
			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[0]" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 95;
		}

		protected override void OnUniqueIDAssigned()
		{
			base.OnUniqueIDAssigned();

			m_customExpressionName = DefaultExpressionName;
			SetTitleText( m_customExpressionName );

			if ( m_nodeAttribs != null )
				m_uniqueName = m_nodeAttribs.Name + UniqueId;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			if ( m_nameModified )
			{
				if ( ( EditorApplication.timeSinceStartup - m_lastTimeModified ) > MaxTimestamp )
				{
					m_nameModified = false;
					m_repopulateNameDictionary = true;
				}
			}

			if ( m_repopulateNameDictionary )
			{
				m_repopulateNameDictionary = false;
				m_usedNames.Clear();
				for ( int i = 0; i < m_inputPorts.Count; i++ )
				{
					m_usedNames.Add( m_inputPorts[ i ].Name, i );
				}
			}

			base.Draw( drawInfo );
		}

		public string GetFirstAvailableName()
		{
			string name = string.Empty;
			for ( int i = 0; i < m_inputPorts.Count + 1; i++ )
			{
				name = DefaultInputName + i;
				if ( !m_usedNames.ContainsKey( name ) )
				{
					return name;
				}
			}
			Debug.LogWarning( "Could not find valid name" );
			return string.Empty;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			NodeUtils.DrawPropertyGroup( ref m_propertiesFoldout, Constants.ParameterLabelStr, DrawBaseProperties );
			NodeUtils.DrawPropertyGroup( ref m_visibleInputsFoldout, InputsStr, DrawInputs, DrawAddRemoveInputs );
			EditorGUILayout.HelpBox( CustomExpressionInfo, MessageType.Info );
		}

		string WrapCodeInFunction()
		{
			//Hack to be used util indent is properly used
			int currIndent = UIUtils.ShaderIndentLevel;
			UIUtils.ShaderIndentLevel = 1;

			UIUtils.ShaderIndentLevel++;
			string functionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName );
			string functionBody = UIUtils.ShaderIndentTabs + UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + functionName + "( ";
			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{
				functionBody += UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_inputPorts[ i ].DataType ) + " " + m_inputPorts[ i ].Name;
				if ( i < ( count - 1 ) )
				{
					functionBody += " , ";
				}
			}
			functionBody += " )\n" + UIUtils.ShaderIndentTabs + "{\n";
			UIUtils.ShaderIndentLevel++;
			{
				string[] codeLines = m_code.Split( IOUtils.LINE_TERMINATOR );
				for ( int i = 0; i < codeLines.Length; i++ )
				{
					if ( codeLines[ i ].Length > 0 )
					{
						functionBody += UIUtils.ShaderIndentTabs + codeLines[ i ] + '\n';
					}
				}
			}
			UIUtils.ShaderIndentLevel--;

			functionBody += UIUtils.ShaderIndentTabs + "}\n";
			UIUtils.ShaderIndentLevel--;

			UIUtils.ShaderIndentLevel = currIndent;
			return functionBody;
		}

		void DrawBaseProperties()
		{
			EditorGUI.BeginChangeCheck();
			m_customExpressionName = EditorGUILayoutTextField( ExpressionNameLabel, m_customExpressionName );
			if ( EditorGUI.EndChangeCheck() )
			{
				SetTitleText( m_customExpressionName );
			}

			DrawPrecisionProperty();

			EditorGUILayout.LabelField( CodeTitleStr );
			m_code = EditorGUILayoutTextArea( m_code, UIUtils.MainSkin.textArea );

			EditorGUI.BeginChangeCheck();
			m_outputTypeIdx = EditorGUILayoutPopup( OutputTypeStr, m_outputTypeIdx, AvailableWireTypesStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
			}
		}

		void DrawAddRemoveInputs()
		{
			if ( m_inputPorts.Count == 0 )
				m_visibleInputsFoldout = false;

			// Add new port
			if ( GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				AddPortAt( m_inputPorts.Count );
			}

			//Remove port
			if ( GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
			{
				RemovePortAt( m_inputPorts.Count - 1 );
			}
		}

		void DrawInputs()
		{
			int count = m_inputPorts.Count;
			for ( int i = 0; i < count; i++ )
			{

				m_foldoutValuesFlags[ i ] = EditorGUILayoutFoldout( m_foldoutValuesFlags[ i ], m_foldoutValuesLabels[ i ] );

				if ( m_foldoutValuesFlags[ i ] )
				{
					EditorGUI.indentLevel += 1;
					EditorGUI.BeginChangeCheck();
					m_inputPorts[ i ].Name = EditorGUILayoutTextField( InputNameStr, m_inputPorts[ i ].Name );
					if ( EditorGUI.EndChangeCheck() )
					{
						m_nameModified = true;
						m_lastTimeModified = EditorApplication.timeSinceStartup;
						m_inputPorts[ i ].Name = UIUtils.RemoveInvalidCharacters( m_inputPorts[ i ].Name );
						if ( string.IsNullOrEmpty( m_inputPorts[ i ].Name ) )
						{
							m_inputPorts[ i ].Name = DefaultInputName + i;
						}
					}

					int typeIdx = WireToIdx[ m_inputPorts[ i ].DataType ];
					EditorGUI.BeginChangeCheck();
					typeIdx = EditorGUILayoutPopup( InputTypeStr, typeIdx, AvailableWireTypesStr );
					if ( EditorGUI.EndChangeCheck() )
					{
						m_inputPorts[ i ].ChangeType( AvailableWireTypes[ typeIdx ], false );
					}

					if ( !m_inputPorts[ i ].IsConnected )
					{
						m_inputPorts[ i ].ShowInternalData( this, true, InputValueStr );
					}

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label( " " );
						// Add new port
						if ( GUILayoutButton( string.Empty, UIUtils.PlusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							AddPortAt( i );
						}

						//Remove port
						if ( GUILayoutButton( string.Empty, UIUtils.MinusStyle, GUILayout.Width( ButtonLayoutWidth ) ) )
						{
							m_markedToDelete = i;
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel -= 1;
				}
			}

			if ( m_markedToDelete > -1 )
			{
				DeleteInputPortByArrayIdx( m_markedToDelete );
				m_markedToDelete = -1;
				m_repopulateNameDictionary = true;
			}
		}

		void AddPortAt( int idx )
		{
			AddInputPortAt( idx, WirePortDataType.FLOAT, false, GetFirstAvailableName()/*DefaultInputName + idx */);

			m_foldoutValuesFlags.Add( true );
			m_foldoutValuesLabels.Add( "[" + idx + "]" );
			m_repopulateNameDictionary = true;
		}

		void RemovePortAt( int idx )
		{
			if ( m_inputPorts.Count > 0 )
			{
				DeleteInputPortByArrayIdx( idx );
				m_foldoutValuesFlags.RemoveAt( idx );
				m_foldoutValuesLabels.RemoveAt( idx );
				m_repopulateNameDictionary = true;
			}
		}

		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			m_repopulateNameDictionary = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( string.IsNullOrEmpty( m_code ) )
			{
				UIUtils.ShowMessage( "Custom Expression need to have code associated", MessageSeverity.Warning );
				return "0";
			}

			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			string expressionName = UIUtils.RemoveInvalidCharacters( m_customExpressionName );
			int count = m_inputPorts.Count;
			if ( m_inputPorts.Count > 0 )
			{

				if ( m_code.Contains( ReturnHelper ) )
				{

					string function = WrapCodeInFunction();
					dataCollector.AddFunction( expressionName, function );
					string functionCall = expressionName + "( ";
					for ( int i = 0; i < count; i++ )
					{
						functionCall += m_inputPorts[ i ].GeneratePortInstructions( ref dataCollector );
						if ( i < ( count - 1 ) )
						{
							functionCall += " , ";
						}
					}
					functionCall += " )";
					RegisterLocalVariable( 0, functionCall, ref dataCollector, "local" + expressionName );
				}
				else
				{
					for ( int i = 0; i < count; i++ )
					{
						if ( m_inputPorts[ i ].IsConnected )
						{
							string result = m_inputPorts[ i ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ i ].DataType, true, true );
							dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, m_inputPorts[ i ].Name, result );
						}
						else
						{
							dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, m_inputPorts[ i ].DataType, m_inputPorts[ i ].Name, m_inputPorts[ i ].WrappedInternalData );
						}
					}
					RegisterLocalVariable( 0, string.Format( Constants.CodeWrapper, m_code ), ref dataCollector, "local" + expressionName );
				}
				return m_outputPorts[ 0 ].LocalValue;
			}
			return string.Format( Constants.CodeWrapper, m_code );
		}

		public override void OnNodeDoubleClicked( Vector2 currentMousePos2D )
		{
			if ( currentMousePos2D.y - m_globalPosition.y > Constants.NODE_HEADER_HEIGHT + Constants.NODE_HEADER_EXTRA_HEIGHT )
			{
				ContainerGraph.ParentWindow.ParametersWindow.IsMaximized = !ContainerGraph.ParentWindow.ParametersWindow.IsMaximized;
			}
			else
			{
				m_editPropertyNameMode = true;
				GUI.FocusControl( m_uniqueName );
				TextEditor te = ( TextEditor ) GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
				if ( te != null )
				{
					te.SelectAll();
				}
			}
		}

		public override void OnNodeSelected( bool value )
		{
			base.OnNodeSelected( value );
			if ( !value )
				m_editPropertyNameMode = false;
		}

		public override void DrawTitle( Rect titlePos )
		{
			if ( m_editPropertyNameMode )
			{
				titlePos.height = Constants.NODE_HEADER_HEIGHT;
				EditorGUI.BeginChangeCheck();
				GUI.SetNextControlName( m_uniqueName );
				m_customExpressionName = GUITextField( titlePos, m_customExpressionName, UIUtils.GetCustomStyle( CustomStyle.NodeTitle ) );
				if ( EditorGUI.EndChangeCheck() )
				{
					SetTitleText( m_customExpressionName );
				}

				if ( Event.current.isKey && ( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ) )
				{
					m_editPropertyNameMode = false;
					GUIUtility.keyboardControl = 0;
				}
			}
			else
			{
				base.DrawTitle( titlePos );
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			// This node is, by default, created with one input port 
			base.ReadFromString( ref nodeParams );
			m_code = GetCurrentParam( ref nodeParams );
			m_code = m_code.Replace( LineFeedSeparator, '\n' );
			m_code = m_code.Replace( SemiColonSeparator, ';' );
			m_outputTypeIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			m_outputPorts[ 0 ].ChangeType( AvailableWireTypes[ m_outputTypeIdx ], false );
			int count = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			if ( count == 0 )
			{
				DeleteInputPortByArrayIdx( 0 );
				m_foldoutValuesLabels.Clear();
			}
			else
			{
				for ( int i = 0; i < count; i++ )
				{
					m_foldoutValuesFlags.Add( Convert.ToBoolean( GetCurrentParam( ref nodeParams ) ) );
					string name = GetCurrentParam( ref nodeParams );
					WirePortDataType type = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
					string internalData = GetCurrentParam( ref nodeParams );
					if ( i == 0 )
					{
						m_inputPorts[ 0 ].ChangeProperties( name, type, false );
					}
					else
					{
						m_foldoutValuesLabels.Add( "[" + i + "]" );
						AddInputPort( type, false, name );
					}
					m_inputPorts[ i ].InternalData = internalData;
				}
			}

			if ( UIUtils.CurrentShaderVersion() > 7205 )
			{
				m_customExpressionName = GetCurrentParam( ref nodeParams );
				SetTitleText( m_customExpressionName );
			}
			m_repopulateNameDictionary = true;
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );

			string parsedCode = m_code.Replace( '\n', LineFeedSeparator );
			parsedCode = parsedCode.Replace( ';', SemiColonSeparator );

			IOUtils.AddFieldValueToString( ref nodeInfo, parsedCode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeIdx );
			int count = m_inputPorts.Count;
			IOUtils.AddFieldValueToString( ref nodeInfo, count );
			for ( int i = 0; i < count; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, m_foldoutValuesFlags[ i ] );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].Name );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].DataType );
				IOUtils.AddFieldValueToString( ref nodeInfo, m_inputPorts[ i ].InternalData );
			}
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customExpressionName );
		}
	}
}

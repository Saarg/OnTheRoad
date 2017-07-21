// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{

	public enum PrecisionType
	{
		Float = 0,
		Half,
		Fixed
	}

	public enum AvailableShaderTypes
	{
		SurfaceShader
	}

	[Serializable]
	public class MasterNode : OutputNode
	{

		private const string IndentationHelper = "\t\t{0}\n";
		private const string ShaderLODFormat = "\t\tLOD {0}\n";

		public delegate void OnMaterialUpdated( MasterNode masterNode );
		public event OnMaterialUpdated OnMaterialUpdatedEvent;
		public event OnMaterialUpdated OnShaderUpdatedEvent;

		protected readonly string[] ShaderModelTypeArr = { "2.0", "2.5", "3.0", "3.5", "4.0", "4.5", "4.6", "5.0" };
		private const string ShaderKeywordsStr = "Shader Keywords";

		[SerializeField]
		protected int m_shaderLOD = 0;

		[SerializeField]
		protected int m_shaderModelIdx = 2;

		[SerializeField]
		protected Shader m_currentShader;

		[SerializeField]
		protected Material m_currentMaterial;

		//[SerializeField]
		//private bool m_isMainMasterNode = false;

		[SerializeField]
		private Rect m_masterNodeIconCoords;

		[SerializeField]
		protected string m_shaderName = "MyNewShader";

		[SerializeField]
		protected string m_customInspectorName = Constants.DefaultCustomInspector;

		[SerializeField]
		protected AvailableShaderTypes m_currentShaderType = AvailableShaderTypes.SurfaceShader;

		private Texture2D m_masterNodeOnTex;
		private Texture2D m_masterNodeOffTex;

		private Texture2D m_gpuInstanceOnTex;
		private Texture2D m_gpuInstanceOffTex;

		// Shader Keywords
		[SerializeField]
		private List<string> m_shaderKeywords = new List<string>();

		[SerializeField]
		private bool m_shaderKeywordsFoldout = true;

		private GUIStyle m_addShaderKeywordStyle;
		private GUIStyle m_removeShaderKeywordStyle;
		private GUIStyle m_smallAddShaderKeywordItemStyle;
		private GUIStyle m_smallRemoveShaderKeywordStyle;
		private const float ShaderKeywordButtonLayoutWidth = 15;


		public MasterNode() : base() { CommonInit(); }
		public MasterNode( int uniqueId, float x, float y, float width, float height ) : base( uniqueId, x, y, width, height ) { CommonInit(); }

		protected GUIContent m_shaderTypeLabel = new GUIContent( "Shader Type ", "Specify the type of shader you want to be working on" );

		void CommonInit()
		{
			m_currentMaterial = null;
			m_masterNodeIconCoords = new Rect( 0, 0, 64, 64 );
			m_isMainOutputNode = false;
			m_connStatus = NodeConnectionStatus.Connected;
			m_activeType = GetType();
			m_currentPrecisionType = PrecisionType.Float;
			AddMasterPorts();
		}

		public virtual void AddMasterPorts() { }

		public virtual void ForcePortType() { }

		public virtual void UpdateMasterNodeMaterial( Material material ) { }

		public virtual void SetName( string name ) { }

		public void DrawShaderKeywords()
		{
			if ( m_addShaderKeywordStyle == null )
				m_addShaderKeywordStyle = UIUtils.PlusStyle;

			if ( m_removeShaderKeywordStyle == null )
				m_removeShaderKeywordStyle = UIUtils.MinusStyle;

			if ( m_smallAddShaderKeywordItemStyle == null )
				m_smallAddShaderKeywordItemStyle = UIUtils.PlusStyle;

			if ( m_smallRemoveShaderKeywordStyle == null )
				m_smallRemoveShaderKeywordStyle = UIUtils.MinusStyle;

			EditorGUILayout.BeginHorizontal();
			{
				m_shaderKeywordsFoldout = EditorGUILayout.Foldout( m_shaderKeywordsFoldout, ShaderKeywordsStr );

				// Add keyword
				if ( GUILayout.Button( string.Empty, m_addShaderKeywordStyle ) )
				{
					m_shaderKeywords.Insert( 0, "" );
				}

				//Remove keyword
				if ( GUILayout.Button( string.Empty, m_removeShaderKeywordStyle ) )
				{
					m_shaderKeywords.RemoveAt( m_shaderKeywords.Count - 1 );
				}
			}
			EditorGUILayout.EndHorizontal();

			if ( m_shaderKeywordsFoldout )
			{
				EditorGUI.indentLevel += 1;
				int itemCount = m_shaderKeywords.Count;
				int markedToDelete = -1;
				for ( int i = 0; i < itemCount; i++ )
				{
					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Label( " " );
						// Add new port
						if ( GUILayoutButton( string.Empty, m_smallAddShaderKeywordItemStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
						{
							m_shaderKeywords.Insert( i, "" );
						}

						//Remove port
						if ( GUILayoutButton( string.Empty, m_smallRemoveShaderKeywordStyle, GUILayout.Width( ShaderKeywordButtonLayoutWidth ) ) )
						{
							markedToDelete = i;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
				if ( markedToDelete > -1 )
				{
					m_shaderKeywords.RemoveAt( markedToDelete );
				}
				EditorGUI.indentLevel -= 1;
			}
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			if ( m_isMainOutputNode )
			{
				if ( m_masterNodeOnTex == null )
				{
					m_masterNodeOnTex = UIUtils.MasterNodeOnTexture;
				}

				if ( m_masterNodeOffTex == null )
				{
					m_masterNodeOffTex = UIUtils.MasterNodeOffTexture;
				}

				if ( m_gpuInstanceOnTex == null )
				{
					m_gpuInstanceOnTex = UIUtils.GPUInstancedOnTexture;
				}

				if ( m_gpuInstanceOffTex == null )
				{
					m_gpuInstanceOffTex = UIUtils.GPUInstancedOffTexture;
				}

				m_masterNodeIconCoords = m_globalPosition;
				m_masterNodeIconCoords.x += m_globalPosition.width - m_masterNodeOffTex.width * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.y += m_globalPosition.height - m_masterNodeOffTex.height * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.width = m_masterNodeOffTex.width * drawInfo.InvertedZoom;
				m_masterNodeIconCoords.height = m_masterNodeOffTex.height * drawInfo.InvertedZoom;

				GUI.DrawTexture( m_masterNodeIconCoords, m_masterNodeOffTex );

				if ( m_gpuInstanceOnTex == null )
				{
					m_gpuInstanceOnTex = UIUtils.GPUInstancedOnTexture;
				}

				if ( UIUtils.IsInstancedShader() )
				{
					m_masterNodeIconCoords = m_globalPosition;
					m_masterNodeIconCoords.x += m_globalPosition.width - 5 - m_gpuInstanceOffTex.width * drawInfo.InvertedZoom;
					m_masterNodeIconCoords.y += m_headerPosition.height;
					m_masterNodeIconCoords.width = m_gpuInstanceOffTex.width * drawInfo.InvertedZoom;
					m_masterNodeIconCoords.height = m_gpuInstanceOffTex.height * drawInfo.InvertedZoom;
					GUI.DrawTexture( m_masterNodeIconCoords, m_gpuInstanceOffTex );
				}
			}
		}
		//public override void DrawProperties()
		//{
		//	base.DrawProperties();
		//	//EditorGUILayout.LabelField( _shaderTypeLabel );
		//}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			//IOUtils.AddFieldValueToString( ref nodeInfo, m_isMainMasterNode );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderModelIdx );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentPrecisionType );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_customInspectorName );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_shaderLOD );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 21 )
			{
				m_shaderModelIdx = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				m_currentPrecisionType = ( PrecisionType ) Enum.Parse( typeof( PrecisionType ), GetCurrentParam( ref nodeParams ) );
			}

			if ( UIUtils.CurrentShaderVersion() > 2404 )
			{
				m_customInspectorName = GetCurrentParam( ref nodeParams );
			}

			if ( UIUtils.CurrentShaderVersion() > 6101 )
			{
				m_shaderLOD = Convert.ToInt32( GetCurrentParam( ref nodeParams ));
			}
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			if ( activateNode )
			{
				InputPort port = GetInputPortByUniqueId( portId );
				port.GetOutputNode().ActivateNode( UniqueId, portId, m_activeType );
			}
		}

		public void FireMaterialChangedEvt()
		{
			if ( OnMaterialUpdatedEvent != null )
			{
				OnMaterialUpdatedEvent( this );
			}
		}

		public void FireShaderChangedEvt()
		{
			if ( OnShaderUpdatedEvent != null )
				OnShaderUpdatedEvent( this );
		}

		// What operation this node does
		public virtual void Execute( Shader selectedShader )
		{
			Execute( AssetDatabase.GetAssetPath( selectedShader ), false );
		}

		public virtual Shader Execute( string pathname, bool isFullPath )
		{
			ContainerGraph.ResetNodesLocalVariables();
			return null;
		}

		public virtual void UpdateFromShader( Shader newShader ) { }

		public void ClearUpdateEvents()
		{
			OnShaderUpdatedEvent = null;
			OnMaterialUpdatedEvent = null;
		}

		public Material CurrentMaterial { get { return m_currentMaterial; } }
		public Shader CurrentShader
		{
			set
			{
				if ( value != null )
				{
					SetName( value.name );
				}

				m_currentShader = value;
				FireShaderChangedEvt();
			}
			get { return m_currentShader; }
		}

		public override void Destroy()
		{
			base.Destroy();
			OnMaterialUpdatedEvent = null;
			OnShaderUpdatedEvent = null;
			m_masterNodeOnTex = null;
			m_masterNodeOffTex = null;
			m_gpuInstanceOnTex = null;
			m_gpuInstanceOffTex = null;
			m_addShaderKeywordStyle = null;
			m_removeShaderKeywordStyle = null;
			m_smallAddShaderKeywordItemStyle = null;
			m_smallRemoveShaderKeywordStyle = null;
		}
		
		public static void OpenShaderBody( ref string result, string name )
		{
			result += string.Format( "Shader \"{0}\"\n", name ) + "{\n";
		}

		public static void CloseShaderBody( ref string result )
		{
			result += "}\n";
		}

		public static void OpenSubShaderBody( ref string result )
		{
			result += "\n\tSubShader\n\t{\n";
		}

		public static void CloseSubShaderBody( ref string result )
		{
			result += "\t}\n";
		}

		public static void AddShaderProperty( ref string result, string name, string value )
		{
			result += string.Format( "\t{0} \"{1}\"\n", name, value );
		}

		public static void AddShaderPragma( ref string result, string value )
		{
			result += string.Format( "\t\t#pragma {0}\n", value );
		}

		public static void AddRenderState( ref string result, string state, string stateParams )
		{
			result += string.Format( "\t\t{0} {1}\n", state, stateParams );
		}

		public static void AddRenderTags( ref string result, string tags )
		{
			result += string.Format( IndentationHelper, tags ); ;
		}

		public static void AddShaderLOD( ref string result, int shaderLOD )
		{
			if ( shaderLOD > 0 )
			{
				result += string.Format( ShaderLODFormat, shaderLOD );
			}
		}

		public static void AddMultilineBody( ref string result, string[] lines )
		{
			for ( int i = 0; i < lines.Length; i++ )
			{
				result += string.Format( IndentationHelper, lines[ i ] );
			}
		}

		public static void OpenCGInclude( ref string result )
		{
			result += "\t\tCGINCLUDE\n";
		}

		public static void OpenCGProgram( ref string result )
		{
			result += "\t\tCGPROGRAM\n";
		}

		public static void CloseCGProgram( ref string result )
		{
			result += "\n\t\tENDCG\n";
		}

		public string ShaderName
		{
			//get { return ( ( _isHidden ? "Hidden/" : string.Empty ) + ( String.IsNullOrEmpty( _shaderCategory ) ? "" : ( _shaderCategory + "/" ) ) + _shaderName ); }
			get { return m_shaderName; }
			set
			{
				m_shaderName = value;
				m_content.text = value;
				m_sizeIsDirty = true;
			}
		}

	}
}

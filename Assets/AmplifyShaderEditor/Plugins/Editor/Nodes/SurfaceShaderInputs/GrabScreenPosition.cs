// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Grab Screen Position", "Surface Standard Inputs", "Screen position correctly transformed to be used with Grab Screen Color" )]
	public sealed class GrabScreenPosition : ParentNode
	{
		private const string ProjectStr = "Project";
		private const string ScreenPosStr = "screenPos";
		//private readonly string ScreenPosOnFragStr = Constants.InputVarStr + "." + ScreenPosStr;

		private readonly string ProjectionInstruction = "{0}.xyzw /= {0}.w;";
		private readonly string[] HackInstruction = {   "#if UNITY_UV_STARTS_AT_TOP",
														"float scale{0} = -1.0;",
														"#else",
														"float scale{0} = 1.0;",
														"#endif",
														"float halfPosW{1} = {0}.w * 0.5;",
														"{0}.y = ( {0}.y - halfPosW{1} ) * _ProjectionParams.x* scale{1} + halfPosW{1};"};


		private readonly string ScreenPosOnVert00Str = "{0} = ComputeScreenPos( mul( UNITY_MATRIX_MVP, {1}.vertex));";
	//	private readonly string ScreenPosOnVert01Str = "{0}.xyz /= {0}.w;";


		private readonly string[] m_outputTypeStr = { "Normalized", "Screen" };
		//[SerializeField]
		//private bool m_project = true;

		[SerializeField]
		private int m_outputTypeInt = 0;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT4, "XYZW" );
			m_autoWrapProperties = true;
			m_textLabelWidth = 65;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			//m_project = EditorGUILayout.Toggle( ProjectStr, m_project );

			EditorGUI.BeginChangeCheck();
			m_outputTypeInt = EditorGUILayoutPopup( "Output", m_outputTypeInt, m_outputTypeStr );
			if ( EditorGUI.EndChangeCheck() )
			{
				ConfigureHeader();
			}
		}

		void ConfigureHeader()
		{
			switch ( m_outputTypeInt )
			{
				case 0:
				default:
				SetAdditonalTitleText( "( Normalized )" );
				break;
				case 1:
				SetAdditonalTitleText( string.Empty );
				break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return GetOutputColorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

			//string localVarName = ScreenPosStr + m_uniqueId;
			string localVarName = string.Empty;

			bool isFragment = dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug;

			if ( isFragment )
			{
				string screenPos = GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, true );
				localVarName = screenPos + UniqueId;
				//dataCollector.AddToInput( m_uniqueId, "float4 " + ScreenPosStr, true );
				string value = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + localVarName + " = " + screenPos + ";";
				dataCollector.AddLocalVariable( UniqueId, value );
			}
			else
			{
				string screenPos = GeneratorUtils.GenerateVertexScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, false );
				localVarName = screenPos + UniqueId;
				string localVarDecl = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType ) + " " + localVarName;
				string value = string.Format( ScreenPosOnVert00Str, localVarDecl, Constants.VertexShaderInputStr );
				dataCollector.AddLocalVariable( UniqueId, value );
				//dataCollector.AddLocalVariable( m_uniqueId, string.Format( ScreenPosOnVert01Str, localVarName ) );
			}
			
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 0 ] );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 1 ], UniqueId ) );
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 2 ] );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 3 ], UniqueId ) );
			dataCollector.AddLocalVariable( UniqueId, HackInstruction[ 4 ] );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 5 ], localVarName, UniqueId ) );
			dataCollector.AddLocalVariable( UniqueId, string.Format( HackInstruction[ 6 ], localVarName, UniqueId ) );
			if ( m_outputTypeInt == 0 )
			{
				dataCollector.AddLocalVariable( UniqueId, string.Format( ProjectionInstruction, localVarName ) );
			}

			m_outputPorts[ 0 ].SetLocalValue( localVarName );
			//RegisterLocalVariable(outputId, localVarName ,ref dataCollector)
			return GetOutputColorItem( 0, outputId, localVarName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 3108 )
			{
				if ( UIUtils.CurrentShaderVersion() < 6102 )
				{
					bool project = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_outputTypeInt = project ? 0 : 1;
				}
				else
				{
					m_outputTypeInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
			}

			ConfigureHeader();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeInt );
		}
	}
}

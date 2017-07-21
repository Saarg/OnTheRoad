// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Screen Position", "Surface Standard Inputs", "Screen space position" )]
	public sealed class ScreenPosInputsNode : SurfaceShaderINParentNode
	{
		private const string ProjectStr = "Project";
		private const string UVInvertHack = "Scale and Offset";
		private readonly string ProjectionInstruction = "{0}.xyzw /= {0}.w;";
		//private readonly string[] HackInstruction = {   "#if UNITY_UV_STARTS_AT_TOP",
		//												"float scale{0} = -1.0;",
		//												"#else",
		//												"float scale{0} = 1.0;",
		//												"#endif",
		//												"float halfPosW{1} = {0}.w * 0.5;",
		//												"{0}.y = ( {0}.y - halfPosW{1} ) * _ProjectionParams.x* scale{1} + halfPosW{1};"};

		private readonly string[] m_outputTypeStr = { "Normalized", "Screen" };

		//[SerializeField]
		//private bool m_project = false;

		[SerializeField]
		private int m_outputTypeInt = 0;

		[SerializeField]
		private bool m_scaleAndOffset = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.SCREEN_POS;
			InitialSetup();
			m_textLabelWidth = 100;
			m_autoWrapProperties = true;

			if ( UIUtils.CurrentShaderVersion() <= 2400 )
				m_outputTypeInt = 1;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			//m_scaleAndOffset = EditorGUILayout.Toggle( UVInvertHack, m_scaleAndOffset );
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

		public override void Reset()
		{
			base.Reset();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
			{
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}

			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );

			string screenPos = string.Empty;

			if( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
				screenPos = GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, false );
			else
				screenPos = GeneratorUtils.GenerateVertexScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, false );

			string localVarName = screenPos + UniqueId;
			string value = UIUtils.PrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[0].DataType ) + " " + localVarName + " = " + screenPos + ";";
			dataCollector.AddLocalVariable( UniqueId, value );

			if ( m_outputTypeInt == 0 )
			{
				dataCollector.AddLocalVariable( UniqueId, string.Format( ProjectionInstruction, localVarName ) );
			}
				
			m_outputPorts[ 0 ].SetLocalValue( localVarName );
			return GetOutputVectorItem( 0, outputId, localVarName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if ( UIUtils.CurrentShaderVersion() > 2400 )
			{
				if ( UIUtils.CurrentShaderVersion() < 6102 )
				{
					bool project = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
					m_outputTypeInt = project ? 0 : 1;
				} else
				{
					m_outputTypeInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
				}
			}

			if ( UIUtils.CurrentShaderVersion() > 3107 )
			{
				//if ( UIUtils.CurrentShaderVersion() < 3109 )
				//{
				m_scaleAndOffset = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
				m_scaleAndOffset = false;
				//}
			}

			ConfigureHeader();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_outputTypeInt );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_scaleAndOffset );
		}
	}
}

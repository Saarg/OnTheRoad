// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Grayscale
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Grayscale", "Image Effects", "Convert image colors to grayscale", null, KeyCode.None, true, false, null, null, true )]
	public sealed class TFHCGrayscale : ParentNode
	{
		private const string GrayscaleStyleStr = "Grayscale Style";

		[SerializeField]
		private int m_grayscaleStyle;

		[SerializeField]
		private readonly string[] _GrayscaleStyleValues = { "Luminance", "Natural Classic", "Old School" };

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "In" );
			AddOutputPort( WirePortDataType.FLOAT, "Out" );
			m_textLabelWidth = 120;
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();
			m_grayscaleStyle = EditorGUILayoutPopup( GrayscaleStyleStr, m_grayscaleStyle, _GrayscaleStyleValues );
			EditorGUILayout.EndVertical();
			EditorGUILayout.HelpBox( "Grayscale Old:\n\n - In: Image to convert.\n - Grayscale Style: Select the grayscale style.\n\n - Out: Grayscale version of the image.", MessageType.None );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_grayscaleStyle = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_grayscaleStyle );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			string i = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string grayscale = string.Empty;
			switch ( m_grayscaleStyle )
			{
				case 1: { grayscale = "dot(" + i + ", float3(0.299,0.587,0.114))"; } break;
				case 2: { grayscale = "(" + i + ".r + " + i + ".g + " + i + ".b) / 3"; } break;
				default: { grayscale = "Luminance(" + i + ")"; } break;
			}
			return grayscale;
		}
	}
}

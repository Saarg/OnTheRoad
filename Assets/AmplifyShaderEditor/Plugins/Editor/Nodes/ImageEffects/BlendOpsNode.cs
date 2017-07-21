// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

//https://www.shadertoy.com/view/XdS3RW
//http://www.deepskycolors.com/archivo/2010/04/21/formulas-for-Photoshop-blending-modes.html

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	public enum BlendOps
	{
		ColorBurn,
		ColorDodge,
		Darken,
		Divide,
		Difference,
		Exclusion,
		HardLight,
		HardMix,
		Lighten,
		LinearBurn,
		LinearDodge,
		LinearLight,
		Multiply,
		Overlay,
		PinLight,
		Subtract,
		Screen,
		VividLight
	}
	[Serializable]
	[NodeAttributes( "Blend Operations", "Image Effects", "Common layer blending modes" )]
	public class BlendOpsNode : ParentNode
	{
		private const string BlendOpsModeStr = "Blend Op";

		[SerializeField]
		private BlendOps m_currentBlendOp = BlendOps.ColorBurn;

		[SerializeField]
		private bool m_saturate = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.COLOR, false, "Source" );
			AddInputPort( WirePortDataType.COLOR, false, "Destiny" );
			AddOutputPort( WirePortDataType.COLOR, Constants.EmptyPortValue );
			m_textLabelWidth = 75;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_currentBlendOp = ( BlendOps ) EditorGUILayoutEnumPopup( BlendOpsModeStr, m_currentBlendOp );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			string src = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string dst = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			
			string result = string.Empty;
			switch ( m_currentBlendOp )
			{
				case BlendOps.ColorBurn:
				{
					result = "( 1.0 - ( ( 1.0 - " + dst + ") / " + src + ") )";
				}
				break;
				case BlendOps.ColorDodge:
				{
					result = "( " + dst + "/ ( 1.0 - " + src + " ) )";
				}
				break;
				case BlendOps.Darken:
				{
					result = "min( " + src + " , " + dst + " )";
				}
				break;
				case BlendOps.Divide:
				{
					result = "( " + dst + " / " + src + " )";
				}
				break;
				case BlendOps.Difference:
				{
					result = "abs( " + src + " - " + dst + " )";
				}
				break;
				case BlendOps.Exclusion:
				{
					result = "( 0.5 - 2.0 * ( " + src + " - 0.5 ) * ( " + dst + " - 0.5 ) )";
				}
				break;
				case BlendOps.HardLight:
				{
					result = " ( " + src + " > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( " + src + " - 0.5 ) ) * ( 1.0 - " + dst + " ) ) : ( 2.0 * " + src + " * " + dst + " ) )";
				}
				break;
				case BlendOps.HardMix:
				{
					result = " round( 0.5 * ( " + src + " + " + dst + " ) )";
				}
				break;
				case BlendOps.Lighten:
				{
					result = "	max( " + src + ", " + dst + " )";
				}
				break;
				case BlendOps.LinearBurn:
				{
					result = "( " + src + " + " + dst + " - 1.0 )";
				}
				break;
				case BlendOps.LinearDodge:
				{
					result = "( " + src + " + " + dst + " )";
				}
				break;
				case BlendOps.LinearLight:
				{
					result = "( " + src + " > 0.5 ? ( " + dst + " + 2.0 * " + src + " - 1.0 ) : ( " + dst + " + 2.0 * ( " + src + " - 0.5 ) ) )";
				}
				break;
				case BlendOps.Multiply:
				{
					result = "( " + src + " * " + dst + " )";
				}
				break;
				case BlendOps.Overlay:
				{
					result = "( " + dst + " > 0.5 ? ( 1.0 - ( 1.0 - 2.0 * ( " + dst + " - 0.5 ) ) * ( 1.0 - " + src + " ) ) : ( 2.0 * " + dst + " * " + src + " ) )";
				}
				break;
				case BlendOps.PinLight:
				{
					result = "( " + src + " > 0.5 ? max( " + dst + ", 2.0 * ( " + src + " - 0.5 ) ) : min( " + dst + ", 2.0 * " + src + " ) )";
				}
				break;
				case BlendOps.Subtract:
				{
					result = "( " + dst + " - " + src + " )";
				}
				break;
				case BlendOps.Screen:
				{
					result = "( 1.0 - ( 1.0 - " + src + " ) * ( 1.0 - " + dst + " ) )";
				}
				break;
				case BlendOps.VividLight:
				{
					result = "( " + src + " > 0.5 ? ( " + dst + " / ( ( 1.0 - " + src + " ) * 2.0 ) ) : ( 1.0 - ( ( ( 1.0 - " + dst + " ) * 0.5 ) / " + src + " ) ) )";
				}
				break;
			}

			if ( m_saturate )
				result = "( saturate( " + result + " ))";

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentBlendOp );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_saturate );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentBlendOp = ( BlendOps ) Enum.Parse( typeof( BlendOps ), GetCurrentParam( ref nodeParams ) );
			m_saturate = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}
	}
}

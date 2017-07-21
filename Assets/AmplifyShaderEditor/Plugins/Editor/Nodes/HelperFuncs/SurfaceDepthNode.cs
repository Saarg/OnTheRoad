using UnityEngine;
using UnityEditor;

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Surface Depth", "Generic", "Returns the surface view depth" )]
	public sealed class SurfaceDepthNode : ParentNode
	{
		[SerializeField]
		private int m_viewSpaceInt = 0;

		private readonly string[] m_viewSpaceStr = { "Eye Space", "0-1 Space" };

		private readonly string[] m_vertexNameStr = { "eyeDepth", "clampDepth" };

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, "Depth" );
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_viewSpaceInt = EditorGUILayoutPopup( "View Space", m_viewSpaceInt, m_viewSpaceStr );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				//UIUtils.ShowMessage( "Depth Fade node doesn't generate any code in vertex function!", MessageSeverity.Warning );
				string vertexSpace = m_viewSpaceInt == 1 ? " * _ProjectionParams.w" : "";
				string vertexInstruction = "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z" + vertexSpace;
				dataCollector.AddVertexInstruction( "float " + m_vertexNameStr[ m_viewSpaceInt ] + " = " + vertexInstruction, UniqueId );

				return m_vertexNameStr[ m_viewSpaceInt ];
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
		

			if ( dataCollector.TesselationActive )
			{
				string eyeDepth = GeneratorUtils.GenerateScreenDepthOnFrag( ref dataCollector, UniqueId, m_currentPrecisionType );
				if ( m_viewSpaceInt == 1 )
				{
					dataCollector.AddLocalVariable( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, m_vertexNameStr[ 1 ], eyeDepth + " * _ProjectionParams.w" );
					return m_vertexNameStr[ 1 ];
				}
				else
				{
					return eyeDepth;
				}
			}
			else
			{
				string space = string.Empty;
				if ( m_viewSpaceInt == 1 )
					space = " * _ProjectionParams.w";

				dataCollector.AddToInput( UniqueId, "float " + m_vertexNameStr[ m_viewSpaceInt ], true );
				string instruction = "-UnityObjectToViewPos( " + Constants.VertexShaderInputStr + ".vertex.xyz ).z" + space;
				dataCollector.AddVertexInstruction( Constants.VertexShaderOutputStr + "." + m_vertexNameStr[ m_viewSpaceInt ] + " = " + instruction, UniqueId );

				return Constants.InputVarStr + "." + m_vertexNameStr[ m_viewSpaceInt ];
			}
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_viewSpaceInt = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );

		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_viewSpaceInt );
		}
	}

}

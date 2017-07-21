using UnityEngine;
using UnityEditor;

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Screen Depth", "Surface Standard Inputs", "Given a screen postion returns the depth of the scene to the object as seen by the camera" )]
	public sealed class ScreenDepthNode : ParentNode
	{
		[SerializeField]
		private int m_viewSpaceInt = 0;

		private readonly string[] m_viewSpaceStr = { "Eye Space", "0-1 Space" };

		private readonly string[] m_vertexNameStr = { "eyeDepth", "clampDepth" };

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT4, false, "Pos" );
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
				UIUtils.ShowNoVertexModeNodeMessage( this );
				return "0";
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityCgLibFuncs );
			dataCollector.AddToUniforms( UniqueId, "uniform sampler2D _CameraDepthTexture;" );
			string screenPos = string.Empty;
			if ( m_inputPorts[ 0 ].IsConnected )
				screenPos = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, false );
			else
				screenPos = GeneratorUtils.GenerateScreenPosition( ref dataCollector, UniqueId, m_currentPrecisionType, true );
			//string screenPos = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT4, false );

			string viewSpace = m_viewSpaceInt == 0 ? "Eye" : "01";
			string screenDepthInstruction = "Linear" + viewSpace + "Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(" + screenPos + "))))";

			dataCollector.AddToLocalVariables( UniqueId, m_currentPrecisionType, WirePortDataType.FLOAT, m_vertexNameStr[ m_viewSpaceInt ] + UniqueId, screenDepthInstruction );

			return m_vertexNameStr[ m_viewSpaceInt ] + UniqueId;
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

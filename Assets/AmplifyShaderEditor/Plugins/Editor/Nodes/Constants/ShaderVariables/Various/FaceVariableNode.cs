// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Face", "Various", "Indicates whether the rendered surface is facing the camera (>0), or facing away from the camera(=0)" )]
	public class FaceVariableNode : ParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work on Tessellation port" );
				return "0";
			}

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
			{
				if ( dataCollector.TesselationActive )
				{
					UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work properly on Vertex/Tessellation ports" );
					return "0";
				}
				else
				{
					UIUtils.ShowMessage( m_nodeAttribs.Name + " does not work propery on Vertex ports" );
				}
			}
			
			dataCollector.AddToInput( UniqueId, Constants.VFaceInput, true);
			string variable =  ( dataCollector.PortCategory == MasterNodePortCategory.Vertex)?Constants.VertexShaderOutputStr: Constants.InputVarStr;
			return variable + "." + Constants.VFaceVariable;
		}
	}
}

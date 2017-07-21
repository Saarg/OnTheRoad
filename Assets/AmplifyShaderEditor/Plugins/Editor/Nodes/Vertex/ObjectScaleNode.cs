using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Object Scale", "Vertex Data", "Object Scale extracted directly from its transform matrix" )]
	public class ObjectScaleNode : ParentNode 
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			m_outputPorts[ 0 ].SetLocalValue( GeneratorUtils.GenerateObjectScale( ref dataCollector, UniqueId ) ); 
			return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
		}
	}
}

// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "ATan2", "Trigonometry", "Arctangent of y/x" )]
	public sealed class ATan2OpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_dynamicOutputType = true;
			m_useInternalPortData = true;
			m_previewShaderGUID = "02e3ff61784e38840af6313936b6a730";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string result = string.Empty;
			switch ( m_mainDataType )
			{
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				{
					result =  "atan2( " + m_inputA + " , " + m_inputB + " )";
				}break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					result = UIUtils.InvalidParameter( this );
				}
				break;
			}
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}

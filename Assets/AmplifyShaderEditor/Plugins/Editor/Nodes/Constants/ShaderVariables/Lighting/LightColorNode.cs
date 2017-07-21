// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Light Color", "Light", "Light Color" )]
	public sealed class LightColorNode : ShaderVariablesNode
	{
		private const string m_lightColorValue = "_LightColor0";
		//private const string m_lightColorDeclaration = "uniform float4 LightColor0;";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "RGBA", WirePortDataType.COLOR );
		}

		public override string GenerateShaderForOutput( int outputId,  ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			//dataCollector.AddToUniforms( m_uniqueId, m_lightColorDeclaration );
			return m_lightColorValue;
		}
	}
}

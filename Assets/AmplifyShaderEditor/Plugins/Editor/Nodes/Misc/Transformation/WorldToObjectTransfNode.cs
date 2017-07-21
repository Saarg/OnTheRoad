// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "World To Object", "Misc", "Transforms input to Object Space" )]
	public sealed class WorldToObjectTransfNode : ParentTransfNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_matrixName = "unity_WorldToObject";
		}
	}
}

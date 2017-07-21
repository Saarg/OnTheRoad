// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "Object To World", "Misc", "Transforms input to World Space" )]
	public sealed class ObjectToWorldTransfNode : ParentTransfNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
#if UNITY_5_4_OR_NEWER
			m_matrixName = "unity_ObjectToWorld";
#else
            m_matrixName = "_Object2World";
#endif
		}
	}
}

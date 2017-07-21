// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Inverse", "Matrix", "Inverse matrix of a matrix" )]
	public sealed class InverseOpNode : SingleInputOp
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_opName = "inverse";
			m_drawPreview = false;
			m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.OBJECT,
													  WirePortDataType.FLOAT3x3,
													  WirePortDataType.FLOAT4x4 );
		}
	}
}

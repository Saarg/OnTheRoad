// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[System.Serializable]
	[NodeAttributes( "World Position", "Surface Standard Inputs", "World space position" )]
	public sealed class WorldPosInputsNode : SurfaceShaderINParentNode
	{
		//[SerializeField]
		//private bool m_addInstruction = false;

		//public override void Reset()
		//{
		//	base.Reset();
		//	m_addInstruction = true;
		//}

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = AvailableSurfaceInputs.WORLD_POS;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "70d5405009b31a349a4d8285f30cf5d9";
			InitialSetup();
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
				base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalVar );

			string worldPosition = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );

			return GetOutputVectorItem( 0, outputId, worldPosition );
		}
	}
}

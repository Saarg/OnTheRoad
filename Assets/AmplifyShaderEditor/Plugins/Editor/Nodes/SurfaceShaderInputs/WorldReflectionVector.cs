// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Reflection", "Surface Standard Inputs", "Per pixel world reflection vector", null, KeyCode.R )]
	public sealed class WorldReflectionVector : ParentNode
	{
		private int m_cachedPropertyId = -1;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, "Normal" );
			AddOutputVectorPorts( WirePortDataType.FLOAT3, "XYZ" );
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "8e267e9aa545eeb418585a730f50273e";
			UIUtils.AddNormalDependentCount();
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedPropertyId == -1 )
				m_cachedPropertyId = Shader.PropertyToID( "_Connected" );

			PreviewMaterial.SetFloat( m_cachedPropertyId, ( m_inputPorts[ 0 ].IsConnected ? 1 : 0 ) );
		}

		public override void Destroy()
		{
			ContainerGraph.RemoveNormalDependentCount();
			base.Destroy();
		}

		public override void PropagateNodeData( NodeData nodeData )
		{
			base.PropagateNodeData( nodeData );
			if ( m_inputPorts[ 0 ].IsConnected )
				UIUtils.CurrentDataCollector.DirtyNormal = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			bool isVertex = ( dataCollector.PortCategory == MasterNodePortCategory.Tessellation || dataCollector.PortCategory == MasterNodePortCategory.Vertex );
			if ( isVertex )
			{
				if ( m_outputPorts[ 0 ].IsLocalValue )
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );

				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_REFL ), true );
				dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
				
				dataCollector.AddToLocalVariables(UniqueId,  string.Format("float3 worldPos = mul( unity_ObjectToWorld, {0}.vertex ).xyz;", Constants.VertexShaderInputStr));
				dataCollector.AddToLocalVariables(UniqueId,  string.Format("float3 worldViewDir = UnityWorldSpaceViewDir( worldPos );"  , Constants.VertexShaderInputStr));
				dataCollector.AddToLocalVariables( UniqueId, string.Format( "fixed3 worldNormal = UnityObjectToWorldNormal( {0}.normal );", Constants.VertexShaderInputStr ) );

				RegisterLocalVariable( 0, "reflect( -worldViewDir, worldNormal )", ref dataCollector, "worldRefl" + OutputId );
				
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}
			else
			{
				if ( m_outputPorts[ 0 ].IsLocalValue )
					return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );


				dataCollector.AddToInput( UniqueId, UIUtils.GetInputDeclarationFromType( m_currentPrecisionType, AvailableSurfaceInputs.WORLD_REFL ), true );
				dataCollector.AddToInput( UniqueId, Constants.InternalData, false );
				string result = string.Empty;
				if ( m_inputPorts[ 0 ].IsConnected )
				{
					result = "WorldReflectionVector( " + Constants.InputVarStr + " , " + m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalVar ) + " )";
					dataCollector.ForceNormal = true;
				}
				else
				{
					if ( !dataCollector.DirtyNormal )
						result = Constants.InputVarStr + ".worldRefl";
					else
						result = "WorldReflectionVector( " + Constants.InputVarStr + " , float3(0,0,1) )";
				}

				RegisterLocalVariable( 0, result, ref dataCollector, "worldrefVec" + OutputId );
				return GetOutputVectorItem( 0, outputId, m_outputPorts[ 0 ].LocalValue );
			}
		}

	}
}

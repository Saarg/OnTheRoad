// Amplify Shader Editor - Advanced Bloom Post-Effect for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

// Billboard based on:
// https://gist.github.com/renaudbedard/7a90ec4a5a7359712202
using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public enum BillboardType
	{
		Cylindrical,
		Spherical
	}

	[Serializable]
	public class BillboardOpHelper
	{
		private const string BillboardTitleStr = " Billboard";
		private const string BillboardTypeStr = "Type";

		public readonly string[] BillboardCylindricalInstructions = {	"//Calculate new billboard vertex position and normal",
																		"float3 upCamVec = float3( 0, 1, 0 )",
																		};

		public readonly string[] BillboardSphericalInstructions = {   "//Calculate new billboard vertex position and normal",
																		"float3 upCamVec = normalize ( UNITY_MATRIX_V._m10_m11_m12 )"
																		};


		public readonly string[] BillboardCommonInstructions = {   	"float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 )",
																	"float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 )",
																	"float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 )",
																	"//This unfortunately must be made to take non-uniform scaling into account",
																	"//Transform to world coors, apply rotation and transform back to local",
																	"v.vertex = mul( v.vertex , unity_ObjectToWorld )",
																	"v.vertex = mul( v.vertex , rotationCamMatrix )",
																	"v.vertex = mul( v.vertex , unity_WorldToObject )",
																	"v.normal = normalize( mul( v.normal, rotationCamMatrix ))"};
		[SerializeField]
		private bool m_isBillboard = false;

		[SerializeField]
		private BillboardType m_billboardType = BillboardType.Cylindrical;

		public void Draw( UndoParentNode owner )
		{
			bool visible = EditorVariablesManager.ExpandedVertexOptions.Value;
			bool enabled = m_isBillboard;
			NodeUtils.DrawPropertyGroup( owner, ref visible, ref m_isBillboard, BillboardTitleStr, () =>
			{
				m_billboardType = ( BillboardType ) EditorGUILayout.EnumPopup( BillboardTypeStr, m_billboardType );
			} );

			EditorVariablesManager.ExpandedVertexOptions.Value = visible;
			if ( m_isBillboard != enabled )
			{
				UIUtils.RequestSave();
			}
		}

		// This should be called after the Vertex Offset and Vertex Normal ports are analised
		public void FillDataCollector( ref MasterNodeDataCollector dataCollector )
		{
			if ( m_isBillboard )
			{
				switch ( m_billboardType )
				{
					case BillboardType.Cylindrical:
					{
						for ( int i = 0; i < BillboardCylindricalInstructions.Length; i++ )
						{
							dataCollector.AddVertexInstruction( BillboardCylindricalInstructions[ i ], -1, true );
						}
					}break;

					case BillboardType.Spherical:
					{
						for ( int i = 0; i < BillboardCylindricalInstructions.Length; i++ )
						{
							dataCollector.AddVertexInstruction( BillboardSphericalInstructions[ i ], -1, true );
						}
					}break;
				}

				for ( int i = 0; i < BillboardCommonInstructions.Length; i++ )
				{
					dataCollector.AddVertexInstruction( BillboardCommonInstructions[ i ], -1, true );
				}
			}
		}
		
		public void ReadFromString( ref uint index, ref string[] nodeParams )
		{
			m_isBillboard = Convert.ToBoolean( nodeParams[ index++ ] );
			m_billboardType = ( BillboardType ) Enum.Parse( typeof( BillboardType ), nodeParams[ index++ ] );
		}

		public void WriteToString( ref string nodeInfo )
		{
			IOUtils.AddFieldValueToString( ref nodeInfo, m_isBillboard );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_billboardType );
		}

		public bool IsBillboard { get { return m_isBillboard; } }
	}
}

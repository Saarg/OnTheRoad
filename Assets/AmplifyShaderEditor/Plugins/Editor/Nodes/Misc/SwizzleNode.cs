// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Swizzle 
// Donated by Tobias Pott - @ Tobias Pott
// www.tobiaspott.de

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Swizzle", "Misc", "swizzle components of vector types " )]
	public sealed class SwizzleNode : SingleInputOp
	{

		private const string OutputTypeStr = "Output type";

		[SerializeField]
		private WirePortDataType _selectedOutputType = WirePortDataType.FLOAT4;

		[SerializeField]
		private int _selectedOutputTypeInt = 4;
		[SerializeField]
		private int[] _selectedOutputSwizzleTypes = new int[] { 0, 1, 2, 3 };


		private readonly string[] SwizzleVectorChannels = { "x", "y", "z", "w" };
		private readonly string[] SwizzleColorChannels = { "r", "g", "b", "a" };
		private readonly string[] SwizzleChannelLabels = { "Channel 0", "Channel 1", "Channel 2", "Channel 3" };

		private readonly string[] _outputValueTypes ={  "Float",
														"Vector2",
														"Vector3",
														"Vector4"};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].CreatePortRestrictions( WirePortDataType.FLOAT,
														WirePortDataType.FLOAT2,
														WirePortDataType.FLOAT3,
														WirePortDataType.FLOAT4,
														WirePortDataType.COLOR,
														WirePortDataType.INT );


			m_inputPorts[ 0 ].DataType = WirePortDataType.FLOAT4;
			m_outputPorts[ 0 ].DataType = _selectedOutputType;
			m_textLabelWidth = 90;
			m_autoWrapProperties = true;
			m_autoUpdateOutputPort = false;
		}

		public override void DrawProperties()
		{

			EditorGUILayout.BeginVertical();
			EditorGUI.BeginChangeCheck();
			_selectedOutputTypeInt = EditorGUILayoutPopup( OutputTypeStr, _selectedOutputTypeInt, _outputValueTypes );
			if ( EditorGUI.EndChangeCheck() )
			{
				switch ( _selectedOutputTypeInt )
				{
					case 0: _selectedOutputType = WirePortDataType.FLOAT; break;
					case 1: _selectedOutputType = WirePortDataType.FLOAT2; break;
					case 2: _selectedOutputType = WirePortDataType.FLOAT3; break;
					case 3: _selectedOutputType = WirePortDataType.FLOAT4; break;
				}

				UpdatePorts();
			}
			EditorGUILayout.EndVertical();

			// Draw base properties
			base.DrawProperties();

			EditorGUILayout.BeginVertical();

			int count = 0;

			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				count = 4;
				break;
				case WirePortDataType.FLOAT3:
				count = 3;
				break;
				case WirePortDataType.FLOAT2:
				count = 2;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				count = 1;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}

			int inputMaxChannelId = 0;
			switch ( m_inputPorts[ 0 ].DataType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				inputMaxChannelId = 3;
				break;
				case WirePortDataType.FLOAT3:
				inputMaxChannelId = 2;
				break;
				case WirePortDataType.FLOAT2:
				inputMaxChannelId = 1;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				inputMaxChannelId = 0;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}

			EditorGUI.BeginChangeCheck();
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				for ( int i = 0; i < count; i++ )
				{
					_selectedOutputSwizzleTypes[ i ] = Mathf.Clamp( EditorGUILayoutPopup( SwizzleChannelLabels[ i ], _selectedOutputSwizzleTypes[ i ], SwizzleColorChannels ), 0, inputMaxChannelId );
				}
			}
			else
			{
				for ( int i = 0; i < count; i++ )
				{
					_selectedOutputSwizzleTypes[ i ] = Mathf.Clamp( EditorGUILayoutPopup( SwizzleChannelLabels[ i ], _selectedOutputSwizzleTypes[ i ], SwizzleVectorChannels ), 0, inputMaxChannelId );
				}
			}
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdatePorts();
			}

			EditorGUILayout.EndVertical();
		}
		void UpdatePorts()
		{
			m_sizeIsDirty = true;
			ChangeOutputType( _selectedOutputType, false );
		}
		public string GetSwizzleComponentForChannel( int channel )
		{
			if ( m_inputPorts[ 0 ].DataType == WirePortDataType.COLOR )
			{
				return SwizzleColorChannels[ channel ];
			}
			else
			{
				return SwizzleVectorChannels[ channel ];
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{

			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;


			string value = string.Format( "({0}).", m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) );
			int count = 0;
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				count = 4;
				break;
				case WirePortDataType.FLOAT3:
				count = 3;
				break;
				case WirePortDataType.FLOAT2:
				count = 2;
				break;
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT:
				count = 1;
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				break;
			}
			for ( int i = 0; i < count; i++ )
			{
				value += GetSwizzleComponentForChannel( _selectedOutputSwizzleTypes[ i ] );
			}

			return CreateOutputLocalVariable( 0, value, ref dataCollector );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			_selectedOutputType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT: _selectedOutputTypeInt = 0; break;
				case WirePortDataType.FLOAT2: _selectedOutputTypeInt = 1; break;
				case WirePortDataType.FLOAT3: _selectedOutputTypeInt = 2; break;
				case WirePortDataType.COLOR:
				case WirePortDataType.FLOAT4: _selectedOutputTypeInt = 3; break;
			}
			for ( int i = 0; i < _selectedOutputSwizzleTypes.Length; i++ )
			{
				_selectedOutputSwizzleTypes[ i ] = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
			}

			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, _selectedOutputType );
			for ( int i = 0; i < _selectedOutputSwizzleTypes.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, _selectedOutputSwizzleTypes[ i ] );
			}
		}
	}
}

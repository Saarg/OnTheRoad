// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Append", "Misc", "Append channels to create a new component" )]
	public sealed class AppendNode : ParentNode
	{
		private const string OutputTypeStr = "Output type";

		[SerializeField]
		private WirePortDataType _selectedOutputType = WirePortDataType.FLOAT4;

		[SerializeField]
		private int _selectedOutputTypeInt = 2;

		[SerializeField]
		private float[] _defaultValues = { 0, 0, 0, 0 };
		private string[] _defaultValuesStr = { "[0]", "[1]", "[2]", "[3]" };

		private readonly string[] _outputValueTypes ={  "Vector2",
														"Vector3",
														"Vector4",
														"Color"};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, "[0]" );
			AddInputPort( WirePortDataType.FLOAT, false, "[1]" );
			AddInputPort( WirePortDataType.FLOAT, false, "[2]" );
			AddInputPort( WirePortDataType.FLOAT, false, "[3]" );
			AddOutputPort( _selectedOutputType, Constants.EmptyPortValue );
			m_textLabelWidth = 90;
			m_autoWrapProperties = true;
			m_previewShaderGUID = "d80ac81aabf643848a4eaa76f2f88d65";
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUILayout.BeginVertical();

			EditorGUI.BeginChangeCheck();
			_selectedOutputTypeInt = EditorGUILayoutPopup( OutputTypeStr, _selectedOutputTypeInt, _outputValueTypes );
			if ( EditorGUI.EndChangeCheck() )
			{
				switch ( _selectedOutputTypeInt )
				{
					case 0: _selectedOutputType = WirePortDataType.FLOAT2; break;
					case 1: _selectedOutputType = WirePortDataType.FLOAT3; break;
					case 2: _selectedOutputType = WirePortDataType.FLOAT4; break;
					case 3: _selectedOutputType = WirePortDataType.COLOR; break;
				}

				UpdatePorts();
			}

			int count = 0;
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				{
					count = 4;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					count = 3;
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					count = 2;
				}
				break;
				case WirePortDataType.OBJECT:
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			for ( int i = 0; i < count; i++ )
			{
				if ( !m_inputPorts[ i ].IsConnected )
					_defaultValues[ i ] = EditorGUILayoutFloatField( _defaultValuesStr[ i ], _defaultValues[ i ] );
			}

			EditorGUILayout.EndVertical();
		}
		void UpdatePorts()
		{
			m_sizeIsDirty = true;
			ChangeOutputType( _selectedOutputType, false );
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					m_inputPorts[ 0 ].Visible = true;
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = true;
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					m_inputPorts[ 0 ].Visible = true;
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = true;
					m_inputPorts[ 3 ].Visible = false;
					UIUtils.DeleteConnection( true, UniqueId, 3, false, true );
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					m_inputPorts[ 0 ].Visible = true;
					m_inputPorts[ 1 ].Visible = true;
					m_inputPorts[ 2 ].Visible = false;
					UIUtils.DeleteConnection( true, UniqueId, 2, false, true );

					m_inputPorts[ 3 ].Visible = false;
					UIUtils.DeleteConnection( true, UniqueId, 3, false, true );
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}
		}
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
				return m_outputPorts[ 0 ].LocalValue;

			string value = string.Empty;
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT4:
				case WirePortDataType.OBJECT:
				case WirePortDataType.COLOR:
				{
					value = "float4( ";
					for ( int i = 0; i < 4; i++ )
					{
						value += m_inputPorts[ i ].IsConnected ? InputPorts[ i ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar, true ) : _defaultValues[ i ].ToString();
						if ( i != 3 )
							value += " , ";
					}
					value += " )";
				}
				break;
				case WirePortDataType.FLOAT3:
				{
					value = "float3( ";
					for ( int i = 0; i < 3; i++ )
					{
						value += m_inputPorts[ i ].IsConnected ? InputPorts[ i ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar, true ) : _defaultValues[ i ].ToString();
						if ( i != 2 )
							value += " , ";
					}
					value += " )";
				}
				break;
				case WirePortDataType.FLOAT2:
				{
					value = "float2( ";
					for ( int i = 0; i < 2; i++ )
					{
						value += m_inputPorts[ i ].IsConnected ? InputPorts[ i ].GenerateShaderForOutput( ref dataCollector, WirePortDataType.FLOAT, ignoreLocalVar, true ) : _defaultValues[ i ].ToString();
						if ( i != 1 )
							value += " , ";
					}
					value += " )";
				}
				break;
				case WirePortDataType.FLOAT:
				case WirePortDataType.INT:
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{ }
				break;
			}

			RegisterLocalVariable( 0, value, ref dataCollector, "appendResult" + OutputId );
			return m_outputPorts[ 0 ].LocalValue;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			_selectedOutputType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT2: _selectedOutputTypeInt = 0; break;
				case WirePortDataType.FLOAT3: _selectedOutputTypeInt = 1; break;
				case WirePortDataType.FLOAT4: _selectedOutputTypeInt = 2; break;
				case WirePortDataType.COLOR: _selectedOutputTypeInt = 3; break;
			}
			for ( int i = 0; i < _defaultValues.Length; i++ )
			{
				_defaultValues[ i ] = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
			}
			UpdatePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, _selectedOutputType );
			for ( int i = 0; i < _defaultValues.Length; i++ )
			{
				IOUtils.AddFieldValueToString( ref nodeInfo, _defaultValues[ i ] );
			}
		}
	}
}

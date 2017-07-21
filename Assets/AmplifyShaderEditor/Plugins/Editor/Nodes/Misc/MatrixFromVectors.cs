using UnityEngine;
using UnityEditor;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Matrix From Vectors", "Misc", "Matrix From Vectors" )]
	public sealed class MatrixFromVectors : ParentNode
	{
		[SerializeField]
		private WirePortDataType _selectedOutputType = WirePortDataType.FLOAT3x3;

		[SerializeField]
		private int _selectedOutputTypeInt = 0;

		[SerializeField]
		private Vector3[] _defaultValuesV3 = { Vector3.zero, Vector3.zero, Vector3.zero };

		[SerializeField]
		private Vector4[] _defaultValuesV4 = { Vector4.zero, Vector4.zero, Vector4.zero, Vector4.zero };

		private string[] _defaultValuesStr = { "[0]", "[1]", "[2]", "[3]" };

		private readonly string[] _outputValueTypes ={  "Matrix3X3",
														"Matrix4X4"};

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort (WirePortDataType.FLOAT4, false, "[0]");
			AddInputPort (WirePortDataType.FLOAT4, false, "[1]");
			AddInputPort (WirePortDataType.FLOAT4, false, "[2]");
			AddInputPort (WirePortDataType.FLOAT4, false, "[3]");
			AddOutputPort( _selectedOutputType, Constants.EmptyPortValue );
			m_textLabelWidth = 90;
			m_autoWrapProperties = true;
			UpdatePorts ();
		}

		public override void DrawProperties()
		{
			base.DrawProperties ();
			EditorGUILayout.BeginVertical ();
			EditorGUI.BeginChangeCheck();
			_selectedOutputTypeInt = EditorGUILayoutPopup( "Output type", _selectedOutputTypeInt, _outputValueTypes );
			if ( EditorGUI.EndChangeCheck() )
			{
				switch ( _selectedOutputTypeInt )
				{
				case 0: _selectedOutputType = WirePortDataType.FLOAT3x3; break;
				case 1: _selectedOutputType = WirePortDataType.FLOAT4x4; break;
				}

				UpdatePorts();
			}

			int count = 0;
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT3x3:
				count = 3;
				for ( int i = 0; i < count; i++ )
				{
					if ( !m_inputPorts[ i ].IsConnected )
						_defaultValuesV3[ i ] = EditorGUILayoutVector3Field( _defaultValuesStr[ i ], _defaultValuesV3[ i ] );
				}
				break;
				case WirePortDataType.FLOAT4x4:
				count = 4;
				for ( int i = 0; i < count; i++ )
				{
					if ( !m_inputPorts[ i ].IsConnected )
						_defaultValuesV4[ i ] = EditorGUILayoutVector4Field( _defaultValuesStr[ i ], _defaultValuesV4[ i ] );
				}
				break;
			}

			EditorGUILayout.EndVertical();
		}

		void UpdatePorts()
		{
			m_sizeIsDirty = true;
			ChangeOutputType( _selectedOutputType, false );
			switch (_selectedOutputType) 
			{
			case WirePortDataType.FLOAT3x3:
				m_inputPorts [0].ChangeType (WirePortDataType.FLOAT3, false);
				m_inputPorts [1].ChangeType (WirePortDataType.FLOAT3, false);
				m_inputPorts [2].ChangeType (WirePortDataType.FLOAT3, false);
				m_inputPorts [3].ChangeType (WirePortDataType.FLOAT3, false);
				m_inputPorts[ 3 ].Visible = false;
				break;
			case WirePortDataType.FLOAT4x4:
				m_inputPorts [0].ChangeType (WirePortDataType.FLOAT4, false);
				m_inputPorts [1].ChangeType (WirePortDataType.FLOAT4, false);
				m_inputPorts [2].ChangeType (WirePortDataType.FLOAT4, false);
				m_inputPorts [3].ChangeType (WirePortDataType.FLOAT4, false);
				m_inputPorts[ 3 ].Visible = true;
				break;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput (outputId, ref dataCollector, ignoreLocalvar);
			string result = "";
			switch (_selectedOutputType) {
				case WirePortDataType.FLOAT3x3:
				result = "float3x3(" + m_inputPorts [0].GenerateShaderForOutput (ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar) + ", "
				+ m_inputPorts [1].GenerateShaderForOutput (ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar) + ", "
				+ m_inputPorts [2].GenerateShaderForOutput (ref dataCollector, WirePortDataType.FLOAT3, ignoreLocalvar) + ")";
				break;
				case WirePortDataType.FLOAT4x4:
				result = "float4x4(" + m_inputPorts [0].GenerateShaderForOutput (ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true) + ", "
				+ m_inputPorts [1].GenerateShaderForOutput (ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true) + ", "
				+ m_inputPorts [2].GenerateShaderForOutput (ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true) + ", "
				+ m_inputPorts [3].GenerateShaderForOutput (ref dataCollector, WirePortDataType.FLOAT4, ignoreLocalvar, true) + ")";
				break;
			}

			return result;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			_selectedOutputType = ( WirePortDataType ) Enum.Parse( typeof( WirePortDataType ), GetCurrentParam( ref nodeParams ) );
			switch ( _selectedOutputType )
			{
				case WirePortDataType.FLOAT3x3:
				_selectedOutputTypeInt = 0;
				break;
				case WirePortDataType.FLOAT4x4:
				_selectedOutputTypeInt = 1; 
				break;
			}
			UpdatePorts ();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, _selectedOutputType );
		}
	}
}

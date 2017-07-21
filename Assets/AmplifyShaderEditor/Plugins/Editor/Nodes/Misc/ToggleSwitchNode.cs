using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Toggle Switch", "Misc", "Switch between any of its input ports" )]
	public class ToggleSwitchNode : PropertyNode
	{
		private const string InputPortName = "In ";
		private const string CurrSelectedStr = "Current";
		private const string LerpOp = "lerp({0},{1},{2})";

		[SerializeField]
		private string[] AvailableInputsLabels = { "0", "1" };

		[SerializeField]
		private int[] AvailableInputsValues = { 0, 1 };

		[SerializeField]
		private int m_currentSelectedInput = 0;

		[SerializeField]
		private WirePortDataType m_mainDataType = WirePortDataType.FLOAT;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( m_mainDataType, false, InputPortName + "0" );
			AddInputPort( m_mainDataType, false, InputPortName + "1" );

			AddOutputPort( m_mainDataType, m_inputPorts[ 0 ].Name );
			m_insideSize.Set( 50, 25 );
			m_currentParameterType = PropertyType.Property;
			m_customPrefix = "Toggle Switch";
			m_availableAttribs.Add( new PropertyAttributes( "Toggle", "[Toggle]" ) );
			m_freeType = false;
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			m_inputPorts[ inputPortId ].MatchPortToConnection();
			UpdateOutputProperties();
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			m_inputPorts[ portId ].MatchPortToConnection();
			UpdateOutputProperties();
		}

		void UpdateOutputProperties()
		{
			m_mainDataType = ( UIUtils.GetPriority( m_inputPorts[ 0 ].DataType ) > UIUtils.GetPriority( m_inputPorts[ 1 ].DataType ) ) ? m_inputPorts[ 0 ].DataType : m_inputPorts[ 1 ].DataType;
			m_outputPorts[ 0 ].ChangeProperties( m_inputPorts[ m_currentSelectedInput ].Name, m_mainDataType, false );
			m_sizeIsDirty = true;
		}

		public override void Draw( DrawInfo drawInfo )
		{
			base.Draw( drawInfo );
			Rect newPos = m_remainingBox;
			newPos.width = 40 * drawInfo.InvertedZoom;
			newPos.height = 25 * drawInfo.InvertedZoom;
			newPos.x += m_remainingBox.width * 0.5f - newPos.width * 0.5f;

			EditorGUI.BeginChangeCheck();
			m_currentSelectedInput = EditorGUIIntPopup( newPos, m_currentSelectedInput, AvailableInputsLabels, AvailableInputsValues, UIUtils.SwitchNodePopUp );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdateOutputProperties();
			}
		}

		public override void DrawMainPropertyBlock()
		{
			base.DrawMainPropertyBlock();
			EditorGUILayout.Separator();
			EditorGUI.BeginChangeCheck();
			m_currentSelectedInput = EditorGUILayoutIntPopup( CurrSelectedStr, m_currentSelectedInput, AvailableInputsLabels, AvailableInputsValues );
			if ( EditorGUI.EndChangeCheck() )
			{
				UpdateOutputProperties();
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			m_precisionString = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, m_outputPorts[ 0 ].DataType );

			string resultA = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, m_mainDataType, ignoreLocalvar, true );
			string resultB = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_mainDataType, ignoreLocalvar, true );
			return string.Format( LerpOp, resultA, resultB, m_propertyName );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_currentSelectedInput = Convert.ToInt32( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_currentSelectedInput );
		}

		public override string GetPropertyValue()
		{
			return PropertyAttributes + m_propertyName + "(\"" + m_propertyInspectorName + "\", Float) = " + m_currentSelectedInput;
		}

		public override string GetUniformValue()
		{
			return string.Format( Constants.UniformDec, UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT ), m_propertyName );
		}

		public override void GetUniformData( out string dataType, out string dataName )
		{
			dataType = UIUtils.FinalPrecisionWirePortToCgType( m_currentPrecisionType, WirePortDataType.FLOAT );
			dataName = m_propertyName;
		}

		public override void UpdateMaterial( Material mat )
		{
			base.UpdateMaterial( mat );
			if ( UIUtils.IsProperty( m_currentParameterType ) )
			{
				mat.SetFloat( m_propertyName, ( float ) m_currentSelectedInput );
			}
		}

		public override void SetMaterialMode( Material mat , bool fetchMaterialValues )
		{
			base.SetMaterialMode( mat , fetchMaterialValues );
			if ( fetchMaterialValues && m_materialMode && UIUtils.IsProperty( m_currentParameterType ) && mat.HasProperty( m_propertyName ) )
			{
				m_currentSelectedInput = ( int ) mat.GetFloat( m_propertyName );
			}
		}

		public override void ForceUpdateFromMaterial( Material material )
		{
			if ( UIUtils.IsProperty( m_currentParameterType ) && material.HasProperty( m_propertyName ) )
				m_currentSelectedInput = ( int ) material.GetFloat( m_propertyName );
		}

		public override string GetPropertyValStr()
		{
			return m_currentSelectedInput.ToString();
		}
	}
}

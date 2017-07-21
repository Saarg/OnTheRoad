// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

namespace AmplifyShaderEditor
{
	public static class GeneratorUtils
	{
		public const string ObjectScaleStr = "ase_objectScale";
		public const string ScreenDepthStr = "ase_screenDepth";
		public const string ViewPositionStr = "ase_viewPos";
		public const string ClipPositionStr = "ase_clipPos";
		public const string VertexPositionStr = "ase_vertexPos";
		public const string ScreenPositionStr = "ase_screenPos";
		public const string WorldPositionStr = "ase_worldPos";
		public const string WorldNormalStr = "ase_worldNormal";
		public const string WorldTangentStr = "ase_worldTangent";
		public const string WorldBitangentStr = "ase_worldBitangent";
		public const string WorldToTangentStr = "ase_worldToTangent";
		private const string Float3Format = "float3 {0} = {1};";
		private const string Float4Format = "float4 {0} = {1};";

		// OBJECT SCALE
		static public string GenerateObjectScale( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			//string value= "1/float3( length( unity_WorldToObject[ 0 ].xyz ), length( unity_WorldToObject[ 1 ].xyz ), length( unity_WorldToObject[ 2 ].xyz ) );";
			string value = "float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );";
			dataCollector.AddLocalVariable( uniqueId, PrecisionType.Float,WirePortDataType.FLOAT3, ObjectScaleStr, value );
			return ObjectScaleStr;
		}

		// WORLD POSITION
		static public string GenerateWorldPosition( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			string result = Constants.InputVarStr + ".worldPos";

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				result = "mul(unity_ObjectToWorld, " + Constants.VertexShaderInputStr + ".vertex)";

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Format( Float3Format, WorldPositionStr, result ) );

			return WorldPositionStr;
		}

		// WORLD NORMAL
		static public string GenerateWorldNormal( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			string result = string.Empty;
			if ( !dataCollector.DirtyNormal )
				result = Constants.InputVarStr + ".worldNormal";
			else
				result = "WorldNormalVector( " + Constants.InputVarStr + ", float3( 0, 0, 1 ) )";

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				result = "UnityObjectToWorldNormal( " + Constants.VertexShaderInputStr + ".normal )";

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Format( Float3Format, WorldNormalStr, result ) );
			return WorldNormalStr;
		}

		// WORLD TANGENT
		static public string GenerateWorldTangent( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			string result = "WorldNormalVector( " + Constants.InputVarStr + ", float3( 1, 0, 0 ) )";

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
				result = "UnityObjectToWorldDir( " + Constants.VertexShaderInputStr + ".tangent.xyz )";

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Format( Float3Format, WorldTangentStr, result ) );
			return WorldTangentStr;
		}

		// WORLD BITANGENT
		static public string GenerateWorldBitangent( ref MasterNodeDataCollector dataCollector, int uniqueId )
		{
			string result = "WorldNormalVector( " + Constants.InputVarStr + ", float3( 0, 1, 0 ) )";

			if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			{
				string worldNormal = GenerateWorldNormal( ref dataCollector, uniqueId );
				string worldTangent = GenerateWorldTangent( ref dataCollector, uniqueId );
				dataCollector.AddToVertexLocalVariables( uniqueId, string.Format( "fixed tangentSign = {0}.tangent.w * unity_WorldTransformParams.w;", Constants.VertexShaderInputStr ) );
				result = "cross( " + worldNormal + ", " + worldTangent + " ) * tangentSign";
			}

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Format( Float3Format, WorldBitangentStr, result ) );
			return WorldBitangentStr;
		}

		// WORLD TO TANGENT MATRIX
		static public string GenerateWorldToTangentMatrix( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			string worldNormal = GenerateWorldNormal( ref dataCollector, uniqueId );
			string worldTangent = GenerateWorldTangent( ref dataCollector, uniqueId );
			string worldBitangent = GenerateWorldBitangent( ref dataCollector, uniqueId );

			dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, precision, WirePortDataType.FLOAT3x3, WorldToTangentStr, "float3x3(" + worldTangent + ", " + worldBitangent + ", " + worldNormal + ")" );
			return WorldToTangentStr;
		}

		// AUTOMATIC UVS
		static public string GenerateAutoUVs( ref MasterNodeDataCollector dataCollector, int uniqueId, int index, string propertyName = null, WirePortDataType size = WirePortDataType.FLOAT2 )
		{
			string result = string.Empty;

			if ( dataCollector.PortCategory == MasterNodePortCategory.Fragment || dataCollector.PortCategory == MasterNodePortCategory.Debug )
			{
				string dummyPropUV = "_texcoord" + ( index > 0 ? ( index + 1 ).ToString() : "" );
				string dummyUV = "uv" + ( index > 0 ? ( index + 1 ).ToString() : "" ) + dummyPropUV;

				dataCollector.AddToProperties( uniqueId, "[HideInInspector] " + dummyPropUV + "( \"\", 2D ) = \"white\" {}", 100 );
				dataCollector.AddToInput( uniqueId, UIUtils.WirePortToCgType( size ) + " " + dummyUV, true );

				result = Constants.InputVarStr + "." + dummyUV;
				if ( !string.IsNullOrEmpty( propertyName ) )
				{
					dataCollector.AddToUniforms( uniqueId, "uniform float4 " + propertyName + "_ST;" );
					//dataCollector.AddToLocalVariables( uniqueId, PrecisionType.Float, size, "uv" + propertyName, result + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );
					dataCollector.AddToLocalVariables(dataCollector.PortCategory, uniqueId, PrecisionType.Float, size, "uv" + propertyName, result + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw" );

					result = "uv" + propertyName;
				}
			}
			else
			{
				result = Constants.VertexShaderInputStr + ".texcoord";
				if ( index > 0 )
				{
					result += index.ToString();
				}

				switch ( size )
				{
					default:
					case WirePortDataType.FLOAT2:
					{
						result += ".xy";
					}
					break;
					case WirePortDataType.FLOAT3:
					{
						result += ".xyz";
					}
					break;
					case WirePortDataType.FLOAT4: break;
				}

				if ( !string.IsNullOrEmpty( propertyName ) )
				{
					dataCollector.AddToUniforms( uniqueId, "uniform float4 " + propertyName + "_ST;" );
					dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, PrecisionType.Float, size, "uv" + propertyName, Constants.VertexShaderInputStr + ".texcoord" + ( index > 0 ? index.ToString() : string.Empty ) + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
					//dataCollector.AddToVertexLocalVariables( uniqueId, UIUtils.WirePortToCgType( size ) + " uv" + propertyName + " = " + Constants.VertexShaderInputStr + ".texcoord" + ( index > 0 ? index.ToString() : string.Empty ) + " * " + propertyName + "_ST.xy + " + propertyName + "_ST.zw;" );
					result = "uv" + propertyName;
				}
			}
			return result;
		}

		// SCREEN POSITION
		static public string GenerateScreenPosition( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool addInput = true )
		{
			if( addInput )
				dataCollector.AddToInput( uniqueId, UIUtils.GetInputDeclarationFromType( precision, AvailableSurfaceInputs.SCREEN_POS ), true );

			string result = Constants.InputVarStr + ".screenPos";

			dataCollector.AddLocalVariable( uniqueId, string.Format( "float4 {0} = float4( {1}.xyz , {1}.w + 0.00000000001 );", ScreenPositionStr, result ) );

			//if ( dataCollector.PortCategory == MasterNodePortCategory.Vertex || dataCollector.PortCategory == MasterNodePortCategory.Tessellation )
			//	result = "mul(unity_ObjectToWorld, " + Constants.VertexShaderInputStr + ".vertex)";

			//dataCollector.AddToLocalVariables( dataCollector.PortCategory, uniqueId, string.Format( Float3Format, WorldPositionStr, result ) );

			return ScreenPositionStr;
		}

		// SCREEN POSITION ON VERT
		static public string GenerateVertexScreenPosition( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision, bool normalize )
		{
			string value = string.Format( "ComputeScreenPos( UnityObjectToClipPos( {0}.vertex ) )", Constants.VertexShaderInputStr );
			dataCollector.AddToVertexLocalVariables( uniqueId, precision,WirePortDataType.FLOAT4, ScreenPositionStr, value );
			if( normalize )
				dataCollector.AddToVertexLocalVariables( uniqueId, string.Format("{0} /= {0}.w", ScreenPositionStr ) );
			return ScreenPositionStr;
		}

		// VERTEX POSITION ON FRAG
		static public string GenerateVertexPositionOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision)
		{
			dataCollector.AddToInput( uniqueId, UIUtils.GetInputDeclarationFromType( precision, AvailableSurfaceInputs.WORLD_POS ), true );
			dataCollector.AddToIncludes( uniqueId, Constants.UnityShaderVariables );

#if UNITY_5_4_OR_NEWER
			string matrix = "unity_WorldToObject";
#else
			string matrix = "_World2Object";
#endif
			string value = "mul( " + matrix + ", float4( " + Constants.InputVarStr + ".worldPos , 1 ) )";
			
			
			dataCollector.AddToLocalVariables(uniqueId, precision, WirePortDataType.FLOAT4, VertexPositionStr, value );
			return VertexPositionStr;
		}

		// CLIP POSITION ON FRAG
		static public string GenerateClipPositionOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			string vertexName = GenerateVertexPositionOnFrag( ref dataCollector, uniqueId, precision );
			string value = string.Format( "ComputeScreenPos( UnityObjectToClipPos( {0} ) )",vertexName );
			dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT4, ClipPositionStr, value );
			return ClipPositionStr;
		}

		// VIEW POS
		static public string GenerateViewPositionOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			string vertexName = GenerateVertexPositionOnFrag( ref dataCollector, uniqueId, precision );
			string value = string.Format( "UnityObjectToViewPos( {0} )", vertexName );
			dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT3, ViewPositionStr, value );
			return ViewPositionStr;
		}

		// SCREEN DEPTH 
		static public string GenerateScreenDepthOnFrag( ref MasterNodeDataCollector dataCollector, int uniqueId, PrecisionType precision )
		{
			string viewPos = GenerateViewPositionOnFrag( ref dataCollector, uniqueId, precision );
			string value = string.Format( "-{0}.z" , viewPos );
			dataCollector.AddToLocalVariables( uniqueId, precision, WirePortDataType.FLOAT, ScreenDepthStr, value );
			return ScreenDepthStr;
		}
	}
}

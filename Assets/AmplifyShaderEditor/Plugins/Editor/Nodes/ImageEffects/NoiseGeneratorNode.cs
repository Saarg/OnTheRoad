// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
// Based on the work by https://github.com/keijiro/NoiseShader

using System;
using UnityEditor;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum NoiseGeneratorType
	{
		Simplex2D,
		Simplex3D
	};

	[Serializable]
	[NodeAttributes( "Noise Generator", "Image Effects", "Collection of procedural noise generators" )]
	public sealed class NoiseGeneratorNode : ParentNode
	{
		private const string TypeLabelStr = "Type";
		
		// Simplex 2D
		private const string Simplex2DFloat3Mod289Func = "float3 mod289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }";
		private const string Simplex2DFloat2Mod289Func = "float2 mod289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }";
		private const string Simplex2DPermuteFunc = "float3 permute( float3 x ) { return mod289( ( ( x * 34.0 ) + 1.0 ) * x ); }";

		private const string SimplexNoise2DHeader = "float snoise( float2 v )";
		private const string SimplexNoise2DFunc = "snoise( {0} )";
		private readonly string[] SimplexNoise2DBody = { "const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );",
														"float2 i = floor( v + dot( v, C.yy ) );",
														"float2 x0 = v - i + dot( i, C.xx );",
														"float2 i1;",
														"i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );",
														"float4 x12 = x0.xyxy + C.xxzz;",
														"x12.xy -= i1;",
														"i = mod289( i );",
														"float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );",
														"float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );",
														"m = m * m;",
														"m = m * m;",
														"float3 x = 2.0 * frac( p * C.www ) - 1.0;",
														"float3 h = abs( x ) - 0.5;",
														"float3 ox = floor( x + 0.5 );",
														"float3 a0 = x - ox;",
														"m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );",
														"float3 g;",
														"g.x = a0.x * x0.x + h.x * x0.y;",
														"g.yz = a0.yz * x12.xz + h.yz * x12.yw;",
														"return 130.0 * dot( m, g );" };
		// Simplex 3D

		

		private const string Simplex3DFloat3Mod289 = "float3 mod289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }";
		private const string Simplex3DFloat4Mod289 = "float4 mod289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }";
		private const string Simplex3DFloat4Permute = "float4 permute( float4 x ) { return mod289( ( x * 34.0 + 1.0 ) * x ); }";
		private const string TaylorInvSqrtFunc = "float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }";

		private const string SimplexNoise3DHeader = "float snoise( float3 v )";
		private const string SimplexNoise3DFunc = "snoise( {0} )";
		private readonly string[] SimplexNoise3DBody = {"const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );",
														"float3 i = floor( v + dot( v, C.yyy ) );",
														"float3 x0 = v - i + dot( i, C.xxx );",
														"float3 g = step( x0.yzx, x0.xyz );",
														"float3 l = 1.0 - g;",
														"float3 i1 = min( g.xyz, l.zxy );",
														"float3 i2 = max( g.xyz, l.zxy );",
														"float3 x1 = x0 - i1 + C.xxx;",
														"float3 x2 = x0 - i2 + C.yyy;",
														"float3 x3 = x0 - 0.5;",
														"i = mod289( i);",
														"float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );",
														"float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)",
														"float4 x_ = floor( j / 7.0 );",
														"float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)",
														"float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;",
														"float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;",
														"float4 h = 1.0 - abs( x ) - abs( y );",
														"float4 b0 = float4( x.xy, y.xy );",
														"float4 b1 = float4( x.zw, y.zw );",
														"float4 s0 = floor( b0 ) * 2.0 + 1.0;",
														"float4 s1 = floor( b1 ) * 2.0 + 1.0;",
														"float4 sh = -step( h, 0.0 );",
														"float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;",
														"float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;",
														"float3 g0 = float3( a0.xy, h.x );",
														"float3 g1 = float3( a0.zw, h.y );",
														"float3 g2 = float3( a1.xy, h.z );",
														"float3 g3 = float3( a1.zw, h.w );",
														"float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );",
														"g0 *= norm.x;",
														"g1 *= norm.y;",
														"g2 *= norm.z;",
														"g3 *= norm.w;",
														"float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );",
														"m = m* m;",
														"m = m* m;",
														"float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );",
														"return 42.0 * dot( m, px);"};


		[SerializeField]
		private NoiseGeneratorType m_type = NoiseGeneratorType.Simplex2D;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "Size" );
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			EditorGUI.BeginChangeCheck();
			m_type = (NoiseGeneratorType)EditorGUILayoutEnumPopup( TypeLabelStr, m_type );
			if ( EditorGUI.EndChangeCheck() )
			{
				ConfigurePorts();
			}
			EditorGUILayout.HelpBox( "Node still under construction. Use with caution" , MessageType.Info );

		}

		void ConfigurePorts()
		{
			switch ( m_type )
			{
				case NoiseGeneratorType.Simplex2D:
				{
					m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT2 , false ); 
				}break;

				case NoiseGeneratorType.Simplex3D:
				{
					m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT3, false );
				}break;
			}
		}
		
		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue )
			{
				return m_outputPorts[ 0 ].LocalValue;
			}
			switch ( m_type )
			{
				case NoiseGeneratorType.Simplex2D:
				{

					string float3Mod289Func = string.Empty;
					IOUtils.AddSingleLineFunction( ref float3Mod289Func, Simplex2DFloat3Mod289Func );
					dataCollector.AddFunction( Simplex2DFloat3Mod289Func, float3Mod289Func );

					string float2Mod289Func = string.Empty;
					IOUtils.AddSingleLineFunction( ref float2Mod289Func, Simplex2DFloat2Mod289Func );
					dataCollector.AddFunction( Simplex2DFloat2Mod289Func, float2Mod289Func );

					string permuteFunc = string.Empty;
					IOUtils.AddSingleLineFunction( ref permuteFunc, Simplex2DPermuteFunc );
					dataCollector.AddFunction( Simplex2DPermuteFunc, permuteFunc );


					string snoiseFunc = string.Empty;
					IOUtils.AddFunctionHeader( ref snoiseFunc, SimplexNoise2DHeader );
					for ( int i = 0; i < SimplexNoise2DBody.Length; i++ )
					{
						IOUtils.AddFunctionLine( ref snoiseFunc, SimplexNoise2DBody[ i ] );
					}
					IOUtils.CloseFunctionBody( ref snoiseFunc );
					dataCollector.AddFunction( SimplexNoise2DHeader, snoiseFunc );

					string size = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
					
					RegisterLocalVariable( 0, string.Format( SimplexNoise2DFunc, size ), ref dataCollector, ( "simplePerlin2D" + OutputId ) );
				}break;
				case NoiseGeneratorType.Simplex3D:
				{
					
					string float3Mod289Func = string.Empty;
					IOUtils.AddSingleLineFunction( ref float3Mod289Func, Simplex3DFloat3Mod289 );
					dataCollector.AddFunction( Simplex3DFloat3Mod289, float3Mod289Func );

					string float4Mod289Func = string.Empty;
					IOUtils.AddSingleLineFunction( ref float4Mod289Func, Simplex3DFloat4Mod289 );
					dataCollector.AddFunction( Simplex3DFloat4Mod289, float4Mod289Func );

					string permuteFunc = string.Empty;
					IOUtils.AddSingleLineFunction( ref permuteFunc, Simplex3DFloat4Permute );
					dataCollector.AddFunction( Simplex3DFloat4Permute, permuteFunc );

					string taylorInvSqrtFunc = string.Empty;
					IOUtils.AddSingleLineFunction( ref taylorInvSqrtFunc, TaylorInvSqrtFunc );
					dataCollector.AddFunction( TaylorInvSqrtFunc, taylorInvSqrtFunc );

					
					string snoiseFunc = string.Empty;
					IOUtils.AddFunctionHeader( ref snoiseFunc, SimplexNoise3DHeader );
					for ( int i = 0; i < SimplexNoise3DBody.Length; i++ )
					{
						IOUtils.AddFunctionLine( ref snoiseFunc, SimplexNoise3DBody[ i ] );
					}
					IOUtils.CloseFunctionBody( ref snoiseFunc );
					dataCollector.AddFunction( SimplexNoise3DHeader, snoiseFunc );

					string size = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );

					RegisterLocalVariable( 0, string.Format( SimplexNoise3DFunc, size ), ref dataCollector, ( "simplePerlin3D" + OutputId ) );
				}
				break;
			}
			return m_outputPorts[ 0 ].LocalValue;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_type = ( NoiseGeneratorType ) Enum.Parse( typeof( NoiseGeneratorType ), GetCurrentParam( ref nodeParams ) );
			ConfigurePorts();
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_type );
		}
	}
}

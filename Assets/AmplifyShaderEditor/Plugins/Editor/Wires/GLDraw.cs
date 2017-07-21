using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class GLDraw
	{
		/*
		* Clipping code: http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=230386#post230386
		* Thick line drawing code: http://unifycommunity.com/wiki/index.php?title=VectorLine
		*/
		public static Material LineMaterial = null;
		public static bool MultiLine = false;
		private static Shader m_shader = null;
		//private static bool m_clippingEnabled;
		//private static Rect m_clippingBounds = new Rect();
		private static Rect m_boundBox = new Rect();
		private static Vector3[] allv3Points = null;
		private static Vector2[] allPerpendiculars = null;
		private static Color[] allColors = null;
		private static Vector2 startPt = Vector2.zero;
		private static Vector2 endPt = Vector2.zero;

		private static Vector3 Up = new Vector3( 0, 1, 0 );
		private static Vector3 Zero = new Vector3( 0, 0, 0 );

		private static Vector2 aux1Vec2 = Vector2.zero;


		//public static void BeginGroup( Rect position )
		//{
		//	m_clippingEnabled = true;
		//	m_clippingBounds.Set( 0, 0, position.width, position.height );
		//	GUI.BeginGroup( position );
		//}

		//public static void EndGroup()
		//{
		//	GUI.EndGroup();
		//	m_clippingBounds.Set( 0, 0, Screen.width, Screen.height );
		//	m_clippingEnabled = false;
		//}

		public static void CreateMaterial()
		{
			if ( (object) LineMaterial != null && ( object ) m_shader != null )
				return;

			m_shader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "50fc796413bac8b40aff70fb5a886273" ) );
			LineMaterial = new Material( m_shader );

			LineMaterial.hideFlags = HideFlags.HideAndDontSave;
		}

		public static void DrawCurve( Vector3[] allPoints, Vector2[] allNormals, Color[] allColors )
		{
			if ( Event.current.type != EventType.repaint )
				return;

			CreateMaterial();
			LineMaterial.SetPass( (MultiLine ? 1 : 0) );

			GL.Begin( GL.TRIANGLE_STRIP );
			for ( int i = 0; i < allPoints.Length; i++ )
			{
				GL.Color( allColors[i] );
				GL.TexCoord( Zero );
				GL.Vertex3( allPoints[ i ].x - allNormals[ i ].x, allPoints[ i ].y - allNormals[ i ].y, 0 );
				GL.TexCoord( Up );
				GL.Vertex3( allPoints[ i ].x + allNormals[ i ].x, allPoints[ i ].y + allNormals[ i ].y, 0 );
			}
			GL.End();

		}

		public static Rect DrawBezier( Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int type = 1 )
		{
			int segments = Mathf.FloorToInt( ( start - end ).magnitude / 20 ) * 3; // Three segments per distance of 20
			return DrawBezier( start, startTangent, end, endTangent, color, width, segments, type );
		}

		public static Rect DrawBezier( Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments, int type = 1 )
		{
			return DrawBezier( start, startTangent, end, endTangent, color, color, width, segments, type );
		}

		public static Rect DrawBezier( Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color startColor, Color endColor, float width, int segments, int type = 1 )
		{
			int pointsCount = segments + 1;
			int linesCount = segments;
			allv3Points = Handles.MakeBezierPoints( start, end, startTangent, endTangent, pointsCount );
			allColors = new Color[ pointsCount ];
			allPerpendiculars = new Vector2[ pointsCount ];

			startColor.a = ( type * 0.25f );
			endColor.a = ( type * 0.25f );

			allColors[ 0 ] = startColor;

			float minX = allv3Points[ 0 ].x;
			float minY = allv3Points[ 0 ].y;
			float maxX = allv3Points[ 0 ].x;
			float maxY = allv3Points[ 0 ].y;

			for ( int i = 1; i < pointsCount; i++ )
			{
				allColors[ i ] = Color.LerpUnclamped( startColor, endColor, i / ( float )linesCount );

				minX = ( allv3Points[ i ].x < minX ) ? allv3Points[ i ].x : minX;
				minY = ( allv3Points[ i ].y < minY ) ? allv3Points[ i ].y : minY;
				maxX = ( allv3Points[ i ].x > maxX ) ? allv3Points[ i ].x : maxX;
				maxY = ( allv3Points[ i ].y > maxY ) ? allv3Points[ i ].y : maxY;
			}

			for ( int i = 0; i < pointsCount; i++ )
			{
				if ( i == 0 )
				{
					startPt.Set( startTangent.y, start.x );
					endPt.Set( start.y, startTangent.x );
				}
				else if ( i == pointsCount - 1 )
				{
					startPt.Set( end.y, endTangent.x );
					endPt.Set( endTangent.y, end.x );
				}
				else
				{
					startPt.Set( allv3Points[ i + 1 ].y, allv3Points[ i - 1 ].x );
					endPt.Set( allv3Points[ i - 1 ].y, allv3Points[ i + 1 ].x );
				}
				aux1Vec2.Set(startPt.x - endPt.x, startPt.y - endPt.y);
				FastNormalized( ref aux1Vec2 );
				aux1Vec2.Set( aux1Vec2.x * width, aux1Vec2.y * width );
				allPerpendiculars[ i ] = aux1Vec2;
			}

			m_boundBox.Set( minX, minY, ( maxX - minX ), ( maxY - minY ) );

			DrawCurve( allv3Points, allPerpendiculars, allColors );
			return m_boundBox;
		}

		private static void FastNormalized( ref Vector2 v )
		{
			float len = Mathf.Sqrt( v.x * v.x + v.y * v.y );
			v.Set( v.x / len, v.y / len );
		}

		public static void Destroy()
		{
			GameObject.DestroyImmediate( LineMaterial );
			LineMaterial = null;

			Resources.UnloadAsset( m_shader );
			m_shader = null;
		}
	}
}

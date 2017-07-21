// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.IO;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public class TemplatesManager
	{
		private static readonly string TemplatesDirStr = Application.dataPath + "/AmplifyShaderEditor/Templates";

		public static void GetAvailableTemplates()
		{
			//Fetch all Templates and Save them on a list
			DirectoryInfo dir = new DirectoryInfo( TemplatesDirStr );
			FileInfo[] info = dir.GetFiles( "*.ase" );
			for ( int i = 0; i < info.Length; i++ )
			{
				Debug.Log( info[ i ] );
			}
		}
	}
}

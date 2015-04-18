/*
 *  TexturePacker Importer
 *  (c) CodeAndWeb GmbH, Saalbaustraße 61, 89233 Neu-Ulm, Germany
 * 
 *  Use this script to import sprite sheets generated with TexturePacker.
 *  For more information see http://www.codeandweb.com/texturepacker/unity
 * 
 *  Thanks to Brendan Campbell for providing a first version of this script!
 *
 */

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TexturePackerImporter : AssetPostprocessor
{

	static string[] textureExtensions = {
		".png",
		".jpg",
		".jpeg",
		".tiff",
		".tga",
		".bmp"
	};

	/*
	 *  Trigger a texture file re-import each time the .tpsheet file changes (or is manually re-imported)
	 */
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (var asset in importedAssets) {
			if (!Path.GetExtension (asset).Equals (".tpsheet"))
				continue;
			foreach (string ext in textureExtensions) {
				string pathToTexture = Path.ChangeExtension (asset, ext);
				if (File.Exists (pathToTexture)) {
					AssetDatabase.ImportAsset (pathToTexture, ImportAssetOptions.ForceUpdate);
					break;
				}
			}
		}
	}


	/*
	 *  Trigger a sprite sheet update each time the texture file changes (or is manually re-imported)
	 */
	void OnPreprocessTexture ()
	{
		TextureImporter importer = assetImporter as TextureImporter;

		string pathToData = Path.ChangeExtension (assetPath, ".tpsheet");
		if (File.Exists (pathToData)) {
			updateSpriteMetaData (importer, pathToData);
		}
	}

	static void updateSpriteMetaData (TextureImporter importer, string pathToData)
	{
		string nom = pathToData;
		string pattern = @"[^/]+$";
		Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
		MatchCollection matches = rgx.Matches(nom);
		nom = matches[ 0 ].Value.Replace( ".tpsheet", "" );

		int textureWidth = 0;
		int textureHeight = 0;

		BorderTextureData scriptableObj = ScriptableObject.CreateInstance<BorderTextureData>();
		
		scriptableObj.idName = nom;
		scriptableObj.names = new List<string>();
		scriptableObj.rects = new List<Rect>();

		scriptableObj.names.Add( "64" );
		scriptableObj.rects.Add( new Rect( 0, 0, 0, 0 ) );

		//

		if (importer.textureType != TextureImporterType.Advanced) {
			importer.textureType = TextureImporterType.Sprite;
		}
		importer.maxTextureSize = 4096;
		importer.spriteImportMode = SpriteImportMode.Multiple;

		string[] dataFileContent = File.ReadAllLines(pathToData);
		int format = 30302;

		foreach (string row in dataFileContent)
		{
			if (row.StartsWith(":format=")) {
				format = int.Parse(row.Remove(0,8));
			} else if( row.StartsWith("#SZW "))
				textureWidth = int.Parse( row.Replace( "#SZW", "" ) );
			else if( row.StartsWith("#SZH "))
				textureHeight = int.Parse( row.Replace( "#SZH", "" ) );
		}
		if (format != 30302) {
			EditorUtility.DisplayDialog("Please update TexturePacker Importer", "Your TexturePacker Importer is too old, \nplease load a new version from the asset store!", "Ok");
			return;
		}

		List<SpriteMetaData> metaData = new List<SpriteMetaData> ();
		foreach (string row in dataFileContent) {
			if (string.IsNullOrEmpty (row) || row.StartsWith ("#") || row.StartsWith (":"))
				continue; // comment lines start with #, additional atlas properties with :

			string [] cols = row.Split (';');
			if (cols.Length < 7)
				return; // format error

			SpriteMetaData smd = new SpriteMetaData ();
			smd.name = cols [0].Replace ("/", "-");  // unity has problems with "/" in sprite names...
			float x = float.Parse (cols [1]);
			float y = float.Parse (cols [2]);
			float w = float.Parse (cols [3]);
			float h = float.Parse (cols [4]);
			float px = float.Parse (cols [5]);
			float py = float.Parse (cols [6]);

			smd.rect = new UnityEngine.Rect (x, y, w, h);
			smd.pivot = new UnityEngine.Vector2 (px, py);

			scriptableObj.names.Add( smd.name );
			scriptableObj.rects.Add( smd.rect );

			if (px == 0 && py == 0)
				smd.alignment = (int)UnityEngine.SpriteAlignment.BottomLeft;
			else if (px == 0.5 && py == 0)
				smd.alignment = (int)UnityEngine.SpriteAlignment.BottomCenter;
			else if (px == 1 && py == 0)
				smd.alignment = (int)UnityEngine.SpriteAlignment.BottomRight;
			else if (px == 0 && py == 0.5)
				smd.alignment = (int)UnityEngine.SpriteAlignment.LeftCenter;
			else if (px == 0.5 && py == 0.5)
				smd.alignment = (int)UnityEngine.SpriteAlignment.Center;
			else if (px == 1 && py == 0.5)
				smd.alignment = (int)UnityEngine.SpriteAlignment.RightCenter;
			else if (px == 0 && py == 1)
				smd.alignment = (int)UnityEngine.SpriteAlignment.TopLeft;
			else if (px == 0.5 && py == 1)
				smd.alignment = (int)UnityEngine.SpriteAlignment.TopCenter;
			else if (px == 1 && py == 1)
				smd.alignment = (int)UnityEngine.SpriteAlignment.TopRight;
			else
				smd.alignment = (int)UnityEngine.SpriteAlignment.Custom;

			metaData.Add (smd);
		}

		scriptableObj.textureWidth = textureWidth;
		scriptableObj.textureHeight = textureHeight;

		AssetDatabase.CreateAsset( scriptableObj, "Assets/CivGrid/Borders/" + nom + ".asset" );
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = scriptableObj;

		// assign the scriptable object here


		importer.spritesheet = metaData.ToArray();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using CivGrid;


namespace CivGrid.Editors
{
    [CustomEditor(typeof(WorldManager))]
    public class WorldManagerEditor : Editor
    {
        WorldManager worldManager;
        bool foldoutOpen;

        void Awake()
        {
            worldManager = (WorldManager)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.SelectableLabel("World Settings", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;

            worldManager.generateOnStart = EditorGUILayout.Toggle("Generate World On Startup", worldManager.generateOnStart);
            worldManager.useCivGridCamera = EditorGUILayout.Toggle("Use Default CivGrid Camera", worldManager.useCivGridCamera);
            worldManager.generateNodeLocations = EditorGUILayout.Toggle("Generate Pathfinding Node Locations", worldManager.generateNodeLocations);
            worldManager.useWorldTypeValues = EditorGUILayout.Toggle("Use Preset World Values", worldManager.useWorldTypeValues);

            EditorGUILayout.Separator();

            if (worldManager.useWorldTypeValues == false)
            {
                worldManager.noiseScale = EditorGUILayout.FloatField("Noise Scale", worldManager.noiseScale);
            }
            else
            {
                worldManager.worldType = (WorldType)EditorGUILayout.EnumPopup("World Type", worldManager.worldType);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Separator();

            EditorGUILayout.SelectableLabel("Map Size", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            worldManager.mapSize = EditorGUILayout.Vector2Field("Map Size", worldManager.mapSize);
            worldManager.chunkSize = EditorGUILayout.IntField("Chunk Size", worldManager.chunkSize);

            if (((worldManager.mapSize.x % worldManager.chunkSize) + (worldManager.mapSize.y % worldManager.chunkSize)) != 0)
            {
                EditorGUILayout.HelpBox("Map Size must be divisible by Chunk Size", MessageType.Error);
            }

            if((worldManager.chunkSize % 2) != 0)
            {
                EditorGUILayout.HelpBox("Chunk Size must be an even number", MessageType.Error);
            }

            worldManager.hexRadiusSize = EditorGUILayout.FloatField("Hex Radius Size", worldManager.hexRadiusSize);

            EditorGUI.indentLevel--;


            EditorGUILayout.SelectableLabel("Tile Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;
            worldManager.levelOfDetail = EditorGUILayout.IntSlider("Level of Detail", worldManager.levelOfDetail, 0, 3);
            worldManager.LOD2 = (Mesh)EditorGUILayout.ObjectField("LOD 2", worldManager.LOD2, typeof(Mesh), false);
            worldManager.LOD3 = (Mesh)EditorGUILayout.ObjectField("LOD 3", worldManager.LOD3, typeof(Mesh), false);
            EditorGUI.indentLevel--;
			
			if(worldManager.levelOfDetail > 0)
			{
				EditorGUI.indentLevel++;
            	EditorGUILayout.SelectableLabel("Flat Settings", EditorStyles.boldLabel);
            	EditorGUI.indentLevel++;
            	worldManager.flatDefines.maximumHeight = EditorGUILayout.FloatField("Maximum Height", worldManager.flatDefines.maximumHeight);
            	worldManager.flatDefines.noiseScale = EditorGUILayout.FloatField("Noise Scale", worldManager.flatDefines.noiseScale);
            	worldManager.flatDefines.noiseSize = EditorGUILayout.FloatField("Noise Size", worldManager.flatDefines.noiseSize);
            	EditorGUI.indentLevel--;
            	EditorGUI.indentLevel--;
				
            	EditorGUI.indentLevel++;
            	EditorGUILayout.SelectableLabel("Hill Settings", EditorStyles.boldLabel);
            	EditorGUI.indentLevel++;
            	worldManager.hillDefines.maximumHeight = EditorGUILayout.FloatField("Maximum Height", worldManager.hillDefines.maximumHeight);
            	worldManager.hillDefines.noiseScale = EditorGUILayout.FloatField("Noise Scale", worldManager.hillDefines.noiseScale);
            	worldManager.hillDefines.noiseSize = EditorGUILayout.FloatField("Noise Size", worldManager.hillDefines.noiseSize);
            	EditorGUI.indentLevel--;
            	EditorGUI.indentLevel--;

            	EditorGUI.indentLevel++;
            	EditorGUILayout.SelectableLabel("Mountain Settings", EditorStyles.boldLabel);
            	EditorGUI.indentLevel++;
				worldManager.mountainHeightMap = (Texture2D)EditorGUILayout.ObjectField("Base Heightmap", worldManager.mountainHeightMap, typeof(Texture2D), false);
            	worldManager.mountainDefines.yScale = EditorGUILayout.FloatField("Vertical Size", worldManager.mountainDefines.yScale);
            	worldManager.mountainDefines.maximumHeight = EditorGUILayout.FloatField("Maximum Height", worldManager.mountainDefines.maximumHeight);
            	worldManager.mountainDefines.noiseScale = EditorGUILayout.FloatField("Noise Scale", worldManager.mountainDefines.noiseScale);
            	worldManager.mountainDefines.noiseSize = EditorGUILayout.FloatField("Noise Size", worldManager.mountainDefines.noiseSize);
            	EditorGUI.indentLevel--;
            	EditorGUI.indentLevel--;
			}
			
			EditorGUILayout.SelectableLabel("Grid Settings", EditorStyles.boldLabel);
			
			EditorGUI.indentLevel++;
			
            worldManager.ShowGrid = EditorGUILayout.Toggle("Show Grid", worldManager.ShowGrid);
			worldManager.gridTexture = (Texture2D)EditorGUILayout.ObjectField("Grid Texture", worldManager.gridTexture, typeof(Texture2D), false);
			
            EditorGUI.indentLevel--;

            EditorGUILayout.SelectableLabel("Fog of War Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            worldManager.lightFogTexture = (Texture2D)EditorGUILayout.ObjectField("Light Fog Texture", worldManager.lightFogTexture, typeof(Texture2D), false);
            worldManager.deepFogTexture = (Texture2D)EditorGUILayout.ObjectField("Deep Fog Texture", worldManager.deepFogTexture, typeof(Texture2D), false);

            EditorGUI.indentLevel--;

            EditorGUILayout.SelectableLabel("Border Settings", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel++;
            
            worldManager.sprShDefBorders = (BorderTextureData)EditorGUILayout.ObjectField("Border Definitions", worldManager.sprShDefBorders, typeof(BorderTextureData), false);
			worldManager.borderTexture = (Texture2D)EditorGUILayout.ObjectField("Border Texture", worldManager.borderTexture, typeof(Texture2D), false);
            if(GUILayout.Button("Add New Border Define"))
            {
                worldManager.borderColors.Add(Color.black);
            }
            for(int i = 0; i < worldManager.borderColors.Count; i++)
            {
				EditorGUILayout.BeginHorizontal();
                worldManager.borderColors[i] = EditorGUILayout.ColorField("Border Color " + i, worldManager.borderColors[i]);
				if(GUILayout.Button("X"))
				{
					worldManager.borderColors.RemoveAt(i);
				}
				EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.indentLevel--;

        }
    }
}
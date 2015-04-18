using UnityEngine;
using System.Collections;
using UnityEditor;

namespace CivGrid.Editors
{
    [CustomEditor(typeof(TileManager))]
    public class TileManagerEditor : Editor
    {
        TileManager tileManager;
        ResourceManager resourceManager;
        ImprovementManager improvementManager;
        bool[] foldoutOpen = new bool[0];


        public void Awake()
        {
            tileManager = (TileManager)target;
            resourceManager = tileManager.GetComponent<ResourceManager>();
            improvementManager = tileManager.GetComponent<ImprovementManager>();
            if (tileManager.tiles != null)
            {
                foldoutOpen = new bool[tileManager.tiles.Count];
            }
        }

        bool done;
        //bool longOpen = false;
        //bool latOpen = false;
        public override void OnInspectorGUI()
        {
            if (done == false)
            {
                Awake();

                done = true;
            }

            if (tileManager == null) { Awake(); }
            if (tileManager.tiles != null && foldoutOpen.Length != tileManager.tiles.Count) { Awake(); }

            if (GUILayout.Button("Add New Tile"))
            {
                TileEditorWindow window = EditorWindow.CreateInstance<TileEditorWindow>();
                EditorWindow.GetWindow<TileEditorWindow>();
                window.editMode = false;
                window.tileIndexToEdit = 0;
            }

            if (tileManager.tiles != null && tileManager.tiles.Count > 0)
            {
                string error;
                if(CheckForNeededTiles(out error) == false)
                {
                    EditorGUILayout.HelpBox(error, MessageType.Error);
                }
                EditorGUI.indentLevel++;
                for (int i = 0; i < tileManager.tiles.Count; i++)
                {
                    Tile tile = tileManager.tiles[i];

                    EditorGUILayout.BeginHorizontal();

                    foldoutOpen[i] = EditorGUILayout.Foldout(foldoutOpen[i], tile.name);

                    if (GUILayout.Button("Edit"))
                    {
                        TileEditorWindow window = EditorWindow.CreateInstance<TileEditorWindow>();
                        EditorWindow.GetWindow<TileEditorWindow>();
                        window.editMode = true;
                        window.tileIndexToEdit = i;
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        resourceManager.DeleteDependencies(i);
                        improvementManager.DeleteDependencies(i);
                        tileManager.DeleteTile(tile);
                    }

                    EditorGUILayout.EndHorizontal();

                    if (foldoutOpen[i])
                    {
                        tile.name = EditorGUILayout.TextField("Name:", tile.name);
                        tile.isShore = EditorGUILayout.Toggle("Is Shore:", tile.isShore);
                        tile.isOcean = EditorGUILayout.Toggle("Is Ocean:", tile.isOcean);
                        tile.isMountain = EditorGUILayout.Toggle("Is Mountain:", tile.isMountain);
                        if (tile.isShore == false && tile.isOcean == false && tile.isMountain == false)
                        {

                            EditorGUI.indentLevel++;

                            EditorGUILayout.SelectableLabel("Rainfall Spawn:", EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.BeginHorizontal();
                            tile.possibleRainfallValues.min = EditorGUILayout.FloatField("Minimum", tile.possibleRainfallValues.min);
                            tile.possibleRainfallValues.max = EditorGUILayout.FloatField("Maximum", tile.possibleRainfallValues.max);
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel--;

                            EditorGUILayout.SelectableLabel("Temperature Spawn:", EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.BeginHorizontal();
                            tile.possibleTemperatureValues.min = EditorGUILayout.FloatField("Minimum", tile.possibleTemperatureValues.min);
                            tile.possibleTemperatureValues.max = EditorGUILayout.FloatField("Maximum", tile.possibleTemperatureValues.max);
                            EditorGUILayout.EndHorizontal();
                            EditorGUI.indentLevel--;

                            EditorGUI.indentLevel--;

                            //if (tileManager.useLatAndLong)
                            //{
                            //    EditorGUI.indentLevel++;
                            //    longOpen = EditorGUILayout.Foldout(longOpen, "Longitude Clamps:");
                            //    if (longOpen)
                            //    {
                            //        tile.possibleWorldDegrees.top = EditorGUILayout.FloatField("Top Longitude:", tile.possibleWorldDegrees.top);
                            //        tile.possibleWorldDegrees.bottom = EditorGUILayout.FloatField("Bottom Longitude:", tile.possibleWorldDegrees.bottom);
                            //    }
                            //    latOpen = EditorGUILayout.Foldout(latOpen, "Latitude Clamps:");
                            //    if (latOpen)
                            //    {
                            //        tile.possibleWorldDegrees.left = EditorGUILayout.FloatField("Left Latitude:", tile.possibleWorldDegrees.left);
                            //        tile.possibleWorldDegrees.right = EditorGUILayout.FloatField("Right Latitude:", tile.possibleWorldDegrees.right);
                            //    }
                            //    EditorGUI.indentLevel--;
                            //}
                        }
                    }
                }
            }
            else
            {
                GUILayout.Label("No Tiles Created; Please Add Some");
            }
            EditorGUI.indentLevel--;

            if (GUILayout.Button("Finalize"))
            {
                tileManager.UpdateTileNames();
            }
        }



        public bool CheckForNeededTiles(out string errorMessage)
        {
            bool hasOcean = false;
            bool hasShore = false;
            foreach(Tile t in tileManager.tiles)
            {
                if(t.isOcean == true)
                {
                    hasOcean = true;
                }
                if(t.isShore == true)
                {
                    hasShore = true;
                }
            }

            if(hasShore == false)
            {
                errorMessage = "A tile marked as \"Shore\" is needed.";
                return false;
            }
            else if(hasOcean == false)
            {
                errorMessage = "A tile marked as \"Ocean\" is needed.";
                return false;
            }
            else
            {
                errorMessage = "";
                return true;
            }
        }
    }
}
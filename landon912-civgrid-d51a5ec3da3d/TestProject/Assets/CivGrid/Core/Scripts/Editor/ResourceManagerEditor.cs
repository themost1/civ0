using UnityEngine;
using System.Collections;
using UnityEditor;
using CivGrid;


namespace CivGrid.Editors
{
    [CustomEditor(typeof(ResourceManager))]
    public class ResourceManagerEditor : Editor
    {

        ResourceManager resourceManager;
        WorldManager worldManager;
        TileManager tileManager;
        bool[] foldoutOpen;
        bool[] extraInfoFoldout;

        void Awake()
        {
            resourceManager = (ResourceManager)target;
            tileManager = resourceManager.GetComponent<TileManager>();
            if (resourceManager.resources != null)
            {
                foldoutOpen = new bool[resourceManager.resources.Count];
                extraInfoFoldout = new bool[resourceManager.resources.Count];
            }
        }
		
        bool done;
        public override void OnInspectorGUI()
        {
            if (done == false)
            {
                Awake();

                done = true;
            }

            if (resourceManager == null) { resourceManager = (ResourceManager)target; }
            if (worldManager == null) { worldManager = resourceManager.GetComponent<WorldManager>(); }
            if (tileManager == null) { tileManager = resourceManager.GetComponent<TileManager>(); }
            if (resourceManager.resources != null && (foldoutOpen == null || foldoutOpen.Length == 0 || foldoutOpen.Length != resourceManager.resources.Count)) { foldoutOpen = new bool[resourceManager.resources.Count]; }
            if (resourceManager.resources != null && (extraInfoFoldout == null || extraInfoFoldout.Length != resourceManager.resources.Count)) { extraInfoFoldout = new bool[resourceManager.resources.Count]; }

            if (GUILayout.Button("Add New Resource"))
            {
                ResourceEditorWindow window = EditorWindow.CreateInstance<ResourceEditorWindow>();
                EditorWindow.GetWindow<ResourceEditorWindow>();
                window.editMode = false;
                window.resourceIndexToEdit = 0;
            }

            if (resourceManager.resources != null && resourceManager.resources.Count > 0)
            {
                for (int i = 0; i < resourceManager.resources.Count; i++)
                {
                    Resource resource = resourceManager.resources[i];

                    EditorGUILayout.BeginHorizontal();

                    foldoutOpen[i] = EditorGUILayout.Foldout(foldoutOpen[i], resource.name);

                    if (GUILayout.Button("Edit"))
                    {
                        ResourceEditorWindow window = EditorWindow.CreateInstance<ResourceEditorWindow>();
                        EditorWindow.GetWindow<ResourceEditorWindow>();
                        window.editMode = true;
                        window.resourceIndexToEdit = i;
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        resourceManager.DeleteResource(resource);
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel++;
                    if (foldoutOpen[i])
                    {
                        resource.name = EditorGUILayout.TextField("Resource Name:", resource.name);
                        resource.rarity = EditorGUILayout.FloatField("Rarity:", resource.rarity);
                        resource.meshSpawnAmount = EditorGUILayout.IntField("Spawn Amount:", resource.meshSpawnAmount);
                        resource.replaceGroundTexture = EditorGUILayout.Toggle("Replace Ground Texture", resource.replaceGroundTexture);

                        extraInfoFoldout[i] = EditorGUILayout.Foldout(extraInfoFoldout[i], "Rules:");

                        if (extraInfoFoldout[i])
                        {
                            EditorGUILayout.SelectableLabel("Possible Tiles", EditorStyles.boldLabel, GUILayout.ExpandHeight(false), GUILayout.MaxHeight(15));
							EditorGUI.indentLevel++;
							if(resource.rule.possibleTiles.Length == 0)
							{
								EditorGUILayout.SelectableLabel("No Possible Tiles");
							}
							else
							{
								foreach (int t in resource.rule.possibleTiles)
                            	{
                                	EditorGUI.indentLevel++;
                                	EditorGUILayout.SelectableLabel(tileManager.tiles[t].name, GUILayout.ExpandHeight(false), GUILayout.MaxHeight(18));
                               		EditorGUI.indentLevel--;
                            	}
							}
							EditorGUI.indentLevel--;

                            EditorGUILayout.SelectableLabel("Possible Features", EditorStyles.boldLabel, GUILayout.ExpandHeight(false), GUILayout.MaxHeight(15));
							EditorGUI.indentLevel++;
							if(resource.rule.possibleFeatures.Length == 0)
							{
								EditorGUILayout.SelectableLabel("No Possible Features");	
							}
							else
							{
								foreach (Feature f in resource.rule.possibleFeatures)
                            	{
                                	EditorGUI.indentLevel++;
                                	EditorGUILayout.SelectableLabel(f.ToString(), GUILayout.ExpandHeight(false), GUILayout.MaxHeight(18));
                                	EditorGUI.indentLevel--;
                            	}
							}
							EditorGUI.indentLevel--;
							
							EditorGUILayout.Separator();
                        }
                        resource.meshToSpawn = (Mesh)EditorGUILayout.ObjectField("Resource Mesh", (Object)resource.meshToSpawn, typeof(Mesh), false);
                        resource.meshTexture = (Texture2D)EditorGUILayout.ObjectField("Resource Mesh Texture:", (Object)resource.meshTexture, typeof(Texture2D), false, GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                GUILayout.Label("No Resources Created; Please Add Some");
            }

            if (GUILayout.Button("Finalize"))
            {
                resourceManager.UpdateResourceNames();
            }
        }
    }
}
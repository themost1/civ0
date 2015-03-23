using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CivGrid;

namespace CivGrid.Editors
{

    public sealed class CivGridEditor
    {

    }

    public sealed class ResourceEditorWindow : EditorWindow
    {
        public bool editMode;
        public int resourceIndexToEdit;

        //adding fields
        string rName = "None";
        float rRariety;
        int rSpawnAmount;
        Mesh rMesh;
        Texture2D rTexture;
        bool rReplaceGroundTexture;
        ResourceManager resourceManager;
        TileManager tileManager;

        List<int> rPossibleTiles = new List<int>();
        List<Feature> rPossibleFeatures = new List<Feature>();

        [MenuItem("CivGrid/New Resource", priority = 3)]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ResourceEditorWindow));
        }

        void OnEnable()
        {
            resourceManager = GameObject.FindObjectOfType<ResourceManager>();
            tileManager = GameObject.FindObjectOfType<TileManager>();
        }


        Vector2 scrollPosition = new Vector2();
        bool doneAddingResources = false;
        void OnGUI()
        {
            if (editMode == false)
            {
                this.title = "Add Resource";

                GUILayout.BeginVertical();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                GUILayout.Label("Add Resource:", EditorStyles.boldLabel);
                rName = EditorGUILayout.TextField("Resource Name", rName);
                rRariety = EditorGUILayout.FloatField("Rariety", rRariety);
                rSpawnAmount = EditorGUILayout.IntField("Mesh Spawn Amount:", rSpawnAmount);
                rReplaceGroundTexture = EditorGUILayout.Toggle("Replace Ground Texture", rReplaceGroundTexture);
                if (rReplaceGroundTexture == true) { EditorGUILayout.HelpBox("Remember to add a texture to the terrain atlas for this resource.", MessageType.Info); }
                rMesh = (Mesh)EditorGUILayout.ObjectField("Resource Mesh:", (Object)rMesh, typeof(Mesh), false);
                rTexture = (Texture2D)EditorGUILayout.ObjectField("Resource Mesh Texture:", (Object)rTexture, typeof(Texture2D), false);
                GUILayout.Label("Possible Tiles", EditorStyles.boldLabel);

                if (tileManager.tiles.Count > 0)
                {
                    if (GUILayout.Button("+"))
                    {
                        rPossibleTiles.Add(0);
                    }

                    for (int i = 0; i < rPossibleTiles.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        rPossibleTiles[i] = (int)EditorGUILayout.Popup(rPossibleTiles[i], tileManager.tileNames);

                        if (GUILayout.Button("-"))
                        {
                            rPossibleTiles.RemoveAt(i);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Label("There are no possible tiles to assign the resource to. Please create some tiles.");
                }

                GUILayout.Label("Possible Features", EditorStyles.boldLabel);

                if (GUILayout.Button("+"))
                {
                    rPossibleFeatures.Add(Feature.Flat);
                }

                for (int i = 0; i < rPossibleFeatures.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    rPossibleFeatures[i] = (Feature)EditorGUILayout.EnumPopup(rPossibleFeatures[i]);

                    if (GUILayout.Button("-"))
                    {
                        rPossibleFeatures.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();



                if (GUILayout.Button("Create"))
                {
                    CreateResource(rName, rRariety, rSpawnAmount, rMesh, rTexture, rReplaceGroundTexture);
                    EditorUtility.UnloadUnusedAssets();
                    Resources.UnloadUnusedAssets();
                    this.Close();
                }

                GUILayout.EndVertical();
            }
            else
            {
                this.title = "Edit Resource";

                Resource r = resourceManager.resources[resourceIndexToEdit];
                GUILayout.Label("Edit Resource: " + r.name);

                GUILayout.BeginVertical();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                r.name = EditorGUILayout.TextField("Resource Name:", r.name);
                r.rarity = EditorGUILayout.FloatField("Rariety:", r.rarity);
                r.meshSpawnAmount = EditorGUILayout.IntField("Mesh Spawn Amount:", r.meshSpawnAmount);
                r.replaceGroundTexture = EditorGUILayout.Toggle("Replace Ground Texture", r.replaceGroundTexture);
                r.meshToSpawn = (Mesh)EditorGUILayout.ObjectField("Resource Mesh:", (Object)r.meshToSpawn, typeof(Mesh), false);
                r.meshTexture = (Texture2D)EditorGUILayout.ObjectField("Resource Mesh Texture:", (Object)r.meshTexture, typeof(Texture2D), false);

                GUILayout.Label("Possible Tiles", EditorStyles.boldLabel);

                if (doneAddingResources == false)
                {
                    foreach (int t in resourceManager.resources[resourceIndexToEdit].rule.possibleTiles)
                    {
                        rPossibleTiles.Add(t);
                    }

                    foreach (Feature f in resourceManager.resources[resourceIndexToEdit].rule.possibleFeatures)
                    {
                        rPossibleFeatures.Add(f);
                    }
                    doneAddingResources = true;
                }

                if (tileManager.tiles.Count > 0)
                {
                    if (GUILayout.Button("+"))
                    {
                        rPossibleTiles.Add(0);
                    }

                    for (int i = 0; i < rPossibleTiles.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        rPossibleTiles[i] = (int)EditorGUILayout.Popup(rPossibleTiles[i], tileManager.tileNames);

                        if (GUILayout.Button("-"))
                        {
                            rPossibleTiles.RemoveAt(i);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Label("There are no possible tiles to assign the resource to. Please create some tiles.");
                }

                GUILayout.Label("Possible Features", EditorStyles.boldLabel);

                if (GUILayout.Button("+"))
                {
                    rPossibleFeatures.Add(Feature.Flat);
                }

                for (int i = 0; i < rPossibleFeatures.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    rPossibleFeatures[i] = (Feature)EditorGUILayout.EnumPopup(rPossibleFeatures[i]);

                    if (GUILayout.Button("-"))
                    {
                        rPossibleFeatures.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();



                if (GUILayout.Button("Close"))
                {
                    EditResource(r.name, r.rarity, r.meshSpawnAmount, r.meshToSpawn, r.meshTexture, r.replaceGroundTexture, resourceIndexToEdit);
                    resourceManager.resources.RemoveAt(resourceIndexToEdit+1);
                    EditorUtility.UnloadUnusedAssets();
                    Resources.UnloadUnusedAssets();
                    this.Close();
                }

                GUILayout.EndVertical();
            }
        }

        void CreateResource(string name, float rariety, int spawnAmount, Mesh mesh, Texture2D texture, bool replaceGroundTexture)
        {
            int[] finalTiles = new int[rPossibleTiles.Count];
            List<Feature> finalFeatures = new List<Feature>();

            finalTiles = rPossibleTiles.ToArray();

            //fill feature list with data from rule.possibleFeatures
            for(int i = 0; i < rPossibleFeatures.Count; i++)
            {
                finalFeatures.Add(rPossibleFeatures[i]);
            }


            //nullify duplicate tiles
            for (int i = 0; i < rPossibleTiles.Count; i++)
            {
                for (int z = 0; z < rPossibleTiles.Count; z++)
                {
                    if (rPossibleTiles[i] == rPossibleTiles[z] && i != z)
                    {
                        rPossibleTiles.RemoveAt(z);
                    }
                }
            }
            finalTiles = rPossibleTiles.ToArray();

            //nullify duplicate features
            for (int i = 0; i < finalFeatures.Count; i++)
            {
                for (int z = 0; z < finalFeatures.Count; z++)
                {
                    if (finalFeatures[i] == finalFeatures[z] && i != z)
                    {
                        finalFeatures.RemoveAt(z);
                    }
                }
            }

            resourceManager.AddResource(new Resource(name, rariety, spawnAmount, mesh, texture, replaceGroundTexture, new HexRule(finalTiles, finalFeatures.ToArray())));
        }

        void EditResource(string name, float rariety, int spawnAmount, Mesh mesh, Texture2D texture, bool replaceGroundTexture, int index)
        {
            int[] finalTiles = new int[rPossibleTiles.Count];
            List<Feature> finalFeatures = new List<Feature>();

            finalTiles = rPossibleTiles.ToArray();

            //fill feature list with data from rule.possibleFeatures
            for (int i = 0; i < rPossibleFeatures.Count; i++)
            {
                finalFeatures.Add(rPossibleFeatures[i]);
            }


            //nullify duplicate tiles
            for (int i = 0; i < rPossibleTiles.Count; i++)
            {
                for (int z = 0; z < rPossibleTiles.Count; z++)
                {
                    if (rPossibleTiles[i] == rPossibleTiles[z] && i != z)
                    {
                        rPossibleTiles.RemoveAt(z);
                    }
                }
            }
            finalTiles = rPossibleTiles.ToArray();

            //nullify duplicate features
            for (int i = 0; i < finalFeatures.Count; i++)
            {
                for (int z = 0; z < finalFeatures.Count; z++)
                {
                    if (finalFeatures[i] == finalFeatures[z] && i != z)
                    {
                        finalFeatures.RemoveAt(z);
                    }
                }
            }

            resourceManager.AddResourceAtIndex(new Resource(name, rariety, spawnAmount, mesh, texture, replaceGroundTexture, new HexRule(finalTiles, finalFeatures.ToArray())), index);
        }
    }

    public sealed class ImprovementEditorWindow : EditorWindow
    {
        public bool editMode;
        public int improvementIndexToEdit;

        //adding fields
        string iName = "None";
        bool iReplaceGroundTexture;
        Mesh iMesh;
        Texture2D iTexture;
        ImprovementManager improvementManager;
        TileManager tileManager;

        List<int> iPossibleTiles = new List<int>();
        List<Feature> iPossibleFeatures = new List<Feature>();

        [MenuItem("CivGrid/New Improvement", priority = 4)]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ImprovementEditorWindow));
        }

        void OnEnable()
        {
            improvementManager = GameObject.FindObjectOfType<ImprovementManager>();
            tileManager = GameObject.FindObjectOfType<TileManager>();
        }


        Vector2 scrollPosition = new Vector2();
        bool doneAddingImprovements = false;
        void OnGUI()
        {
            if (editMode == false)
            {
                this.title = "Add Improvement";
                GUILayout.BeginVertical();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                GUILayout.Label("Add Improvement:", EditorStyles.boldLabel);
                iName = EditorGUILayout.TextField("Improvement Name", iName);
                iReplaceGroundTexture = EditorGUILayout.Toggle("Replace Ground Texture", iReplaceGroundTexture);
                if (iReplaceGroundTexture == true) { EditorGUILayout.HelpBox("Remember to add a texture to the terrain atlas for this resource.", MessageType.Info); }
                iMesh = (Mesh)EditorGUILayout.ObjectField("Improvement Mesh:", (Object)iMesh, typeof(Mesh), false);
                iTexture = (Texture2D)EditorGUILayout.ObjectField("Improvement Mesh Texture:", (Object)iTexture, typeof(Texture2D), false);
                GUILayout.Label("Possible Tiles", EditorStyles.boldLabel);

                if (tileManager.tiles.Count > 0)
                {
                    if (GUILayout.Button("+"))
                    {
                        iPossibleTiles.Add(0);
                    }

                    for (int i = 0; i < iPossibleTiles.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        iPossibleTiles[i] = (int)EditorGUILayout.Popup(iPossibleTiles[i], tileManager.tileNames);

                        if (GUILayout.Button("-"))
                        {
                            iPossibleTiles.RemoveAt(i);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Label("There are no possible tiles to assign the improvement to. Please create some tiles.");
                }

                GUILayout.Label("Possible Features", EditorStyles.boldLabel);

                if (GUILayout.Button("+"))
                {
                    iPossibleFeatures.Add(Feature.Flat);
                }

                for (int i = 0; i < iPossibleFeatures.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    iPossibleFeatures[i] = (Feature)EditorGUILayout.EnumPopup(iPossibleFeatures[i]);

                    if (GUILayout.Button("-"))
                    {
                        iPossibleFeatures.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();



                if (GUILayout.Button("Create"))
                {
                    CreateImprovement(iName, iMesh, iTexture , iReplaceGroundTexture);
                    EditorUtility.UnloadUnusedAssets();
                    Resources.UnloadUnusedAssets();
                    this.Close();
                }

                GUILayout.EndVertical();
            }
            else
            {
                this.title = "Edit Improvement";

                Improvement improvement = improvementManager.improvements[improvementIndexToEdit];
                GUILayout.Label("Edit Improvement: " + improvement.name);

                GUILayout.BeginVertical();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                improvement.name = EditorGUILayout.TextField("Resource Name", improvement.name);
                improvement.replaceGroundTexture = EditorGUILayout.Toggle("Replace Ground Texture", improvement.replaceGroundTexture);
                improvement.meshToSpawn = (Mesh)EditorGUILayout.ObjectField("Resource Mesh:", (Object)improvement.meshToSpawn, typeof(Mesh), false);
                improvement.meshTexture = (Texture2D)EditorGUILayout.ObjectField("Resource Mesh Texture:", (Object)improvement.meshTexture, typeof(Texture2D), false);
                GUILayout.Label("Possible Tiles", EditorStyles.boldLabel);

                if (doneAddingImprovements == false)
                {
                    foreach (int t in improvementManager.improvements[improvementIndexToEdit].rule.possibleTiles)
                    {
                        iPossibleTiles.Add(t);
                    }

                    foreach (Feature f in improvementManager.improvements[improvementIndexToEdit].rule.possibleFeatures)
                    {
                        iPossibleFeatures.Add(f);
                    }
                    doneAddingImprovements = true;
                }

                if (tileManager.tiles.Count > 0)
                {
                    if (GUILayout.Button("+"))
                    {
                        iPossibleTiles.Add(0);
                    }

                    for (int i = 0; i < iPossibleTiles.Count; i++)
                    {
                        GUILayout.BeginHorizontal();

                        iPossibleTiles[i] = (int)EditorGUILayout.Popup(iPossibleTiles[i], tileManager.tileNames);

                        if (GUILayout.Button("-"))
                        {
                            iPossibleTiles.RemoveAt(i);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.Label("There are no possible tiles to assign the improvement to. Please create some tiles.");
                }

                GUILayout.Label("Possible Features", EditorStyles.boldLabel);

                if (GUILayout.Button("+"))
                {
                    iPossibleFeatures.Add(Feature.Flat);
                }

                for (int i = 0; i < iPossibleFeatures.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    iPossibleFeatures[i] = (Feature)EditorGUILayout.EnumPopup(iPossibleFeatures[i]);

                    if (GUILayout.Button("-"))
                    {
                        iPossibleFeatures.RemoveAt(i);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();



                if (GUILayout.Button("Close"))
                {
                    EditImprovement(improvement.name, improvement.meshToSpawn, improvement.meshTexture, improvement.replaceGroundTexture, improvementIndexToEdit);
                    improvementManager.improvements.RemoveAt(improvementIndexToEdit+1);
                    EditorUtility.UnloadUnusedAssets();
                    Resources.UnloadUnusedAssets();
                    this.Close();
                }

                GUILayout.EndVertical();
            }
        }

        void CreateImprovement(string name, Mesh mesh, Texture2D texture, bool replaceGroundTexture)
        {
            int[] finalTiles = new int[iPossibleTiles.Count];
            List<Feature> finalFeatures = new List<Feature>();

            finalTiles = iPossibleTiles.ToArray();

            //fill feature list with data from rule.possibleFeatures
            for(int i = 0; i < iPossibleFeatures.Count; i++)
            {
                finalFeatures.Add(iPossibleFeatures[i]);
            }


            //nullify duplicate tiles
            for (int i = 0; i < iPossibleTiles.Count; i++)
            {
                for (int z = 0; z < iPossibleTiles.Count; z++)
                {
                    if (iPossibleTiles[i] == iPossibleTiles[z] && i != z)
                    {
                        iPossibleTiles.RemoveAt(z);
                    }
                }
            }
            finalTiles = iPossibleTiles.ToArray();

            //nullify duplicate features
            for (int i = 0; i < finalFeatures.Count; i++)
            {
                for (int z = 0; z < finalFeatures.Count; z++)
                {
                    if (finalFeatures[i] == finalFeatures[z] && i != z)
                    {
                        finalFeatures.RemoveAt(z);
                    }
                }
            }

            improvementManager.AddImprovement(new Improvement(name, mesh, texture, replaceGroundTexture, new HexRule(finalTiles, finalFeatures.ToArray())));
        }

        void EditImprovement(string name, Mesh mesh, Texture2D texture, bool replaceGroundTexture, int index)
        {
            int[] finalTiles = new int[iPossibleTiles.Count];
            List<Feature> finalFeatures = new List<Feature>();

            finalTiles = iPossibleTiles.ToArray();

            //fill feature list with data from rule.possibleFeatures
            for (int i = 0; i < iPossibleFeatures.Count; i++)
            {
                finalFeatures.Add(iPossibleFeatures[i]);
            }


            //nullify duplicate tiles
            for (int i = 0; i < iPossibleTiles.Count; i++)
            {
                for (int z = 0; z < iPossibleTiles.Count; z++)
                {
                    if (iPossibleTiles[i] == iPossibleTiles[z] && i != z)
                    {
                        iPossibleTiles.RemoveAt(z);
                    }
                }
            }
            finalTiles = iPossibleTiles.ToArray();

            //nullify duplicate features
            for (int i = 0; i < finalFeatures.Count; i++)
            {
                for (int z = 0; z < finalFeatures.Count; z++)
                {
                    if (finalFeatures[i] == finalFeatures[z] && i != z)
                    {
                        finalFeatures.RemoveAt(z);
                    }
                }
            }

            improvementManager.AddImprovementAtIndex(new Improvement(name, mesh, texture, replaceGroundTexture, new HexRule(finalTiles, finalFeatures.ToArray())), index);
        }
    }

    public sealed class TileEditorWindow : EditorWindow
    {
        public bool editMode;
        public int tileIndexToEdit;

        public string tName;
        public bool tIsShore;
        public bool tIsOcean;
        public bool tIsMountain;
        public float tRainfallMin;
        public float tRainfallMax;
        public float tTemperatureMin;
        public float tTemperatureMax;

        TileManager tileManager;

        [MenuItem("CivGrid/New Tile", priority = 2)]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TileEditorWindow));
        }

        public void OnEnable()
        {
            tileManager = GameObject.FindObjectOfType<TileManager>();
        }

        Vector2 scrollPosition = new Vector2();
        void OnGUI()
        {
            if (editMode == false)
            {
                this.title = "Add Tile";

                GUILayout.BeginVertical();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                GUILayout.Label("Add Tile:", EditorStyles.boldLabel);

                tName = EditorGUILayout.TextField("Name:", tName);
                tIsShore = EditorGUILayout.Toggle("Is Shore:", tIsShore);
                tIsOcean = EditorGUILayout.Toggle("Is Ocean:", tIsOcean);
                tIsMountain = EditorGUILayout.Toggle("Is Mountain:", tIsMountain);
                if (tIsShore == false && tIsOcean == false && tIsMountain == false)
                {
                    EditorGUILayout.SelectableLabel("Rainfall Amount", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    tRainfallMin = EditorGUILayout.FloatField("Minimum", tRainfallMin);
                    tRainfallMax = EditorGUILayout.FloatField("Maximum", tRainfallMax);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.SelectableLabel("Temperature", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    tTemperatureMin = EditorGUILayout.FloatField("Minimum", tTemperatureMin);
                    tTemperatureMax = EditorGUILayout.FloatField("Maximum", tTemperatureMax);
                    EditorGUI.indentLevel--;

                }

                GUILayout.EndScrollView();

                GUILayout.EndVertical();

                if (GUILayout.Button("Create"))
                {
                    CreateTile(tName, tIsShore, tIsOcean, tIsMountain, tRainfallMin, tRainfallMax, tTemperatureMin, tTemperatureMax);
                    EditorUtility.UnloadUnusedAssets();
                    Resources.UnloadUnusedAssets();
                    this.Close();
                }
            }
            else
            {
                this.title = "Edit Tile";

                GUILayout.BeginVertical();

                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                GUILayout.Label("Edit Tile:", EditorStyles.boldLabel);

                Tile tile = tileManager.tiles[tileIndexToEdit];

                tile.name = EditorGUILayout.TextField("Name:", tile.name);
                tile.isShore = EditorGUILayout.Toggle("Is Shore:", tile.isShore);
                tile.isOcean = EditorGUILayout.Toggle("Is Ocean:", tile.isOcean);
                tile.isMountain = EditorGUILayout.Toggle("Is Mountain:", tile.isMountain);
                if (tile.isShore == false && tile.isOcean == false && tile.isMountain == false)
                {
                    EditorGUILayout.SelectableLabel("Rainfall Amount", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    tile.possibleRainfallValues.min = EditorGUILayout.FloatField("Minimum", tile.possibleRainfallValues.min);
                    tile.possibleRainfallValues.max = EditorGUILayout.FloatField("Maximum", tile.possibleRainfallValues.max);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.SelectableLabel("Temperature", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    tile.possibleTemperatureValues.min = EditorGUILayout.FloatField("Minimum", tile.possibleTemperatureValues.min);
                    tile.possibleTemperatureValues.max = EditorGUILayout.FloatField("Maximum", tile.possibleTemperatureValues.max);
                    EditorGUI.indentLevel--;
                }

                GUILayout.EndScrollView();

                GUILayout.EndVertical();

                if (GUILayout.Button("Close"))
                {
                    EditorUtility.UnloadUnusedAssets();
                    Resources.UnloadUnusedAssets();
                    this.Close();
                }
            }
        }

        private void CreateTile(string name, bool isShore, bool isOcean, bool isMountain, float rainfallMin, float rainfallMax, float temperatureMin, float temperateMax)
        {
            tileManager.AddTile(new Tile(name, isShore, isOcean, isMountain, rainfallMin, rainfallMax, temperatureMin, temperateMax));
        }
    }

    public enum TypeofEditorTile { Terrain, Resource, Improvement }

//    public sealed class TerrainManagerWindow : EditorWindow
//    {
//        WorldManager worldManager;
//        ImprovementManager improvementManager;
//        ResourceManager resourceManager;
//        TileManager tileManager;

//        string loc = Application.dataPath;
//        string editedLoc;
    
//        Vector2 terrainAtlasSize = new Vector2(1,1);
//        Texture2D[,] textures;
//        int[,] tempTileType;
//        int[,] tempResourceType;
//        int[,] tempImprovementType;
//        TypeofEditorTile[,] catagory;
//        List<TileItem> tileLocations = new List<TileItem>();
//        List<ResourceItem> resourceLocations = new List<ResourceItem>();
//        List<ImprovementItem> improvementLocations = new List<ImprovementItem>();

//        private Vector2 internalAtlasDimension;

//        [MenuItem("CivGrid/Terrain Manager", priority = 1)]
//        static void ShowWindow()
//        {
//            EditorWindow.GetWindow(typeof(TerrainManagerWindow));
//        }

//        void OnEnable()
//        {
//            this.title = "Terrain Manager";
//            worldManager = GameObject.FindObjectOfType<WorldManager>();
//            improvementManager = GameObject.FindObjectOfType<ImprovementManager>();
//            resourceManager = GameObject.FindObjectOfType<ResourceManager>();
//            tileManager = GameObject.FindObjectOfType<TileManager>();

//            terrainAtlasSize = new Vector2(Mathf.CeilToInt(tileManager.tileNames.Length / 2f), 2);
//            AssignAtlasSize(true);

//            if (worldManager == null || improvementManager == null)
//            {
//                Debug.LogError("Need to have WorldManager and ImprovementManager in current scene");
//            }

//            tileManager.UpdateTileNames();
//            resourceManager.UpdateResourceNames();
//            improvementManager.UpdateImprovementNames();
//        }

//        Vector2 scrollPosition = new Vector2();
//        void OnGUI()
//        {
//            terrainAtlasSize = EditorGUILayout.Vector2Field("Terrain Texture Size", terrainAtlasSize);

//            if (internalAtlasDimension.x == 0)
//            {
//                if (GUILayout.Button("Generate Grid"))
//                {
//                    AssignAtlasSize(true);
//                }
//            }
//            else
//            {
//                if (GUILayout.Button("Resize Grid"))
//                {
//                    AssignAtlasSize(false);
//                }
//            }

//            if(GUILayout.Button("Clear"))
//            {
//                ClearAtlasSize();
//            }

//            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

//            int tileIndex = -1;
//            int resourceIndex = -1;
//            int improvementIndex = -1;
//            for (int y = 0; y < internalAtlasDimension.y; y++)
//            {
//                //start new row
//                GUILayout.BeginHorizontal();
//                for (int x = 0; x < internalAtlasDimension.x; x++)
//                {
//                    textures[x, y] = (Texture2D)EditorGUILayout.ObjectField((Object)textures[x, y], typeof(Texture2D), false, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(150f), GUILayout.MaxWidth(150f));
//                    GUILayout.BeginVertical();
//                    GUILayout.Label("Settings for texture (" + x + "," + y + "):");
//                    catagory[x, y] = (TypeofEditorTile)EditorGUILayout.EnumPopup("Type:", catagory[x, y], GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
//                    if (catagory[x, y] == TypeofEditorTile.Terrain && (tileManager.tiles != null && tileManager.tiles.Count > 0))
//                    {
//                        if(tileIndex < (tileManager.tileNames.Length-1))
//                        {
//                            tileIndex++;
//                            tempTileType[x, y] = tileIndex;
//                        }
//                        tempTileType[x, y] = (int)EditorGUILayout.Popup("Tile Type:", tempTileType[x, y], tileManager.tileNames, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
//                        tempResourceType[x,y] = 0;
//                        tempImprovementType[x,y] = 0;
//                    }
//                    else if (catagory[x, y] == TypeofEditorTile.Resource && (resourceManager.resources != null && resourceManager.resources.Count > 0))
//                    {
//                        if (resourceIndex < (resourceManager.resourceNames.Length - 1))
//                        {
//                            resourceIndex++;
//                            tempResourceType[x, y] = resourceIndex;
//                        }
//                        tempResourceType[x, y] = (int)EditorGUILayout.Popup("Resource Type:", tempResourceType[x, y], resourceManager.resourceNames, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
//                        tempTileType[x, y] = 0;
//                        tempImprovementType[x,y] = 0;
//                    }
//                    else if (catagory[x, y] == TypeofEditorTile.Improvement && (improvementManager.improvements != null && improvementManager.improvements.Count > 0))
//                    {
//                        if (improvementIndex < (improvementManager.improvementNames.Length - 1))
//                        {
//                            improvementIndex++;
//                            tempImprovementType[x, y] = improvementIndex;
//                        }
//                        tempImprovementType[x, y] = (int)EditorGUILayout.Popup("Improvement Type:", tempImprovementType[x, y], improvementManager.improvementNames, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
//                        tempTileType[x,y] = 0;
//                        tempResourceType[x, y] = 0;
//                    }
//                    else
//                    {
//                        EditorGUILayout.SelectableLabel("There are no options for this type.", GUILayout.ExpandHeight(true));
//                    }
//                    GUILayout.EndVertical();
//                }
//                GUILayout.EndHorizontal();
//            }
//            GUILayout.EndScrollView();

//            editedLoc = ((string)loc.Clone()).Remove(0, loc.IndexOf("Assets"));

//            GUILayout.Label("Location to save:");
//            GUILayout.Label(editedLoc);

//            if (GUILayout.Button("Edit"))
//            {
//                string tempLoc = EditorUtility.OpenFolderPanel("Folder to save texture...", loc, "");
//                if (tempLoc != null && tempLoc != "")
//                {
//                    loc = tempLoc;
//                }
//            }

//            if (GUILayout.Button("Generate Atlas"))
//            {
//                GenerateAtlas();
//            }
//        }

//        private void AssignAtlasSize(bool freshStart)
//        {
//            if (freshStart == true)
//            {
//                internalAtlasDimension = terrainAtlasSize;
//                textures = new Texture2D[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//                tempTileType = new int[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//                tempResourceType = new int[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//                tempImprovementType = new int[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//                catagory = new TypeofEditorTile[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];

//                improvementLocations.Clear();
//                resourceLocations.Clear();
//                tileLocations.Clear();
//            }
//            else
//            {
//                internalAtlasDimension = terrainAtlasSize;
//                textures = Utility.Resize2DArray<Texture2D>(textures, (int)internalAtlasDimension.x, (int)internalAtlasDimension.y);
//                tempTileType = Utility.Resize2DArray<int>(tempTileType, (int)internalAtlasDimension.x, (int)internalAtlasDimension.y);
//                tempResourceType = Utility.Resize2DArray<int>(tempResourceType, (int)internalAtlasDimension.x, (int)internalAtlasDimension.y);
//                tempImprovementType = Utility.Resize2DArray<int>(tempImprovementType, (int)internalAtlasDimension.x, (int)internalAtlasDimension.y);
//                catagory = Utility.Resize2DArray<TypeofEditorTile>(catagory, (int)internalAtlasDimension.x, (int)internalAtlasDimension.y);
//            }
//        }

//        private void ClearAtlasSize()
//        {
//            internalAtlasDimension = new Vector2(0, 0);

//            textures = new Texture2D[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//            tempTileType = new int[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//            tempResourceType = new int[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//            tempImprovementType = new int[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];
//            catagory = new TypeofEditorTile[(int)internalAtlasDimension.x, (int)internalAtlasDimension.y];

//            improvementLocations.Clear();
//            resourceLocations.Clear();
//            tileLocations.Clear();
//        }


//        private void GenerateAtlas()
//        {

//            Rect[] rectAreas;
//            Texture2D returnTexture = TexturePacker.AtlasTextures(Utility.ToSingleArray<Texture2D>(textures), 2048, out rectAreas);

//            int lengthOfArraysX = catagory.GetLength(0); 
//            int lengthOfArraysY = catagory.GetLength(1); 

//            for (int x = 0; x < lengthOfArraysX; x++)
//            {
//                for (int y = 0; y < lengthOfArraysY; y++)
//                {
//                    if (catagory[x, y] == TypeofEditorTile.Terrain)
//                    {
//                        tileLocations.Add(tileManager.tiles[tempTileType[x, y]], rectAreas[x * lengthOfArraysY + y]);
//                    }
//                    else if (catagory[x, y] == TypeofEditorTile.Resource)
//                    {
//                        resourceLocations.Add(resourceManager.resources[tempResourceType[x, y]], rectAreas[x * lengthOfArraysY + y]);
//                    }
//                    else
//                    {
//                        improvementLocations.Add(improvementManager.improvements[tempImprovementType[x, y]], rectAreas[x * lengthOfArraysY + y]);
//                    }
//                }
//            }

//            FileUtility.SaveTexture(returnTexture, loc, "TerrainAtlas", false);
//            worldManager.textureAtlas.terrainAtlas = (Texture2D)AssetDatabase.LoadAssetAtPath(editedLoc + "/TerrainAtlas.png", typeof(Texture2D));
//            worldManager.textureAtlas.tileLocations = (TileItem[])tileLocations.ToArray().Clone();
//            worldManager.textureAtlas.resourceLocations = (ResourceItem[])resourceLocations.ToArray().Clone();
//            worldManager.textureAtlas.improvementLocations = (ImprovementItem[])improvementLocations.ToArray().Clone();


//            EditorUtility.UnloadUnusedAssets();
//            Resources.UnloadUnusedAssets();
//            this.Close();
//        }
//    }
//}

    public sealed class TerrainManagerWindow : EditorWindow
    {
        WorldManager worldManager;
        ImprovementManager improvementManager;
        ResourceManager resourceManager;
        TileManager tileManager;

        string loc = Application.dataPath;
        string editedLoc;

        //Vector2 terrainAtlasSize = new Vector2(1, 1);
        int atlasLength = 0;
        List<Texture2D> textures = new List<Texture2D>();
        List<int> tempTileType = new List<int>();
        List<int> tempResourceType = new List<int>();
        List<int> tempImprovementType = new List<int>();
        List<TypeofEditorTile> catagory = new List<TypeofEditorTile>();
        List<TileItem> tileLocations = new List<TileItem>();
        List<ResourceItem> resourceLocations = new List<ResourceItem>();
        List<ImprovementItem> improvementLocations = new List<ImprovementItem>();

        bool start = false;

        [MenuItem("CivGrid/Terrain Manager", priority = 1)]
        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TerrainManagerWindow));
        }

        void OnEnable()
        {
            this.title = "Terrain Manager";
            worldManager = GameObject.FindObjectOfType<WorldManager>();
            improvementManager = GameObject.FindObjectOfType<ImprovementManager>();
            resourceManager = GameObject.FindObjectOfType<ResourceManager>();
            tileManager = GameObject.FindObjectOfType<TileManager>();

            if (worldManager == null || improvementManager == null)
            {
                Debug.LogError("Need to have WorldManager and ImprovementManager in current scene");
            }

            int length = 0;

            foreach(Resource r in resourceManager.resources)
            {
                if (r.replaceGroundTexture == true) { length++; }
            }
            foreach(Improvement i in improvementManager.improvements)
            {
                if (i.replaceGroundTexture == true) { length++; }
            }
            length += tileManager.tileNames.Length;

            for (int i = 0; i < length; i++)
            {
                AddElement();
            }

                tileManager.UpdateTileNames();
            resourceManager.UpdateResourceNames();
            improvementManager.UpdateImprovementNames();

            start = true;
        }

        void AddElement()
        {
            atlasLength++;
            textures.Add(null);
            tempTileType.Add(0);
            tempResourceType.Add(0);
            tempImprovementType.Add(0);
            catagory.Add(TypeofEditorTile.Terrain);
        }

        Vector2 scrollPosition = new Vector2();
        void OnGUI()
        {
            if (GUILayout.Button("Add Element"))
            {
                AddElement();
            }
            if(GUILayout.Button("Clear"))
            {
                atlasLength = 0;
                textures.Clear();
                tempTileType.Clear();
                tempResourceType.Clear();
                tempImprovementType.Clear();
                catagory.Clear();
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            int tileIndex = -1;
            int resourceIndex = -1;
            int improvementIndex = -1;

                //start new row
                GUILayout.BeginHorizontal();
                for (int i = 0; i < atlasLength; i++)
                {
                    GUILayout.BeginVertical();
                    textures[i] = (Texture2D)EditorGUILayout.ObjectField((Object)textures[i], typeof(Texture2D), false, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MaxHeight(150f), GUILayout.MaxWidth(150f));
                    GUILayout.Label("Settings for texture (" + i);
                    catagory[i] = (TypeofEditorTile)EditorGUILayout.EnumPopup("Type:", catagory[i], GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
                    if (catagory[i] == TypeofEditorTile.Terrain && (tileManager.tiles != null && tileManager.tiles.Count > 0))
                    {
                        if (tileIndex < (tileManager.tileNames.Length - 1) && start)
                        {
                            tileIndex++;
                            tempTileType[i] = tileIndex;
                        }
                        tempTileType[i] = (int)EditorGUILayout.Popup("Tile Type:", tempTileType[i], tileManager.tileNames, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
                        tempResourceType[i] = 0;
                        tempImprovementType[i] = 0;
                    }
                    else if (catagory[i] == TypeofEditorTile.Resource && (resourceManager.resources != null && resourceManager.resources.Count > 0))
                    {
                        if (resourceIndex < (resourceManager.resourceNames.Length - 1) && start)
                        {
                            resourceIndex++;
                            tempResourceType[i] = resourceIndex;
                        }
                        tempResourceType[i] = (int)EditorGUILayout.Popup("Resource Type:", tempResourceType[i], resourceManager.resourceNames, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
                        tempTileType[i] = 0;
                        tempImprovementType[i] = 0;
                    }
                    else if (catagory[i] == TypeofEditorTile.Improvement && (improvementManager.improvements != null && improvementManager.improvements.Count > 0))
                    {
                        if (improvementIndex < (improvementManager.improvementNames.Length - 1) && start)
                        {
                            improvementIndex++;
                            tempImprovementType[i] = improvementIndex;
                        }
                        tempImprovementType[i] = (int)EditorGUILayout.Popup("Improvement Type:", tempImprovementType[i], improvementManager.improvementNames, GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f));
                        tempTileType[i] = 0;
                        tempResourceType[i] = 0;
                    }
                    else
                    {
                        EditorGUILayout.SelectableLabel("There are no options for this type.", GUILayout.ExpandHeight(true));
                    }
                    if (GUILayout.Button("-", GUILayout.ExpandWidth(true), GUILayout.MaxWidth(300f)))
                    {
                        atlasLength--;
                        textures.RemoveAt(i);
                        tempTileType.RemoveAt(i);
                        tempResourceType.RemoveAt(i);
                        tempImprovementType.RemoveAt(i);
                        catagory.RemoveAt(i);
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            editedLoc = ((string)loc.Clone()).Remove(0, loc.IndexOf("Assets"));

            GUILayout.Label("Location to save:");
            GUILayout.Label(editedLoc);

            if (GUILayout.Button("Edit"))
            {
                string tempLoc = EditorUtility.OpenFolderPanel("Folder to save texture...", loc, "");
                if (tempLoc != null && tempLoc != "")
                {
                    loc = tempLoc;
                }
            }

            if (GUILayout.Button("Generate Atlas"))
            {
                GenerateAtlas();
            }

            
            start = false;
        }


        private void GenerateAtlas()
        {

            Rect[] rectAreas;
            Texture2D returnTexture = TexturePacker.AtlasTextures(textures.ToArray(), 2048, out rectAreas);

            int lengthOfArrays = catagory.Count;

            for (int i = 0; i < lengthOfArrays; i++)
            {
                    if (catagory[i] == TypeofEditorTile.Terrain)
                    {
                        tileLocations.Add(tileManager.tiles[tempTileType[i]], rectAreas[i]);
                    }
                    else if (catagory[i] == TypeofEditorTile.Resource)
                    {
                        resourceLocations.Add(resourceManager.resources[tempResourceType[i]], rectAreas[i]);
                    }
                    else
                    {
                        improvementLocations.Add(improvementManager.improvements[tempImprovementType[i]], rectAreas[i]);
                    }
            }

            FileUtility.SaveTexture(returnTexture, loc, "TerrainAtlas", false);
            worldManager.textureAtlas.terrainAtlas = (Texture2D)AssetDatabase.LoadAssetAtPath(editedLoc + "/TerrainAtlas.png", typeof(Texture2D));
            worldManager.textureAtlas.tileLocations = (TileItem[])tileLocations.ToArray().Clone();
            worldManager.textureAtlas.resourceLocations = (ResourceItem[])resourceLocations.ToArray().Clone();
            worldManager.textureAtlas.improvementLocations = (ImprovementItem[])improvementLocations.ToArray().Clone();


            EditorUtility.UnloadUnusedAssets();
            Resources.UnloadUnusedAssets();
            this.Close();
        }
    }
}
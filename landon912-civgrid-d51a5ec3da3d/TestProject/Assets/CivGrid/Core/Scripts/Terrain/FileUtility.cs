using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CivGrid;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CivGrid
{

    /// <summary>
    /// Saves and loads numerious data types.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Saves the provided Texture2D to a the file name in the application path.
        /// </summary>
        /// <param name="texture">Texture to save to file</param>
        /// <param name="name">Name of the file</param>
        /// <param name="openTextureToView">If the texture is opened in a window to view after saving</param>
        /// <example>
        /// The following will save the texture to a file named <b>savedTexture</b> in the root folder of the game
        /// and open the file for viewing.
        /// <code>
        /// class SaveTexture : MonoBehaviour
        /// {
        ///     public Texture2D textureToSave;
        ///
        ///     void Start()
        ///     {
        ///         CivGridFileUtility.SaveTexture(textureToSave, "savedTexture", true);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// If <b>openTextureToView</b> is true, it will open the saved file in your default image editor/browser.
        /// </remarks>
        public static void SaveTexture(Texture2D texture, string name, bool openTextureToView)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/../" + name + ".png", bytes);
            if (openTextureToView) { Application.OpenURL(Application.dataPath + "/../" + name + ".png"); }
			#if UNITY_EDITOR
            AssetDatabase.Refresh();
			#endif
        }

        /// <summary>
        /// Saves the provided Texture2D to a file name in the provided location.
        /// </summary>
        /// <param name="texture">Texture to save to file</param>
        /// <param name="location">Location to save the file</param>
        /// <param name="name">Name of the file</param>
        /// <param name="openTextureToView">If the texture is opened in a window to view after saving</param>
        /// <example>
        /// The following will save the texture to a file named <b>savedTexture</b> in the path provided, in this case the root
        /// folder of the game, and open the file for viewing.
        /// <code>
        /// class SaveTexture : MonoBehaviour
        /// {
        ///     public Texture2D textureToSave;
        ///
        ///     void Start()
        ///     {
        ///         CivGridFileUtility.SaveTexture(textureToSave, Application.dataPath, "savedTexture", true);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// If <b>openTextureToView</b> is true, it will open the saved file in your default image editor/browser.
        /// </remarks>
        public static void SaveTexture(Texture2D texture, string location, string name, bool openTextureToView)
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(location + "/" + name + ".png", bytes);
            if (openTextureToView) { Application.OpenURL(location + "/" + name + ".png"); }
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
			#endif
        }

        /// <summary>
        /// Saves the provided texture provided as a byte[] to a file name in the provided location.
        /// </summary>
        /// <param name="texture">Texture to save to file</param>
        /// <param name="location">Location to save the file</param>
        /// <param name="name">Name of the file</param>
        /// <param name="openTextureToView">If the texture is opened in a window to view after saving</param>
        /// <example>
        /// The following will save the texture to a file named <b>savedTexture</b> in the path provided, in this case the root
        /// folder of the game, and open the file for viewing.
        /// <code>
        /// class SaveTexture : MonoBehaviour
        /// {
        ///    public byte[] textureToSave;
        ///
        ///    void Start()
        ///    {
        ///        CivGridFileUtility.SaveTexture(textureToSave, Application.dataPath, "savedTexture", true);
        ///    }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// If <b>openTextureToView</b> is true, it will open the saved file in your default image editor/browser.
        /// </remarks>
        public static void SaveTexture(byte[] texture, string location, string name, bool openTextureToView)
        {
            File.WriteAllBytes(location + "/" + name + ".png", texture);
            if (openTextureToView) { Application.OpenURL(location + "/" + name + ".png"); }
            #if UNITY_EDITOR
            AssetDatabase.Refresh();
			#endif
        }

        /// <summary>
        /// Saves the current terrain to a file. <see cref="WorldManager.SaveMap"/> is a wrapper for this method, both are congruent.
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <example>
        /// The following code saves the terrain to a file called <b>terrainSave</b>.
        /// <code>
        /// class SaveTerrain : MonoBehaviour
        /// {
        ///    void Start()
        ///    {
        ///        CivGridFileUtility.SaveTerrain("terrainSave");
        ///    }
        /// }
        /// </code>
        /// <remarks>
        /// <see cref="WorldManager"/> must be in the scene and currently have a world in memory. Method will return errors if a WorldManager
        /// is not currently in the scene or if it doesn't have a world currently loaded into memory.
        /// </remarks>
        /// </example>
        public static void SaveTerrain(string name)
        {
            WorldManager worldManager = GameObject.FindObjectOfType<WorldManager>();

            if(worldManager == null)
            {
                Debug.LogError("WorldManager not found while attempting to save the game. \n Game save failed. ");
                return;
            }

            System.Text.StringBuilder assetPrefix = new System.Text.StringBuilder(Application.dataPath);
            assetPrefix.Remove((assetPrefix.Length - 6), 6);

            using (XmlWriter writer = XmlWriter.Create(name + ".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Root");

                #region WorldManager
                writer.WriteStartElement("WorldManager");

                writer.WriteAttributeString("useCivGridCamera", XmlConvert.ToString(worldManager.useCivGridCamera));
                writer.WriteAttributeString("usePresetWorldValue", XmlConvert.ToString(worldManager.useWorldTypeValues));
                writer.WriteAttributeString("worldType", ((int)worldManager.worldType).ToString());
                writer.WriteAttributeString("mapSizeX", worldManager.mapSize.x.ToString());
                writer.WriteAttributeString("mapSizeY", worldManager.mapSize.y.ToString());
                writer.WriteAttributeString("chunkSize", worldManager.chunkSize.ToString());
                writer.WriteAttributeString("hexRadiusSize", worldManager.hexRadiusSize.ToString());


                FileUtility.SaveTexture(worldManager.mountainHeightMap, "Base_Mountain_Map", false);

                writer.WriteEndElement();
                #endregion

                #region VoidedManagers
                /*
                #region TileManager
                
                TileManager tileManager = worldManager.tileManager;

                writer.WriteStartElement("TileManager");

                writer.WriteStartElement("Tiles");
                for (int i = 0; i < tileManager.tiles.Count; i++)
                {
                    writer.WriteStartElement("NewTile");

                    writer.WriteAttributeString("name", tileManager.tiles[i].name);
                    writer.WriteAttributeString("bottomLongitude", XmlConvert.ToString(tileManager.tiles[i].bottomLongitude));
                    writer.WriteAttributeString("topLongitude", XmlConvert.ToString(tileManager.tiles[i].topLongitude));
                    writer.WriteAttributeString("isShore", XmlConvert.ToString(tileManager.tiles[i].isShore));
                    writer.WriteAttributeString("isOcean", XmlConvert.ToString(tileManager.tiles[i].isOcean));
                    writer.WriteAttributeString("isMountain", XmlConvert.ToString(tileManager.tiles[i].isMountain));

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement();

                #endregion

                #region ResourceManager

                ResourceManager resourceManager = worldManager.resourceManager;

                writer.WriteStartElement("ResourceManager");

                    writer.WriteStartElement("Resources");
                        for (int i = 1; i < resourceManager.resources.Count; i++)
                        {
                            writer.WriteStartElement("NewResource");

                                writer.WriteAttributeString("name", resourceManager.resources[i].name);
                                writer.WriteAttributeString("rarity", resourceManager.resources[i].rarity.ToString());
                                writer.WriteAttributeString("spawnAmount", resourceManager.resources[i].spawnAmount.ToString());
                                writer.WriteAttributeString("meshToSpawn", (AssetDatabase.GetAssetPath(resourceManager.resources[i].meshToSpawn)));
                                writer.WriteAttributeString("meshTexture", (AssetDatabase.GetAssetPath(resourceManager.resources[i].meshTexture)));
                                writer.WriteAttributeString("replaceGroundTexture", XmlConvert.ToString(resourceManager.resources[i].replaceGroundTexture));

                                writer.WriteAttributeString("numOfPossibleTiles", resourceManager.resources[i].rule.possibleTiles.Length.ToString());

                                for (int y = 0; y < resourceManager.resources[i].rule.possibleTiles.Length; y++)
                                {
                                    writer.WriteAttributeString("possibleTile" + y, resourceManager.resources[i].rule.possibleTiles[y].ToString());
                                }

                                writer.WriteAttributeString("numOfPossibleFeatures", resourceManager.resources[i].rule.possibleFeatures.Length.ToString());

                                for (int y = 0; y < resourceManager.resources[i].rule.possibleFeatures.Length; y++)
                                {
                                    writer.WriteAttributeString("possibleFeature" + y, resourceManager.resources[i].rule.possibleFeatures[y].ToString());
                                }

                            writer.WriteEndElement();
                        }
                    writer.WriteEndElement();

                writer.WriteEndElement();

                #endregion

                #region ImprovementManager

                ImprovementManager improvementManager = worldManager.improvementManager;

                writer.WriteStartElement("ImprovementManager");

                writer.WriteElementString("improvementCount", improvementManager.searalizableImprovements.Count.ToString());

                writer.WriteStartElement("Improvements");
                for (int i = 1; i < improvementManager.searalizableImprovements.Count; i++)
                {
                    writer.WriteStartElement("Improvement");

                    writer.WriteElementString("name", improvementManager.searalizableImprovements[i].name);
                    writer.WriteElementString("meshToSpawn", (assetPrefix.ToString() + AssetDatabase.GetAssetPath(improvementManager.searalizableImprovements[i].meshToSpawn)));
                    writer.WriteElementString("meshTexture", (assetPrefix.ToString() + AssetDatabase.GetAssetPath(improvementManager.searalizableImprovements[i].meshTexture)));
                    writer.WriteElementString("replaceGroundTexture", improvementManager.searalizableImprovements[i].replaceGroundTexture.ToString());

                    writer.WriteStartElement("ImprovementRule");

                    writer.WriteStartElement("PossibleTiles");
                    for (int y = 0; y < improvementManager.searalizableImprovements[i].rule.possibleTiles.Length; y++)
                    {
                        writer.WriteStartElement("Tile");
                        writer.WriteElementString("tile", improvementManager.searalizableImprovements[i].rule.possibleTiles[i].ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("PossibleFeatures");
                    for (int y = 0; y < improvementManager.searalizableImprovements[i].rule.possibleFeatures.Length; y++)
                    {
                        writer.WriteStartElement("Feature");
                        writer.WriteElementString("feature", improvementManager.searalizableImprovements[i].rule.possibleFeatures[i].ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement();

                #endregion
                 */

                #endregion

                #region Chunk
                for (int x = 0; x < worldManager.hexChunks.GetLength(0); x++)
                {

                    for (int y = 0; y < worldManager.hexChunks.GetLength(1); y++)
                    {
                        Chunk chunk = worldManager.hexChunks[x, y];

                        writer.WriteStartElement("Chunk");

                        writer.WriteAttributeString("xChunkLoc", XmlConvert.ToString(x));
                        writer.WriteAttributeString("yChunkLoc", XmlConvert.ToString(y));

                        for (int _x = 0; _x < chunk.hexArray.GetLength(0); _x++)
                        {
                            for (int _y = 0; _y < chunk.hexArray.GetLength(1); _y++)
                            {
                                Hex hex = chunk.hexArray[_x, _y];

                                #region Hex

                                writer.WriteStartElement("Hexagon");

                                writer.WriteAttributeString("xHexLoc", XmlConvert.ToString(_x));

                                writer.WriteAttributeString("yHexLoc", XmlConvert.ToString(_y));

                                writer.WriteAttributeString("xParentChunk", XmlConvert.ToString(x));

                                writer.WriteAttributeString("yParentChunk", XmlConvert.ToString(y));

                                //Texture Rect Location
                                writer.WriteAttributeString("rLeft", XmlConvert.ToString(hex.currentRectLocation.xMin));
                                writer.WriteAttributeString("rTop", XmlConvert.ToString(hex.currentRectLocation.yMin));
                                writer.WriteAttributeString("rWidth", XmlConvert.ToString(hex.currentRectLocation.width));
                                writer.WriteAttributeString("rHeight", XmlConvert.ToString(hex.currentRectLocation.height));

                                writer.WriteAttributeString("type", hex.terrainType.name);

                                writer.WriteAttributeString("feature", ((int)hex.terrainFeature).ToString());

                                writer.WriteAttributeString("resource", hex.currentResource.name);

                                writer.WriteAttributeString("improvement", hex.currentImprovement.name);

                                writer.WriteEndElement();

                                #endregion
                            }
                        }
                        writer.WriteEndElement();
                    }
                }
                #endregion

                #region CivGridCamera

                writer.WriteStartElement("CivGridCamera");

                //camera settings
                writer.WriteAttributeString("enableWrapping", XmlConvert.ToString(worldManager.civGridCamera.enableWrapping));
                writer.WriteAttributeString("cameraHeight", worldManager.civGridCamera.cameraHeight.ToString());
                writer.WriteAttributeString("cameraAngle", worldManager.civGridCamera.cameraAngle.ToString());
                writer.WriteAttributeString("cameraSpeed", worldManager.civGridCamera.cameraSpeed.ToString());

                writer.WriteEndElement();

                #endregion

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Loads a terrain into the scene from a file.
        /// <see cref="WorldManager.LoadAndGenerateMap"/> should be used unless additional custom modification will be used.
        /// This method only loads the data into memory and does not generate the world from the data.
        /// </summary>
        /// <param name="location">File to load</param>
        /// <remarks>
        /// <see cref="WorldManager.LoadAndGenerateMap"/> should be used instead of this method unless advanced custom work needs to be implimented.
        /// </remarks>
        public static void LoadTerrain(string location)
        {

            WorldManager worldManager = GameObject.FindObjectOfType<WorldManager>();

            if (worldManager == null)
            {
                Debug.LogError("WorldManager not found while attempting to load the game. \n Game load failed. ");
                return;
            }

            bool startedGen = false;

            using (XmlReader reader = XmlReader.Create(location + ".xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "WorldManager":
                                worldManager.useCivGridCamera = XmlConvert.ToBoolean(reader["useCivGridCamera"]);
                                worldManager.useWorldTypeValues = XmlConvert.ToBoolean(reader["usePresetWorldValue"]);
                                worldManager.worldType = (WorldType)XmlConvert.ToInt32(reader["worldType"]);
                                worldManager.mapSize = new Vector2(XmlConvert.ToInt32(reader["mapSizeX"]), XmlConvert.ToInt32(reader["mapSizeY"]));
                                worldManager.chunkSize = XmlConvert.ToInt32(reader["chunkSize"]);
                                worldManager.hexRadiusSize = (float)XmlConvert.ToDouble(reader["hexRadiusSize"]);
                                worldManager.mountainHeightMap = FileUtility.LoadTexture("Base_Mountain_Map");
                                break;
                            case "CivGridCamera":

                                worldManager.civGridCamera.enableWrapping = XmlConvert.ToBoolean(reader["enableWrapping"]);
                                worldManager.civGridCamera.cameraHeight = (float)XmlConvert.ToDouble(reader["cameraHeight"]);
                                worldManager.civGridCamera.cameraAngle = (float)XmlConvert.ToDouble(reader["cameraAngle"]);
                                worldManager.civGridCamera.cameraSpeed = (float)XmlConvert.ToDouble(reader["cameraSpeed"]);
                                break;
                            case "Chunk":
                                if (startedGen == false)
                                {
                                    worldManager.GenerateNewMap(false);
                                    startedGen = true;
                                }
                                break;
                            case "Hexagon":
                                Hex hex = worldManager.hexChunks[XmlConvert.ToInt32(reader["xParentChunk"]), XmlConvert.ToInt32(reader["yParentChunk"])].hexArray[XmlConvert.ToInt32(reader["xHexLoc"]), XmlConvert.ToInt32(reader["yHexLoc"])];
                                
                                hex.terrainType = worldManager.tileManager.TryGetTile(reader["type"]);
                                hex.terrainFeature = (Feature)XmlConvert.ToInt32(reader["feature"]);
                                hex.parentChunk = worldManager.hexChunks[XmlConvert.ToInt32(reader["xParentChunk"]), XmlConvert.ToInt32(reader["yParentChunk"])];

                                //rectLocation
                                hex.currentRectLocation = new Rect((float)XmlConvert.ToDouble(reader["rLeft"]), (float)XmlConvert.ToDouble(reader["rTop"]), (float)XmlConvert.ToDouble(reader["rWidth"]), (float)XmlConvert.ToDouble(reader["rHeight"]));
                                
                                hex.currentResource = worldManager.resourceManager.TryGetResource(reader["resource"]);
                                
                                hex.currentImprovement = worldManager.improvementManager.TryGetImprovement(reader["improvement"]);
                                break;
                            #region VoidedManagers
                            /*
                            #region TileManager
                            case "TileManager":
                                Debug.Log("opening tile manager");
                                break;
                            case "Tiles":
                                Debug.Log("finding tiles");
                                break;
                            case "NewTile":
                                Debug.Log("adding Tile: " + reader["name"]);
                                tileManager.AddTile(new Tile(reader["name"], XmlConvert.ToBoolean(reader["isShore"]), XmlConvert.ToBoolean(reader["isOcean"]), XmlConvert.ToBoolean(reader["isMountain"]), (float)XmlConvert.ToDouble(reader["bottomLongitude"]), (float)XmlConvert.ToDouble(reader["topLongitude"])));
                                break;
                            #endregion
                            #region ResourceManager
                            case "ResourceManager":
                                Debug.Log("opening resource manager");
                                break;
                            case "Resources":
                                Debug.Log("opening resources");
                                break;
                            case "NewResource":
                                string loc1 = reader["meshToSpawn"];
                                string loc2 = reader["meshTexture"];

                                List<int> possibleTiles = new List<int>();
                                List<Feature> possibleFeatures = new List<Feature>();

                                for(int i = 0; i < XmlConvert.ToInt32(reader["numOfPossibleTiles"]); i++)
                                {
                                    possibleTiles.Add(XmlConvert.ToInt32(reader["possibleTile" + i]));
                                }

                                for (int i = 0; i < XmlConvert.ToInt32(reader["numOfPossibleFeatures"]); i++)
                                {
                                    possibleFeatures.Add((Feature)System.Enum.Parse(typeof(Feature), reader["possibleFeature" + i]));
                                }

                                resourceManager.resources.Add(new Resource(
                                    reader["name"],
                                    ((float)XmlConvert.ToDouble(reader["rarity"])),
                                    XmlConvert.ToInt32(reader["spawnAmount"]),
                                    ((Mesh)AssetDatabase.LoadAssetAtPath(loc1, typeof(Mesh))),
                                    ((Texture2D)AssetDatabase.LoadAssetAtPath(loc2, typeof(Texture2D))),
                                    XmlConvert.ToBoolean(reader["replaceGroundTexture"]), new ResourceRule(possibleTiles.ToArray(), possibleFeatures.ToArray())));
                                break;
                            #endregion
                                 */
                            #endregion
                            default:
                                //Debug.Log("unhandled exception: " + reader.Name);
                                break;

                        }
                    }
                }
            }

            foreach(Chunk chunk in worldManager.hexChunks)
            {
                chunk.StartHexGeneration();
                foreach (Hex hex in chunk.hexArray)
                {
                    if (hex.currentResource.name != "None")
                    {
                        worldManager.resourceManager.SpawnResource(hex, hex.currentResource, true);
                    }
                    if (hex.currentImprovement.name != "None")
                    {
                        worldManager.improvementManager.AddImprovement(hex.currentImprovement);
                    }
                }
            }
        }

        /// <summary>
        /// Load a texture into memory from a file.
        /// </summary>
        /// <param name="location">Location of the file</param>
        /// <param name="name">Name of the file</param>
        /// <returns>The texture loaded from the location</returns>
        /// <example>
        /// The following code loads a .png file from a file location.
        /// <code>
        /// class LoadTexture : MonoBehaviour
        /// {
        ///    Texture2D texture;
        ///    string fileLocation = Application.dataPath;
        ///
        ///    void Start()
        ///    {
        ///       CivGridFileUtility.LoadTexture(fileLocation, "savedTexture");
        ///    }
        /// }
        /// </code>
        /// </example>
        public static Texture2D LoadTexture(string location, string name)
        {
            byte[] bytes;
            bytes = File.ReadAllBytes(location + "/../" + name + ".png");
            Texture2D tex = new Texture2D(0, 0);

            tex.LoadImage(bytes);
            return tex;
        }

        /// <summary>
        /// Load a texture into memory from a file.
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <returns>The texture loaded from the file name</returns>
        /// <example>
        /// The following code loads a .png file from a file location.
        /// <code>
        /// class LoadTexture : MonoBehaviour
        /// {
        ///    Texture2D texture;
        ///
        ///    void Start()
        ///    {
        ///       CivGridFileUtility.LoadTexture("savedTexture");
        ///    }
        /// }
        /// </code>
        /// <remarks>
        /// This overload assumes that you are loading from the games root folder, if you want to load
        /// from another location use <see cref="LoadTexture(string,string)"/>.
        /// </remarks>
        /// </example>
        public static Texture2D LoadTexture(string name)
        {
            byte[] bytes;
            bytes = File.ReadAllBytes(Application.dataPath + "/../" + name + ".png");
            Texture2D tex = new Texture2D(0, 0);

            tex.LoadImage(bytes);
            return tex;
        }
    }
}
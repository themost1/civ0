using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CivGrid;

namespace CivGrid
{
    /// <summary>
    /// Contains all possible tiles.
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        //public bool useLatAndLong = false;

        /// <summary>
        /// Possible tiles to assign.
        /// </summary>
        public List<Tile> tiles;
        /// <summary>
        /// Names of the possible tiless to assign.
        /// </summary>
        public string[] tileNames;

        //internal array for speed
        private Tile[] internalTiles;

        /// <summary>
        /// Sets up the tile manager.
        /// Caches all needed values.
        /// </summary>
        internal void SetUp()
        {
            internalTiles = tiles.ToArray();
            UpdateTileNames();
        }

        /// <summary>
        /// Creates an array of the tiles names. <see cref="tileNames"/>
        /// </summary>
        public void UpdateTileNames()
        {
            //only update if there are tiles
            if (internalTiles != null && internalTiles.Length > 0)
            {
                //instantiate tile names array
                tileNames = new string[internalTiles.Length];

                //loop through all tiles
                for (int i = 0; i < internalTiles.Length; i++)
                {
                    //asign each name into the array
                    tileNames[i] = internalTiles[i].name;
                }
            }
        }

        /// <summary>
        /// Adds a tile to the tile array.
        /// </summary>
        /// <param name="t">Tile to add</param>
        /// <remarks>
        /// This method should only be used before world generation.
        /// </remarks>
        public void AddTile(Tile t)
        {
            tiles.Add(t);
            internalTiles = tiles.ToArray();
            UpdateTileNames();
        }

        /// <summary>
        /// Removes a tile from the tile array.
        /// </summary>
        /// <param name="t">Improvement to remove</param>
        /// <remarks>
        /// Removing a tile that is referenced elsewhere will cause null reference errors. Only use this
        /// method if you are personally managing the specific tiles memory lifetime.
        /// </remarks>
        public void DeleteTile(Tile t)
        {
            tiles.Remove(t);
            internalTiles = tiles.ToArray();
            UpdateTileNames();
        }

        /// <summary>
        /// Attempts to return a tile from a provided name.
        /// </summary>
        /// <param name="name">The name of the tile to look for</param>
        /// <returns>The tile with the name provided; null if not found</returns>
        /// <example>
        /// The following example adds a resource, then retrieves it by it's name. Using <see cref="ResourceManager.AddResource(Resource)"/> is
        /// not encouraged. Add resources in the inspector.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    TileManager tileManager;
        ///
        ///    void Start()
        ///    {
        ///        tileManager = GameObject.FindObjectOfType&lt;TileManager&gt;();
        ///
        ///        //this method is not encouraged, used as a specific example and not best practice. Add tiles in the
        ///        //inspector instead.
        ///        tileManager.AddTile(new Tile("Test", 0.0f, 0.2f));
        ///
        ///        Tile tile = tileManager.TryGetTile("Test");
        ///
        ///        Debug.Log(tile.name);
        ///    }
        /// }
        ///
        /// //Output:
        /// //"Test"
        /// </code>
        /// </example>
        public Tile TryGetTile(string name)
        {
            //cycle through all tiles
            foreach (Tile t in internalTiles)
            {
                //if the improvement shares the name; return it
                if (name == t.name)
                {
                    return t;
                }
            }
            //not found; return null
            return null;
        }

        /// <summary>
        /// Wrapper method of TryGetTile() to find a tile marked as an ocean.
        /// </summary>
        /// <returns>The tile found as ocean; null if not found</returns>
        /// <example>
        /// The following code adds an "Ocean" tile and then retrieves it automatically.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    TileManager tileManager;
        ///
        ///    void Start()
        ///    {
        ///        tileManager = GameObject.FindObjectOfType&lt;TileManager&gt;();
        ///
        ///        //this method is not encouraged, used as a specific example and not best practice. Add tiles in the
        ///        //inspector instead.
        ///        tileManager.AddTile(new Tile("Ocean", false, true, false));
        ///
        ///        Tile tile = tileManager.TryGetOcean();
        ///
        ///        Debug.Log(tile.name);
        ///    }
        /// }
        ///
        /// //Output:
        /// //"Ocean"
        /// </code>
        /// </example>
        public Tile TryGetOcean()
        {
            //loop through all tiles and find one marked as the ocean tile
            foreach (Tile t in internalTiles)
            {
                if (t.isOcean)
                {
                    return t;
                }
            }
            //not found
            return null;
        }

        /// <summary>
        /// Wrapper method of TryGetTile() to find a tile marked as a mountain.
        /// </summary>
        /// <returns>The tile found as mountain; null if not found</returns>
        /// <example>
        /// The following code adds a "Mountain" tile and then retrieves it automatically.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    TileManager tileManager;
        ///
        ///    void Start()
        ///    {
        ///        tileManager = GameObject.FindObjectOfType&lt;TileManager&gt;();
        ///
        ///        //this method is not encouraged, used as a specific example and not best practice. Add tiles in the
        ///        //inspector instead.
        ///        tileManager.AddTile(new Tile("Mountain", false, false, true));
        ///
        ///        Tile tile = tileManager.TryGetMountain();
        ///
        ///        Debug.Log(tile.name);
        ///    }
        /// }
        ///
        /// //Output:
        /// //"Mountain"
        /// </code>
        /// </example>
        public Tile TryGetMountain()
        {
            //loop through all tiles and find one marked as the mountain tile
            foreach (Tile t in internalTiles)
            {
                if (t.isMountain)
                {
                    return t;
                }
            }
            //not found
            return null;
        }

        /// <summary>
        /// Wrapper method of TryGetTile() to find a tile marked as a shore.
        /// </summary>
        /// <returns>The tile founds as shore; null if not found</returns>
        /// <example>
        /// The following code adds a "Shore" tile and then retrieves it automatically.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///    TileManager tileManager;
        ///
        ///    void Start()
        ///    {
        ///        tileManager = GameObject.FindObjectOfType&lt;TileManager&gt;();
        ///
        ///        //this method is not encouraged, used as a specific example and not best practice. Add tiles in the
        ///        //inspector instead.
        ///        tileManager.AddTile(new Tile("Shore", true, false, true));
        ///
        ///        Tile tile = tileManager.TryGetShore();
        ///
        ///        Debug.Log(tile.name);
        ///    }
        /// }
        ///
        /// //Output:
        /// //"Shore"
        /// </code>
        /// </example>
        public Tile TryGetShore()
        {
            //loop through all tiles and find one marked as a shore tile
            foreach (Tile t in internalTiles)
            {
                if (t.isShore)
                {
                    return t;
                }
            }
            //not found
            return null;
        }

        List<int> possibleTiles = new List<int>();
        public Tile DetermineTile(float rainfall, float temperature)
        {
            possibleTiles.Clear();
            //loop through all tiles
            for (int i = 0; i < internalTiles.Length; i++)
            {
                //if its not a special tile
                if ((internalTiles[i].isMountain == false && internalTiles[i].isOcean == false && internalTiles[i].isShore == false))
                {
                    if (temperature < internalTiles[i].possibleTemperatureValues.min) { continue; }
                    if (temperature > internalTiles[i].possibleTemperatureValues.max) { continue; }
                    if (rainfall < internalTiles[i].possibleRainfallValues.min) { continue; }
                    if (rainfall > internalTiles[i].possibleRainfallValues.max) { continue; }

                    possibleTiles.Add(i);
                }
            }

            if (possibleTiles.Count == 0)
            {
                Debug.LogError("Tile not found for this specific setting; Temperature: " + temperature + " ; Rainfall: " + rainfall);
                return null;
            }
            else
            {
                int result = Random.Range(0, possibleTiles.Count);

                return internalTiles[possibleTiles[result]];
            }
        }

        public Tile DetermineTile(float rainfall, float temperature, float latitude)
        {
            possibleTiles.Clear();

            temperature = Mathf.Round(100 * temperature) / 100;
            rainfall = Mathf.Round(100 * rainfall) / 100;

            for(int i = 0; i < internalTiles.Length; i++)
            {
                //if its not a special tile
                if ((internalTiles[i].isMountain == false && internalTiles[i].isOcean == false && internalTiles[i].isShore == false))
                {
                    if (temperature < internalTiles[i].possibleTemperatureValues.min) { continue; }
                    if (temperature > internalTiles[i].possibleTemperatureValues.max) { continue; }
                    if (rainfall < internalTiles[i].possibleRainfallValues.min) { continue; }
                    if (rainfall > internalTiles[i].possibleRainfallValues.max) { continue; }

                    possibleTiles.Add(i);
                }
            }

            if (possibleTiles.Count == 0)
            {
                Debug.LogError("Tile not found for this specific setting; Temperature: " + temperature + " ; Rainfall: " + rainfall);
                return null;
            }
            else
            {
                int result = Random.Range(0, possibleTiles.Count);

                return internalTiles[possibleTiles[result]];
            }
        }
    }

    [System.Serializable]
    public class WorldDegree
    {
        [SerializeField]
        public float bottom;
        [SerializeField]
        public float top;
        [SerializeField]
        public float left;
        [SerializeField]
        public float right;
    }

    [System.Serializable]
    public class Clamp
    {
        [SerializeField]
        public float min;
        [SerializeField]
        public float max;
    }

    /// <summary>
    /// Tile class that contains all values for the base tile
    /// </summary>
    [System.Serializable]
    public class Tile
    {
        /// <summary>
        /// Name of the tile
        /// </summary>
        public string name = "None";
        /// <summary>
        /// Possible latitude and longitudes to spawn this <see cref="Tile"/>
        /// </summary>
        //[SerializeField]
        //public WorldDegree possibleWorldDegrees = new WorldDegree();
        [SerializeField]
        public Clamp possibleRainfallValues = new Clamp();
        [SerializeField]
        public Clamp possibleTemperatureValues = new Clamp();
        /// <summary>
        /// Is an ocean tile
        /// </summary>
        public bool isOcean;
        /// <summary>
        /// Is a shore tile
        /// </summary>
        public bool isShore;
        /// <summary>
        /// Is a mountain tile
        /// </summary>
        public bool isMountain;

        ///// <summary>
        ///// Constructor for this class.
        ///// </summary>
        ///// <param name="name">Name of the tile</param>
        ///// <param name="bottomLongitude">Bottom lattitude clamp</param>
        ///// <param name="topLongitude">"Top lattitude clamp</param>
        //public Tile(string name, float bottomLat, float topLat)
        //{
        //    this.name = name;
        //    this.possibleWorldDegrees.bottom = bottomLat;
        //    this.possibleWorldDegrees.top = topLat;
        //}

        /// <summary>
        /// Full constructor for this class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isShore"></param>
        /// <param name="isOcean"></param>
        /// <param name="isMountain"></param>
        /// <param name="rainfallMin"></param>
        /// <param name="rainfallMax"></param>
        /// <param name="temperatureMin"></param>
        /// <param name="temperatureMax"></param>
        public Tile(string name, bool isShore, bool isOcean, bool isMountain, float rainfallMin, float rainfallMax, float temperatureMin, float temperatureMax)
        {
            this.name = name;
            this.isShore = isShore;
            this.isOcean = isOcean;
            this.isMountain = isMountain;
            this.possibleRainfallValues.min = rainfallMin;
            this.possibleRainfallValues.max = rainfallMax;
            this.possibleTemperatureValues.min = temperatureMin;
            this.possibleTemperatureValues.max = temperatureMax;
        }

        /// <summary>
        /// Special tile constructor for this class.
        /// </summary>
        /// <param name="name">Name of the tile</param>
        /// <param name="isShore">Is a shore tile</param>
        /// <param name="isOcean">Is an ocean tile</param>
        /// <param name="isMountain">Is a mountain</param>
        public Tile(string name, bool isShore, bool isOcean, bool isMountain)
        {
            this.name = name;
            this.isShore = isShore;
            this.isOcean = isOcean;
            this.isMountain = isMountain;
        }
    }
}
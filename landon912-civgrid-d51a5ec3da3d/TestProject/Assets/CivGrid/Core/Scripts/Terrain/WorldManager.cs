using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CivGrid;

namespace CivGrid
{
    /// <summary>
    /// Enum for the feature on a tile.<br />
    /// <br />
    /// Contains three basic types of features. See remarks for descriptions of each.
    /// </summary>
    /// <remarks>
    /// <list type="definition">
    /// <item>
    /// <term>Flat</term>
    /// <description>A completly flat hexagon with no change in the vertical axis.</description>
    /// </item>
    /// <item>
    /// <term>Hill</term>
    /// <description>A hill with vertical noise.</description>
    /// </item>
    /// <item>
    /// <term>Mountain</term>
    /// <description>A large pointed mountain with vertical noise.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public enum Feature
    {
        /// <summary>
        /// A completly flat hexagon with no change in the vertical axis.
        /// </summary>
        Flat = 0,
        /// <summary>
        /// A hill with vertical noise.
        /// </summary>
        Hill = 1,
        /// <summary>
        /// A large pointed mountain with vertical noise.
        /// </summary>
        Mountain = 3
    }

    /// <summary>
    /// Preset world generator values that create numerous world types.<br />
    /// <br />
    /// Contains six basic types of worlds. See remarks for description of each.<br />
    /// </summary>
    /// <remarks>
    /// Description for each world type.
    /// <list type="definition">
    /// <item>
    /// <term>Diced</term>
    /// <description>A very random map with many very small noisy island. No large landmasses, with a high ratio of water.</description>
    /// </item>
    /// <item>
    /// <term>Continents</term>
    /// <description>A world like ours. A few large land masses with numerous smaller islands. Fair amount of both water and land.</description>
    /// </item>
    /// <item>
    /// <term>Pangaea</term>
    /// <description>An extremely large landmass with a few smaller islands offshore. A large amount of land.</description>
    /// </item>
    /// <item>
    /// <term>Strings</term>
    /// <description>Long snakey islands throughout. No large landmasses, with a high ratio of water.</description>
    /// </item>
    /// <item>
    /// <term>Small Islands</term>
    /// <description>Many small islands. Islands are larger and more regular than with Diced. No large landmasses, with a high ratio of water.</description>
    /// </item>
    /// <item>
    /// <term>Large Islands</term>
    /// <description>A fair amount of medium sized landmasses. Medium landmasses, with a somewhat high ratio of water.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public enum WorldType
    {
        /// <summary>
        /// A very random map with many very small noisy island. No large landmasses, with a high ratio of water.
        /// </summary>
        Diced,
        /// <summary>
        /// A world like ours. A few large land masses with numerous smaller islands. Fair amount of both water and land.
        /// </summary>
        Continents,
        /// <summary>
        /// An extremely large landmass with a few smaller islands offshore. A large amount of land.
        /// </summary>
        Pangaea,
        /// <summary>
        /// Long snakey islands throughout. No large landmasses, with a high ratio of water.
        /// </summary>
        Strings,
        /// <summary>
        /// Many small islands. Islands are larger and more regular than with Diced. No large landmasses, with a high ratio of water.
        /// </summary>
        SmallIslands,
        /// <summary>
        /// A fair amount of medium sized landmasses. Medium landmasses, with a somewhat high ratio of water.
        /// </summary>
        LargeIslands
    }

    /// <summary>
    /// This script runs the entire CivGrid system. <br />
    /// <br />
    /// Holds all chunks, and in turn each hexagon, in memory and runs all the operations throughout them when needed. Contains the methods to generate worlds, load worlds, and save worlds.
    /// While some generation methods are exposed for use, it is best to not try and use the lower level methods.
    /// </summary>
    [RequireComponent(typeof(TileManager), typeof(ResourceManager), typeof(ImprovementManager))]
    public class WorldManager : MonoBehaviour
    {
        #region fields

        private int[][][] offsetNeighbors = new int[][][]
        {
            new int[][]
            {
                new int[]{+1, 0}, new int[]{0, -1}, new int[]{-1, -1},
                new int[]{-1, 0}, new int[]{-1, +1}, new int[]{0, +1}
            },
            new int[][]
            {
                new int[]{+1, 0}, new int[]{+1, -1}, new int[]{0, -1},
                new int[]{-1, 0}, new int[]{0, +1}, new int[]{+1, +1}
            },
        };

        /// <summary>
        /// The extents of a hexagon from the origin
        /// </summary>
        public Vector3 hexExt;
        /// <summary>
        /// The size of a hexagon from side to side
        /// </summary>
        public Vector3 hexSize;
        /// <summary>
        /// The center of a hexagon
        /// </summary>
        public Vector3 hexCenter;

        //internals
        private Vector3 moveVector;
        private RaycastHit chunkHit;
        private GameObject selectedHex;
        /// <summary>
        /// The position of the mouse in screen coordinates
        /// </summary>
        public Vector2 mousePos;
        private GameObject chunkHolder;
        /// <summary>
        /// The chunks in the generated world, <see cref="Chunk"/>
        /// </summary>
        public Chunk[,] hexChunks;
        private int xSectors;
        private int zSectors;
        /// <summary>
        /// A cached flat hexagon mesh
        /// </summary>
        public Mesh flatHexagonSharedMesh;

        /// <summary>
        /// Array of <see cref="Edge"/>s in the base mesh
        /// </summary>
        public Edge[] edges;
        public int[] edgeVertices;

        private bool doneGenerating;
        internal CivGridCamera civGridCamera;

        //World Values
        private Texture2D elevationMap;
        private Texture2D rainfallMap;
        private Texture2D temperatureMap;

        /// <summary>
        /// Scale of the noise map, and in turn the world
        /// </summary>
        public float noiseScale;
        /// <summary>
        /// The world texture atlas
        /// </summary>
        [SerializeField]
        public TextureAtlas textureAtlas;
        /// <summary>
        /// The type of the world, if one selected
        /// </summary>
        public WorldType worldType;
        /// <summary>
        /// If latitude should be factored into the tile determination
        /// </summary>
        public bool useLatitude = true;
        /// <summary>
        /// The size of the map in hexagons
        /// </summary>
        public Vector2 mapSize;
        /// <summary>
        /// The number of hexagons in one chunk, in one axis
        /// </summary>
        /// <remarks>
        /// The real amount of hexagons in the chunk is represented as: <b>(chunkSize)^2</b>
        /// </remarks>
        public int chunkSize;
        /// <summary>
        /// The radius of the hexagon
        /// </summary>
        public float hexRadiusSize;

        /// <summary>
        /// The level of detail of a hexagon
        /// </summary>
        public int levelOfDetail;

        public Mesh LOD2;
        public Mesh LOD3;

        [SerializeField]
        public BorderTextureData sprShDefBorders;
        public List<Color> borderColors = new List<Color>();
        [SerializeField] 
		public Texture2D borderTexture;
		
		[SerializeField]
		private bool showGrid;
		
		public bool ShowGrid
		{
			get { return showGrid; } 
			set { showGrid = value; 
			
			if(Application.isPlaying)
				{
					foreach(Chunk chunk in hexChunks)
					{
						chunk.UpdateGridOverlay();	
					}
				}
			}
		}
		
		[SerializeField] 
		public Texture2D gridTexture;

        [SerializeField]
        public Texture2D deepFogTexture;
        [SerializeField]
        public Texture2D lightFogTexture;

        //world setup
        /// <summary>
        /// Whether or not to use the built in <see cref="CivGridCamera"/> 
        /// </summary>
        public bool useCivGridCamera;
        /// <summary>
        /// Whether or not to generate the world on start up
        /// </summary>
        public bool generateOnStart;
        internal bool generateNewValues;
        /// <summary>
        /// Whether or not to use the built in world type values or custom user ones
        /// </summary>
        public bool useWorldTypeValues;

        /// <summary>
        /// The base heightmap for mountains
        /// </summary>
        [SerializeField]
        public Texture2D mountainHeightMap;
		
		[SerializeField]
		public TileDefines flatDefines = new TileDefines(0,0,0);
		[SerializeField]
		public TileDefines hillDefines = new TileDefines(0,0,0);// max:2, scale 0.01, size 0.03
		[SerializeField]
		public TileDefines mountainDefines = new TileDefines(0,0,0);//scale 1.05, max 1.2, scale 0.01, size 0.85


        //pathfinding
        public bool generateNodeLocations = true;
        public Vector3[,] nodeLocations;

        //hashtable
        public Dictionary<Vector2, Hex> axialToHexDictionary;

        //managers
        public ResourceManager resourceManager;
        public ImprovementManager improvementManager;
        public TileManager tileManager;

        /// <summary>
        /// Delegate for when a hexagon is clicked with a mouse button.
        /// </summary>
        /// <param name="hex">The hexagon clicked</param>
        /// <param name="mouseButton">The mouse button used</param>
        public delegate void OnHexClick(Hex hex, int mouseButton);
        /// <summary>
        /// Delegate to listen to for OnHexClick events.
        /// </summary>
        public static OnHexClick onHexClick;

        /// <summary>
        /// Delegate for when the mouse pointer is over a hexagon.
        /// </summary>
        /// <param name="hex">The hexagon that the mouse is over</param>
        public delegate void OnMouseOverHex(Hex hex);
        /// <summary>
        /// Delegate to listen to for OnMouseOverHex events.
        /// </summary>
        public static OnMouseOverHex onMouseOverHex;

        public delegate void StartHexOperations();
        public static StartHexOperations startHexOperations;
        #endregion

        /// <summary>
        /// Sets up values for world generation.
        /// </summary>
        void Awake()
        {
			showGrid = ShowGrid;
            resourceManager = GetComponent<ResourceManager>();
            improvementManager = GetComponent<ImprovementManager>();
            tileManager = GetComponent<TileManager>();
            civGridCamera = GameObject.FindObjectOfType<CivGridCamera>();
            axialToHexDictionary = new Dictionary<Vector2, Hex>();

            if (generateNodeLocations)
            {
                nodeLocations = new Vector3[(int)mapSize.x, (int)mapSize.y];
            }

            if (generateOnStart == true)
            {
                //LoadAndGenerateMap("terrainTest");
                GenerateNewMap(true);
                //FileUtility.SaveTerrain("terrainTest");
            }
            else { civGridCamera.enabled = false; }
        }

        /// <summary>
        /// Starts world generation.
        /// </summary>
        /// <param name="assignTypes">If it should assign values to hexagons</param>
        /// <remarks>
        /// For generating a new map, and not loading values, set the parameter to true.
        /// </remarks>
        public void GenerateNewMap(bool assignTypes)
        {
            this.generateNewValues = assignTypes;
            StartGeneration(true);
        }

        /// <summary>
        /// Starts world generation.
        /// </summary>
        public void GenerateNewMap()
        {
            this.generateNewValues = true;
            StartGeneration(true);
        }

        /// <summary>
        /// Handles destruction of world dependencies and generates a brand new world.
        /// </summary>
        public void RegenerateNewMap()
        {
            Destroy(GameObject.Find("ChunkHolder"));
            if (useCivGridCamera)
            {
                civGridCamera.enabled = false;
                Destroy(GameObject.Find("Cam2"));
            }
            StartGeneration(false);
        }

        /// <summary>
        /// Loads a map from a file name.
        /// </summary>
        /// <param name="name">Name of the saved map</param>
        /// <remarks>
        /// The file name should not be a complete file path, only the name given to the saved map.
        /// </remarks>
        public void LoadAndGenerateMap(string name)
        {
            generateNewValues = false;
            string savedMapLocation = Application.dataPath + "/../" + name;
            FileUtility.LoadTerrain(savedMapLocation);
            resourceManager.InitiateResourceTexturesOnHexs();
        }

        /// <summary>
        /// Saves a map under the given name.
        /// </summary>
        /// <param name="name">Name of the save</param>
        public void SaveMap(string name)
        {
            FileUtility.SaveTerrain(name);
        }

        /// <summary>
        /// Disbatches generation work.
        /// </summary>
        /// <param name="setUpManagers">If the manager need setup</param>
        private void StartGeneration(bool setUpManagers)
        {
            if (setUpManagers)
            {
                resourceManager.SetUp();
                improvementManager.SetUp();
                tileManager.SetUp();
            }


            DetermineWorldType();
            GetHexProperties();
            GenerateMap();

            if (useCivGridCamera)
            {
                civGridCamera.enabled = true;
                if (civGridCamera == null) { Debug.LogError("Please add the 'CivGridCamera' to the mainCamera"); }
                else { civGridCamera.SetupCameras(); }
            }

            if (generateNewValues == true)
            {
                resourceManager.InitiateResourceTexturesOnHexs();
            }

            doneGenerating = true;
        }

        /// <summary>
        /// Scales noise to be consistant between world sizes.
        /// </summary>
        private void SetNoiseScaleToTrueValue()
        {
            noiseScale /= mapSize.magnitude;
        }

        /// <summary>
        /// Sets the tileMap to the correct mapping settings.
        /// </summary>
        private void DetermineWorldType()
        {
            int smoothingCutoff = 0;
            if (useWorldTypeValues)
            {
                if (worldType == WorldType.Continents) { noiseScale = 5f; smoothingCutoff = 3; }
                else if (worldType == WorldType.Pangaea) { noiseScale = 3f; smoothingCutoff = 2; }
                else if (worldType == WorldType.Strings) { noiseScale = 25f; smoothingCutoff = 3; }
                else if (worldType == WorldType.Diced) { noiseScale = 25f; smoothingCutoff = 7; }
                else if (worldType == WorldType.LargeIslands) { noiseScale = 25f; smoothingCutoff = 5; }
                else if (worldType == WorldType.SmallIslands) { noiseScale = 25f; smoothingCutoff = 6; }
            }

            SetNoiseScaleToTrueValue();
            if (noiseScale == 0) { Debug.LogException(new UnityException("Noise scale is zero, this produces artifacts.")); }
            noiseScale = UnityEngine.Random.Range(noiseScale / 1.05f, noiseScale * 1.05f);

            elevationMap = NoiseGenerator.SmoothPerlinNoise((int)mapSize.x, (int)mapSize.y, noiseScale, smoothingCutoff);
            rainfallMap = NoiseGenerator.PerlinNoiseRaw((int)mapSize.x, (int)mapSize.y, 8, 0.5f, 1f);
            temperatureMap = NoiseGenerator.PerlinNoiseRaw((int)mapSize.x, (int)mapSize.y, 8, 1f, 1f);
        }

        /// <summary>
        /// Generates and caches a flat hexagon mesh for all the hexagon's to pull down into their localMesh, if they are flat.
        /// </summary>
        private void GetHexProperties()
        {
            //Creates mesh to calculate bounds
            GameObject inst = new GameObject("Bounds Set Up: Flat");
            //add mesh filter to our temp object
            inst.AddComponent<MeshFilter>();
            //add a renderer to our temp object
            inst.AddComponent<MeshRenderer>();
            //add a mesh collider to our temp object; this is for determining dimensions cheaply and easily
            inst.AddComponent<MeshCollider>();
            //reset the position to global zero
            inst.transform.position = Vector3.zero;
            //reset all rotation
            inst.transform.rotation = Quaternion.identity;

            //create new mesh to hold the data for the flat hexagon
            flatHexagonSharedMesh = new Mesh();

            if (levelOfDetail > 0)
            {
                if (levelOfDetail == 1)
                { flatHexagonSharedMesh = LOD2; }
                if (levelOfDetail == 2)
                { flatHexagonSharedMesh = LOD3; }
            }
            else
            {
                Vector3[] vertices;
                int[] triangles;
                Vector2[] uv;

                #region verts

                float floorLevel = 0;
                //positions vertices of the hexagon to make a normal hexagon
                vertices = new Vector3[]
                {
                    /*0*/new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(3+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(3+0.5)/6)))),
                    /*1*/new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(2+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(2+0.5)/6)))),
                    /*2*/new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(1+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(1+0.5)/6)))),
                    /*3*/new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(0+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(0+0.5)/6)))),
                    /*4*/new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(5+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(5+0.5)/6)))),
                    /*5*/new Vector3((hexRadiusSize * Mathf.Cos((float)(2*Mathf.PI*(4+0.5)/6))), floorLevel, (hexRadiusSize * Mathf.Sin((float)(2*Mathf.PI*(4+0.5)/6))))
                };

                #endregion

                #region triangles

                //triangles connecting the verts
                triangles = new int[] 
                {
                    1,5,0,
                    1,4,5,
                    1,2,4,
                    2,3,4
                };

                #endregion

                #region uv
                //uv mappping
                uv = new Vector2[]
                {
                    new Vector2(0.05f,0.25f),
                    new Vector2(0.05f,0.75f),
                    new Vector2(0.5f,0.95f),
                    new Vector2(0.95f,0.75f),
                    new Vector2(0.95f,0.25f),
                    new Vector2(0.5f,0.05f),
                };
                #endregion

                //assign verts
                flatHexagonSharedMesh.vertices = vertices;
                //assign triangles
                flatHexagonSharedMesh.triangles = triangles;
                //assign uv
                flatHexagonSharedMesh.uv = uv;
            }

            edgeVertices = MeshUtility.FindEdgeVertices(flatHexagonSharedMesh, out edges);

            #region finalize
            //set temp gameObject's mesh to the flat hexagon mesh
            inst.GetComponent<MeshFilter>().mesh = flatHexagonSharedMesh;
            //make object play nicely with lighting
            inst.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            //set mesh collider's mesh to the flat hexagon
            inst.GetComponent<MeshCollider>().sharedMesh = flatHexagonSharedMesh;

            //calculate the extents of the flat hexagon
            hexExt = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.extents.x - 0.01f, inst.gameObject.GetComponent<Collider>().bounds.extents.y, inst.gameObject.GetComponent<Collider>().bounds.extents.z - 0.01f);
            //calculate the size of the flat hexagon
            hexSize = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.size.x - 0.01f, inst.gameObject.GetComponent<Collider>().bounds.size.y, inst.gameObject.GetComponent<Collider>().bounds.size.z - 0.01f);
            //calculate the center of the flat hexagon
            hexCenter = new Vector3(inst.gameObject.GetComponent<Collider>().bounds.center.x, inst.gameObject.GetComponent<Collider>().bounds.center.y, inst.gameObject.GetComponent<Collider>().bounds.center.z);
            //calculate edge vertices
            //destroy the temp object that we used to calculate the flat hexagon's size
            Destroy(inst);
            #endregion
        }

        /// <summary>
        /// Creates a new chunk.
        /// </summary>
        /// <param name="x">The width interval of the chunks</param>
        /// <param name="y">The height interval of the chunks</param>
        /// <returns>The new chunk's script</returns>
        private Chunk NewChunk(int x, int y)
        {
            //if this the first chunk made?
            if (x == 0 && y == 0)
            {
                chunkHolder = new GameObject("ChunkHolder");
            }
            GameObject chunkObj = new GameObject("Chunk[" + x + "," + y + "]");
            Chunk hexChunk = chunkObj.AddComponent<Chunk>();
            hexChunk.SetSize(chunkSize, chunkSize);
            hexChunk.AllocateHexArray();
			chunkObj.AddComponent<MeshRenderer>();

			Renderer chunkRenderer = chunkObj.GetComponent<Renderer>();
			chunkRenderer.material.shader = Shader.Find( "Hexagon" );
			chunkRenderer.material.mainTexture = textureAtlas.terrainAtlas;
            chunkRenderer.material.SetTexture("_GridTex", gridTexture);
            chunkRenderer.material.SetTexture("_BlendTex", borderTexture);
            chunkRenderer.material.SetTexture("_FOWTex", deepFogTexture);

            //add the mesh filter
            chunkObj.AddComponent<MeshFilter>();
            //make this chunk a child of "ChunkHolder"s
            chunkObj.transform.parent = chunkHolder.transform;

            //return the script on the new chunk 
            return chunkObj.GetComponent<Chunk>();
        }

        /// <summary>
        /// Generate Chunks to make the map.
        /// </summary>
        private void GenerateMap()
        {

            //determine number of chunks
            xSectors = Mathf.CeilToInt(mapSize.x / chunkSize);
            zSectors = Mathf.CeilToInt(mapSize.y / chunkSize);

            //allocate chunk array
            hexChunks = new Chunk[xSectors, zSectors];

            //cycle through all chunks
            for (int x = 0; x < xSectors; x++)
            {
                for (int z = 0; z < zSectors; z++)
                {
                    //create the new chunk
                    hexChunks[x, z] = NewChunk(x, z);
                    //set the position of the new chunk
                    hexChunks[x, z].gameObject.transform.position = new Vector3(x * (chunkSize * hexSize.x - (0.01f * chunkSize)), 0f, (z * (chunkSize * hexSize.z - (0.01f * chunkSize)) * .75f));
                    //set hex size for hexagon positioning
                    hexChunks[x, z].hexSize = hexSize;
                    //set the number of hexagons for the chunk to generate
                    hexChunks[x, z].SetSize(chunkSize, chunkSize);
                    //the sector location of the chunk
                    hexChunks[x, z].chunkLocation = new Vector2(x, z);
                    //assign the world manager(this)
                    hexChunks[x, z].worldManager = this;
                }
            }

            //cycle through all chunks
            foreach (Chunk chunk in hexChunks)
            {
                //begin chunk operations since we are done with value generation
                chunk.Begin();
            }
            if (startHexOperations != null)
            {
                startHexOperations.Invoke();
            }
        }

        /// <summary>
        /// Use lattitude to determine the biome the tile is in.
        /// </summary>
        /// <param name="x">The x cords of the tile</param>
        /// <param name="h">The h(height) cord of the tile</param>
        /// <returns>An int corresponding to the biome it should be within</returns>
        internal Tile GenerateTileType(int x, int h)
        {
            Tile tile;

            //if water
            if (elevationMap.GetPixel(x, h).r == 0)
            {
                if (CheckIfCoast(x, h))
                {
                    tile = tileManager.TryGetShore();
                }
                else
                {
                    tile = tileManager.TryGetOcean();
                }
            }
            else
            {
                if (useLatitude)
                {
                    ////temp no influence from rainfall values
                    float latitude = Mathf.Abs((mapSize.y / 2) - x) / (mapSize.x / 2);
                    //float longitude = Mathf.Abs((mapSize.x / 2) - h) / (mapSize.y / 2);
                    ////add more results
                    latitude *= (1 + UnityEngine.Random.Range(-0.2f, 0.2f));
                    //longitude *= (1 + UnityEngine.Random.Range(-0.2f, 0.2f));
                    latitude = Mathf.Clamp(latitude, 0f, 1f);
                    //longitude = Mathf.Clamp(longitude, 0f, 1f);

                    tile = tileManager.DetermineTile(rainfallMap.GetPixel(x, h).grayscale, temperatureMap.GetPixel(x, h).grayscale, latitude);
                }
                else
                {
                    tile = tileManager.DetermineTile(rainfallMap.GetPixel(x, h).grayscale, temperatureMap.GetPixel(x, h).grayscale);
                }
            }

            return (tile);
        }

        /// <summary>
        /// Determines the tile's <see cref="Feature"/> type from the world map.
        /// </summary>
        /// <param name="xArrayPosition">X array position</param>
        /// <param name="yArrayPosition">Y array position</param>
        /// <param name="edge">If the world is an edge of a chunk</param>
        /// <returns>The correct <see cref="Feature"/> for this tile</returns>
        internal Feature GenerateFeatureType(int xArrayPosition, int yArrayPosition, bool edge)
        {
            float value = elevationMap.GetPixel(xArrayPosition, yArrayPosition).r;
            Feature returnVal = 0;

            if (value == 0.8f)
            {
                returnVal = Feature.Hill;
            }
            else if (value == 1f)
            {
                returnVal = Feature.Mountain;
            }
            else
            {
                returnVal = Feature.Flat;
            }
            if (edge)
            {
                returnVal = Feature.Flat;
            }
            return returnVal;
        }

        private bool CheckIfCoast(int x, int y)
        {
            float[] surrondingPixels = Utility.GetSurrondingPixels(elevationMap, x, y);

            int numberWater = 0;
            for (int i = 0; i < 8; i++)
            {
                if (surrondingPixels[i] < 0.5f)
                {
                    numberWater++;
                }
            }

            if (numberWater < 8)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void Update()
        {
            if (doneGenerating)
            {
                mousePos = Input.mousePosition;
            }
            RegisterDelegates();
        }

        private void RegisterDelegates()
        {
            Hex hex = GetHexFromMouse();

            //OnHexClick
            if (onHexClick != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (hex != null) { onHexClick.Invoke(hex, 0); }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    if (hex != null) { onHexClick.Invoke(hex, 1); }
                }
            }

            //OnMouseOverHex
            if (onMouseOverHex != null)
            {
                if (hex != null) { onMouseOverHex.Invoke(hex); }
            }
        }


        #region GetHex
        /// <summary>
        /// Get a hexagon from a world position.
        /// </summary>
        /// <param name="worldPosition">The position of the needed hexagon</param>
        /// <returns>The hex at the nearest position</returns>
        public Hex GetHexFromWorldPosition(Vector3 worldPosition)
        {
            Hex hexToReturn = null;

            float minDistance = 100;
            foreach (Chunk chunk in hexChunks)
            {
                foreach (Hex hex in chunk.hexArray)
                {
                    //find lowest distance to point
                    float distance = Vector3.Distance(hex.worldPosition, worldPosition);
                    if (distance < minDistance)
                    {
                        hexToReturn = hex;
                        minDistance = distance;
                    }
                }
            }

            return hexToReturn;
        }


        /// <summary>
        /// Get a hexagon from a world position; This is faster than not giving a chunk.
        /// </summary>
        /// <param name="worldPosition">The position of the needed hexagon</param>
        /// <param name="originalChunk">The chunk that contains the hexagon</param>
        /// <returns>The hexagon at the nearest position within the provided chunk</returns>
        public Hex GetHexFromWorldPosition(Vector3 worldPosition, Chunk originalChunk)
        {
            Hex hexToReturn = null;

            Chunk[] possibleChunks = FindPossibleChunks(originalChunk);

            float minDistance = 100;

            foreach (Chunk chunk in possibleChunks)
            {
                foreach (Hex hex in chunk.hexArray)
                {
                    //find lowest distance to point
                    float distance = Vector3.Distance(hex.worldPosition, worldPosition);
                    if (distance < minDistance)
                    {
                        hexToReturn = hex;
                        minDistance = distance;
                    }
                }
            }

            return hexToReturn;
        }

        /// <summary>
        /// Get a hexagon from axial coordinates
        /// </summary>
        /// <param name="axialCoordinates">Axial coordinates of the needed hexagon</param>
        /// <returns>The hexagon with the requested axial coordinates within the provided chunk</returns>
        public Hex GetHexFromAxialCoordinates(Vector2 axialCoordinates)
        {
            Hex returnHex;
            axialToHexDictionary.TryGetValue(axialCoordinates, out returnHex);

            return returnHex;
        }

        /// <summary>
        /// Get a hexagon from cube coordinates.
        /// </summary>
        /// <param name="cubeCoordinates">Cube coordinates of the needed hexagon</param>
        /// <returns>The hexagon with the requested cube coordinates</returns>
        public Hex GetHexFromCubeCoordinates(Vector3 cubeCoordinates)
        {
            return GetHexFromAxialCoordinates(new Vector2(cubeCoordinates.x, cubeCoordinates.z));
        }

        public Hex GetHexFromOffsetCoordinates(Vector2 position)
        {

            int hexChunksLengthX = hexChunks.GetLength(0) - 1;
            int hexChunksLengthY = hexChunks.GetLength(1) - 1;

            int chunkX = Mathf.FloorToInt(position.x / chunkSize);
            int chunkY = Mathf.FloorToInt(position.y / chunkSize);

            if (chunkX < 0 || chunkY < 0 || chunkX > hexChunksLengthX || chunkY > hexChunksLengthY)
                return null;

            Chunk chunk = hexChunks[chunkX, chunkY];
            Hex hex = chunk.hexArray[(int)position.x - (chunkX * chunkSize), (int)position.y - (chunkY * chunkSize)];

            return hex;
        }
         
         


        /// <summary>
        /// Gets a hexagon from the mouse posion.
        /// </summary>
        /// <returns>The hexagon closest to the mouse position</returns>
        public Hex GetHexFromMouse()
        {
            Ray ray1 = civGridCamera.GetCamera(0).ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray1, out chunkHit, 100f))
            {
                Chunk chunkHexIsLocatedIn = chunkHit.collider.gameObject.GetComponent<Chunk>();
                if (chunkHit.collider != null)
                {
                    return GetHexFromWorldPosition(chunkHit.point, chunkHexIsLocatedIn);
                }
            }
            if (civGridCamera.enableWrapping == true)
            {
                Ray ray2 = civGridCamera.GetCamera(1).ScreenPointToRay(mousePos);
                if (Physics.Raycast(ray2, out chunkHit, 100f))
                {
                    Chunk chunkHexIsLocatedIn = chunkHit.collider.gameObject.GetComponent<Chunk>();
                    if (chunkHit.collider != null)
                    {
                        return GetHexFromWorldPosition(chunkHit.point, chunkHexIsLocatedIn);
                    }
                }
            }

            return null;
        }
        #endregion

        public Hex[] GetNeighborsOfHex(Hex centerTile)
        { 
            int[] d;

            Hex[] neighbors = new Hex[6];
            Vector2 neighborOffsetGridPos = new Vector2(0, 0);

            int parity = (int)centerTile.OffsetCoordinates.y & 1;

            for (int i = 0; i < 6; i++)
            {

                d = offsetNeighbors[parity][i];

                neighborOffsetGridPos.x = centerTile.OffsetCoordinates.x + d[0];
                neighborOffsetGridPos.y = centerTile.OffsetCoordinates.y + d[1];

                neighbors[i] = GetHexFromOffsetCoordinates(neighborOffsetGridPos);
            }
            return neighbors;
        }

        public Hex GetOffsetNeighbour(Hex centreTile, int addedX, int addedY)
        {
            int parity = (int)centreTile.OffsetCoordinates.y & 1;

            Vector2 neighbourOffsetGridPos =
                new Vector2(centreTile.OffsetCoordinates.x + (addedY == 0 ? addedX : (addedX + parity)), centreTile.OffsetCoordinates.y + addedY);

            return GetHexFromOffsetCoordinates(neighbourOffsetGridPos);
        }

        private Chunk[] FindPossibleChunks(Chunk chunk)
        {
            List<Chunk> chunkArray = new List<Chunk>();

            if((chunk.chunkLocation.x + 1) >= 0 && (chunk.chunkLocation.x + 1) < xSectors && (chunk.chunkLocation.y) >= 0 && (chunk.chunkLocation.y) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x + 1, (int)chunk.chunkLocation.y]);
            }
            if ((chunk.chunkLocation.x + 1) >= 0 && (chunk.chunkLocation.x + 1) < xSectors && (chunk.chunkLocation.y + 1) >= 0 && (chunk.chunkLocation.y + 1) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x + 1, (int)chunk.chunkLocation.y + 1]);
            }
            if ((chunk.chunkLocation.x) >= 0 && (chunk.chunkLocation.x) < xSectors && (chunk.chunkLocation.y+1) >= 0 && (chunk.chunkLocation.y+1) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x, (int)chunk.chunkLocation.y + 1]);
            }
            if ((chunk.chunkLocation.x - 1) >= 0 && (chunk.chunkLocation.x - 1) < xSectors && (chunk.chunkLocation.y+1) >= 0 && (chunk.chunkLocation.y+1) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x - 1, (int)chunk.chunkLocation.y + 1]);
            }
            if ((chunk.chunkLocation.x - 1) >= 0 && (chunk.chunkLocation.x - 1) < xSectors && (chunk.chunkLocation.y) >= 0 && (chunk.chunkLocation.y) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x - 1, (int)chunk.chunkLocation.y]);
            }
            if ((chunk.chunkLocation.x - 1) >= 0 && (chunk.chunkLocation.x -1) < xSectors && (chunk.chunkLocation.y-1) >= 0 && (chunk.chunkLocation.y-1) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x - 1, (int)chunk.chunkLocation.y - 1]);
            }
            if ((chunk.chunkLocation.x) >= 0 && (chunk.chunkLocation.x) < xSectors && (chunk.chunkLocation.y-1) >= 0 && (chunk.chunkLocation.y-1) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x, (int)chunk.chunkLocation.y - 1]);
            }
            if ((chunk.chunkLocation.x + 1) >= 0 && (chunk.chunkLocation.x + 1) < xSectors && (chunk.chunkLocation.y-1) >= 0 && (chunk.chunkLocation.y-1) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x + 1, (int)chunk.chunkLocation.y - 1]);
            }
            if ((chunk.chunkLocation.x) >= 0 && (chunk.chunkLocation.x) < xSectors && (chunk.chunkLocation.y) >= 0 && (chunk.chunkLocation.y) < zSectors)
            {
                chunkArray.Add(hexChunks[(int)chunk.chunkLocation.x, (int)chunk.chunkLocation.y]);
            }
            
            return chunkArray.ToArray();
        }

        /*
        void CalculateDistance()
        {
            int dx = Mathf.Abs(Mathf.RoundToInt(goToHex.x - currentHex.x));
            int dy = Mathf.Abs(Mathf.RoundToInt(goToHex.y - currentHex.y));
            int dz = Mathf.Abs(Mathf.RoundToInt(goToHex.z - currentHex.z));

            int distanceA = Mathf.Max(dx, dy, dz);
            int distanceB = Mathf.Abs(distanceA - Mathf.Abs(Mathf.RoundToInt(mapSize.x + dx)));

            if (distanceA == distanceB)
            {
                distance = distanceA;
            }
            else
            {
                distance = Mathf.Min(distanceA, distanceB);
            }
        }

        int CalculateDistance(HexInfo start, HexInfo end)
        {
            int dx = Mathf.Abs(Mathf.RoundToInt(start.CubeGridPosition.x - end.CubeGridPosition.x));
            int dy = Mathf.Abs(Mathf.RoundToInt(start.CubeGridPosition.y - end.CubeGridPosition.y));
            int dz = Mathf.Abs(Mathf.RoundToInt(start.CubeGridPosition.z - end.CubeGridPosition.z));

            int distanceA = Mathf.Max(dx, dy, dz);
            int distanceB = Mathf.Abs(distanceA - Mathf.Abs(Mathf.RoundToInt(mapSize.x + dx)));

            if (distanceA == distanceB)
            {
                return distanceA;
            }
            else
            {
                return Mathf.Min(distanceA, distanceB);
            }
        }

        void OnGUI()
        {
            //GUI.Label(new Rect(20, 0, 100, 20), goToHex.ToString());
            //GUI.Label(new Rect(20, 30, 100, 20), distance.ToString("Distance: #."));
        }
         */

		// Added

        public IEnumerator RefreshBorders(Hex modifiedHex)
        {
            Chunk originalChunk = modifiedHex.parentChunk;
            Chunk[] possibleChunks = FindPossibleChunks(originalChunk);

            foreach (Chunk chunk in possibleChunks)
            {
                foreach (Hex hex in chunk.hexArray)
                {
                    // Have to make every hex update their border texture and value
                    hex.UpdateBorder();
                }
                chunk.RegenerateMesh();
                yield return new WaitForEndOfFrame();
            }
        }

		public void SetBorders(Hex[] hexes, int borderID)
		{
			foreach(Hex h in hexes)
			{
				h.SetBorder(borderID);	
			}
		}
		
		public void ClearBorders(Hex[] hexes)
		{
			foreach(Hex h in hexes)
			{
				h.ClearBorder();
			}
		}
    }
	
	#region HelperClasses
	
    /// <summary>
    /// The world texture atlas.
    /// </summary>
    /// <remarks>
    /// Contains the locations of each element within the texture.
    /// </remarks>
    [System.Serializable]
    public class TextureAtlas
    {
        /// <summary>
        /// The terrain texture
        /// </summary>
        [SerializeField]
        public Texture2D terrainAtlas;
        /// <summary>
        /// The location of each tile texture in the atlas
        /// </summary>
        [SerializeField]
        public TileItem[] tileLocations;
        /// <summary>
        /// The location of each resource texture in the atlas
        /// </summary>
        [SerializeField]
        public ResourceItem[] resourceLocations;
        /// <summary>
        /// The location of each improvement texture in the atlas
        /// </summary>
        [SerializeField]
        public ImprovementItem[] improvementLocations;
    }

    /// <summary>
    /// A tile item in the texture atlas.
    /// </summary>
    /// <remarks>
    /// Mimics Dictionary behaviour using serializable methods.
    /// </remarks>
    [System.Serializable]
    public class TileItem
    {
        [SerializeField]
        private Tile key;

        /// <summary>
        /// The key, in this case a <see cref="Tile"/>, in which a Rect is given
        /// </summary>
        [SerializeField]
        public Tile Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        [SerializeField]
        private Rect value;

        /// <summary>
        /// The Rect value for this key
        /// </summary>
        [SerializeField]
        public Rect Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value for the given key</param>
        [SerializeField]
        public TileItem(Tile key, Rect value)
        {
            this.key = key;
            this.value = value;
        }

    }

    /// <summary>
    /// A resource item in the texture atlas.
    /// </summary>
    /// <remarks>
    /// Mimics Dictionary behaviour using serializable methods.
    /// </remarks>
    [System.Serializable]
    public class ResourceItem
    {
        [SerializeField]
        private Resource key;

        /// <summary>
        /// The key, in this case a <see cref="Resource"/>, in which a Rect is given
        /// </summary>
        [SerializeField]
        public Resource Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        [SerializeField]
        private Rect value;

        /// <summary>
        /// The Rect value for this key
        /// </summary>
        [SerializeField]
        public Rect Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// The constructor for this class.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value for the given key</param>
        [SerializeField]
        public ResourceItem(Resource key, Rect value)
        {
            this.key = key;
            this.value = value;
        }
    }

    /// <summary>
    /// A improvement item in the texture atlas.
    /// </summary>
    /// <remarks>
    /// Mimics Dictionary behaviour using serializable methods.
    /// </remarks>
    [System.Serializable]
    public class ImprovementItem
    {
        [SerializeField]
        private Improvement key;

        /// <summary>
        /// The key, in this case a <see cref="Improvement"/>, in which a Rect is given
        /// </summary>
        [SerializeField]
        public Improvement Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        [SerializeField]
        private Rect value;

        /// <summary>
        /// The Rect value for this key
        /// </summary>
        [SerializeField]
        public Rect Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        /// <summary>
        /// Constructor for this class.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value for the given key</param>
        [SerializeField]
        public ImprovementItem(Improvement key, Rect value)
        {
            this.key = key;
            this.value = value;
        }
    }
	
	[System.Serializable]
	public class TileDefines
	{
		public float yScale;
		public float noiseScale;
        public float noiseSize;
        public float maximumHeight;
		
		public TileDefines(float noiseScale, float noiseSize, float maximumHeight)
		{
			this.yScale = 1;
			this.noiseScale = noiseScale;
			this.noiseSize = noiseSize;
			this.maximumHeight = maximumHeight;
		}
		
		public TileDefines(float yScale, float noiseScale, float noiseSize, float maximumHeight)
		{
			this.yScale = yScale;
			this.noiseScale = noiseScale;
			this.noiseSize = noiseSize;
			this.maximumHeight = maximumHeight;
		}
	}
	
	#endregion
}
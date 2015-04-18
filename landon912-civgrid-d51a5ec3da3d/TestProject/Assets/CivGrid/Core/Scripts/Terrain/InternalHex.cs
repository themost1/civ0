using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CivGrid;
using CivGrid.SampleResources;

namespace CivGrid
{
    internal enum EdgeType
    {
        WaterEdge,
        LandEdge,
        None
    }

    /// <summary>
    /// Contains all hexagon data and methods.
    /// Generates it's localMesh and uploads this to the chunk.
    /// Generates it's UV data depending on constraints.
    /// </summary>
    [System.Serializable]
    public class InternalHex
    {
        //positioning
        private Vector3 cubeCoordinates;
        private Vector2 axialCoordinates;
        private Vector2 offsetCoordinates;
        /// <summary>
        /// The position of this hexagon local to the parent chunk.
        /// </summary>
        /// <remarks>
        /// This is the local position with the origin being the chunk. Therefore, if the chunk is located
        /// at (10,10,10) in world space and the hexagon is located at (12,12,12) in world space. This values would
        /// contain (2,2,2).
        /// </remarks>
        public Vector3 localPosition;
        /// <summary>
        /// The position of the hexagon in world space.
        /// </summary>
        public Vector3 worldPosition;
        /// <summary>
        /// The position of the hexagon in the parent chunk array.
        /// </summary>
        public Vector2 chunkArrayPosition;
        
        /// <summary>
        /// The type of terrain that this hexagon represents.
        /// </summary>
        /// <remarks>
        /// Since this is represented by a class that is generated from user data, differation is possible.
        /// </remarks>
        public Tile terrainType;
        /// <summary>
        /// The type of terrain feature that this hexagon includes.
        /// </summary>
        /// <remarks>
        /// Since this is represented by an <see cref="System.Enum"/>, it is considered failsafe.
        /// </remarks>
        public Feature terrainFeature;
       
        /// <summary>
        /// The chunk that this hexagon is within.
        /// </summary>
        public Chunk parentChunk;

        private TextureAtlas worldTextureAtlas;

        /// <summary>
        /// The current location of the texture that the hexagon is using.
        /// </summary>
        public Rect currentRectLocation;
        /// <summary>
        /// The location of the base terrain texture.
        /// </summary>
        /// <remarks>
        /// This value should not be changed, as it holds the location to pull the default terrain
        /// texture from.
        /// </remarks>
        /// <example>
        /// If the <see cref="terrainType"/> is set to a <see cref="Tile"/> with the name of "Grass", this value
        /// will hold the location of the selected grass texture in the terrain atlas.
        /// </example>
        public Rect defaultRectLocation;
        internal Vector3 hexExt;
        internal Vector3 hexCenter;
        /// <summary>
        /// The mesh of this hexagon.
        /// </summary>
        /// <remarks>
        /// This mesh is used in the parent chunk to represent this hexagon in the chunk mesh.
        /// </remarks>
        public Mesh localMesh;

        //resources
        /// <summary>
        /// The current resource on this hexagon.
        /// </summary>
        /// <remarks>
        /// This holds a reference to the global resource. All changes to this <see cref="Resource"/> will be reflected in
        /// other hexagon sharing the resource.
        /// </remarks>
        [SerializeField]
        public Resource currentResource;
        internal ResourceManager resourceManager;
        /// <summary>
        /// The locations of each resource mesh.
        /// </summary>
        public List<Vector3> resourceLocations = new List<Vector3>();
        /// <summary>
        /// The GameObject that holds the resource meshes for this hexagon.
        /// </summary>
        public GameObject rObject;

        //improvments
        /// <summary>
        /// The current improvement on this hexagon.
        /// </summary>
        /// <remarks>
        /// This holds a reference to the global improvement. All changes to this <see cref="Improvement"/> will be reflected in
        /// other hexagon sharing the impovement.
        /// </remarks>
        [SerializeField]
        public Improvement currentImprovement;
        internal ImprovementManager improvementManager;
        /// <summary>
        /// The GameObject that holds the improvement meshes for this hexagon.
        /// </summary>
        public GameObject iObject;

        /// <summary>
        /// Bordering hexagons of this hexagon.
        /// </summary>
        public Hex[] neighbors;

        /// <summary>
        /// Team ID that owns this hex. uint.MaxValue is the undefined team id, because 0 is a valid team id.
        /// </summary>
        private uint borderID = uint.MaxValue;
		
		private bool underFogOfWar;

        public uint BorderID
        {
            get { return borderID; }
            set
            {
				if(value <= parentChunk.worldManager.borderColors.Count)
				{
                	borderID = value;
                	parentChunk.worldManager.StartCoroutine(parentChunk.worldManager.RefreshBorders((Hex)this));
				}
            }
        }
		
		public bool UnderFogOfWar
		{
			get { return underFogOfWar; }
			set
			{
				underFogOfWar = value;
			}
		}

        /// <summary>
        /// The coordinates of the hexagon in cube coordinates.
        /// </summary>
        /// <remarks>
        /// This is simply the complete version of the grid location. You can use <see cref="AxialCoordinates"/> and imply
        /// the "z" location with the rule of x + y + z = 0.
        /// </remarks>
        public Vector3 CubeCoordinates
        {
            get { return cubeCoordinates; }
            set { cubeCoordinates = value; }
        }

        /// <summary>
        /// The coordinates of the hexagon in axial coordinates.
        /// </summary>
        /// <remarks>
        /// This is simply a lighter version of <see cref="CubeCoordinates"/>, made possible by the fact, x + y + z = 0.
        /// With this equation we can include only the (x,y) cordinates and assume
        /// </remarks>
        public Vector2 AxialCoordinates
        {
            get
            {
                if (axialCoordinates == new Vector2(0,0)) { axialCoordinates = new Vector2(CubeCoordinates.x, CubeCoordinates.y); }
                return axialCoordinates;
            }
        }

        /// <summary>
        /// The coordinates of the hexagon in odd-r offset coordinates.
        /// </summary>
        /// <remarks>
        /// This is a different format of coordinates. Safe to use, however is not the most efficient method. <see cref="CubeCoordinates"/> is used internally
        /// and all other formats are converted on demand.
        /// </remarks>
        public Vector2 OffsetCoordinates
        {
            get
            {
                if (offsetCoordinates == new Vector2(0, 0))
                {
                    int x = (int)cubeCoordinates.x;
                    int y = (int)cubeCoordinates.y;
                    offsetCoordinates = new Vector2(x + (y + (y & 1)) / 2, y);

                    return offsetCoordinates;
                }
                else { return offsetCoordinates; }
            }
        }

        /// <summary>
        /// This is the setup called from HexChunk when it's ready for us to generate our meshes.
        /// </summary>
        /// <example>
        /// The following code will start hex operations on a new hex provided that the hexagon has a valid parent chunk and world manager.
        /// <code>
        /// class HexTest : MonoBehaviour
        /// {
        ///     HexInfo hex;
        ///     
        ///     void Start()
        ///     {
        ///         hex = new HexInfo();
        ///         
        ///         hex.Start();
        ///     }
        /// }
        /// </code>
        /// </example>
        public void Start()
        {
            //set tile type to the mountain tile if the feature is a mountain and the type exists
            if (terrainFeature == Feature.Mountain) { Tile mountain = parentChunk.worldManager.tileManager.TryGetMountain(); if (mountain != null) {  terrainType = mountain; } }

            //get the texture atlas from world manager
            worldTextureAtlas = parentChunk.worldManager.textureAtlas;

            parentChunk.worldManager.axialToHexDictionary.Add(AxialCoordinates, (Hex)this);

            //cache neighbors of this hexagon
            neighbors = parentChunk.worldManager.GetNeighborsOfHex((Hex)this);

            //generate local mesh
            MeshSetup();

            //if we are NOT loading a map
            if (parentChunk.worldManager.generateNewValues == true)
            {
                //check for resources and default to no improvement
                currentImprovement = improvementManager.improvements[0];
                resourceManager.CheckForResource((Hex)this);
            }
        }

        /// <summary>
        /// Applies any changes on this hex to it's parent chunk.
        /// </summary>
        /// <remarks>
        /// This method must be called to apply any changes to a hexagon's <see cref="Hex.localMesh"/>. Without calling
        /// this method the changes won't be seen in the chunk mesh.
        /// </remarks>
        /// <example>
        /// The following code changes the very middle hexagon in the map to show its resource texture.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// class ExampleClass : MonoBehaviour
        /// {
        ///    WorldManager worldManager;
        ///
        ///    public void Start()
        ///    {
        ///        //cache and find the world manager
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///
        ///        //gets the very middle hexagon in the map
        ///        HexChunk chunk = worldManager.hexChunks[worldManager.hexChunks.GetLength(0) / 2, worldManager.hexChunks.GetLength(1) / 2];
        ///        HexInfo hex = chunk.hexArray[chunk.hexArray.GetLength(0) / 2, chunk.hexArray.GetLength(1) / 2];
        ///
        ///        //change the hexes texture to the resource version
        ///        hex.ChangeTextureToResource();
        ///
        ///        //update the chunk mesh to apply the changes
        ///        hex.ApplyChanges();
        ///    }
        /// }
        ///
        /// </code>
        /// </example>
        public void ApplyChanges()
        {
            parentChunk.RegenerateMesh();
        }

        /// <summary>
        /// Switches this hexagon's UV data to display it's resource texture.
        /// </summary>
        /// <example>
        /// The following code changes this hexagon to show its resource texture if a resource and a resource tile
        /// texture is present.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///   WorldManager worldManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///
        ///        worldManager.hexChunks[0, 0].hexArray[0, 0].ChangeTextureToResource();
        ///    }
        /// }
        /// </code>
        /// </example>
        public void ChangeTextureToResource()
        {
            //if the current resource contains a ground texture
            if (currentResource.replaceGroundTexture == true)
            {
                //assign flat UV data
                if (terrainFeature == Feature.Flat)
                {
                    worldTextureAtlas.resourceLocations.TryGetValue(currentResource, out currentRectLocation);
                    AssignUVToTile(currentRectLocation);
                }
                //assign feature UV data
                else if (terrainFeature == Feature.Hill || terrainFeature == Feature.Mountain)
                {
                    worldTextureAtlas.resourceLocations.TryGetValue(currentResource, out currentRectLocation);
                    AssignUVToDefaultTile();
                }

                //regenerate chunk to incorperate our changes
                parentChunk.RegenerateMesh();
            }
        }

        /// <summary>
        /// Switches this hexagon's UV data to display it's improvement texture.
        /// </summary>
        /// <example>
        /// The following code changes this hexagon to show its improvement texture if a improvement and a improvement tile texture
        /// is present.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///   WorldManager worldManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///
        ///        worldManager.hexChunks[0, 0].hexArray[0, 0].ChangeTextureToImprovement();
        ///    }
        /// }
        /// </code>
        /// </example>
        public void ChangeTextureToImprovement()
        {
            //if the current improvement contains a ground texture
            if (currentImprovement.replaceGroundTexture == true)
            {
                //assign flat UV data
                if (terrainFeature == Feature.Flat)
                {
                    worldTextureAtlas.improvementLocations.TryGetValue(currentImprovement, out currentRectLocation);
                    AssignUVToTile(currentRectLocation);
                }
                //assign feature UV data
                else if (terrainFeature == Feature.Hill || terrainFeature == Feature.Mountain)
                {
                    worldTextureAtlas.improvementLocations.TryGetValue(currentImprovement, out currentRectLocation);
                    AssignUVToDefaultTile();
                }

                //regenerate chunk to incorperate our changes
                parentChunk.RegenerateMesh();
            }
        }

        /// <summary>
        /// Switches this hexagon's UV data to display it's normal base texture.
        /// </summary>
        /// <example>
        /// The following code changes this hexagon to show its normal texture.
        /// <code>
        /// using System;
        /// using UnityEngine;
        /// using CivGrid;
        ///
        /// public class ExampleClass : MonoBehaviour
        /// {
        ///   WorldManager worldManager;
        ///
        ///    void Start()
        ///    {
        ///        worldManager = GameObject.FindObjectOfType&lt;WorldManager&gt;();
        ///
        ///        worldManager.hexChunks[0, 0].hexArray[0, 0].ChangeTextureToNormalTile();
        ///    }
        /// }
        /// </code>
        /// </example>
        public void ChangeTextureToNormalTile()
        {
            //if we have a texture that is not the base tile texture; switch it
            if (currentImprovement.replaceGroundTexture == true || currentResource.replaceGroundTexture == true)
            {
                //assign flat UV data
                if (terrainFeature == Feature.Flat)
                {
                    AssignUVToDefaultTile();
                }
                //assign feature UV data
                else if (terrainFeature == Feature.Hill || terrainFeature == Feature.Mountain)
                {
                    AssignUVToDefaultTile();
                }

                //regenerate chunk to incorperate our changes
                parentChunk.RegenerateMesh();
            }
        }

        /// <summary>
        /// Generate a mesh, normals, and UV data according to the tile type.
        /// </summary>
        public void MeshSetup()
        {
            //create new mesh to start fresh
            localMesh = new Mesh();
			
            //if we are generating a flat regular hexagon
            if (parentChunk.worldManager.levelOfDetail == 0 || terrainType.isShore || terrainType.isOcean)
            {
                //pull mesh data from WorldManager
                localMesh.vertices = parentChunk.worldManager.flatHexagonSharedMesh.vertices;
                localMesh.triangles = parentChunk.worldManager.flatHexagonSharedMesh.triangles;
                localMesh.uv = parentChunk.worldManager.flatHexagonSharedMesh.uv;

                //we use tangents to pass in another uv map, and if the tile is under FOW(z component)
				localMesh.tangents = parentChunk.worldManager.flatHexagonSharedMesh.uv.ToVector4(1,0);

                //recalculate normals to play nicely with lighting
                localMesh.RecalculateNormals();

                //assign tile texture
                AssignUVToDefaultTile();
            }
            else if(parentChunk.worldManager.levelOfDetail == 1)
            {
                if (terrainType.isMountain)
                {
                    GenerateCustomHexMesh(false);
                }
                else
                {
                    //pull mesh data from WorldManager
                    localMesh.vertices = parentChunk.worldManager.flatHexagonSharedMesh.vertices;
                    localMesh.triangles = parentChunk.worldManager.flatHexagonSharedMesh.triangles;
                    localMesh.uv = parentChunk.worldManager.flatHexagonSharedMesh.uv;


                    //we use tangents to pass in another uv map, and if the tile is under FOW(z component)
					localMesh.tangents = parentChunk.worldManager.flatHexagonSharedMesh.uv.ToVector4(1,0);

                    //recalculate normals to play nicely with lighting
                    localMesh.RecalculateNormals();

                    //assign tile texture
                    AssignUVToDefaultTile();
                }
            }
            //LOD 2/3
            else
            {
                GenerateCustomHexMesh(true);
            }

			RefreshBorderTextureUV( -1 );
        }

        private void GenerateCustomHexMesh(bool raiseBorders)
        {
            Texture2D localHeightMap;

            localMesh.vertices = parentChunk.worldManager.flatHexagonSharedMesh.vertices;
            localMesh.triangles = parentChunk.worldManager.flatHexagonSharedMesh.triangles;
            localMesh.uv = parentChunk.worldManager.flatHexagonSharedMesh.uv;

            //we use tangents as an extra uv channel and to pass in the FOW settings
			localMesh.tangents = parentChunk.worldManager.flatHexagonSharedMesh.uv.ToVector4(1,0);
		
			
            if (terrainFeature == Feature.Mountain)
            {
                localHeightMap = NoiseGenerator.RandomOverlay(parentChunk.worldManager.mountainHeightMap,
                    Random.Range(-500f, 500f),
                    parentChunk.worldManager.mountainDefines.noiseScale, ///0.005f-0.18f
                    parentChunk.worldManager.mountainDefines.noiseSize, //0.2-0.5f
                    Random.Range(0.3f, 0.6f), //0.3-0.6
                    parentChunk.worldManager.mountainDefines.maximumHeight, //2 
                    true,
                    false);
            }
            else if (terrainFeature == Feature.Hill)
            {
                localHeightMap = NoiseGenerator.RandomOverlay(parentChunk.worldManager.mountainHeightMap,
                    Random.Range(-100f, 100f),
                    parentChunk.worldManager.hillDefines.noiseScale, //0.005-0.18
                    parentChunk.worldManager.hillDefines.noiseSize, //0.75-1
                    Random.Range(0.4f, 0.7f),
                    parentChunk.worldManager.hillDefines.maximumHeight, //2 
                    true,
                    false);
            }
            else
            {
                localHeightMap = NoiseGenerator.RandomOverlay(parentChunk.flatHeightMap,
                    Random.Range(-100f, 100f),
                    parentChunk.worldManager.flatDefines.noiseScale,
                    parentChunk.worldManager.flatDefines.noiseSize,
                    Random.Range(0.4f, 0.7f),
                    parentChunk.worldManager.flatDefines.maximumHeight,
                    false,
                    false);
            }

            Vector3[] vertices = localMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (VertexIsEdge(i))
                {
                    if (IsLandVertex(i) && raiseBorders == true)
                    {
                        vertices[i].Set(vertices[i].x, 0.06f, vertices[i].z);
                    }
                    else
                    {
                        vertices[i].Set(vertices[i].x, 0f, vertices[i].z);
                    }
                }
                else
                {
                    float pixelHeight = localHeightMap.GetPixelBilinear(localMesh.uv[i].x, localMesh.uv[i].y).grayscale;
                    if (terrainFeature == Feature.Mountain) { pixelHeight *= 1.5f; vertices[i].Set(vertices[i].x, vertices[i].y + (pixelHeight - (parentChunk.worldManager.mountainDefines.yScale / 100)), vertices[i].z); }
                    if (terrainFeature == Feature.Hill) { vertices[i].Set(vertices[i].x, vertices[i].y + (pixelHeight - (parentChunk.worldManager.hillDefines.yScale / 100)), vertices[i].z); }
                    //flat
                    else
                    {
                        pixelHeight = localHeightMap.GetPixelBilinear(localMesh.uv[i].x + (chunkArrayPosition.x * localMesh.uv[i].x), localMesh.uv[i].y + (chunkArrayPosition.y * localMesh.uv[i].y)).grayscale;
                        vertices[i].Set(vertices[i].x, vertices[i].y + pixelHeight / 10, vertices[i].z);
                    }
                }
            }
            localMesh.vertices = vertices;


            //recalculate normals to play nicely with lighting
            localMesh.RecalculateNormals();

            //assign tile texture
            AssignUVToDefaultTile();
        }

        private bool VertexIsEdge(int vertexIndex)
        {
            for (int i = 0; i < parentChunk.worldManager.edgeVertices.Length; i++)
            {
                if (parentChunk.worldManager.edgeVertices[i] == vertexIndex) { return true; }
            }
            return false;
        }

        private Edge[] GetAllEdgesFromVertex(int index)
        {
            List<Edge> returnArray = new List<Edge>();
            for (int i = 0; i < parentChunk.worldManager.edges.Length; i++)
            {
                if (parentChunk.worldManager.edges[i].vertexIndex[0] == index) { returnArray.Add(parentChunk.worldManager.edges[i]); }
                if (parentChunk.worldManager.edges[i].vertexIndex[1] == index) { returnArray.Add(parentChunk.worldManager.edges[i]); }
            }
            return returnArray.ToArray();
        }

        private bool IsLandVertex(int vertexIndex)
        {
            Edge[] containerEdges = GetAllEdgesFromVertex(vertexIndex);

            bool cornerEdge0;
            bool cornerEdge1;

            containerEdges[0].adjacentHex = GetAdjacentHexFromEdgeDirection(containerEdges[0].edgeLocation, out cornerEdge0);
            containerEdges[1].adjacentHex = GetAdjacentHexFromEdgeDirection(containerEdges[1].edgeLocation, out cornerEdge1);

            if (containerEdges[0].adjacentHex != null || containerEdges[1].adjacentHex != null)
            {
                if (cornerEdge0)
                {
                    if (containerEdges[1].adjacentHex != null)
                    {
                        if (containerEdges[1].adjacentHex.terrainType.isOcean || containerEdges[1].adjacentHex.terrainType.isShore)
                        {
                            //water edge
                            return false;
                        }
                    }
                    if (containerEdges[0].adjacentHex != null)
                    {
                        if (containerEdges[0].adjacentHex.terrainType.isOcean || containerEdges[0].adjacentHex.terrainType.isShore)
                        {
                            //water edge
                            return false;
                        }
                    }
                }
                else
                {
                    if (containerEdges[0].adjacentHex != null)
                    {
                        if (containerEdges[0].adjacentHex.terrainType.isOcean || containerEdges[0].adjacentHex.terrainType.isShore)
                        {
                            //water edge
                            return false;
                        }
                    }
                    if (containerEdges[1].adjacentHex != null)
                    {
                        if (containerEdges[1].adjacentHex.terrainType.isOcean || containerEdges[1].adjacentHex.terrainType.isShore)
                        {
                            //water edge
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private Hex DetermineSideToReturn(int first, int second)
        {
            if (neighbors[first] != null)
            {
                if (neighbors[first].terrainType.isOcean || neighbors[first].terrainType.isShore) { return neighbors[first]; }
            }
            if (neighbors[second] != null)
            {
                if (neighbors[second].terrainType.isOcean || neighbors[second].terrainType.isShore) { return neighbors[second]; }
            }
            return neighbors[first];
        }

        private Hex GetAdjacentHexFromEdgeDirection(EdgeLocation direction, out bool isCornerDirection)
        {
            if (neighbors.Length == 6)
            {
                switch (direction)
                {
                    ///
                    /// Points
                    /// 
                    case EdgeLocation.Top:
                        isCornerDirection = true;
                        return DetermineSideToReturn(4, 5);
                    case EdgeLocation.Bottom:
                        isCornerDirection = true;
                        return DetermineSideToReturn(1, 2);
                    case EdgeLocation.BottomLeftCorner:
                        isCornerDirection = true;
                        return DetermineSideToReturn(2, 3);
                    case EdgeLocation.BottomRightCorner:
                        isCornerDirection = true;
                        return DetermineSideToReturn(0, 1);
                    case EdgeLocation.TopLeftCorner:
                        isCornerDirection = true;
                        return DetermineSideToReturn(3, 4);
                    case EdgeLocation.TopRightCorner:
                        isCornerDirection = true;
                        return DetermineSideToReturn(0, 5);
                    ///
                    /// Sides
                    ///
                    case EdgeLocation.Right:
                        isCornerDirection = false;
                        return neighbors[0];
                    case EdgeLocation.BottomRight:
                        isCornerDirection = false;
                        return neighbors[1];
                    case EdgeLocation.BottomLeft:
                        isCornerDirection = false;
                        return neighbors[2];
                    case EdgeLocation.Left:
                        isCornerDirection = false;
                        return neighbors[3];
                    case EdgeLocation.TopLeft:
                        isCornerDirection = false;
                        return neighbors[4];
                    case EdgeLocation.TopRight:
                        isCornerDirection = false;
                        return neighbors[5];
                }
            }
            Debug.LogError("Adjacent hex not found");
            isCornerDirection = false;
            return null;
        }

        /// <summary>
        /// Assigns the flat hexagon's UV data to the tile type.
        /// </summary>
        private void AssignUVToDefaultTile()
        {
            Vector2[] UV;

            //the base 1:1 UV map
            Vector2[] rawUV = parentChunk.worldManager.flatHexagonSharedMesh.uv;

            //if we are NOT loading
            if (parentChunk.worldManager.generateNewValues == true)
            {
                //if defualtRect location is null
                if (defaultRectLocation == new Rect())
                {
                    //get the postion of the texture on the texture atlas
                    parentChunk.worldManager.textureAtlas.tileLocations.TryGetValue(terrainType, out currentRectLocation);
                    defaultRectLocation = currentRectLocation;
                }
                //use cached rect location
                else { currentRectLocation = defaultRectLocation; }
            }

            //temp UV data
            UV = new Vector2[rawUV.Length];

            //shift base 1:1 UV map to the scale and location of the texture on the texture atlas
            for (int i = 0; i < rawUV.Length; i++)
            {
                UV[i] = new Vector2(rawUV[i].x * currentRectLocation.width + currentRectLocation.x, rawUV[i].y * currentRectLocation.height + currentRectLocation.y);
            }

            //assign the created UV data
            localMesh.uv = UV;
        }

        /// <summary>
        /// Assigns the flat hexagon's UV data to provided location on the texture atlas.
        /// </summary>
        /// <param name="rectArea">The location of the texture in the texture atlas</param>
        private void AssignUVToTile(Rect rectArea)
        {
            Vector2[] UV;

            //the base 1:1 UV map
            Vector2[] rawUV = parentChunk.worldManager.flatHexagonSharedMesh.uv;

            //temp UV data
            UV = new Vector2[rawUV.Length];

            //shift base 1:1 UV map to the scale and location of the texture on the texture atlas
            for (int i = 0; i < rawUV.Length; i++)
            {
                UV[i] = new Vector2(rawUV[i].x * rectArea.width + rectArea.x, rawUV[i].y * rectArea.height + rectArea.y);
            }

            //assign the created UV data
            localMesh.uv = UV;
        }

		//Borders
		
		public void SetBorder(int borderID)
		{
			BorderID = (uint)borderID;
		}
		
		public void ClearBorder()
		{
			BorderID = uint.MaxValue;	
		}

        private void RefreshBorderTextureUV(int borderTileType)
        {
            // This does the assigning of this tile's UV2 to the corrensponding cell, depending on the borderTileType

            int borderTypeId = borderTileType < 0 ? 64 : borderTileType;

            Rect rectArea = parentChunk.worldManager.sprShDefBorders.GetRectFromBorderId(borderTypeId);

            Vector2[] rawUV = parentChunk.worldManager.flatHexagonSharedMesh.uv;

            Vector2[] UV2 = new Vector2[rawUV.Length];
            Color[] colors = new Color[rawUV.Length];

            int sprShTextureWidth = parentChunk.worldManager.sprShDefBorders.textureWidth;
            int sprShTextureHeight = parentChunk.worldManager.sprShDefBorders.textureHeight;

            for (int i = 0; i < rawUV.Length; i++)
            {

                UV2[i] = new Vector2(rawUV[i].x * (rectArea.width / sprShTextureWidth) + (rectArea.x / sprShTextureWidth),
                                  rawUV[i].y * (rectArea.height / sprShTextureHeight) + (rectArea.y / sprShTextureHeight));

                colors[i] = borderTypeId == 64 ? Color.white : parentChunk.worldManager.borderColors[(int)borderID];

            }

            localMesh.uv2 = UV2;
            localMesh.colors = colors;
        }

        public int QueryBorderValue()
        {

            // Queries the border value - That is, the number defined by how many of this tile's neighbours also belong to the same
            // team. This number corresponds to a specific tile in the Borders.png, and thus a specific rect in Borders.asset as defined
            // by the sprite sheet creator. Thus we can get the region of the texture for that cell, assign tile's UV2's to it.

            if (borderID == uint.MaxValue)
                return -1;

            Hex val_1 = parentChunk.worldManager.GetOffsetNeighbour((Hex)this, 0, 1);
            Hex val_2 = parentChunk.worldManager.GetOffsetNeighbour((Hex)this, 1, 0);
            Hex val_4 = parentChunk.worldManager.GetOffsetNeighbour((Hex)this, 0, -1);
            Hex val_8 = parentChunk.worldManager.GetOffsetNeighbour((Hex)this, -1, -1);
            Hex val_16 = parentChunk.worldManager.GetOffsetNeighbour((Hex)this, -1, 0);
            Hex val_32 = parentChunk.worldManager.GetOffsetNeighbour((Hex)this, -1, 1);

            int border_1 = val_1 == null || val_1.borderID != borderID ? 0 : 1;
            int border_2 = val_2 == null || val_2.borderID != borderID ? 0 : 2;
            int border_4 = val_4 == null || val_4.borderID != borderID ? 0 : 4;
            int border_8 = val_8 == null || val_8.borderID != borderID ? 0 : 8;
            int border_16 = val_16 == null || val_16.borderID != borderID ? 0 : 16;
            int border_32 = val_32 == null || val_32.borderID != borderID ? 0 : 32;

            return border_1 + border_2 + border_4 + border_8 + border_16 + border_32;

        }

		public void UpdateBorder()
        {
			RefreshBorderTextureUV( QueryBorderValue() );
		}

    }
}
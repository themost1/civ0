using UnityEngine;
using System.Collections;

public enum Tile { Main, Ocean }

/// <summary>
/// Master controller of the world; makes the chunks and handles world values
/// </summary>
public class WorldManager : MonoBehaviour {

    #region fields
    [HideInInspector]
    public Mesh flatHexagonSharedMesh;
    public float hexRadiusSize;

    //hexInstances
    [HideInInspector]
    public Vector3 hexExt;
    [HideInInspector]
    public Vector3 hexSize;
    [HideInInspector]
    public Vector3 hexCenter;
    [HideInInspector]
    public GameObject chunkHolder;

    public Texture2D terrainTexture;

    int xSectors;
    int zSectors;

    public HexChunk[,] hexChunks;

    public Vector2 mapSize;
    public int chunkSize;
    #endregion


    public void Awake()
    {
        //get the flat hexagons size; we use this to space out the hexagons
        GetHexProperties();
        //generate the chunks of the world
        GenerateMap();
    }


    /// <summary>
    /// Generates and caches a flat hexagon mesh for all the hexagon's to pull down into their localMesh, if they are flat
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
                new Vector2(0,0.25f),
                new Vector2(0,0.75f),
                new Vector2(0.5f,1),
                new Vector2(1,0.75f),
                new Vector2(1,0.25f),
                new Vector2(0.5f,0),
            };
        #endregion

        #region finalize
        //create new mesh to hold the data for the flat hexagon
        flatHexagonSharedMesh = new Mesh();
        //assign verts
        flatHexagonSharedMesh.vertices = vertices;
        //assign triangles
        flatHexagonSharedMesh.triangles = triangles;
        //assign uv
        flatHexagonSharedMesh.uv = uv;
        //set temp gameObject's mesh to the flat hexagon mesh
        inst.GetComponent<MeshFilter>().mesh = flatHexagonSharedMesh;
        //make object play nicely with lighting
        inst.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        //set mesh collider's mesh to the flat hexagon
        inst.GetComponent<MeshCollider>().sharedMesh = flatHexagonSharedMesh;
        #endregion

        //calculate the extents of the flat hexagon
        hexExt = new Vector3(inst.GetComponent<Collider>().bounds.extents.x, inst.GetComponent<Collider>().bounds.extents.y, inst.GetComponent<Collider>().bounds.extents.z);
        //calculate the size of the flat hexagon
        hexSize = new Vector3(inst.GetComponent<Collider>().bounds.size.x, inst.GetComponent<Collider>().bounds.size.y, inst.GetComponent<Collider>().bounds.size.z);
        //calculate the center of the flat hexagon
        hexCenter = new Vector3(inst.GetComponent<Collider>().bounds.center.x, inst.GetComponent<Collider>().bounds.center.y, inst.GetComponent<Collider>().bounds.center.z);
        //destroy the temp object that we used to calculate the flat hexagon's size
        Destroy(inst);
    }

    /// <summary>
    /// Generate Chunks to make the map
    /// </summary>
    void GenerateMap()
    {

        //determine number of chunks
        xSectors = Mathf.CeilToInt(mapSize.x / chunkSize);
        zSectors = Mathf.CeilToInt(mapSize.y / chunkSize);

        //allocate chunk array
        hexChunks = new HexChunk[xSectors, zSectors];

        //cycle through all chunks
        for (int x = 0; x < xSectors; x++)
        {
            for (int z = 0; z < zSectors; z++)
            {
                //create the new chunk
                hexChunks[x, z] = NewChunk(x, z);
                //set the position of the new chunk
                hexChunks[x, z].gameObject.transform.position = new Vector3(x * (chunkSize * hexSize.x), 0f, (z * (chunkSize * hexSize.z) * (.75f)));
                //set hex size for hexagon positioning
                hexChunks[x, z].hexSize = hexSize;
                //set the number of hexagons for the chunk to generate
                hexChunks[x, z].SetSize(chunkSize, chunkSize);
                //the width interval of the chunk
                hexChunks[x, z].xSector = x;
                //set the height interval of the chunk
                hexChunks[x, z].ySector = z;
                //assign the world manager(this)
                hexChunks[x, z].worldManager = this;
            }
        }

        //cycle through all chunks
        foreach (HexChunk chunk in hexChunks)
        {
            //begin chunk operations since we are done with value generation
            chunk.Begin();
        }

    }

    /// <summary>
    /// Creates a new chunk
    /// </summary>
    /// <param name="x">The width interval of the chunks</param>
    /// <param name="y">The height interval of the chunks</param>
    /// <returns>The new chunk's script</returns>
    public HexChunk NewChunk(int x, int y)
    {
        //if this the first chunk made?
        if (x == 0 && y == 0)
        {
            chunkHolder = new GameObject("ChunkHolder");
        }
        //create the chunk object
        GameObject chunkObj = new GameObject("Chunk[" + x + "," + y + "]");
        //add the hexChunk script and set it's size
        chunkObj.AddComponent<HexChunk>();
        //set the texture map for this chunk and add the mesh renderer
        chunkObj.AddComponent<MeshRenderer>().material.mainTexture = terrainTexture;
        //add the mesh filter
        chunkObj.AddComponent<MeshFilter>();
        //make this chunk a child of "ChunkHolder"s
        chunkObj.transform.parent = chunkHolder.transform;

        //return the script on the new chunk 
        return chunkObj.GetComponent<HexChunk>();
    }
}
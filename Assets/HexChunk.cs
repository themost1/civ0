using UnityEngine;
using System.Collections;

/// <summary>
/// Provides chunking services to hexagons :)
/// </summary>
public class HexChunk : MonoBehaviour
{
    #region fields
    [SerializeField]
    public HexInfo[,] hexArray;
    public Vector2 chunkSize;
    public Vector3 hexSize;

    //set by world master
    public int xSector;
    public int ySector;
    public WorldManager worldManager;

    private MeshFilter filter;
    private new BoxCollider collider;
    #endregion

    /// <summary>
    /// Sets the amount of hexagons in the chunk
    /// </summary>
    /// <param name="x">Amount of hexagons in "x" axis</param>
    /// <param name="y">Amount of hexagons in "y" axis</param>
    public void SetSize(int x, int y)
    {
        chunkSize = new Vector2(x, y);
    }

    /// <summary>
    /// Cleans up the material on this object after it is destroyed
    /// </summary>
    public void OnDestroy()
    {
        Destroy(GetComponent<Renderer>().material);
    }

    /// <summary>
    /// Starts chunk operations of spawning the hexagons and then chunking them
    /// </summary>
    public void Begin()
    {
        //begin making hexagons
        GenerateChunk();

        //cycle through all hexagons
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int z = 0; z < chunkSize.y; z++)
            {
                //check if this hexagon is null; if so throw an error
                if (hexArray[x, z] != null)
                {
                    //set parent chunk of the hex to this
                    hexArray[x, z].parentChunk = this;
                    //start hex operations(pulling down the mesh)
                    hexArray[x, z].Start();
                }
                else
                {
                    //throw error if the hexagon is null in memory
                    Debug.LogError("null hexagon found in memory: " + x + " " + z);
                }
            }
        }

        //combine all the hexagon's meshes in this chunk into one mesh
        Combine();
    }

    void MakeCollider()
    {
        //if we dont have a collider, add one
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        //set the collider's center and size to our mesh's size/center
        collider.center = filter.mesh.bounds.center;
        collider.size = filter.mesh.bounds.size;
    }

    private void Combine()
    {
        //make a two-dimensional array to remain constant with our hexArray
        CombineInstance[,] combine = new CombineInstance[(int)chunkSize.x, (int)chunkSize.y];

        //cycle through all the hexagons in this chunk
        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int z = 0; z < chunkSize.y; z++)
            {
                //set the CombineInstance's mesh to this hexagon's localMesh
                combine[x, z].mesh = hexArray[x, z].localMesh;
                //create a Matrix4x4 for the meshes positioning
                Matrix4x4 matrix = new Matrix4x4();
                //set the matrix position, rotation, and scale to correct values
                matrix.SetTRS(hexArray[x, z].localPosition, Quaternion.identity, Vector3.one);
                //assign the CombineInstance's transform to the matrix; therefore correctly positioning it
                combine[x, z].transform = matrix;
            }
        }

        //get the filter on the chunk gameObject
        filter = gameObject.GetComponent<MeshFilter>();
        //create a new mesh on it
        filter.mesh = new Mesh();

        //convert our two-dimensional array into a normal array so that we can use mesh.CombineMeshes()
        CombineInstance[] final;

        //conver to single array
        CivGridUtility.ToSingleArray(combine, out final);

        //set the chunk's mesh to the combined mesh of all the hexagon's in this chunk
        filter.mesh.CombineMeshes(final);
        //recalculate the normals of the new mesh to play nicely with lighting
        filter.mesh.RecalculateNormals();
        //generate/set the collider dimensions for this chunk
        MakeCollider();
    }

    public void GenerateChunk()
    {
        //create the hexagons in memory
        hexArray = new HexInfo[(int)chunkSize.x, (int)chunkSize.y];

        //if this is an offset row
        bool notOdd;

        //cycle through all hexagons in this chunk in the "y" axis
        for (int y = 0; y < chunkSize.y; y++)
        {
            //determine if we are in an odd row; if so we need to offset the hexagons
            notOdd = ((y % 2) == 0);
            //actually not 
            if (notOdd == true)
            {
                //generate the hexagons in the normal positioning for this row
                for (int x = 0; x < chunkSize.x; x++)
                {
                    //spawn hexagon
                    GenerateHex(x, y);
                }
            }
            else
            {
                //generate the hexagons in the offset positioning for this row
                for (int x = 0; x < chunkSize.x; x++)
                {
                    //spawn offset hexagon
                    GenerateHexOffset(x, y);
                }
            }
        }
    }

    public void GenerateHex(int x, int y)
    {
        //cache and create hex hex
        HexInfo hex;
        Vector2 worldArrayPosition;
        hexArray[x, y] = new HexInfo();
        hex = hexArray[x, y];

        //set world array position for real texture positioning
        worldArrayPosition.x = x + (chunkSize.x * xSector);
        worldArrayPosition.y = y + (chunkSize.y * ySector);

        hex.CubeGridPosition = new Vector3(worldArrayPosition.x - Mathf.Round((worldArrayPosition.y / 2) + .1f), worldArrayPosition.y, -(worldArrayPosition.x - Mathf.Round((worldArrayPosition.y / 2) + .1f) + worldArrayPosition.y));
        //set local position of hex; this is the hex cord postion local to the chunk
        hex.localPosition = new Vector3((x * (worldManager.hexExt.x * 2)), 0, (y * worldManager.hexExt.z) * 1.5f);
        //set world position of hex; this is the hex cord postion local to the world
        hex.worldPosition = new Vector3(hex.localPosition.x + (xSector * (chunkSize.x * hexSize.x)), hex.localPosition.y, hex.localPosition.z + ((ySector * (chunkSize.y * hexSize.z)) * (.75f)));

        ///Set Hex values
        hex.hexExt = worldManager.hexExt;
        hex.hexCenter = worldManager.hexCenter;
    }

    public void GenerateHexOffset(int x, int y)
    {
        //cache and create hex hex
        HexInfo hex;
        Vector2 worldArrayPosition;
        hexArray[x, y] = new HexInfo();
        hex = hexArray[x, y];

        //set world array position for real texture positioning
        worldArrayPosition.x = x + (chunkSize.x * xSector);
        worldArrayPosition.y = y + (chunkSize.y * ySector);

        hex.CubeGridPosition = new Vector3(worldArrayPosition.x - Mathf.Round((worldArrayPosition.y / 2) + .1f), worldArrayPosition.y, -(worldArrayPosition.x - Mathf.Round((worldArrayPosition.y / 2) + .1f) + worldArrayPosition.y));
        //set local position of hex; this is the hex cord postion local to the chunk
        hex.localPosition = new Vector3((x * (worldManager.hexExt.x * 2)) + worldManager.hexExt.x, 0, (y * worldManager.hexExt.z) * 1.5f);
        //set world position of hex; this is the hex cord postion local to the world
        hex.worldPosition = new Vector3(hex.localPosition.x + (xSector * (chunkSize.x * hexSize.x)), hex.localPosition.y, hex.localPosition.z + ((ySector * (chunkSize.y * hexSize.z)) * (.75f)));

        ///Set Hex values
        hex.hexExt = worldManager.hexExt;
        hex.hexCenter = worldManager.hexCenter;
    }
}
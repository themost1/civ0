using UnityEngine;
using System.Collections;

/// <summary>
/// The individual hexagon; lives only in the heap
/// </summary>
public class HexInfo
{
    #region fields
    private Vector3 gridPosition;//cube cordinates stored(x,y == axial)
    public Vector3 localPosition;
    public Vector3 worldPosition;

    public Vector3 hexExt;
    public Vector3 hexCenter;

    public HexChunk parentChunk;

    public Mesh localMesh;


    //basic hexagon mesh making
    public Vector3[] vertices;
    public Vector2[] uv;
    public int[] triangles;

    //get axial grid position
    public Vector2 AxialGridPosition
    {
        get { return new Vector2(CubeGridPosition.x, CubeGridPosition.y); }
    }
    //get/set cube grid position
    public Vector3 CubeGridPosition
    {
        get { return gridPosition; }
        set { gridPosition = value; }
    }
    #endregion

    /// <summary>
    /// This is the setup called from HexChunk when it's ready for us to generate our meshes
    /// </summary>
    public void Start()
    {
        MeshSetup();
    }

    /// <summary>
    /// This pulls the cached hexagon mesh data from the WorldManager down into our localMesh
    /// </summary>
    private void MeshSetup()
    {
        //create new mesh to start fresh
        localMesh = new Mesh();

        //pull mesh data from WorldManager
        localMesh.vertices = parentChunk.worldManager.flatHexagonSharedMesh.vertices;
        localMesh.triangles = parentChunk.worldManager.flatHexagonSharedMesh.triangles;
        localMesh.uv = parentChunk.worldManager.flatHexagonSharedMesh.uv;

        //recalculate normals to play nicely with lighting
        localMesh.RecalculateNormals();
    }
}

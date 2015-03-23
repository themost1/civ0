using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace CivGrid
{
    /// <summary>
    /// Helper class for basic utility methods.
    /// </summary>
    public static class Utility
    {
#if UNITY_EDITOR
        #region CivGrid Component Spawner

        [UnityEditor.MenuItem("CivGrid/Create CivGrid Camera", priority = 6)]
        public static void CreateCivGridCamera()
        {
            new GameObject("CivGrid Camera", typeof(Camera), typeof(GUILayer), typeof(AudioListener), typeof(CivGridCamera));
        }

        [UnityEditor.MenuItem("CivGrid/Create CivGrid World Manager", priority = 5)]
        public static void CreateCivGridWorldManager()
        {
            new GameObject("CivGrid World Manager", typeof(TileManager), typeof(ResourceManager), typeof(ImprovementManager), typeof(WorldManager));
        }
		#endregion
#endif


        /// <summary>
        /// Get the surronding pixels of the referenced pixel location.
        /// </summary>
        /// <param name="tex">Texture where the pixel is located</param>
        /// <param name="x">"X" cords of the pixel</param>
        /// <param name="y">"Y" cords of the pixel</param>
        /// <returns>The eight surronding pixels</returns>
        /// <example>
        /// The following code retrieves the pixels surrounding the given pixel.
        /// <code>
        /// class GetPixels : MonoBehaviour
        /// {
        ///    public Texture2D texture;
        ///    public Vector2 pixelLocation;
        ///
        ///    float[] surroundingPixels;
        ///
        ///    void Start()
        ///    {
        ///       surroundingPixels = CivGridUtility.GetSurrondingPixels(texture, (int)pixelLocation.x, (int)pixelLocation.y);
        ///
        ///        for (int i = 0; i &lt; surroundingPixels.Length; i++)
        ///        {
        ///            Debug.Log("Pixel " + i + ": " + surroundingPixels[i]);
        ///        }
        ///    }
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// This method only returns the <b>r</b> channel of the pixel. It does not return the full Color struct. This method is
        /// written for internal use, and a more general method that returns a Color struct can easily be constructed by viewing
        /// the source for this method.
        /// </remarks>
        public static float[] GetSurrondingPixels(Texture2D tex, int x, int y)
        {
            //array of the surronding pixel values
            float[] returnArray = new float[8];


            if (x == tex.width - 1)
            {
                returnArray[0] = tex.GetPixel(0, y).r;
                returnArray[1] = tex.GetPixel(0, y + 1).r;
                returnArray[2] = tex.GetPixel(x, y + 1).r;
                returnArray[3] = tex.GetPixel(x - 1, y + 1).r;
                returnArray[4] = tex.GetPixel(x - 1, y).r;
                returnArray[5] = tex.GetPixel(x - 1, y - 1).r;
                returnArray[6] = tex.GetPixel(x, y - 1).r;
                returnArray[7] = tex.GetPixel(0, x - 1).r;
            }
            else if (x == 0)
            {
                returnArray[0] = tex.GetPixel(x + 1, y).r;
                returnArray[1] = tex.GetPixel(x + 1, y + 1).r;
                returnArray[2] = tex.GetPixel(x, y + 1).r;
                returnArray[3] = tex.GetPixel(tex.width-1, y + 1).r;
                returnArray[4] = tex.GetPixel(tex.width-1, y).r;
                returnArray[5] = tex.GetPixel(tex.width-1, y - 1).r;
                returnArray[6] = tex.GetPixel(x, y - 1).r;
                returnArray[7] = tex.GetPixel(x + 1, x - 1).r;
            }
            else if (y == tex.height - 1)
            {
                returnArray[0] = tex.GetPixel(x + 1, y).r;
                returnArray[1] = tex.GetPixel(x + 1, 0).r;
                returnArray[2] = tex.GetPixel(x, 0).r;
                returnArray[3] = tex.GetPixel(x - 1, 0).r;
                returnArray[4] = tex.GetPixel(x - 1, y).r;
                returnArray[5] = tex.GetPixel(x - 1, y - 1).r;
                returnArray[6] = tex.GetPixel(x, y - 1).r;
                returnArray[7] = tex.GetPixel(x + 1, x - 1).r;
            }
            else if (y == 0)
            {
                returnArray[0] = tex.GetPixel(x + 1, y).r;
                returnArray[1] = tex.GetPixel(x + 1, y + 1).r;
                returnArray[2] = tex.GetPixel(x, y + 1).r;
                returnArray[3] = tex.GetPixel(x - 1, y + 1).r;
                returnArray[4] = tex.GetPixel(x - 1, y).r;
                returnArray[5] = tex.GetPixel(x - 1, tex.height-1).r;
                returnArray[6] = tex.GetPixel(x, y - 1).r;
                returnArray[7] = tex.GetPixel(x + 1, x - 1).r;
            }
            else if (x == tex.width - 1 && y == tex.height - 1)
            {
                returnArray[0] = tex.GetPixel(0, y).r;
                returnArray[1] = tex.GetPixel(0, 0).r;
                returnArray[2] = tex.GetPixel(x, 0).r;
                returnArray[3] = tex.GetPixel(x - 1, 0).r;
                returnArray[4] = tex.GetPixel(x - 1, y).r;
                returnArray[5] = tex.GetPixel(x - 1, y - 1).r;
                returnArray[6] = tex.GetPixel(x, y - 1).r;
                returnArray[7] = tex.GetPixel(0, x - 1).r;
            }
            else if (x == tex.width - 1 && y == 0)
            {
                returnArray[0] = tex.GetPixel(0, y).r;
                returnArray[1] = tex.GetPixel(0, y + 1).r;
                returnArray[2] = tex.GetPixel(x, y + 1).r;
                returnArray[3] = tex.GetPixel(x - 1, y + 1).r;
                returnArray[4] = tex.GetPixel(x - 1, y).r;
                returnArray[5] = tex.GetPixel(x - 1, tex.height-1).r;
                returnArray[6] = tex.GetPixel(x, tex.height-1).r;
                returnArray[7] = tex.GetPixel(0, x - 1).r;
            }
            else if (x == 0 && y == tex.height - 1)
            {
                returnArray[0] = tex.GetPixel(x + 1, y).r;
                returnArray[1] = tex.GetPixel(x + 1, 0).r;
                returnArray[2] = tex.GetPixel(x, y + 1).r;
                returnArray[3] = tex.GetPixel(tex.width-1, 0).r;
                returnArray[4] = tex.GetPixel(tex.width-1, y).r;
                returnArray[5] = tex.GetPixel(tex.width-1, y - 1).r;
                returnArray[6] = tex.GetPixel(x, y - 1).r;
                returnArray[7] = tex.GetPixel(x + 1, x - 1).r;
            }
            else if (x == 0 && y == 0)
            {
                returnArray[0] = tex.GetPixel(x + 1, y).r;
                returnArray[1] = tex.GetPixel(x + 1, y + 1).r;
                returnArray[2] = tex.GetPixel(x, y + 1).r;
                returnArray[3] = tex.GetPixel(tex.width - 1, y + 1).r;
                returnArray[4] = tex.GetPixel(tex.width - 1, y).r;
                returnArray[5] = tex.GetPixel(tex.width - 1, tex.height - 1).r;
                returnArray[6] = tex.GetPixel(x, tex.height - 1).r;
                returnArray[7] = tex.GetPixel(x + 1, x - 1).r;
            }
            else
            {
                returnArray[0] = tex.GetPixel(x + 1, y).r;
                returnArray[1] = tex.GetPixel(x + 1, y + 1).r;
                returnArray[2] = tex.GetPixel(x, y + 1).r;
                returnArray[3] = tex.GetPixel(x - 1, y + 1).r;
                returnArray[4] = tex.GetPixel(x - 1, y).r;
                returnArray[5] = tex.GetPixel(x - 1, y - 1).r;
                returnArray[6] = tex.GetPixel(x, y - 1).r;
                returnArray[7] = tex.GetPixel(x + 1, x - 1).r;
            }

            //return array of surronding pixel values
            return returnArray;
        }



        /// <summary>
        /// Converts a two-dimensional array into a single array
        /// </summary>
        /// <param name="doubleArray">The two-dimensional array of type T to convert into a single array</param>
        /// <param name="singleArray">The converted array</param>
        /// <typeparam name="T">The type of the arrays</typeparam>
        /// <example>
        /// <code>
        /// class ArraySizing : MonoBehaviour
        /// {
        ///    float[,] randomValues2D = new float[,] { { 0, 1 }, { 2, 3 }, { 4, 5 } };
        ///   float[] randomValues1D;
        ///
        ///    void Start()
        ///    {
        ///        CivGridUtility.ToSingleArray&lt;float&gt;(randomValues2D, out randomValues1D);
        ///    }
        ///    //Output:
        ///    //randomValues1D = { 0, 1, 2, 3, 4, 5 };
        /// }
        /// </code>
        /// </example>
        public static void ToSingleArray<T>(T[,] doubleArray, out T[] singleArray)
        {
            //list to copy the values from the two-dimensional array into
            List<T> combineList = new List<T>();

            //cycle through all the members and copy them into the List
            foreach (T combine in doubleArray)
            {
                combineList.Add(combine);
            }

            //convert our List into a single array
            singleArray = combineList.ToArray();
        }


        /// <summary>
        /// Converts a two-dimensional array into a single array
        /// </summary>
        /// <param name="doubleArray">The two-dimensional array of CombineInstance to convert into a single array</param>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <returns>The converted array</returns>
        /// <example>
        /// <code>
        /// class ArraySizing : MonoBehaviour
        /// {
        ///   float[,] randomValues2D = new float[,] { { 0, 1 }, { 2, 3 }, { 4, 5 } };
        ///   float[] randomValues1D;
        ///
        ///    void Start()
        ///    {
        ///        randomValues1D = CivGridUtility.ToSingleArray&lt;float&gt;(randomValues2D);
        ///    }
        ///    //Output:
        ///    //randomValues1D = { 0, 1, 2, 3, 4, 5 };
        /// }
        /// </code>
        /// </example>
        public static T[] ToSingleArray<T>(T[,] doubleArray)
        {
            //list to copy the values from the two-dimensional array into
            List<T> combineList = new List<T>();

            //cycle through all the members and copy them into the List
            foreach (T combine in doubleArray)
            {
                combineList.Add(combine);
            }

            //convert our List into a single array
            return (combineList.ToArray());
        }

        /// <summary>
        /// Upsizes or downsizes an array to a new size.
        /// </summary>
        /// <typeparam name="T">Type of object in the array</typeparam>
        /// <param name="original">The orginal 2D array to resize</param>
        /// <param name="rows">The number of rows that should be in the new array</param>
        /// <param name="cols">The number of columns that should be in the new array</param>
        /// <returns>A new resized array holding all of the original values</returns>
        /// <example>
        /// The following code take a two-dimensional array and enlargens it to hold more, while keeping
        /// the orignal values intact.
        /// <code>
        /// class ArraySizing : MonoBehaviour
        /// {
        ///   float[,] randomValues2D = new float[3,2] { { 0, 1 }, { 2, 3 }, { 4, 5 } };
        ///
        ///    void Start()
        ///    {
        ///        Debug.Log("Array Size Before: " + randomValues2D.Length);
        ///        randomValues2D = CivGridUtility.Resize2DArray&lt;float&gt;(randomValues2D, 5, 2);
        ///        Debug.Log("Array Size After: " + randomValues2D.Length);
        ///    }
        ///    //Output:
        ///    //randomValues2D = { { 0, 1 }, { 2, 3 }, { 4, 5 }, { 0, 0 }, { 0, 0 } };
        ///    //Array Size Before: 6
        ///    //Array Size After: 10
        /// }
        /// </code>
        /// </example>
        public static T[,] Resize2DArray<T>(T[,] original, int rows, int cols)
        {
            //create the new array at the new size
            T[,] newArray = new T[rows, cols];

            //calculate the number of values going into the new array
            int minRows = Math.Min(rows, original.GetLength(0));
            int minCols = Math.Min(cols, original.GetLength(1));

            //fills the new array with the old values
            for (int i = 0; i < minRows; i++)
            {
                for (int j = 0; j < minCols; j++)
                {
                    newArray[i, j] = original[i, j];
                }
            }

            //return the resized array
            return newArray;
        }
    }

    public static class MeshUtility
    {
    
     /// <summary>
     /// Builds an array of edges that connect to only one triangle.
     /// In other words, the outline of the mesh
     /// </summary>
     /// <param name="mesh">Mesh to outline</param>
     /// <returns>Array of <see cref="Edge"/></returns>
   
        public static Edge[] BuildManifoldEdges(Mesh mesh)
        {
            // Build a edge list for all unique edges in the mesh
            Edge[] edges = BuildEdges(mesh.vertexCount, mesh.triangles);

            // We only want edges that connect to a single triangle
            List<Edge> culledEdges = new List<Edge>();
            foreach (Edge edge in edges)
            {
                if (edge.faceIndex[0] == edge.faceIndex[1])
                {
                    edge.edgeLocation = DetermineEdgeDirection(edge);
                    culledEdges.Add(edge);
                }
            }

            return culledEdges.ToArray();
        }

        private static EdgeLocation DetermineEdgeDirection(Edge edge)
        {
            WorldManager worldManager = GameObject.FindObjectOfType<WorldManager>();
            Vector3[] verts = worldManager.flatHexagonSharedMesh.vertices;

            ///
            /// Points
            ///

            //extreme top
            if (verts[edge.vertexIndex[0]].z > 0.98f || verts[edge.vertexIndex[1]].z > 0.98f)
            {
                //Debug.Log("Top; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                return EdgeLocation.Top;
            }
            //extreme bottom
            if (verts[edge.vertexIndex[0]].z < -0.98f || verts[edge.vertexIndex[1]].z < -0.98f)
            {
                //Debug.Log("Bottom; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                return EdgeLocation.Bottom;
            }
            //bottom
            if (Mathf.Approximately(verts[edge.vertexIndex[0]].z,-0.5f) || Mathf.Approximately(verts[edge.vertexIndex[1]].z,-0.5f))
            {
                //left corner
                if(verts[edge.vertexIndex[0]].x <= -.85f || verts[edge.vertexIndex[1]].x <= -.85f)
                {
                    //Debug.Log("Bottom Left Corner; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                    return EdgeLocation.BottomLeftCorner;
                }
                //right corner
                if (verts[edge.vertexIndex[0]].x >= .85f || verts[edge.vertexIndex[1]].x >= .85f)
                {
                    //Debug.Log("Bottom Right Corner; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                    return EdgeLocation.BottomRightCorner;
                }
            }
            //top
            if (Mathf.Approximately(verts[edge.vertexIndex[0]].z, 0.5f) || Mathf.Approximately(verts[edge.vertexIndex[1]].z, 0.5f))
            {
                //left corner
                if (verts[edge.vertexIndex[0]].x <= -.85f || verts[edge.vertexIndex[1]].x <= -.85f)
                {
                    //Debug.Log("Top Left Corner; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                    return EdgeLocation.TopLeftCorner;
                }
                //right corner
                if (verts[edge.vertexIndex[0]].x >= .85f || verts[edge.vertexIndex[1]].x >= .85f)
                {
                    //Debug.Log("Top Right Corner; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                    return EdgeLocation.TopRightCorner;
                }
            }

            ///
            /// Sides
            ///

            //bottom
            if (verts[edge.vertexIndex[0]].z < -0.5f || verts[edge.vertexIndex[1]].z < -0.5f)
            {
                if (verts[edge.vertexIndex[0]].x < 0 || verts[edge.vertexIndex[1]].x < 0)
                {
                    //Debug.Log("Bottom Left; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                    return EdgeLocation.BottomLeft;
                }
                else
                {
                    //Debug.Log("Bottom Right; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]])); 
                    return EdgeLocation.BottomRight; }
            }
            //top
            if (verts[edge.vertexIndex[0]].z > 0.5f || verts[edge.vertexIndex[1]].z > 0.5f)
            {
                if (verts[edge.vertexIndex[0]].x < 0 || verts[edge.vertexIndex[1]].x < 0)
                {
                    //Debug.Log("Top Left; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                    return EdgeLocation.TopLeft;
                }
                else
                {
                    //Debug.Log("Top Right; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]])); 
                    return EdgeLocation.TopRight; }
            }
            //middle
            else
            {
                if (verts[edge.vertexIndex[0]].x < 0.5f || verts[edge.vertexIndex[1]].x < 0.5f)
                {
                    //Debug.Log("Left; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]]));
                    return EdgeLocation.Left;
                }
                else
                {
                    //Debug.Log("Right; " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[0]]) + " " + Camera.main.transform.TransformPoint(worldManager.flatHexagonSharedMesh.vertices[edge.vertexIndex[1]])); 
                    return EdgeLocation.Right; }
            }
        }

        public static int[] FindEdgeVertices(Mesh mesh, out Edge[] edges)
        {
            List<int> edgeVertices = new List<int>();
            edges = BuildManifoldEdges(mesh);

            foreach(Edge edge in edges)
            {
                if (!edgeVertices.Contains(edge.vertexIndex[0])) { edgeVertices.Add(edge.vertexIndex[0]); }
                if (!edgeVertices.Contains(edge.vertexIndex[1])) { edgeVertices.Add(edge.vertexIndex[1]); }
            }

            return edgeVertices.ToArray();
        }

        /// Builds an array of unique edges
        /// This requires that your mesh has all vertices welded. However on import, Unity has to split
        /// vertices at uv seams and normal seams. Thus for a mesh with seams in your mesh you
        /// will get two edges adjoining one triangle.
        /// Often this is not a problem but you can fix it by welding vertices
        /// and passing in the triangle array of the welded vertices.
        private static Edge[] BuildEdges(int vertexCount, int[] triangleArray)
        {
            int maxEdgeCount = triangleArray.Length;
            int[] firstEdge = new int[vertexCount + maxEdgeCount];
            int nextEdge = vertexCount;
            int triangleCount = triangleArray.Length / 3;

            for (int a = 0; a < vertexCount; a++)
                firstEdge[a] = -1;

            // First pass over all triangles. This finds all the edges satisfying the
            // condition that the first vertex index is less than the second vertex index
            // when the direction from the first vertex to the second vertex represents
            // a counterclockwise winding around the triangle to which the edge belongs.
            // For each edge found, the edge index is stored in a linked list of edges
            // belonging to the lower-numbered vertex index i. This allows us to quickly
            // find an edge in the second pass whose higher-numbered vertex index is i.
            Edge[] edgeArray = new Edge[maxEdgeCount];

            int edgeCount = 0;
            for (int a = 0; a < triangleCount; a++)
            {
                int i1 = triangleArray[a * 3 + 2];
                for (int b = 0; b < 3; b++)
                {
                    int i2 = triangleArray[a * 3 + b];
                    if (i1 < i2)
                    {
                        Edge newEdge = new Edge();
                        newEdge.vertexIndex[0] = i1;
                        newEdge.vertexIndex[1] = i2;
                        newEdge.faceIndex[0] = a;
                        newEdge.faceIndex[1] = a;
                        edgeArray[edgeCount] = newEdge;

                        int edgeIndex = firstEdge[i1];
                        if (edgeIndex == -1)
                        {
                            firstEdge[i1] = edgeCount;
                        }
                        else
                        {
                            while (true)
                            {
                                int index = firstEdge[nextEdge + edgeIndex];
                                if (index == -1)
                                {
                                    firstEdge[nextEdge + edgeIndex] = edgeCount;
                                    break;
                                }

                                edgeIndex = index;
                            }
                        }

                        firstEdge[nextEdge + edgeCount] = -1;
                        edgeCount++;
                    }

                    i1 = i2;
                }
            }

            // Second pass over all triangles. This finds all the edges satisfying the
            // condition that the first vertex index is greater than the second vertex index
            // when the direction from the first vertex to the second vertex represents
            // a counterclockwise winding around the triangle to which the edge belongs.
            // For each of these edges, the same edge should have already been found in
            // the first pass for a different triangle. Of course we might have edges with only one triangle
            // in that case we just add the edge here
            // So we search the list of edges
            // for the higher-numbered vertex index for the matching edge and fill in the
            // second triangle index. The maximum number of comparisons in this search for
            // any vertex is the number of edges having that vertex as an endpoint.

            for (int a = 0; a < triangleCount; a++)
            {
                int i1 = triangleArray[a * 3 + 2];
                for (int b = 0; b < 3; b++)
                {
                    int i2 = triangleArray[a * 3 + b];
                    if (i1 > i2)
                    {
                        bool foundEdge = false;
                        for (int edgeIndex = firstEdge[i2]; edgeIndex != -1; edgeIndex = firstEdge[nextEdge + edgeIndex])
                        {
                            Edge edge = edgeArray[edgeIndex];
                            if ((edge.vertexIndex[1] == i1) && (edge.faceIndex[0] == edge.faceIndex[1]))
                            {
                                edgeArray[edgeIndex].faceIndex[1] = a;
                                foundEdge = true;
                                break;
                            }
                        }

                        if (!foundEdge)
                        {
                            Edge newEdge = new Edge();
                            newEdge.vertexIndex[0] = i1;
                            newEdge.vertexIndex[1] = i2;
                            newEdge.faceIndex[0] = a;
                            newEdge.faceIndex[1] = a;
                            edgeArray[edgeCount] = newEdge;
                            edgeCount++;
                        }
                    }

                    i1 = i2;
                }
            }

            Edge[] compactedEdges = new Edge[edgeCount];
            for (int e = 0; e < edgeCount; e++)
                compactedEdges[e] = edgeArray[e];

            return compactedEdges;
        }
    }


    internal enum EdgeLocation
    {
        //exacts
        Top,
        Bottom,
        TopRightCorner,
        TopLeftCorner,
        BottomRightCorner,
        BottomLeftCorner,


        //sides
        BottomLeft,
        BottomRight,
        Left,
        Right,
        TopLeft,
        TopRight
    }
    public class Edge
    {
        // The index to each vertex
        public int[] vertexIndex = new int[2];
        internal EdgeLocation edgeLocation;
        internal Hex adjacentHex;
        // The index into the face.
        // (faceindex[0] == faceindex[1] means the edge connects to only one triangle)
        internal int[] faceIndex = new int[2];
    }

    /// <summary>
    /// Extends arrays of TileItem, ResourceItem, and ImprovementItem to act similar to a dictionary.
    /// Extends strings to cast to enums.
    /// </summary>
    public static class ExtensionMethods
    {

        #region TryGetValue

        /// <summary>
        /// Attempts to return the texture atlas location of this Improvement from the array based on a key.
        /// </summary>
        /// <param name="list">The array to get the value from</param>
        /// <param name="key">The key to search for a match within the array</param>
        /// <param name="location">A out reference of the Rect location of this Tile within the texture atlas</param>
        /// <returns>If the key was found in the array</returns>
        public static bool TryGetValue(this TileItem[] list, Tile key, out Rect location)
        {
            //list to hold each possible return value
            List<Rect> tempList = new List<Rect>();

            //add each possible return value to the tempList
            foreach (TileItem item in list)
            {
                if (item.Key.name == key.name)
                {
                    tempList.Add(item.Value);
                }
            }

            //if we have more than one that is returnable
            if (tempList.Count > 1)
            {
                //select one at random and return it
                int index = UnityEngine.Random.Range(0, tempList.Count);

                location = tempList[index];
                return true;
            }
            //only have one that is returnable; return that one
            else if (tempList.Count == 1)
            {
                location = tempList[0];
                return true;
            }
            //couldn't find the key
            else
            {
                //not found
                Debug.LogError("Couldn't get a value from the given key: " + key.name + " setting to default : " + list[0].Key.name + "/n Add this to the texture atlas!");
                location = list[0].Value;
                return false;
            }
        }

        /// <summary>
        /// Attempts to return the texture atlas location of this Improvement from the array based on a key.
        /// </summary>
        /// <param name="list">The array to get the value from</param>
        /// <param name="key">The key to search for a match within the array</param>
        /// <param name="location">A out reference of the Rect location of this Resource within the texture atlas</param>
        /// <returns>If the key was found in the array</returns>
        public static bool TryGetValue(this ResourceItem[] list, Resource key, out Rect location)
        {
            //list to hold each possible return value
            List<Rect> tempList = new List<Rect>();

            //add each possible return value to the tempList
            foreach (ResourceItem item in list)
            {
                if (item.Key.name == key.name)
                {
                    tempList.Add(item.Value);
                }
            }

            //if we have more than one that is returnable
            if (tempList.Count > 1)
            {
                //select one at random and return it
                int index = UnityEngine.Random.Range(0, tempList.Count);

                location = tempList[index];
                return true;
            }
            //only have one that is returnable; return that one
            else if (tempList.Count == 1)
            {
                location = tempList[0];
                return true;
            }
            //couldn't find the key
            else
            {
                //not found
                Debug.LogError("Couldn't get a value from the given key: " + key.name);
                location = new Rect();
                return false;
            }
        }

        /// <summary>
        /// Attempts to return the texture atlas location of this Improvement from the array based on a key.
        /// </summary>
        /// <param name="list">The array to get the value from</param>
        /// <param name="key">The key to search for a match within the array</param>
        /// <param name="location">A out reference of the Rect location of this Improvement within the texture atlas</param>
        /// <returns>If the key was found in the array</returns>
        public static bool TryGetValue(this ImprovementItem[] list, Improvement key, out Rect location)
        {
            //list to hold each possible return value
            List<Rect> tempList = new List<Rect>();

            //add each possible return value to the tempList
            foreach (ImprovementItem item in list)
            {
                if (item.Key.name == key.name)
                {
                    tempList.Add(item.Value);
                }
            }

            //if we have more than one that is returnable
            if (tempList.Count > 1)
            {
                //select one at random and return it
                int index = UnityEngine.Random.Range(0, tempList.Count);

                location = tempList[index];
                return true;
            }
            //only have one that is returnable; return that one
            else if (tempList.Count == 1)
            {
                location = tempList[0];
                return true;
            }
            //couldn't find the key
            else
            {
                //not found
                Debug.LogError("Couldn't get a value from the given key: " + key.name);
                location = new Rect();
                return false;
            }
        }

        #endregion

        #region ContainsKey

        /// <summary>
        /// Checks if a key exists in the list.
        /// </summary>
        /// <param name="list">The list to check if the key exists within</param>
        /// <param name="key">The key to look for within this array</param>
        /// <returns>If the key was found</returns>
        public static bool ContainsKey(this List<TileItem> list, Tile key)
        {
            //cycle through each key in the array and return true if we find one matching the supplied key; otherwise return false
            foreach (TileItem item in list)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Checks if a key exists in the array.
        /// </summary>
        /// <param name="list">The list to check if the key exists within</param>
        /// <param name="key">The key to look for within this array</param>
        /// <returns>If the key was found</returns>
        public static bool ContainsKey(this TileItem[] list, Tile key)
        {
            //cycle through each key in the array and return true if we find one matching the supplied key; otherwise return false
            foreach (TileItem item in list)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a key exists in the array.
        /// </summary>
        /// <param name="list">The list to check if the key exists within</param>
        /// <param name="key">The key to look for within this array</param>
        /// <returns>If the key was found</returns>
        public static bool ContainsKey(this List<ResourceItem> list, Resource key)
        {
            //cycle through each key in the array and return true if we find one matching the supplied key; otherwise return false
            foreach (ResourceItem item in list)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a key exists in the array.
        /// </summary>
        /// <param name="list">The list to check if the key exists within</param>
        /// <param name="key">The key to look for within this array</param>
        /// <returns>If the key was found</returns>
        public static bool ContainsKey(this ResourceItem[] list, Resource key)
        {
            //cycle through each key in the array and return true if we find one matching the supplied key; otherwise return false
            foreach (ResourceItem item in list)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a key exists in the array.
        /// </summary>
        /// <param name="list">The list to check if the key exists within</param>
        /// <param name="key">The key to look for within this array</param>
        /// <returns>If the key was found</returns>
        public static bool ContainsKey(this List<ImprovementItem> list, Improvement key)
        {
            //cycle through each key in the array and return true if we find one matching the supplied key; otherwise return false
            foreach (ImprovementItem item in list)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a key exists in the array.
        /// </summary>
        /// <param name="list">The list to check if the key exists within</param>
        /// <param name="key">The key to look for within this array</param>
        /// <returns>If the key was found</returns>
        public static bool ContainsKey(this ImprovementItem[] list, Improvement key)
        {
            //cycle through each key in the array and return true if we find one matching the supplied key; otherwise return false
            foreach (ImprovementItem item in list)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Add

        /// <summary>
        /// Adds a new entry of a key and matching value to this array.
        /// </summary>
        /// <param name="list">List to add the tile to</param>
        /// <param name="tileToAdd">Tile to add as the key</param>
        /// <param name="rectToAdd">Rect to add as the value</param>
        public static void Add(this List<TileItem> list, Tile tileToAdd, Rect rectToAdd)
        {
            list.Add(new TileItem(tileToAdd, rectToAdd));
        }

        /// <summary>
        /// Adds a new entry of a key and matching value to this array.
        /// </summary>
        /// <param name="list">List to add the resource to</param>
        /// <param name="resourceToAdd">Resource to add as the key</param>
        /// <param name="rectToAdd">Rect to add as the value</param>
        public static void Add(this List<ResourceItem> list, Resource resourceToAdd, Rect rectToAdd)
        {
            list.Add(new ResourceItem(resourceToAdd, rectToAdd));
        }

        /// <summary>
        /// Adds a new entry of a key and matching value to this array.
        /// </summary>
        /// <param name="list">List to add the improvement to</param>
        /// <param name="improvementToAdd">Improvement to add as the key</param>
        /// <param name="rectToAdd">Rect to add as the value</param>s
        public static void Add(this List<ImprovementItem> list, Improvement improvementToAdd, Rect rectToAdd)
        {
            list.Add(new ImprovementItem(improvementToAdd, rectToAdd));
        }

        #endregion

        /// <summary>
        /// Converts a base string into the enum type given.
        /// </summary>
        /// <typeparam name="T">Enum to cast into</typeparam>
        /// <param name="value">base string to cast</param>
        /// <returns>The enum value from the given string</returns>
        public static T ConvertToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static void RemoveEmptyStrings(this string[] value)
        {
            List<string> result = new List<string>();

            for(int i = 0; i < value.Length; i++)
            {
                if(value[i].Equals(""))
                {
                    result.Add(value[i]);
                }
            }

            value = result.ToArray();
        }

        public static List<T> ToList<T>(this T[] value)
        {
            List<T> list = new List<T>();
            foreach(T v in value)
            {
                list.Add(v);
            }
            return list;
        }

        public static Vector4[] ToVector4(this Vector2[] input, float z, float w)
        {
            List<Vector4> output = new List<Vector4>();

            foreach (Vector2 v in input)
            {
                output.Add(new Vector4(v.x, v.y, z, w));
            }

            return output.ToArray();
        }
    }

}
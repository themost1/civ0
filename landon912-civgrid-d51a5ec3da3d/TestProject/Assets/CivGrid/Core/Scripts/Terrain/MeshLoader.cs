using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CivGrid;

namespace CivGrid
{
    public static class MeshLoader
    {
        public static Mesh LoadMesh(string filepath)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> tex = new List<Vector2>();

            string[] meshFileLines = File.ReadAllLines(filepath);

            for(int i = 0; i < meshFileLines.Length; i++)
            {
                string[] tokens = meshFileLines[i].Split(' ');
                tokens.RemoveEmptyStrings();

                if (tokens.Length == 0 || tokens[0] == "#") { continue; }
                else if (tokens[0] == "v")
                {
                    vertices.Add(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
                }
                else if (tokens[0] == "vt")
                {
                    tex.Add(new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2])));
                }
                else if (tokens[0] == "f")
                {
                    triangles.Add(int.Parse((tokens[1].Split('/')[0])) - 1);
                    triangles.Add(int.Parse((tokens[2].Split('/')[0])) - 1);
                    triangles.Add(int.Parse((tokens[3].Split('/')[0])) - 1);

                    if (tokens.Length > 4)
                    {
                        triangles.Add(int.Parse((tokens[1].Split('/')[0])) - 1);
                        triangles.Add(int.Parse((tokens[3].Split('/')[0])) - 1);
                        triangles.Add(int.Parse((tokens[4].Split('/')[0])) - 1);
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = tex.ToArray();

            return mesh;
        }
    }
}

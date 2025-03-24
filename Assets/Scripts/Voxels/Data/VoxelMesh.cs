using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    public struct VoxelMesh
    {
        public Mesh mesh;
        public List<Vector3> vertices;
        public List<int> triangles;
        public List<Vector2> UVs;
        public List<Vector2> UVs2;
        public List<Color> colors;
        public bool Initialized;

        public void ClearData()
        {
            if (!Initialized)
            {
                vertices = new List<Vector3>();
                triangles = new List<int>();
                UVs = new List<Vector2>();
                UVs2 = new List<Vector2>();
                colors = new List<Color>();

                Initialized = true;
                mesh = new Mesh();
            }
            else
            {
                vertices.Clear();
                triangles.Clear();
                UVs.Clear();
                UVs2.Clear();
                colors.Clear();

                mesh.Clear();
            }
        }
        public void UploadMesh(bool sharedVertices = false)
        {
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0, false);
            mesh.SetColors(colors);

            mesh.SetUVs(0, UVs);
            mesh.SetUVs(2, UVs2);

            mesh.Optimize();

            mesh.RecalculateNormals();

            mesh.RecalculateBounds();

            mesh.UploadMeshData(false);
        }
    }
}

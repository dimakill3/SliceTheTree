using System.Collections.Generic;
using UnityEngine;

namespace Voxels
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    public class Chunk : MonoBehaviour
    {
        static readonly Vector3[] voxelVertices = new Vector3[8]
        {
            new Vector3(0,0,0),//0
            new Vector3(1,0,0),//1
            new Vector3(0,1,0),//2
            new Vector3(1,1,0),//3

            new Vector3(0,0,1),//4
            new Vector3(1,0,1),//5
            new Vector3(0,1,1),//6
            new Vector3(1,1,1),//7
        };
        static readonly Vector3[] voxelFaceChecks = new Vector3[6]
        {
            new Vector3(0,0,-1),//back
            new Vector3(0,0,1),//front
            new Vector3(-1,0,0),//left
            new Vector3(1,0,0),//right
            new Vector3(0,-1,0),//bottom
            new Vector3(0,1,0)//top
        };
        static readonly int[,] voxelVertexIndex = new int[6, 4]
        {
            {0,1,2,3},
            {4,5,6,7},
            {4,0,6,2},
            {5,1,7,3},
            {0,1,4,5},
            {2,3,6,7},
        };
        static readonly Vector2[] voxelUVs = new Vector2[4]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,0),
            new Vector2(1,1)
        };
        static readonly int[,] voxelTris = new int[6, 6]
        {
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
            {0,1,2,1,3,2},
            {0,1,2,1,3,2},
            {0,2,3,0,3,1},
        };
        static readonly Voxel emptyVoxel = new Voxel() { Id = 0 };
    
        public Vector3 chunkPosition;

        private Dictionary<Vector3, Voxel> data;
        private VoxelMesh meshData;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        public void Initialize(Material mat, Vector3 position)
        {
            ConfigureComponents();
            data = new Dictionary<Vector3, Voxel>();
            meshRenderer.sharedMaterial = mat;
            chunkPosition = position;
        }

        public void ClearData()
        {
            data.Clear();
        }

        public void GenerateMesh()
        {
            meshData.ClearData();

            Vector3 blockPos;
            Voxel block;

            int counter = 0;
            Vector3[] faceVertices = new Vector3[4];
            Vector2[] faceUVs = new Vector2[4];

            VoxelColor voxelColor;
            Color voxelColorAlpha;
            Vector2 voxelSmoothness;

            foreach (KeyValuePair<Vector3, Voxel> kvp in data)
            {
                //Only check on solid blocks
                if (!kvp.Value.IsSolid)
                    continue;

                blockPos = kvp.Key;
                block = kvp.Value;

                voxelColor = WorldManager.Instance.WorldColors[block.Id - 1];
                voxelColorAlpha = voxelColor.color;
                voxelColorAlpha.a = 1;
                voxelSmoothness = new Vector2(voxelColor.metallic, voxelColor.smoothness);
                //Iterate over each face direction
                for (int i = 0; i < 6; i++)
                {
                    //Check if there's a solid block against this face
                    if (this[blockPos + voxelFaceChecks[i]].IsSolid)
                        continue;

                    //Draw this face

                    //Collect the appropriate vertices from the default vertices and add the block position
                    for (int j = 0; j < 4; j++)
                    {
                        faceVertices[j] = voxelVertices[voxelVertexIndex[i, j]] + blockPos;
                        faceUVs[j] = voxelUVs[j];
                    }

                    for (int j = 0; j < 6; j++)
                    {
                        meshData.vertices.Add(faceVertices[voxelTris[i, j]]);
                        meshData.UVs.Add(faceUVs[voxelTris[i, j]]);
                        meshData.colors.Add(voxelColorAlpha);
                        meshData.UVs2.Add(voxelSmoothness);

                        meshData.triangles.Add(counter++);

                    }
                }

            }
        }

        public void UploadMesh()
        {
            meshData.UploadMesh();

            if (meshRenderer == null)
                ConfigureComponents();

            meshFilter.mesh = meshData.mesh;
            if (meshData.vertices.Count > 3)
                meshCollider.sharedMesh = meshData.mesh;
        }

        private void ConfigureComponents()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
        }

        public Voxel this[Vector3 index]
        {
            get
            {
                if (data.ContainsKey(index))
                    return data[index];
                else
                    return emptyVoxel;
            }

            set
            {
                if (data.ContainsKey(index))
                    data[index] = value;
                else
                    data.Add(index, value);
            }
        }
    }
}

using UnityEngine;

namespace Voxels.Generators
{
    public class TreeGenerator
    {
        [SerializeField] private Material treeMaterial;
        [SerializeField] private VoxelColor[] treeColors;
        [SerializeField] private Chunk emptyTreePrefab;

        public Chunk GenerateTree(Transform parent, Vector3 position = new())
        {
            var chunk = GameObject.Instantiate(emptyTreePrefab, parent);
            chunk.Initialize(treeMaterial, position);

            //Tree Generation Algo
            for (int x = 0; x < 16; x++)
                for (int z = 0; z < 16; z++)
                {
                    int randomYHeight = Random.Range(1, 16);
                    for (int y = 0; y < randomYHeight; y++)
                        chunk[new Vector3(x, y, z)] = new Voxel() { Id = 1 };
                }

            chunk.GenerateMesh();
            chunk.UploadMesh();

            return chunk;
        }
    }
}
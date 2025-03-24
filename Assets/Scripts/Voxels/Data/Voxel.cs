namespace Voxels
{
    public struct Voxel
    {
        public byte Id;
    
        public bool IsSolid => Id != 0;
    }
}

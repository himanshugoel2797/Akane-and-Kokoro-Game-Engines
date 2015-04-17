using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.HighLevel.Voxel
{
    public class VoxelCollection
    {
        public Dictionary<uint, VoxelProperties> VoxelIDs;

        public VoxelCollection()
        {
            VoxelIDs = new Dictionary<uint, VoxelProperties>();
        }
    }

    public struct VoxelProperties
    {
        public float Permeability;
        public Texture ColorTexture;
        public Texture NormalTexture;
        public Texture SpecularTexture;
    }
}

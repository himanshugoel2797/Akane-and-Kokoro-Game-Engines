using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.HighLevel.Voxel
{
    public class VoxelData
    {
        public uint[, ,] VoxelIDs;

        public VoxelData()
        {
            VoxelIDs = new uint[128, 128, 128];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.HighLevel.Voxel
{
    public class VoxelTerrain : Model
    {
        #region VoxelTerrain Settings
        public struct Settings
        {
            public bool Caves;
            public bool Mountains;
            public bool Rivers;
            public bool Oceans;
        }
        public Settings TerrainSettings;
        #endregion

        VoxelData[, ,] Chunks;
        public VoxelCollection Voxels;

        public VoxelTerrain(int x, int y, int z)
        {
            Chunks = new VoxelData[x, y, z];
            Voxels = new VoxelCollection();
        }

        //TODO generate terrain by calling custom collection of noise functions based off of how actual terrain is formed
        public void Draw(GraphicsContext context)
        {
            //Get the player position, have the chunks in the Radius been generated, if they haven't then generate them, this should be done on another thread to prevent render stalling
            //We might want to eventually just move it all to the GPU
            //Fill voxel chunks from VoxelTerrain class with third dedicated VertexArray
            //Surface extraction: Extract all without worrying about material, draw each as a square to see how good performance is
        }
    }
}

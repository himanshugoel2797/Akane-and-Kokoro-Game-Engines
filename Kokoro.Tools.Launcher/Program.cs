using Kokoro.Engine;
using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Tools.Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            Voxelizer.Voxelizer voxelizer = new Voxelizer.Voxelizer();
            voxelizer.Voxelize();
            return;
        }
    }
}

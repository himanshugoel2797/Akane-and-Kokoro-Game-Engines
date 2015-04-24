using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;
using Kokoro.Engine;
using Kokoro.Debug;
using Kokoro.Engine.SceneGraph;
using Kokoro.Engine.Prefabs;
using Kokoro.Engine.Shaders;
using Kokoro.Engine.HighLevel.CharacterControllers;
using Kokoro.Engine.HighLevel.Cameras;
using Kokoro.Engine.HighLevel;
using Kokoro.Engine.HighLevel.Rendering;
using Kokoro.Engine.HighLevel.Rendering.Compositor;
using Kokoro.Engine.Input;

namespace Messier104
{
    class SystemManager
    {
        Sphere[] planets;       //Each of these consists of various levels of detail of each kind of mesh
        Sphere[] star;
        Sphere[] moons;
        Vector3 Center;

        Random rng;
        int planetCount;
        int[] moonCount;

        public SystemManager(int seed, Vector3 Center)
        {
            rng = new Random(seed);
            planets = new Sphere[5];

            planetCount = rng.Next(0, 15);
            moonCount = new int[planetCount];
            for (int i = 0; i < planetCount; i++)
            {
                moonCount[i] = rng.Next(0, 5);
            }
        }
    }
}

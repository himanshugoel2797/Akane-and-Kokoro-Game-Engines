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
using Kokoro.Engine.HighLevel.Cameras;

namespace Kokoro.Engine.HighLevel.Rendering
{
    public class LightPass
    {

        FrameBuffer lightBuffer;

        public Texture LightData
        {
            get
            {
                return lightBuffer["Light0"];
            }
        }

        public Texture LightColor
        {
            get
            {
                return lightBuffer["Color0"];
            }
        }

        public LightPass()
        {

        }

    }
}

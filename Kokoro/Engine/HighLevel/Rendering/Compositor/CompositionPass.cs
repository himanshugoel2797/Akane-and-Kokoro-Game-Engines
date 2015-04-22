using Kokoro.Engine.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.HighLevel.Rendering.Compositor
{
    public class CompositionPass
    {
        public List<ICompositorNode> Steps;
        public GBuffer InternalBufferA, InternalBufferB;
        public Model Surface;

        public CompositionPass(int width, int height, GraphicsContext context, Model surf = null)
        {
            Steps = new List<ICompositorNode>();
            InternalBufferA = new GBuffer(width, height, context);
            InternalBufferB = new GBuffer(width, height, context);
            if (surf == null) Surface = new FullScreenQuad();
            else Surface = surf;
        }

        public GBuffer ApplyPass(GraphicsContext context, GBuffer src)
        {
            Surface.Materials[0].ColorMap = src["RGBA0"];
            Surface.Materials[0].Shader["PositionMap"] = src["Depth0"];
            Surface.Materials[0].NormalMap = src["Normal0"];

            var Target = InternalBufferA;
            var Src = InternalBufferB;
            GBuffer tmpVar;

            Target.Bind(context);
            Steps[0].Apply(context, Surface);

            Target = InternalBufferB;
            Src = InternalBufferA;

            for (int i = 1; i < Steps.Count; i++)
            {
                Target.Bind(context);
                Surface.Materials[0].ColorMap = Src["RGBA0"];
                Surface.Materials[0].Shader["PositionMap"] = Src["Depth0"];
                Surface.Materials[0].NormalMap = Src["Normal0"];
                Steps[i].Apply(context, Surface);

                tmpVar = Target;
                Target = Src;
                Src = tmpVar;
            }

            return Src;
        }

    }
}

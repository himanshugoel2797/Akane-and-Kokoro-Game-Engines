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

        public GBuffer ApplyPass(GraphicsContext context, GBuffer input)
        {
            var Target = InternalBufferA;
            var Src = InternalBufferB;
            GBuffer tmpVar;

            Target.Bind(context);
            context.Clear(0, 0, 0, 0);
            Steps[0].Apply(context, Surface, input, input);
            context.ForceDraw();

            Target = InternalBufferB;
            Src = InternalBufferA;

            for (int i = 1; i < Steps.Count; i++)
            {
                Target.Bind(context);
                context.Clear(0, 0, 0, 0);
                Steps[i].Apply(context, Surface, Src, input);
                context.ForceDraw();

                tmpVar = Target;
                Target = Src;
                Src = tmpVar;
            }

            Src.UnBind(context);
            return Src;
        }

    }
}

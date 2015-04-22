using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.HighLevel.Rendering.Compositor
{
    public interface ICompositorNode
    {
        void Apply(GraphicsContext context, Model surface, GBuffer src, GBuffer origSrc);
    }
}

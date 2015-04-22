using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;
using Kokoro.Engine.Shaders;

namespace Kokoro.Engine.HighLevel.Rendering.Compositor
{
    public class HorizontalGaussianBlurNode : ICompositorNode
    {
        ShaderProgram Horizontal;

        public float Radius { get; set; }

        public HorizontalGaussianBlurNode(int taps, float radius)
        {
            Horizontal = Kokoro.ShaderLib.GaussianBlurShader.Create(taps, false);
            Radius = radius;
        }

        public void Apply(GraphicsContext context, Model surface, GBuffer srcBuffer, GBuffer origBuffer)
        {
            srcBuffer["RGBA0"].WrapX = false;
            srcBuffer["RGBA0"].WrapY = false;
            srcBuffer["RGBA0"].FilterMode = TextureFilter.Linear;
            surface.Materials[0].Shader = Horizontal;
            surface.Materials[0].ColorMap = srcBuffer["RGBA0"];
            surface.Materials[0].Shader["KernelRad"] = Radius;
            surface.Draw(context);
        }
    }

    public class VerticalGaussianBlurNode : ICompositorNode
    {
        ShaderProgram Vertical;

        public float Radius { get; set; }

        public VerticalGaussianBlurNode(int taps, float radius)
        {
            Vertical = Kokoro.ShaderLib.GaussianBlurShader.Create(taps, true);
            Radius = radius;
        }

        public void Apply(GraphicsContext context, Model surface, GBuffer srcBuffer, GBuffer origBuffer)
        {
            srcBuffer["RGBA0"].WrapX = false;
            srcBuffer["RGBA0"].WrapY = false;
            srcBuffer["RGBA0"].FilterMode = TextureFilter.Linear;
            surface.Materials[0].Shader = Vertical;
            surface.Materials[0].ColorMap = srcBuffer["RGBA0"];
            surface.Materials[0].Shader["KernelRad"] = Radius * context.AspectRatio;
            surface.Draw(context);
        }
    }
}

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
    public class GBuffer
    {
        FrameBuffer buffer;

        public ShaderProgram GBufferShader;

        public GBuffer(int width, int height, GraphicsContext context)
        {
            buffer = new FrameBuffer(width, height, PixelComponentType.RGBA16f, context);

            //Create the GBuffer texture targets
            buffer.Add("RGBA0", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float), FrameBufferAttachments.ColorAttachment0, context);
            buffer.Add("Depth0", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.UInt1010102), FrameBufferAttachments.ColorAttachment1, context);
            buffer.Add("Normal0", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float), FrameBufferAttachments.ColorAttachment2, context);

            GBufferShader = new ShaderProgram(new Ubershader(new ShaderLib.GBufferShader()));
        }

        public Texture this[string key]
        {
            get
            {
                return buffer[key];
            }
        }

        public void Bind(GraphicsContext context)
        {
            buffer.Bind(context);
            /*context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.One,
                Dst = BlendingFactor.Zero
            };*/

            //buffer.SetBlendFunc(context.Blending, FrameBufferAttachments.ColorAttachment0);
        }

        public void SetBlendFunc(BlendFunc func)
        {
            buffer.SetBlendFunc(func, FrameBufferAttachments.ColorAttachment0);
            buffer.SetBlendFunc(func, FrameBufferAttachments.ColorAttachment1);
            buffer.SetBlendFunc(func, FrameBufferAttachments.ColorAttachment2);
        }

        public void UnBind(GraphicsContext context)
        {
            buffer.UnBind(context);
            //context.Viewport = new Vector4(0, 0, context.WindowSize.X, context.WindowSize.Y);
        }

    }
}

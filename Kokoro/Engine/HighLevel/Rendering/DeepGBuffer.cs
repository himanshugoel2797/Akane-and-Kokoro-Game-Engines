using Kokoro.Engine.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.HighLevel.Rendering
{
    public class DeepGBuffer
    {
        public FrameBuffer framebuffer;
        public ShaderProgram deepGBuffer;

        public DeepGBuffer(int width, int height, GraphicsContext context)
        {
            //5 layers
            framebuffer = new FrameBuffer(width, height, PixelComponentType.RGBA16f, context);

            framebuffer.Add("NormalE0", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment0, context);
            framebuffer.Add("RGBA0", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment1, context);
            framebuffer.Add("Depth0", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float), FrameBufferAttachments.ColorAttachment2, context);

            framebuffer.Add("NormalE1", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment3, context);
            framebuffer.Add("RGBA1", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment4, context);
            framebuffer.Add("Depth1", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float), FrameBufferAttachments.ColorAttachment5, context);

            //framebuffer.Add("NormalE2", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment6, context);
            //framebuffer.Add("RGBA2", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment7, context);
            //framebuffer.Add("Depth2", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float), FrameBufferAttachments.ColorAttachment8, context);

            //framebuffer.Add("NormalE3", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment9);
            //framebuffer.Add("RGBA3", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment10);
            //framebuffer.Add("Depth3", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float), FrameBufferAttachments.ColorAttachment11);

            //framebuffer.Add("NormalE4", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment12);
            //framebuffer.Add("RGBA4", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment13);
            //framebuffer.Add("Depth4", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA8, PixelType.Float), FrameBufferAttachments.ColorAttachment14);

            deepGBuffer = new ShaderProgram("Shaders/Deep GBuffer");
        }

        public void Bind(GraphicsContext context)
        {
            framebuffer.Bind(context);
        }

        public void Unbind()
        {
            framebuffer.UnBind();
        }

    }
}

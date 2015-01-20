using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine
{
    public enum FrameBufferAttachments
    {
        ColorAttachment0,
        ColorAttachment1,
        ColorAttachment2,
        ColorAttachment3,
        ColorAttachment4,
        ColorAttachment5,
        ColorAttachment6,
        ColorAttachment7,
        ColorAttachment8,
        ColorAttachment9,
        ColorAttachment10,
        ColorAttachment11,
        ColorAttachment12,
        ColorAttachment13,
        ColorAttachment14,
        ColorAttachment15,
        DepthAttachment,
        DepthStencilAttachment,
        StencilAttachment
    }

    public class FrameBuffer : FrameBufferLL, IDisposable
    {
        public Vector2 Size;
        public List<string> RenderTargets { get; set; }

        private Dictionary<string, FrameBufferTexture> fbufTextures;
        private Dictionary<string, int> fbufAttachmentsIDs;
        private Dictionary<string, FrameBufferAttachments> attachments;
        private int id;

        public FrameBuffer(int width, int height, PixelComponentType pct)
        {
            RenderTargets = new List<string>();
            fbufTextures = new Dictionary<string, FrameBufferTexture>();
            fbufAttachmentsIDs = new Dictionary<string, int>();
            attachments = new Dictionary<string, FrameBufferAttachments>();

            id = base.Generate();
            base.Bind(id);

            Add("DepthBuffer", new FrameBufferTexture(width, height, PixelFormat.Depth, PixelComponentType.D32, PixelType.Float), FrameBufferAttachments.DepthAttachment); //Attach the depth buffer to the framebuffer
            Add("Color", new FrameBufferTexture(width, height, PixelFormat.BGRA, PixelComponentType.RGBA16f, PixelType.Float), FrameBufferAttachments.ColorAttachment0);

            base.CheckError();
            base.Bind(0);

            Kokoro.Debug.ObjectAllocTracker.NewCreated(this, id, "Framebuffer");
        }

        public void Add(string id, FrameBufferTexture fbufTex, FrameBufferAttachments attachment)
        {
            RenderTargets.Add(id);
            fbufTextures.Add(id, fbufTex);
            if (attachment != FrameBufferAttachments.DepthAttachment) attachments.Add(id, attachment);
            fbufTex.BindToFrameBuffer(attachment);

            base.DrawBuffers(attachments.Values.ToArray());
        }

        public FrameBufferTexture this[string key]
        {
            get
            {
                return fbufTextures[key];
            }
            set
            {
                fbufTextures[key] = value;
            }
        }

        public void SetBlendFunc(BlendFunc func, FrameBufferAttachments attachment)
        {
            int index = int.Parse(attachment.ToString().Replace("ColorAttachment", ""));
            base.BlendFunction(func, index);
        }

        public void Bind(GraphicsContext context)
        {
            if (id != -1)
            {
                base.Bind(id);
                context.Viewport = new Vector4(0, 0, Size.X, Size.Y);
                currentFBUF = this;
            }
        }

        public void Dispose()
        {
            base.Delete(id);
        }

        private static FrameBuffer currentFBUF;
        public static FrameBuffer GetCurrentFrameBuffer()
        {
            return currentFBUF;
        }

        ~FrameBuffer()
        {
            Kokoro.Debug.ObjectAllocTracker.ObjectDestroyed(this, id, "FrameBuffer");
        }

    }
}

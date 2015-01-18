using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Debug;

namespace Kokoro.Engine
{
    public class FrameBufferTexture : Texture
    {
        public FrameBufferTexture(int width, int height, PixelFormat pf, PixelComponentType pct, PixelType pixelType)
            : base(width, height, pf, pct, pixelType)
        {
            base.id = base.Create(width, height, pct, pf, pixelType);
            ObjectAllocTracker.NewCreated(this, id);
        }

        public void BindToFrameBuffer(FrameBufferAttachments texUnit)
        {
            base.BindToFBuffer(texUnit, id);
        }

        public static void UnBindFromFrameBuffer(int texUnit)
        {
            UnBindFromFBuffer(texUnit);
        }

    }
}

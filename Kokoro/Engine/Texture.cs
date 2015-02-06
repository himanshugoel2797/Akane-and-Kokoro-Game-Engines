using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Debug;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
using System.Drawing;
#endif
#endif

namespace Kokoro.Engine
{
    /// <summary>
    /// Texture
    /// </summary>
    public class Texture : TextureLL, IDisposable
    {
        protected int id;

        public Texture(int width, int height, PixelFormat pf, PixelComponentType pct, PixelType pixelType)
        {
            id = base.Create(width, height, pct, pf, pixelType);
            ObjectAllocTracker.NewCreated(this, id, " { " + pf.ToString() + ", " + pct.ToString() + ", " + pixelType.ToString() + "}");
        }
        public Texture(string filename)
        {
            id = base.Create(filename);
            ObjectAllocTracker.NewCreated(this, id, " " + filename);
        }
        public Texture(int id)
        {
            this.id = id;
            ObjectAllocTracker.NewCreated(this, id, " Duplicate");
        }

#if DEBUG
        ~Texture()
        {
            ObjectAllocTracker.ObjectDestroyed(this, id);
        }
#endif

        public Bitmap ToBMP()
        {
            return base.FetchTextureData(id);
        }

        public virtual void Bind(int texUnit)
        {
            base.BindTexture(texUnit, id);
        }

        public static void UnBind(int texUnit)
        {
            UnBindTexture(texUnit);
        }

        public void Dispose()
        {
            base.Delete(id);
        }

    }
}

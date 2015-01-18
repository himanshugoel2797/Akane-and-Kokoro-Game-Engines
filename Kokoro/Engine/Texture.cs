using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Debug;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
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
            ObjectAllocTracker.NewCreated(this, id);
        }
        public Texture(string filename)
        {
            id = base.Create(filename);
            ObjectAllocTracker.NewCreated(this, id, filename);
        }
        public Texture(int id)
        {
            this.id = id;
            ObjectAllocTracker.NewCreated(this, id, "Duplicate");
        }

#if DEBUG
        ~Texture()
        {
            ObjectAllocTracker.ObjectDestroyed(this, id);
        }
#endif

        public virtual void Bind(int texUnit)
        {
            base.BindTexture(texUnit, id);
            Kokoro.Debug.ErrorLogger.AddMessage(id, "Bound", DebugType.Marker, Severity.Notification);
        }

        public static void UnBind(int texUnit)
        {
            UnBindTexture(texUnit);
            Kokoro.Debug.ErrorLogger.AddMessage(-1, "UnBound", DebugType.Marker, Severity.Notification);
        }

        public void Dispose()
        {
            base.Delete(id);
        }

    }
}

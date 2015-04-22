using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Debug;
using Kokoro.Math;

#if PC
using System.Drawing;
#endif

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
#endif

#elif OPENGL_AZDO
#if PC
using Kokoro.OpenGL.AZDO;
#endif

#endif

namespace Kokoro.Engine
{
    public enum TextureFilter
    {
        Linear, Nearest
    }

    /// <summary>
    /// Texture
    /// </summary>
    public class Texture : TextureLL, IDisposable
    {
        private static readonly object locker = new object();
        protected int id;
        private string file;
        bool loaded = false;

        public Vector2 Size
        {
            get
            {
                return new Vector2(base.width, base.height);
            }
        }

        public TextureFilter FilterMode
        {
            set
            {
                SetFilterMode(value);
            }
        }

        public bool WrapX
        {
            set
            {
                SetWrapX(value);
            }
        }

        public bool WrapY
        {
            set
            {
                SetWrapY(value);
            }
        }

        public Texture(int width, int height, PixelFormat pf, PixelComponentType pct, PixelType pixelType)
        {
            Sinus.SinusManager.QueueCommand(() =>
            {
                lock (locker)
                {
                    id = base.Create(width, height, pct, pf, pixelType);
                    ObjectAllocTracker.NewCreated(this, id, " { " + pf.ToString() + ", " + pct.ToString() + ", " + pixelType.ToString() + "}");
                    loaded = true;  //There's nothing to load
                }
            });
        }
        public Texture(string filename, bool delayedLoad = false)
        {
            Sinus.SinusManager.QueueCommand(() =>
            {
                lock (locker)
                {
                    //If requested, don't load the texture yet
                    if (!delayedLoad)
                    {
                        id = base.Create(filename);
                        loaded = true;
                    }
                    else
                    {
                        this.file = filename;
                        loaded = false;
                    }
                    ObjectAllocTracker.NewCreated(this, id, " " + filename);
                }
            });
        }
        public Texture(int id)
        {
            lock (locker)
            {
                loaded = true;
                this.id = id;
                ObjectAllocTracker.NewCreated(this, id, " Duplicate");
            }
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
            //load the texture if it wasn't loaded before
            if (!loaded)
            {
                lock (locker)
                {
                    id = base.Create(file);
                    loaded = true;
                }
            }
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

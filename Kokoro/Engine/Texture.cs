﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Debug;
using Kokoro.Math;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
using System.Drawing;
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

        public Texture(int width, int height, PixelFormat pf, PixelComponentType pct, PixelType pixelType)
        {
            lock (locker)
            {
                id = base.Create(width, height, pct, pf, pixelType);
                ObjectAllocTracker.NewCreated(this, id, " { " + pf.ToString() + ", " + pct.ToString() + ", " + pixelType.ToString() + "}");
            }
        }
        public Texture(string filename)
        {
            lock (locker)
            {
                id = base.Create(filename);
                ObjectAllocTracker.NewCreated(this, id, " " + filename);
            }
        }
        public Texture(int id)
        {
            lock (locker)
            {
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

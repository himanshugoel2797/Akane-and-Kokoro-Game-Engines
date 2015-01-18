using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;

namespace Kokoro.OpenGL.PC
{
    public class TextureLL
    {
        protected int Create(int width, int height, Kokoro.Engine.PixelComponentType pfI, Kokoro.Engine.PixelFormat pf, Kokoro.Engine.PixelType type)
        {
            int id = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexImage2D(TextureTarget.Texture2D, 0, EnumConverters.EPixelComponentType(pfI), width, height, 0,
                EnumConverters.EPixelFormat(pf), EnumConverters.EPixelType(type), (IntPtr)0);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            return id;
        }

        protected int Create(string filename)
        {
            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);

            // We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            return id;
        }

        protected void BindTexture(int texUnit, int id)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
            GL.BindTexture(TextureTarget.Texture2D, id);
        }

        protected static void UnBindTexture(int texUnit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + texUnit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected void Delete(int id)
        {
            GL.DeleteTexture(id);
        }

        protected void BindToFBuffer(Engine.FrameBufferAttachments texUnit, int id)
        {
            FramebufferAttachment attach = EnumConverters.EFrameBufferAttachment(texUnit);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, attach, id, 0);
        }

        protected static void UnBindFromFBuffer(int texUnit)
        {
            FramebufferAttachment attach = (FramebufferAttachment)texUnit;
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, attach, 0, 0);
        }
    }
}

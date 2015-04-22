#if OPENGL && PC

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace Kokoro.OpenGL.PC
{
    public class FrameBufferLL
    {
        protected int Generate()
        {
            int id = GL.GenFramebuffer();
            return id;
        }

        protected void Bind(int id)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, id);
        }

        protected void DrawBuffers(Kokoro.Engine.FrameBufferAttachments[] attachments)
        {
            //TODO Setup DrawBuffer attachments
            DrawBuffersEnum[] dbEnum = new DrawBuffersEnum[attachments.Length];
            for (int i = 0; i < dbEnum.Length; i++) { dbEnum[i] = EnumConverters.EDrawBufferAttachment(attachments[i]); }
            GL.DrawBuffers(dbEnum.Length, dbEnum.OrderBy(x => x).ToArray());

        }

        protected void BlendFunction(Engine.BlendFunc func, int index)
        {
            Sinus.SinusManager.QueueCommand(() =>
            {
                GL.BlendFunc(index, EnumConverters.EBlendFuncSRC(func.Src), EnumConverters.EBlendFuncDST(func.Dst));
            });
        }

        protected void Delete(int id)
        {
            Sinus.SinusManager.QueueCommand(() =>
            {
                GL.DeleteFramebuffer(id);
            });
        }

        protected void CheckError()
        {
            Sinus.SinusManager.QueueCommand(() =>
            {
                Kokoro.Debug.ErrorLogger.AddMessage(0, GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer).ToString(), Kokoro.Debug.DebugType.Marker, Kokoro.Debug.Severity.Notification);
            });
        }

    }
}

#endif
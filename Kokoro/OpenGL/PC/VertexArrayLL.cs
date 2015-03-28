#if OPENGL && PC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Kokoro.Sinus;

namespace Kokoro.OpenGL.PC
{
    public class VertexArrayLL : IDisposable
    {
        GPUBufferLL[] buffers;
        int vaID = 0;

        int[] elementCount;
        Kokoro.Engine.BufferUse[] bufferUses;

        GPUBufferLL ibo;

        /// <summary>
        /// Allows access of the underlying GPU buffers
        /// </summary>
        /// <param name="index">The index of the GPU buffer to access</param>
        /// <returns>Returnss the relevant GPUBufferLL object</returns>
        public GPUBufferLL this[int index]
        {
            get
            {
                return buffers[index];
            }
        }

        public void PostFence()
        {
            for (int i = 0; i < buffers.Length; i++)
            {
                buffers[i].PostFence();
            }
        }

        public VertexArrayLL(int bufferCount, long bufferSize, Kokoro.Engine.UpdateMode updateMode, Kokoro.Engine.BufferUse[] bufferUses, int[] elementCount)
        {
            //Generate all GPUBuffers
            buffers = new GPUBufferLL[bufferCount];
            for (int i = 0; i < bufferCount; i++)
            {
                buffers[i] = new GPUBufferLL(updateMode, bufferUses[i], bufferSize * elementCount[i] * 8);  //NOTE: bufferSize no longer refers to the number of bytes of data, but the number of elements in total
            }

            this.elementCount = elementCount;
            this.bufferUses = bufferUses;

            SinusManager.QueueCommand
                (() =>
                {
                    vaID = GL.GenVertexArray();
                    GL.BindVertexArray(vaID);

                    if (bufferUses[0] == Engine.BufferUse.Index)
                    {
                        ibo = buffers[0];
                        ibo.Bind();
                        bufferCount--;
                    }

                    for (int j = 0; j < bufferCount; j++)
                    {
                        int i = j;
                        if (bufferUses[0] == Engine.BufferUse.Index)
                        {
                            i = j + 1;
                        }
                        GL.EnableVertexAttribArray(j);
                        buffers[i].Bind();
                        GL.VertexAttribPointer(j, elementCount[i], VertexAttribPointerType.Float, false, 0, 0);
                        GL.VertexAttribDivisor(j, 0);

                    }
                    GL.EnableVertexAttribArray(0);
                    GL.BindVertexArray(0);
                });
        }

        //Bind the VAO
        public void Bind()
        {
            SinusManager.QueueCommand(() =>
            {
                GL.BindVertexArray(vaID);
                int bufferCount = elementCount.Length;

                if (bufferUses[0] == Engine.BufferUse.Index)
                {
                    ibo = buffers[0];
                    ibo.Bind();
                    bufferCount--;
                }

                for (int j = 0; j < bufferCount; j++)
                {
                    int i = j;
                    if (bufferUses[0] == Engine.BufferUse.Index)
                    {
                        i++;
                    }
                    GL.EnableVertexAttribArray(j);
                    buffers[i].Bind();
                    GL.VertexAttribPointer(j, elementCount[i], VertexAttribPointerType.Float, false, 0, 0);
                    GL.VertexAttribDivisor(j, 0);

                }

                if (ibo != null) ibo.Bind();

            });
        }

        //Unbind the VAO
        public void UnBind()
        {
            SinusManager.QueueCommand(() =>
            {
                GL.BindVertexArray(0);
                if (ibo != null) ibo.UnBind();
            });
        }

        public void Dispose()
        {
#if DEBUG
            disposed = true;
#endif
            Kokoro.Debug.ErrorLogger.AddMessage(vaID, "VertexArrayLL marked for deletion", Kokoro.Debug.DebugType.Marker, Kokoro.Debug.Severity.Notification);

            //Mark all owned GPUBuffers to be erased as well
            for (int i = 0; i < buffers.Length; i++)
            {
                buffers[i].Dispose();
            }

            SinusManager.QueueCommand(() =>
            {
                Kokoro.Debug.ObjectAllocTracker.ObjectDestroyed(this, vaID, "VertexArrayLL Destroyed");
                GL.DeleteVertexArray(vaID);
            });
        }


#if DEBUG
        bool disposed = false;
        ~VertexArrayLL()
        {
            if (!disposed)
            {
                Kokoro.Debug.ErrorLogger.AddMessage(vaID, "VertexArray was automatically marked for deletion, Will cause memory leak", Kokoro.Debug.DebugType.Performance, Kokoro.Debug.Severity.High);
                Dispose();
            }
        }
#endif

    }
}
#endif
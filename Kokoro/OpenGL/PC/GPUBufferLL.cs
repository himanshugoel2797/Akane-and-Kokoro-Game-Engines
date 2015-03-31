#if OPENGL && PC
using Kokoro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Kokoro.Sinus;
using System.Runtime.InteropServices;


namespace Kokoro.OpenGL.PC
{
    public class GPUBufferLL : IDisposable
    {
#if GL44
        private int staticID = -1;
        private IntPtr mappedPtr = IntPtr.Zero;
        private UpdateMode updateMode;
        private BufferTarget target;
        private BufferStorageFlags flags;
        private IntPtr syncObj;
        private int offset;

        //TODO Setup a mapping system for the offsets so that the buffers can be defragmented if they start to run low on memory without needing elaborate tricks to update all the models

        public GPUBufferLL(UpdateMode mode, BufferUse use, long memSize)
        {
            updateMode = mode;
            target = EnumConverters.EBufferTarget(use);

            if (use == BufferUse.Array || use == BufferUse.Index || use == BufferUse.Indirect) flags = BufferStorageFlags.MapWriteBit | BufferStorageFlags.MapPersistentBit | BufferStorageFlags.MapCoherentBit;

#if DEBUG
            flags |= BufferStorageFlags.MapReadBit;
#endif

            SinusManager.QueueCommand(() =>
            {
                syncObj = GL.FenceSync(SyncCondition.SyncGpuCommandsComplete, WaitSyncFlags.None);  //Initialize the first sync object
                staticID = GL.GenBuffer();      //Generate the buffer

                if (updateMode == UpdateMode.Dynamic)
                {
                    //Setup persistent mapping if this is a dynamic buffer
                    GL.BindBuffer(target, staticID);
                    GL.BufferStorage(target, (IntPtr)(memSize), IntPtr.Zero, flags);
                    mappedPtr = GL.MapBuffer(target, BufferAccess.WriteOnly);
                    GL.BindBuffer(target, 0);
                }
            });
        }

        public void Bind()
        {
            GL.BindBuffer(target, staticID);
        }

        public void UnBind()
        {
            GL.BindBuffer(target, 0);
        }

        /// <summary>
        /// Append data to the buffer and return the offset it was buffered to
        /// </summary>
        /// <param name="data">The data to append</param>
        /// <returns>The offset it was buffered to</returns>
        public uint AppendData(float[] data)
        {
            BufferData(data, offset, data.Length * sizeof(float));
            offset += data.Length * sizeof(float);
            return (uint)(offset - data.Length * sizeof(float));
        }

        /// <summary>
        /// Append data to the buffer and return the offset it was buffered to
        /// </summary>
        /// <param name="data">The data to append</param>
        /// <returns>The offset it was buffered to</returns>
        public uint AppendData(uint[] data)
        {
            BufferData(data, offset, data.Length * sizeof(uint));
            offset += data.Length * sizeof(uint);
            return (uint)(offset - data.Length * sizeof(uint));
        }

        public uint AppendData(byte[] data)
        {
            BufferData(data, offset, data.Length);
            offset += data.Length;
            return (uint)(offset - data.Length);
        }

        public void PostFence()
        {
            if (updateMode == UpdateMode.Dynamic)
            {
                // lock the buffer:
                GL.DeleteSync(syncObj);
                syncObj = GL.FenceSync(SyncCondition.SyncGpuCommandsComplete, WaitSyncFlags.None);
            }
        }

        /// <summary>
        /// Put data into the buffer
        /// </summary>
        /// <param name="data">The Data</param>
        /// <param name="offset">The offset at which to start writing(only used for dynamic objects)</param>
        /// <param name="length">The amount of data to copy (only used for dynamic objects)</param>
        public void BufferData(float[] data, int offset = 0, int length = -1)
        {
            if(length == -1)length = data.Length * sizeof(float);
            byte[] tmp = new byte[length];
            Buffer.BlockCopy(data, 0, tmp, 0, tmp.Length);
            BufferData(tmp, offset, length);
        }

        /// <summary>
        /// Put data into the buffer
        /// </summary>
        /// <param name="data">The Data</param>
        /// <param name="offset">The offset at which to start writing(only used for dynamic objects)</param>
        /// <param name="length">The amount of data to copy (only used for dynamic objects)</param>
        public void BufferData(byte[] data, int offset = 0, int length = -1)
        {
            if (updateMode == UpdateMode.Dynamic)   //Write to the persistent mapped buffer
            {
                #region Persistent Mapping
                SinusManager.QueueCommand(() =>
                {
                    // waiting for the buffer
                    WaitSyncStatus waitReturn = WaitSyncStatus.TimeoutExpired;
                    while (waitReturn != WaitSyncStatus.AlreadySignaled && waitReturn != WaitSyncStatus.ConditionSatisfied)
                    {
                        waitReturn = GL.ClientWaitSync(syncObj, ClientWaitSyncFlags.SyncFlushCommandsBit, 1);   //TODO depending on how much time this can take, we might want to do other work in the meantime
                    }

                    //Write the data
                    unsafe
                    {
                        fixed (byte* SystemMemory = &data[0])
                        {
                            Marshal.Copy(data, 0, (mappedPtr + offset), ((length == -1) ? data.Length : length));
                        }
                    }
                });
                #endregion
            }
            else if (updateMode == UpdateMode.Static)
            {
                #region Buffer Data
                SinusManager.QueueCommand(() =>
                {
                    GL.BindBuffer(target, staticID);
                    if (offset == 0) GL.BufferData(target, (IntPtr)((length == -1) ? data.Length : length), data, BufferUsageHint.StaticDraw);
                    else GL.BufferSubData(target, (IntPtr)(offset), (IntPtr)((length == -1) ? data.Length : length), data);
                    GL.BindBuffer(target, 0);
                });
                #endregion
            }
        }


        /// <summary>
        /// Put data into the buffer
        /// </summary>
        /// <param name="data">The Data</param>
        /// <param name="offset">The offset at which to start writing(only used for dynamic objects)</param>
        /// <param name="length">The amount of data to copy (only used for dynamic objects)</param>
        public void BufferData(uint[] data, int offset = 0, int length = -1)
        {
            if (length == -1) length = data.Length * sizeof(uint);
            byte[] tmp = new byte[length];
            Buffer.BlockCopy(data, 0, tmp, 0, length);
            BufferData(tmp, offset, length);
        }
#else

#endif

        public void Dispose()
        {
#if DEBUG
            disposed = true;
#endif
            Kokoro.Debug.ErrorLogger.AddMessage(staticID, "GPUBuffer marked for deletion", Kokoro.Debug.DebugType.Marker, Kokoro.Debug.Severity.Notification);
            SinusManager.QueueCommand(() =>
            {
                Kokoro.Debug.ObjectAllocTracker.ObjectDestroyed(this, staticID, "GPUBuffer Destroyed");
                GL.DeleteBuffer(staticID);
            });
        }

#if DEBUG
        bool disposed = false;
        ~GPUBufferLL()
        {
            if (!disposed)
            {
                Kokoro.Debug.ErrorLogger.AddMessage(staticID, "GPUBuffer was automatically marked for deletion, Will cause memory leak", Kokoro.Debug.DebugType.Performance, Kokoro.Debug.Severity.High);
                Dispose();
            }
        }
#endif
    }
}
#endif
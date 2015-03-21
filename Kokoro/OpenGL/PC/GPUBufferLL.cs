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
    class GPUBufferLL
    {
#if GL44
        private int staticID = -1;
        private IntPtr mappedPtr = IntPtr.Zero;
        private UpdateMode updateMode;
        private BufferTarget target;
        private BufferStorageFlags flags;
        private IntPtr syncObj;

        public GPUBufferLL(UpdateMode mode, BufferUse use, long memSize)
        {
            updateMode = mode;
            target = EnumConverters.EBufferTarget(use);

            if (use == BufferUse.Array || use == BufferUse.Index) flags = BufferStorageFlags.MapWriteBit | BufferStorageFlags.MapPersistentBit | BufferStorageFlags.MapCoherentBit;

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

        /// <summary>
        /// Put data into the buffer
        /// </summary>
        /// <param name="data">The Data</param>
        /// <param name="offset">The offset at which to start writing(only used for dynamic objects)</param>
        /// <param name="length">The amount of data to copy (only used for dynamic objects)</param>
        public void BufferData(float[] data, int offset = 0, int length = -1)
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
                        fixed (float* SystemMemory = &data[0])
                        {
                            Marshal.Copy(data, offset, mappedPtr, ((length == -1) ? data.Length : length));
                        }
                    }

                    // lock the buffer:
                    GL.DeleteSync(syncObj);
                    syncObj = GL.FenceSync(SyncCondition.SyncGpuCommandsComplete, WaitSyncFlags.None);
                });
                #endregion
            }
            else if (updateMode == UpdateMode.Static)
            {
                #region Buffer Data
                SinusManager.QueueCommand(() => GL.BufferData(target, (IntPtr)(sizeof(float) * data.Length), data, BufferUsageHint.StaticDraw));
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
                        fixed (uint* SystemMemory = &data[0])
                        {
                            uint* VideoMemory = (uint*)mappedPtr.ToPointer();
                            Buffer.MemoryCopy(mappedPtr, new IntPtr((void*)SystemMemory), (uint)((length == -1) ? data.Length : length));
                        }
                    }

                    // lock the buffer:
                    GL.DeleteSync(syncObj);
                    syncObj = GL.FenceSync(SyncCondition.SyncGpuCommandsComplete, WaitSyncFlags.None);
                });
                #endregion
            }
            else if (updateMode == UpdateMode.Static)
            {
                #region Buffer Data
                SinusManager.QueueCommand(() =>
                {
                    GL.BindBuffer(target, staticID);
                    GL.BufferData(target, (IntPtr)(sizeof(uint) * data.Length), data, BufferUsageHint.StaticDraw);
                    GL.BindBuffer(target, 0);
                });
                #endregion
            }
        }
#else

#endif
    }
}
#endif
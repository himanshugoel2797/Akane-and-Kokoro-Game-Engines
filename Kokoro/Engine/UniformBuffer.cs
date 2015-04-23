using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;
using Kokoro.Debug;

#if OPENGL
#if PC
using Kokoro.OpenGL;
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine
{
    public class UniformBuffer
    {
        GPUBufferLL gpuBuffer;

        public UniformBuffer()
        {
            gpuBuffer = new GPUBufferLL(UpdateMode.Static, BufferUse.Uniform, 16 * 1024);
        }
    }
}

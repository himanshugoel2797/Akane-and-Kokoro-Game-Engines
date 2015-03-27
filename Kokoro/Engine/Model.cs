using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kokoro.Math;
using Kokoro.Engine.Shaders;

#if PC
using Assimp;
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
    public enum DrawMode
    {
        Triangles,
        TriangleStrip,
        Lines,
        LineStrip,
        Points,
        Patches
    }

    enum BufferUse
    {
        Array, Index, Uniform, ShaderStorage, Indirect
    }

    public class Model : IDisposable
    {
        //Models will only store vertex information and a system will be invoked to batch together static meshes?
        //Should multidrawindirect be limited to things loaded by the World?

        public struct BoundingBox
        {
            public Vector3 Min;
            public Vector3 Max;

            public Vector3 Up;
        };

        protected static VertexArrayLL staticBuffer;
        protected static long staticBufferOffset;
        protected static long staticBufferLength;

        protected static VertexArrayLL dynamicBuffer;
        protected static long dynamicBufferOffset;
        protected static long dynamicBufferLength;


        protected string filepath;
        protected uint offset;
        protected uint length;

        public Matrix4 World { get; set; }
        public Material[] Materials { get; set; }
        public DrawMode DrawMode { get; set; }
        public BoundingBox Bound;
        public uint IndexCount { get; set; }

        private static BoundingBox tmpBound;

        static Model()
        {
            staticBufferOffset = 0;
            staticBufferLength = /*How much should we allocate?*/10000000;  //Current limit = 10 Million Elements
            staticBuffer = new VertexArrayLL(4, staticBufferLength, UpdateMode.Static, new BufferUse[] { BufferUse.Index, BufferUse.Array, BufferUse.Array, BufferUse.Array }, new int[] { 1, 3, 3, 2 });

            dynamicBufferOffset = 0;
            dynamicBufferLength =  /*How much should we allocate?*/1000000; //Current limit = 1 Million Elements
            dynamicBuffer = new VertexArrayLL(4, dynamicBufferLength, UpdateMode.Static, new BufferUse[] { BufferUse.Index, BufferUse.Array, BufferUse.Array, BufferUse.Array }, new int[] { 1, 3, 3, 2 });
        }

        public Model()
        {
            Materials = new Material[] { new Material() };
            DrawMode = Engine.DrawMode.Triangles;
#if DEBUG
            Kokoro.Debug.ObjectAllocTracker.NewCreated(this, 0, "Model");
#endif
        }
#if DEBUG
        ~Model()
        {
            Kokoro.Debug.ObjectAllocTracker.ObjectDestroyed(this, 0, "Model");
        }
#endif

        public Action<GraphicsContext> PreDraw { get; set; }

        //Use this to build a list of all the commands to send to the appropriate multidraw indirect buffers
        public void Draw(GraphicsContext context)
        {
            //Append a draw command to the MDI queue
            for (int a = 0; a < buffers.Length; a++)
            {
                //Apply the Material
                Materials[a].Apply(context, this);      //Material pipeline will just setup textures and uniform buffer parameters somehow

                GraphicsContextLL.AddDrawCall(0, IndexCount, offset);   //Send the draw call

                //Cleanup the Material
                Materials[a].Cleanup(context, this);    //Queue the material to be cleaned out after everything has been done
            }
        }

        public void Dispose()
        {
            //Nothing unless we end up needing to setup the defragmentation mechanism
        }
    }
}

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

    public enum BufferUse
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

        internal static VertexArrayLL staticBuffer;
        protected static long[] staticBufferOffset;
        protected static long[] staticBufferLength;

        internal static VertexArrayLL dynamicBuffer;
        protected static long[] dynamicBufferOffset;
        protected static long[] dynamicBufferLength;


        protected string filepath;
        protected uint[][] offsets;
        protected uint[] lengths;

        protected void Init(int num)
        {
            offsets = new uint[num][];
            for (int a = 0; a < num; a++) offsets[a] = new uint[4];
            lengths = new uint[num];
        }
        protected void SetUVs(UpdateMode mode, float[] uvs, int index)
        {
            if (mode == UpdateMode.Dynamic)
            {
                offsets[index][3] = dynamicBuffer[3].AppendData(uvs);
            }
            else if (mode == UpdateMode.Static)
            {
                offsets[index][3] = staticBuffer[3].AppendData(uvs);
            }
        }
        protected void SetNormals(UpdateMode mode, float[] norms, int index)
        {
            if (mode == UpdateMode.Dynamic)
            {
                offsets[index][2] = dynamicBuffer[2].AppendData(norms);
            }
            else if (mode == UpdateMode.Static)
            {
                offsets[index][2] = staticBuffer[2].AppendData(norms);
            }
        }
        protected void SetVertices(UpdateMode mode, float[] verts, int index)
        {
            if (mode == UpdateMode.Dynamic)
            {
                offsets[index][1] = dynamicBuffer[1].AppendData(verts);
            }
            else if (mode == UpdateMode.Static)
            {
                offsets[index][1] = staticBuffer[1].AppendData(verts);
            }
        }
        protected void SetIndices(UpdateMode mode, uint[] indices, int index)
        {
            if (mode == UpdateMode.Dynamic)
            {
                offsets[index][0] = dynamicBuffer[0].AppendData(indices);
            }
            else if (mode == UpdateMode.Static)
            {
                offsets[index][0] = staticBuffer[0].AppendData(indices);
            }
            lengths[index] = (uint)indices.Length;
        }

        public Matrix4 World { get; set; }
        public Material[] Materials { get; set; }
        public DrawMode DrawMode { get; set; }
        public BoundingBox Bound;

        private static BoundingBox tmpBound;

        static Model()
        {
            int numBufs = 4;

            /*
             * 0: Index
             * 1: Vertex
             * 2: Normal
             * 3: UV
             */

            staticBufferOffset = new long[numBufs];
            staticBufferLength = new long[numBufs]; /*How much should we allocate?*/  //Current limit = 10 Million Elements
            staticBuffer = new VertexArrayLL(4, 10000000, UpdateMode.Static, new BufferUse[] { BufferUse.Index, BufferUse.Array, BufferUse.Array, BufferUse.Array }, new int[] { 1, 3, 3, 2 });
            staticBufferLength[0] = 10000000;
            staticBufferLength[1] = 30000000;
            staticBufferLength[2] = 30000000;
            staticBufferLength[3] = 20000000;


            dynamicBufferOffset = new long[numBufs];
            dynamicBufferLength = new long[numBufs]; /*How much should we allocate?*/ //Current limit = 1 Million Elements
            dynamicBuffer = new VertexArrayLL(4, 1000000, UpdateMode.Dynamic, new BufferUse[] { BufferUse.Index, BufferUse.Array, BufferUse.Array, BufferUse.Array }, new int[] { 1, 3, 3, 2 });
            dynamicBufferLength[0] = 1000000;
            dynamicBufferLength[1] = 3000000;
            dynamicBufferLength[2] = 3000000;
            dynamicBufferLength[3] = 2000000;

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

        //Use this to build a list of all the commands to send to the appropriate multidraw indirect buffers
        public void Draw(GraphicsContext context)
        {
            //Append a draw command to the MDI queue
            for (int a = 0; a < offsets.Length; a++)
            {
                //Apply the Material
                Materials[a].Apply(context, this);      //Material pipeline will just setup textures and uniform buffer parameters somehow

                GraphicsContextLL.AddDrawCall(offsets[a][0] / sizeof(uint), lengths[a], offsets[a][1] / sizeof(float));   //Send the draw call

                //Cleanup the Material
                //Materials[a].Cleanup(context, this);    //Queue the material to be cleaned out after everything has been done
            }
        }

        public void Dispose()
        {
            //Nothing unless we end up needing to setup the defragmentation mechanism
        }
    }
}

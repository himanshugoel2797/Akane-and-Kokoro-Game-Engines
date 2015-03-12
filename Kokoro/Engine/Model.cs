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
        Array, Index, Uniform, ShaderStorage
    }

    public class Model : IDisposable
    {
        public struct BoundingBox
        {
            public Vector3 Min;
            public Vector3 Max;

            public Vector3 Up;
        };

        protected GPUBufferLL[] buffers { get; set; }

        protected string filepath;

        public Matrix4 World { get; set; }
        public Material[] Materials { get; set; }
        public DrawMode DrawMode { get; set; }
        public BoundingBox Bound;

        private static BoundingBox tmpBound;
        
        private static Tuple<GPUBufferLL[], Texture[]> LoadModelVB(string filename)
        {

        }

        public static Model Load(string filename)
        {
            var tmp = LoadModelVB(filename);
            Model m = new Model();
            m.filepath = filename;
            m.buffers = tmp.Item1;
            m.Materials = new Material[tmp.Item2.Length];
            for (int i = 0; i < tmp.Item2.Length; i++)
            {
                m.Materials[i] = new Material
                {
                    ColorMap = tmp.Item2[i],
                    Shader = new ShaderProgram(new ShaderLib.GBufferShader())
                };
            }
            m.World = Matrix4.Identity;

            m.Bound.Max = tmpBound.Max;
            m.Bound.Min = tmpBound.Min;


            return m;
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

        public void Draw(GraphicsContext context)
        {

            for (int a = 0; a < vbufs.Length; a++)
            {
                vbufs[a].DrawMode = this.DrawMode;
                vbufs[a].Bind();

                if (PreDraw != null) PreDraw(context);

                //Apply the Material
                Materials[a].Apply(context, this);

                vbufs[a].Draw((int)vbufs[a].IndexCount);

                //Cleanup the Material
                Materials[a].Cleanup(context, this);
            }

            VertexBufferLL.UnBind();
        }

        public void Dispose()
        {
            for (int i = 0; i < vbufs.Length; i++)
            {
                vbufs[i].Dispose();
            }
        }
    }
}

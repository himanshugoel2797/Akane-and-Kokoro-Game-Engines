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

    public class Model : IDisposable
    {
        public struct BoundingBox
        {
            public Vector3 Min;
            public Vector3 Max;

            public Vector3 Up;
        };

        protected VertexBufferLL[] vbufs { get; set; }

        protected string filepath;

        public Matrix4 World { get; set; }
        public Material[] Materials { get; set; }
        public DrawMode DrawMode { get; set; }
        public BoundingBox Bound;

        private static BoundingBox tmpBound;
        protected static Tuple<VertexBufferLL[], Texture[]> LoadModelVB(string filename, int frame)
        {
            AssimpContext context = new AssimpContext();
            Scene model = context.ImportFile(filename, PostProcessSteps.CalculateTangentSpace | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.ImproveCacheLocality | PostProcessSteps.RemoveRedundantMaterials);

            string baseDir = Path.GetDirectoryName(filename);

            List<VertexBufferLL> vbufs = new List<VertexBufferLL>();
            List<Texture> texs = new List<Texture>();

            for (int a = 0; a < model.MeshCount; a++)
            {
                VertexBufferLL vbuf = new VertexBufferLL();
                Texture t;
                Mesh m = model.Meshes[a];


                if (m.MaterialIndex >= 0 && model.Materials[m.MaterialIndex].TextureDiffuse.FilePath != null) t = new Texture(Path.Combine(baseDir, model.Materials[m.MaterialIndex].TextureDiffuse.FilePath));
                else
                {
                    t = null;
                }
                texs.Add(t);

                tmpBound.Min = new Vector3(m.Vertices[0].X, m.Vertices[0].Y, m.Vertices[0].Z);
                tmpBound.Max = new Vector3(m.Vertices[0].X, m.Vertices[0].Y, m.Vertices[0].Z);

                float[] vertices = new float[m.VertexCount * 3];
                for (int v = 0; v < m.VertexCount * 3; v += 3)
                {
                    vertices[v] = m.Vertices[(v - (v % 3)) / 3].X;
                    vertices[v + 1] = m.Vertices[(v - (v % 3)) / 3].Y;
                    vertices[v + 2] = m.Vertices[(v - (v % 3)) / 3].Z;

                    if (vertices[v] < tmpBound.Min.X) tmpBound.Min.X = vertices[v];
                    if (vertices[v + 1] < tmpBound.Min.Y) tmpBound.Min.Y = vertices[v + 1];
                    if (vertices[v + 2] < tmpBound.Min.Z) tmpBound.Min.Z = vertices[v + 2];
                    if (vertices[v] > tmpBound.Max.X) tmpBound.Max.X = vertices[v];
                    if (vertices[v + 1] > tmpBound.Max.Y) tmpBound.Max.Y = vertices[v + 1];
                    if (vertices[v + 2] > tmpBound.Max.Z) tmpBound.Max.Z = vertices[v + 2];

                    //TODO Setup editor viewport, work on bounding box rendering, object selection

                }
                vbuf.SetVertices(vertices);

                if (m.GetIndices() != null)
                {
                    uint[] indices = Array.ConvertAll(m.GetIndices(), x => (uint)x);
                    vbuf.SetIndices(indices);
                }

                if (m.TextureCoordinateChannels.Length > 0 && m.TextureCoordinateChannels[0].Count > 0)
                {
                    float[] texcoords = new float[m.VertexCount * 2];
                    for (int v = 0; v < m.VertexCount * 2; v += 2)
                    {
                        texcoords[v] = m.TextureCoordinateChannels[0][(v - (v % 2)) / 2].X;
                        texcoords[v + 1] = m.TextureCoordinateChannels[0][(v - (v % 2)) / 2].Y;
                    }
                    vbuf.SetUVs(texcoords);
                }
                else
                {
                    float[] texcoords = new float[m.VertexCount * 2];
                    for (int v = 0; v < m.VertexCount * 2; v += 2)
                    {
                        texcoords[v] = m.Vertices[(v - (v % 2)) / 2].X;
                        texcoords[v + 1] = m.Vertices[(v - (v % 2)) / 2].Y;
                    }
                    vbuf.SetUVs(texcoords);
                }

                if (m.HasNormals)
                {

                    float[] normals = new float[m.VertexCount * 3];
                    for (int v = 0; v < m.VertexCount * 3; v += 3)
                    {
                        normals[v] = m.Normals[(v - (v % 3)) / 3].X;
                        normals[v + 1] = m.Normals[(v - (v % 3)) / 3].Y;
                        normals[v + 2] = m.Normals[(v - (v % 3)) / 3].Z;
                    }
                    vbuf.SetNormals(normals);
                }



                if (m.HasTangentBasis)
                {
                    float[] tangents = new float[m.VertexCount * 3];
                    for (int v = 0; v < m.VertexCount * 3; v += 3)
                    {
                        tangents[v] = m.Tangents[(v - (v % 3)) / 3].X;
                        tangents[v + 1] = m.Tangents[(v - (v % 3)) / 3].Y;
                        tangents[v + 2] = m.Tangents[(v - (v % 3)) / 3].Z;
                    }
                    vbuf.SetTangents(tangents);
                }

                vbufs.Add(vbuf);
            }
            return new Tuple<VertexBufferLL[], Texture[]>(vbufs.ToArray(), texs.ToArray());
        }

        public static Model Load(string filename)
        {
            var tmp = LoadModelVB(filename, 0);
            Model m = new Model();
            m.filepath = filename;
            m.vbufs = tmp.Item1;
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

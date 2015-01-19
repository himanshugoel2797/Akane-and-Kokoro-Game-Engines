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
        protected VertexBufferLL[] vbufs { get; set; }

        protected string filepath;

        public Matrix4 World { get; set; }
        public Material[] Materials { get; set; }
        public DrawMode DrawMode { get; set; }

        private static Tuple<VertexBufferLL[], Texture[]> LoadModelVB(string filename, int frame)
        {
            AssimpContext context = new AssimpContext();
            Scene model = context.ImportFile(filename);

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

                float[] vertices = new float[m.VertexCount * 3];
                for (int v = 0; v < m.VertexCount * 3; v += 3)
                {
                    vertices[v] = m.Vertices[(v - (v % 3)) / 3].X;
                    vertices[v + 1] = m.Vertices[(v - (v % 3)) / 3].Y;
                    vertices[v + 2] = m.Vertices[(v - (v % 3)) / 3].Z;
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
                    Diffuse = tmp.Item2[i],
                    Shader = new ShaderProgram(new VertexShader("Shaders/GBuffer"), new FragmentShader("Shaders/GBuffer"))
                };
            }
            m.World = Matrix4.Identity;

            return m;
        }

        public Action<GraphicsContext> PreDraw { get; set; }

        public void Draw(GraphicsContext context)
        {

            for (int a = 0; a < vbufs.Length; a++)
            {
                vbufs[a].DrawMode = this.DrawMode;
                vbufs[a].Bind();
                Materials[a].Shader.SetShaderMatrix("Model", World);
                Materials[a].Shader.SetShaderMatrix("View", context.View);
                Materials[a].Shader.SetShaderMatrix("Proj", context.Projection);
                Materials[a].Shader.SetShaderFloat("zNear", context.ZNear);
                Materials[a].Shader.SetShaderFloat("zFar", context.ZFar);

                //Calculate normal matrix
                //var nrm = Matrix4.Invert(context.View * World);
                //nrm.Transpose();
                //Materials[a].Shader.VertexShader.SetShaderMatrix("Normal", nrm);

                if (Materials[a].Diffuse != null) Materials[a].Shader.SetTexture("diffuse", Materials[a].Diffuse);
                if (Materials[a].Specular != null) Materials[a].Shader.SetTexture("specular", Materials[a].Specular);
                if (Materials[a].NormalMap != null) Materials[a].Shader.SetTexture("normalMap", Materials[a].NormalMap);
                Materials[a].Shader.SetShaderFloat("roughness", Materials[a].Lit);
                Materials[a].Shader.SetShaderFloat("fresnel", Materials[a].FresnelTerm);
                Materials[a].Shader.SetShaderFloat("k", Materials[a].DiffuseReflectivity);
                Materials[a].Shader.SetShaderFloat("reflectiveness", Materials[a].Reflectivity);

                if (PreDraw != null) PreDraw(context);

                Materials[a].Shader.Apply(context);
                vbufs[a].Draw((int)vbufs[a].IndexCount);
                Materials[a].Shader.Cleanup(context);
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

using Assimp;
using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.ContentPipeline
{
    public class ModelConvert
    {
        internal static byte[] Process(string filename)
        {
            Kokoro.Engine.Model.BoundingBox tmpBound;
            AssimpContext context = new AssimpContext();
            Scene model = context.ImportFile(filename, PostProcessSteps.CalculateTangentSpace | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.ImproveCacheLocality | PostProcessSteps.RemoveRedundantMaterials);

            string baseDir = Path.GetDirectoryName(filename);

            List<string> texs = new List<string>();
            List<float[]> Vertices = new List<float[]>();
            List<float[]> UV = new List<float[]>();
            List<float[]> Normals = new List<float[]>();
            List<uint[]> Indices = new List<uint[]>();


            for (int a = 0; a < model.MeshCount; a++)
            {
                string t;
                Mesh m = model.Meshes[a];


                if (m.MaterialIndex >= 0 && model.Materials[m.MaterialIndex].TextureDiffuse.FilePath != null) t = Path.Combine(baseDir, model.Materials[m.MaterialIndex].TextureDiffuse.FilePath);
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
                Vertices.Add(vertices);
                
                if (m.GetIndices() != null)
                {
                    uint[] indices = Array.ConvertAll(m.GetIndices(), x => (uint)x);
                    Indices.Add(indices);
                }

                if (m.TextureCoordinateChannels.Length > 0 && m.TextureCoordinateChannels[0].Count > 0)
                {
                    float[] texcoords = new float[m.VertexCount * 2];
                    for (int v = 0; v < m.VertexCount * 2; v += 2)
                    {
                        texcoords[v] = m.TextureCoordinateChannels[0][(v - (v % 2)) / 2].X;
                        texcoords[v + 1] = m.TextureCoordinateChannels[0][(v - (v % 2)) / 2].Y;
                    }
                    UV.Add(texcoords);
                }
                else
                {
                    float[] texcoords = new float[m.VertexCount * 2];
                    for (int v = 0; v < m.VertexCount * 2; v += 2)
                    {
                        texcoords[v] = m.Vertices[(v - (v % 2)) / 2].X;
                        texcoords[v + 1] = m.Vertices[(v - (v % 2)) / 2].Y;
                    }
                    UV.Add(texcoords);
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
                    Normals.Add(normals);
                }
            }
            return new Tuple<VertexBufferLL[], Texture[]>(vbufs.ToArray(), texs.ToArray());
 
        }
    }
}

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
            List<List<float>[]> VertexWeights = new List<List<float>[]>();
            List<List<int>[]> VertexBones = new List<List<int>[]>();
            List<float[][]> SkeletonBones = new List<float[][]>();


            for (int a = 0; a < model.MeshCount; a++)
            {
                string t;
                Mesh m = model.Meshes[a];

                #region Textures
                if (m.MaterialIndex >= 0 && model.Materials[m.MaterialIndex].TextureDiffuse.FilePath != null) t = Path.Combine(baseDir, model.Materials[m.MaterialIndex].TextureDiffuse.FilePath);
                else
                {
                    t = null;
                }
                texs.Add(t);
                #endregion

                tmpBound.Min = new Vector3(m.Vertices[0].X, m.Vertices[0].Y, m.Vertices[0].Z);
                tmpBound.Max = new Vector3(m.Vertices[0].X, m.Vertices[0].Y, m.Vertices[0].Z);

                #region Vertices
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
                }
                Vertices.Add(vertices);
                #endregion

                //Handle bones
                #region Bones
                if (m.HasBones)
                {
                    List<float>[] Weights = new List<float>[m.VertexCount];
                    List<int>[] Bones = new List<int>[m.VertexCount];
                    for (int b = 0; b < m.BoneCount; b++)
                    {
                        //Save the offset matrix
                        SkeletonBones.Add(
                            new float[][] { 
                                new float[]{ m.Bones[b].OffsetMatrix.A1,m.Bones[b].OffsetMatrix.A2,m.Bones[b].OffsetMatrix.A3,m.Bones[b].OffsetMatrix.A4  },
                                new float[]{ m.Bones[b].OffsetMatrix.B1,m.Bones[b].OffsetMatrix.B2,m.Bones[b].OffsetMatrix.B3,m.Bones[b].OffsetMatrix.B4,},
                                new float[]{ m.Bones[b].OffsetMatrix.C1,m.Bones[b].OffsetMatrix.C2,m.Bones[b].OffsetMatrix.C3,m.Bones[b].OffsetMatrix.C4,},
                                new float[]{ m.Bones[b].OffsetMatrix.D1,m.Bones[b].OffsetMatrix.D2,m.Bones[b].OffsetMatrix.D3,m.Bones[b].OffsetMatrix.D4,}
                            });


                        foreach (VertexWeight w in m.Bones[b].VertexWeights)
                        {
                            //Save the vertex weight
                            if (Weights[w.VertexID] == null) Weights[w.VertexID] = new List<float>();
                            Weights[w.VertexID].Add(w.Weight);

                            //Save the bone id
                            if (Bones[w.VertexID] == null) Bones[w.VertexID] = new List<int>();
                            Bones[w.VertexID].Add(b);
                        }

                    }
                    VertexBones.Add(Bones);
                    VertexWeights.Add(Weights);
                }
                #endregion

                //Handle indices
                #region Indices
                if (m.GetIndices() != null)
                {
                    uint[] indices = Array.ConvertAll(m.GetIndices(), x => (uint)x);
                    Indices.Add(indices);
                }
                #endregion

                #region Texture Coordinates
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
                #endregion

                #region Normals
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
                #endregion
            }

            //Now that we have all the data in a closer to what we want format, restructure it so that it can be drawn in a single draw call
            #region Final conversion steps
            float[][][] vweights = new float[VertexWeights.Count][][];
            for (int a0 = 0; a0 < VertexWeights.Count; a0++)
            {
                for (int a1 = 0; a1 < VertexWeights[a0].Length; a1++)
                {
                    vweights[a0][a1] = VertexWeights[a0][a1].ToArray();
                }
            }

            int[][][] vbones = new int[VertexBones.Count][][];
            for (int a0 = 0; a0 < VertexBones.Count; a0++)
            {
                for (int a1 = 0; a1 < VertexBones[a0].Length; a1++)
                {
                    vbones[a0][a1] = VertexBones[a0][a1].ToArray();
                }
            }

            return FinalProcess(texs.ToArray(), Vertices.ToArray(), UV.ToArray(), Normals.ToArray(),
                Indices.ToArray(), vweights, vbones, SkeletonBones.ToArray());
            #endregion
        }


        private static byte[] FinalProcess(string[] tex, float[][] verts, float[][] uvs, float[][] norms,
            uint[][] indices, float[][][] weights, int[][][] bones, float[][][] skeleton)
        {
            byte[] outdata;

            using (MemoryStream strm = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(strm))
                {
                    writer.Write(new char[] { 'A', 'K', '3', 'D' }, 0, 4);    //Write the magic number "AK3D"
                    writer.Write(indices.Length);    //Get the total number of models in question
                    writer.Write(verts.Length);       //Get the total number of vertices in question
                    writer.Write(uvs.Length);         //Get the total number of UVs in question
                    writer.Write(norms.Length);       //Get the total number of normals available
                    writer.Write(weights.Length);
                    writer.Write(bones.Length);
                    writer.Write(skeleton.Length);

                    //We are done writing all the first dimensions, now write second dimensions
                    for (int i = 0; i < indices.Length; i++)
                    {
                        writer.Write(indices[i].Length);    //Write all the lengths
                    }

                    //We are done writing all the first dimensions, now write second dimensions
                    for (int i = 0; i < verts.Length; i++)
                    {
                        writer.Write(verts[i].Length);    //Write all the lengths
                    }

                    //We are done writing all the first dimensions, now write second dimensions
                    for (int i = 0; i < uvs.Length; i++)
                    {
                        writer.Write(uvs[i].Length);    //Write all the lengths
                    }

                    //We are done writing all the first dimensions, now write second dimensions
                    for (int i = 0; i < norms.Length; i++)
                    {
                        writer.Write(norms[i].Length);    //Write all the lengths
                    }

                    //We are done writing all the first dimensions, now write second dimensions
                    for (int i = 0; i < weights.Length; i++)
                    {
                        writer.Write(weights[i].Length);    //Write all the lengths
                    }

                    //We are done writing all the first dimensions, now write second dimensions
                    for (int i = 0; i < bones.Length; i++)
                    {
                        writer.Write(bones[i].Length);    //Write all the lengths
                    }

                    //We are done writing all the first dimensions, now write second dimensions
                    for (int i = 0; i < skeleton.Length; i++)
                    {
                        writer.Write(skeleton[i].Length);    //Write all the lengths
                    }
                }
                outdata = strm.ToArray();
            }

            return outdata;
        }

    }
}

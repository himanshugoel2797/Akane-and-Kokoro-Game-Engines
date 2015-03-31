using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Kokoro.Engine.Prefabs
{
    public class VertexMesh : Model
    {
        [ProtoContract]
        struct Model_m
        {
            [ProtoMember(0)]
            public float[][] vertices;

            [ProtoMember(1)]
            public float[][] BoundingBox;

            [ProtoMember(2)]
            public float[][] uvs;

            [ProtoMember(4)]
            public float[][] normals;

            [ProtoMember(8)]
            public uint[][] indices;

            [ProtoMember(16)]
            public float[][][] weights;

            [ProtoMember(32)]
            public int[][][] bones;

            [ProtoMember(64)]
            public float[][][] skeleton;

            [ProtoMember(128)]
            public string[] texPaths;
        }

        public VertexMesh(string filename)
        {
            Model_m tmp = Serializer.Deserialize<Model_m>(VFS.FSReader.OpenFile(filename, false));

            Init(tmp.indices.Length);
            for (int i = 0; i < tmp.indices.Length; i++)
            {
                SetIndices(UpdateMode.Dynamic, tmp.indices[i], i);
                SetVertices(UpdateMode.Dynamic, tmp.vertices[i], i);
                SetUVs(UpdateMode.Dynamic, tmp.uvs[i], i);
                SetNormals(UpdateMode.Dynamic, tmp.normals[i], i);
            }

            Bound = new BoundingBox()
            {
                Min = new Math.Vector3(tmp.BoundingBox[0][0], tmp.BoundingBox[0][1], tmp.BoundingBox[0][2]),
                Max = new Math.Vector3(tmp.BoundingBox[1][0], tmp.BoundingBox[1][1], tmp.BoundingBox[1][2]),
                Up = Math.Vector3.UnitY
            };

        }

    }
}

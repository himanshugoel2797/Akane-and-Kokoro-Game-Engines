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
        #region Model Structures
        [ProtoContract]
        class MeshInfo_m
        {
            [ProtoMember(1)]
            public float[] Vertices;

            [ProtoMember(2)]
            public float[] uvs;

            [ProtoMember(3)]
            public float[] normals;

            [ProtoMember(4)]
            public uint[] indices;

            [ProtoMember(5)]
            public string tex;

            [ProtoMember(6)]
            public Weight_m[] Weights;

            [ProtoMember(7)]
            public Bone_m[] Bones;

            [ProtoMember(8)]
            public Skeleton_m[] skeleton;
        }

        [ProtoContract]
        class Weight_m
        {
            [ProtoMember(1)]
            public float[] Weights;
        }

        [ProtoContract]
        class Bone_m
        {
            [ProtoMember(1)]
            public int[] Bones;
        }

        [ProtoContract]
        class Skeleton_m
        {
            [ProtoMember(1)]
            public float[] Skeletons;
        }


        [ProtoContract]
        class Model_m
        {
            [ProtoMember(1)]
            public MeshInfo_m[] Mesh;

            [ProtoMember(2)]
            public float[] BoundingBox;

        }
        #endregion

        private bool animatedMesh;

        public VertexMesh(string filename, bool animated)
        {
            Model_m tmp = Serializer.Deserialize<Model_m>(VFS.FSReader.OpenFile(filename, false));
            animatedMesh = animated;

            Init(tmp.Mesh.Length);
            for (int i = 0; i < tmp.Mesh.Length; i++)
            {
                SetIndices(UpdateMode.Static, tmp.Mesh[i].indices, i);
                SetVertices(UpdateMode.Static, tmp.Mesh[i].Vertices, i);
                SetUVs(UpdateMode.Static, tmp.Mesh[i].uvs, i);
                SetNormals(UpdateMode.Static, tmp.Mesh[i].normals, i);
            }

            Bound = new BoundingBox()
            {
                Min = new Math.Vector3(tmp.BoundingBox[0], tmp.BoundingBox[1], tmp.BoundingBox[2]),
                Max = new Math.Vector3(tmp.BoundingBox[3], tmp.BoundingBox[4], tmp.BoundingBox[5]),
                Up = Math.Vector3.UnitY
            };

        }

    }
}

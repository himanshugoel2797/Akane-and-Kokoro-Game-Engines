using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.Prefabs
{
    /// <summary>
    /// Represents an Axis Aligned Bounding Box
    /// </summary>
    public class AABB : Model
    {
        /// <summary>
        /// Create a new AABB object
        /// </summary>
        /// <param name="box">The bounding box as calculated from another Model</param>
        public AABB(BoundingBox box)
        {
            this.World = Matrix4.CreateTranslation((box.Min + box.Max) / 2) * Matrix4.Scale(box.Max - box.Min);
            this.DrawMode = Engine.DrawMode.Lines;
            vbufs = new OpenGL.PC.VertexBufferLL[1];
            vbufs[0] = new OpenGL.PC.VertexBufferLL();

            vbufs[0].SetVertices(new float[]{
                -0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f,  0.5f, -0.5f,
                -0.5f,  0.5f, -0.5f,
                -0.5f, -0.5f,  0.5f,
                0.5f, -0.5f,  0.5f,
                0.5f,  0.5f,  0.5f,
                -0.5f,  0.5f,  0.5f
            });

            vbufs[0].SetIndices(new ushort[] {
                0, 1,
                1, 2,
                2, 3,
                3, 0,
                4, 5,
                5, 6,
                6, 7,
                7, 4,
                0, 4,
                1, 5,
                2, 6,
                3, 7
            });

            vbufs[0].SetUVs(new float[]{
                -0.5f, -0.5f,
                0.5f, -0.5f,
                0.5f,  0.5f,
                -0.5f,  0.5f,
                -0.5f, -0.5f,
                0.5f, -0.5f,
                0.5f,  0.5f,
                -0.5f,  0.5f
            });

        }
    }
}

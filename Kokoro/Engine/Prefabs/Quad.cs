using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Math;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine.Prefabs
{
    /// <summary>
    /// Represents a Unit quad
    /// </summary>
    public class Quad : Model
    {
        /// <summary>
        /// Create a new unit quad
        /// </summary>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        /// <param name="width">The width of the unit quad</param>
        /// <param name="height">The height of the unit quad</param>
        /// <param name="tex">An optional texture to be applied to the quad</param>
        public Quad(float x, float y, float width, float height, Texture tex = null) : base()
        {
            filepath = "";
            this.DrawMode = DrawMode.Triangles;
            vbufs = new VertexBufferLL[1];
            vbufs[0] = new VertexBufferLL();

            vbufs[0].SetIndices(new ushort[] { 3, 2, 0, 0, 2, 1 });
            vbufs[0].SetUVs(new float[] { 
                0,1,
                1,1,
                1,0,
                0,0
            });

            vbufs[0].SetVertices(new float[]{
                x, 0, y + height,
                x + width, 0, y + height,
                x + width, 0, y,
                x, 0, y
            });
            Materials[0] = new Material { ColorMap = tex };

            World = Matrix4.Identity;
        }

    }
}

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
    public class Quad : Model
    {
        public Quad(float x, float y, float width, float height, Texture tex = null)
        {
            filepath = "";
            this.DrawMode = DrawMode.Triangles;
            vbufs = new VertexBufferLL[1];
            vbufs[0] = new VertexBufferLL();

            Materials = new Material[1];

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
            Materials[0] = new Material { Diffuse = tex };

            World = Matrix4.Identity;
        }

    }
}

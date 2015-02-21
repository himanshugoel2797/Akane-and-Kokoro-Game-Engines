using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Engine.Shaders;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine.Prefabs
{
    public class FullScreenQuad : Model
    {
        public FullScreenQuad() : base()
        {
            vbufs = new VertexBufferLL[1];
            vbufs[0] = new VertexBufferLL();

            this.DrawMode = DrawMode.Triangles;

            vbufs[0].SetIndices(new ushort[] { 3, 2, 0, 0, 2, 1 });
            vbufs[0].SetUVs(new float[] {
                0,1,
                1,1,
                1,0,
                0,0
            });

            vbufs[0].SetVertices(new float[]{
                -1, 1, 0.5f,
                1, 1, 0.5f,
                1, -1,0.5f,
                -1, -1,0.5f
            });

            World = Math.Matrix4.Identity;
            Materials[0].Shader = new ShaderProgram(ShaderLib.FrameBufferShader.Create(ShaderTypes.Vertex), ShaderLib.DefaultShader.Create(ShaderTypes.Fragment));
        }
    }
}

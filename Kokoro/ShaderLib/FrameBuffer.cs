using Kokoro.Engine.Shaders;
using Kokoro.KSL;
using Kokoro.KSL.Lib;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.ShaderLib
{
    public class FrameBufferShader : IKShaderProgram
    {
        public void Fragment()
        {
            var Vars = Manager.ShaderStart("FrameBuf_S_Frag");
            Manager.SharedIn<Vec2>("UV");
            Manager.StreamOut<Vec4>("Color", 0);

            Vars.Color = Texture.Read2D(Vars.ColorMap, Vars.UV);
        }

        public void Vertex()
        {
            var Vars = Manager.ShaderStart("FrameBuf_S_Vert");
            Manager.StreamIn<Vec3>("VertexPos", 0);
            Manager.StreamIn<Vec2>("UV0", 2);

            Manager.SharedOut<Vec2>("UV");

            Vars.VertexPosition.Construct(Vars.VertexPos, 1);
            Vars.UV = Vars.UV0;
        }

        public static HLShader Create(ShaderTypes t)
        {
            return new HLShader()
            {
                Shader = new FrameBufferShader(),
                ShaderType = t
            };
        }
    }
}

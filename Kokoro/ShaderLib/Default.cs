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
    public class DefaultShader : IKShaderProgram
    {
        public void Fragment()
        {
            var Vars = Manager.ShaderStart();
            Manager.SharedIn<Vec2>("UV");
            Manager.StreamOut<Vec4>("Color", 0);

            Vars.Color = Texture.Read2D(Vars.ColorMap, Vars.UV);

			Manager.ShaderEnd();
        }

        public void Vertex()
        {
            var Vars = Manager.ShaderStart();
            Manager.StreamIn<Vec3>("VertexPos", 0);
            Manager.StreamIn<Vec2>("UV0", 1);

            Manager.SharedOut<Vec2>("UV");
            Manager.Create<Mat4>("MVP");

            Vars.MVP = Vars.Projection * Vars.View * Vars.World;
            Vars.VertexPosition.Construct(Vars.VertexPos, 1);
            Vars.VertexPosition *= Vars.MVP;
            Vars.UV = Vars.UV0;

			Manager.ShaderEnd();
        }

        public static HLShader Create(ShaderTypes t)
        {
            return new HLShader()
            {
                Shader = new DefaultShader(),
                ShaderType = t
            };
        }
    }
}

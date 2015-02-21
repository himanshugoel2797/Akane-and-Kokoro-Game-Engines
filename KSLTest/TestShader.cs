using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.KSL;
using Kokoro.KSL.Lib.General;
using Kokoro.KSL.Lib.Texture;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib;

namespace KSLTest
{
    class TestShader : IKShaderProgram
    {
        public void Fragment()
        {

        }

        public void Vertex()
        {
            dynamic Variables = Manager.ShaderStart();
            Manager.StreamIn<Vec4>("vertexPos", 0);
            Manager.StreamIn<Vec2>("UV", 1);
            Manager.StreamIn<Vec3>("Normals", 2);
            Manager.StreamIn<Vec3>("Tangents", 3);

            Manager.StreamOut<Vec4>("Color", 0);

            Manager.Uniform<Mat4>("World");
            Manager.Uniform<Mat4>("View");
            Manager.Uniform<Mat4>("Projection");
            Manager.Uniform<Sampler2D>("ColorMap");


            Manager.Uniform<KInt>("k");
            Manager.Create<KInt>("l");
            Manager.Create<Vec4>("pos");
            Manager.Create<Mat4>("WVP");

            Variables.WVP = Variables.Projection * Variables.View * Variables.World;
            Variables.VertexPosition = Variables.vertexPos * Variables.WVP;
            Variables.Color = Texture.Read2D(Variables.ColorMap, Variables.UV);
        }
    }
}

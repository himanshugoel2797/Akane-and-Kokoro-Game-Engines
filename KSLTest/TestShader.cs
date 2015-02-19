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


            Manager.Uniform<KInt>("k");
            Manager.Create<KInt>("l");
            Manager.Create<Vec4>("pos");

            R(Variables);
        }

        public void R(dynamic Variables)
        {
            Variables.l = (KInt)5;
            Variables.k *= Variables.l;
            Variables.color = (Vec4)Variables.k;
            Variables.pos = Variables.color;
        }
    }
}

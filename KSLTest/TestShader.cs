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
            Manager.StreamOut<Vec4>("color", 0);
            Manager.Uniform<KInt>("k");

            Manager.Create<KInt>("l", Variables.k);

            Variables.l += 5;
            Variables.color = Variables.l ;

        }

        public void R()
        {

        }
    }
}

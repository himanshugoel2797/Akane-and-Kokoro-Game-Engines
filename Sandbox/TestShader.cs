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

namespace Sandbox
{
    class TestShader : IKShaderProgram
    {
        public Vec4 Fragment()
        {
            return new Vec4(0);
        }

        public Vec4 Vertex()
        {
            return new Vec4(1);
        }
    }
}

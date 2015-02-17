using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kokoro.Engine.Shaders
{
    public class FragmentShader : Shader
    {
        public FragmentShader(string fshader) : base(fshader, ShaderTypes.Fragment){  }

    }
}

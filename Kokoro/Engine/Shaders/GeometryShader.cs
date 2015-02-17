using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kokoro.Engine.Shaders
{
    public class GeometryShader : Shader
    {
        public GeometryShader(string fshader) : base(fshader, ShaderTypes.Geometry) { }
    }
}

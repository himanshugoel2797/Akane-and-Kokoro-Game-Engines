using Kokoro.KSL.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.Shaders
{
    public struct HLShader
    {
        public ShaderTypes ShaderType;
        public IKShaderProgram Shader;
    }
}

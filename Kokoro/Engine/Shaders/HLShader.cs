using Kokoro.KSL;
using Kokoro.KSL.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.Shaders
{
    /// <summary>
    /// Represents a KSL shader
    /// </summary>
    public struct HLShader
    {
        /// <summary>
        /// The Shader stage this shader represents
        /// </summary>
        public ShaderTypes ShaderType;

        /// <summary>
        /// The KSL Shader object
        /// </summary>
        public IKShaderProgram Shader;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine.Shaders
{
    public enum ShaderTypes
    {
        Vertex = 0, Fragment = 4, Geometry = 3, TessellationControl = 1, TessellationEval = 2
    }

    /// <summary>
    /// A Shader Program Object
    /// </summary>
    public class Shader : ShaderLL
    {
        internal ShaderTypes GetShaderType()
        {
            return pGetShaderType();
        }

        internal int GetID()
        {
            return pGetID();
        }
    }
}

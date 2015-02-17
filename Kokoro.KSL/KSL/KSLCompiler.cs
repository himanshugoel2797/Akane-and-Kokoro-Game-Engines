using Kokoro.KSL.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if GLSL
using Kokoro.KSL.GLSL;
#if PC
using Kokoro.KSL.GLSL.PC;
#endif
#endif

namespace Kokoro.KSL
{
    public class KSLCompiler
    {
        public enum KShaderType
        {
            Vertex = 0, Fragment = 4, Geometry = 3, TessellationControl = 1, TessellationEval = 2, TessellationComb = 5
        }

        public static string Compile(IKShaderProgram shader, KShaderType s)
        {
            
            //Execute the object and collect the output code from the code generator
            GLSLCodeGenerator.Init();
            shader.Vertex();
            string vshader = GLSLCodeGenerator.GetShader();

            GLSLCodeGenerator.Init();
            shader.Fragment();
            string fshader = GLSLCodeGenerator.GetShader();

            return null;
        }

    }
}

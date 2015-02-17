using Kokoro.KSL.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC.GLSL;
#endif
#endif

namespace Kokoro.KSL
{
    public class KSLCompiler
    {
        private static Dictionary<string, KShader> ShaderCache = new Dictionary<string, KShader>();

        public static KShader Compile(IKShaderProgram shader)
        {
            //Execute the object and collect the output code from the code generator
            CodeGenerator.Init();
            shader.Vertex();
            string vshader = CodeGenerator.GetShader();

            CodeGenerator.Init();
            shader.Fragment();
            string fshader = CodeGenerator.GetShader();

            return null;
        }

    }
}

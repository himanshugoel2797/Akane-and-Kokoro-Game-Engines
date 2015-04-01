using Kokoro.KSL.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if GLSL
using CodeGenerator = Kokoro.KSL.GLSL.GLSLCodeGenerator;
#if PC
using Kokoro.KSL.GLSL.PC;
#endif
#endif

namespace Kokoro.KSL
{
    public class KSLCompiler
    {
        internal static Dictionary<string, Obj> preDefUniforms = new Dictionary<string, Obj>();

        public enum KShaderType
        {
            Vertex = 0, Fragment = 4, Geometry = 3, TessellationControl = 1, TessellationEval = 2, TessellationComb = 5
        }

        public static void Initialize()
        {
            Manager.Init();
        }

        /// <summary>
        /// Register a predefined uniform name to be made available to shaders
        /// </summary>
        /// <typeparam name="T">The type of the uniform</typeparam>
        /// <param name="name">The name of the uniform</param>
        public static void RegisterPreDefinedUniform<T>(string name) where T : Obj, new()
        {
            preDefUniforms.Add(name, new T());
        }

        /// <summary>
        /// Unregister a predefined uniform
        /// </summary>
        /// <param name="name">The name of the uniform to unregister</param>
        public static void UnRegisterPreDefinedUniform(string name)
        {
            preDefUniforms.Remove(name);
        }

        //Generate the shader header with params in UBO/SSBO format with given number of subroutines, this functions should be called in the end to generate the header for hte ubershader
        /// <summary>
        /// Generate the header for the shader (inputs outputs)
        /// </summary>
        /// <param name="num">The number of materials the shader needs to support</param>
        /// <param name="s">The shader type for which to generate the header</param>
        /// <returns></returns>
        public static string GenerateHeader(KShaderType s, int num = 0)
        {
            return CodeGenerator.GenerateHeader(s, num);      //TODO make it generate the code for shader subroutines
        }

        /// <summary>
        /// Compile a shader program into its platform dependent equivalent
        /// </summary>
        /// <param name="shader">The shader to compile</param>
        /// <param name="s">The shader type to compile</param>
        /// <returns>The string representation of the shader in the language specified during build</returns>
        public static string Compile(IKShaderProgram shader, KShaderType s)
        {
            //Execute the object and collect the output code from the code generator
            switch (s)
            {
                case KShaderType.Fragment:
                    shader.Fragment();
                    break;
            }

            string vshader = CodeGenerator.CompileFromSyntaxTree(s);
            return vshader;
        }

        /// <summary>
        /// Compile the main component of an ubershader
        /// </summary>
        /// <param name="shader">The shader to compile</param>
        /// <param name="s">The shader type to compile</param>
        /// <param name="num">The number of possible subroutines to make available</param>
        /// <returns>The string representation of the shader in the language specified during build</returns>
        public static string Compile(IKUbershader shader, KShaderType s, int num = 0)
        {
            switch (s)
            {
                case KShaderType.Fragment:
                    shader.Fragment(num);
                    break;
                case KShaderType.Vertex:
                    shader.Vertex();
                    break;
            }

            SyntaxTree.ShaderName = "main"; //Ubershader methods are always called 'main'
            string vshader = CodeGenerator.CompileFromSyntaxTree(s);
            return vshader;
        }

    }
}

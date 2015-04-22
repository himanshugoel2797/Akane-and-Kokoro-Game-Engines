using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Kokoro.Math;
using Kokoro.KSL;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib.Texture;
#endif
#endif

namespace Kokoro.Engine.Shaders
{
    /// <summary>
    /// A Program consisting of shader stages
    /// </summary>
    public class ShaderProgram : ShaderProgramLL, IDisposable
    {
        static ShaderProgram()
        {
            KSL.KSLCompiler.RegisterPreDefinedUniform<Sampler2D>("ColorMap");
            KSL.KSLCompiler.RegisterPreDefinedUniform<Sampler2D>("LightingMap");
            KSL.KSLCompiler.RegisterPreDefinedUniform<Sampler2D>("NormalMap");

            KSL.KSLCompiler.RegisterPreDefinedUniform<Mat4>("World");
            KSL.KSLCompiler.RegisterPreDefinedUniform<Mat4>("View");
            KSL.KSLCompiler.RegisterPreDefinedUniform<Mat4>("Projection");

            KSL.KSLCompiler.RegisterPreDefinedUniform<KFloat>("ZNear");
            KSL.KSLCompiler.RegisterPreDefinedUniform<KFloat>("ZFar");
        }

        /// <summary>
        /// Called before the application of the shader program
        /// </summary>
        public Action<GraphicsContext, ShaderProgram> PreApply { get; set; }

        /// <summary>
        /// Create a new instance of a ShaderProgram using the platform specific language
        /// </summary>
        /// <param name="shaders">The shaders</param>
        public ShaderProgram(params Shader[] shaders) : base(shaders) { }

        /// <summary>
        /// Create a new instance of a ShaderProgram using the provided KSL shader stages
        /// </summary>
        /// <param name="shaders">The shaders</param>
        public ShaderProgram(params HLShader[] shaders) : base(PreProcess(shaders)) { }

        /// <summary>
        /// Create a new instance of a ShaderProgram using all the stages of the provided KSL shader object
        /// </summary>
        /// <param name="shader">The shaders</param>
        public ShaderProgram(Ubershader shader) : base(PreProcess(shader)) { }

        //Preprocess arguments before passing them to the base constructor
        private static Shader[] PreProcess(HLShader[] shaders)
        {
            List<Shader> compiledshaders = new List<Shader>();

            foreach (HLShader s in shaders)
            {
                switch (s.ShaderType)
                {
                    case ShaderTypes.Vertex:
                        compiledshaders.Add(new VertexShader(KSL.KSLCompiler.Compile(s.Shader, (KSL.KSLCompiler.KShaderType)(int)s.ShaderType)));
                        break;
                    case ShaderTypes.Fragment:
                        compiledshaders.Add(new FragmentShader(KSL.KSLCompiler.Compile(s.Shader, (KSL.KSLCompiler.KShaderType)(int)s.ShaderType)));
                        break;
                    case ShaderTypes.Geometry:
                        compiledshaders.Add(new GeometryShader(KSL.KSLCompiler.Compile(s.Shader, (KSL.KSLCompiler.KShaderType)(int)s.ShaderType)));
                        break;
                }
            }
            return compiledshaders.ToArray();
        }
        private static Shader[] PreProcess(Ubershader shader)
        {
            //Combine all the pieces together from the ubershader
            int subCount = shader.Subroutines.Count;
            string[] fragmentSubroutines = new string[subCount];

            KSL.KSLCompiler.Initialize();
            string vshader = KSL.KSLCompiler.Compile(shader.UberMain, KSLCompiler.KShaderType.Vertex);
            vshader = KSL.KSLCompiler.GenerateHeader(KSLCompiler.KShaderType.Vertex) + vshader;

            KSL.KSLCompiler.Initialize();
            string fshader = KSL.KSLCompiler.Compile(shader.UberMain, KSLCompiler.KShaderType.Fragment, subCount);
            string fragHeader = KSL.KSLCompiler.GenerateHeader(KSLCompiler.KShaderType.Fragment, subCount);

            for (int i = 0; i < subCount; i++)
            {
                fragmentSubroutines[i] = KSL.KSLCompiler.Compile(shader.Subroutines[i], KSLCompiler.KShaderType.Fragment);
                fragHeader += fragmentSubroutines[i];
            }

            return new Shader[]{
                new VertexShader(vshader),
                new FragmentShader(fragHeader + fshader)
            };
        }

        /// <summary>
        /// Apply the shader program
        /// </summary>
        /// <param name="context">The current GraphicsContext</param>
        public void Apply(GraphicsContext context)
        {
            if (PreApply != null) PreApply(context, this);
            base.sApply(context);
        }

        /// <summary>
        /// Clean up after applying the shader program
        /// </summary>
        /// <param name="context">The current GraphicsContext</param>
        public void Cleanup(GraphicsContext context)
        {
            base.sCleanup(context);
        }

        /// <summary>
        /// Set the value of a shader uniform
        /// </summary>
        /// <param name="name">The uniform name</param>
        /// <returns>The value of the uniform</returns>
        public object this[string name]
        {
            set
            {
                Type t = value.GetType();

                if (t == typeof(bool)) SetShaderBool(name, (bool)value);
                else if (t == typeof(Matrix4)) SetShaderMatrix(name, (Matrix4)value);
                else if (t == typeof(Matrix3)) SetShaderMatrix(name, (Matrix3)value);
                else if (t == typeof(Matrix2)) SetShaderMatrix(name, (Matrix2)value);
                else if (t == typeof(Vector4)) SetShaderVector(name, (Vector4)value);
                else if (t == typeof(Vector3)) SetShaderVector(name, (Vector3)value);
                else if (t == typeof(Vector2)) SetShaderVector(name, (Vector2)value);
                else if (t == typeof(float)) SetShaderFloat(name, (float)value);
                else if (t == typeof(double)) SetShaderFloat(name, (float)(double)value);
                else if (t == typeof(int)) SetShaderFloat(name, (float)(int)value);
                else if (t == typeof(Texture)) SetTexture(name, (Texture)value);
                else if (t == typeof(FrameBufferTexture)) SetTexture(name, (Texture)value);
                else throw new Exception("Unknown type " + name);
            }
        }

        public void SetShaderBool(string name, bool val)
        {
            base.aSetShaderBool(name, val);
        }

        public void SetShaderMatrix(string name, Matrix4 val)
        {
            base.aSetShaderMatrix(name, val);
        }

        public void SetShaderMatrix(string name, Matrix2 val)
        {
            base.aSetShaderMatrix(name, val);
        }

        public void SetShaderMatrix(string name, Matrix3 val)
        {
            base.aSetShaderMatrix(name, val);
        }

        public void SetShaderVector(string name, Vector4 val)
        {
            base.aSetShaderVector(name, val);
        }

        public void SetShaderVector(string name, Vector3 val)
        {
            base.aSetShaderVector(name, val);
        }

        public void SetShaderVector(string name, Vector2 val)
        {
            base.aSetShaderVector(name, val);
        }

        public void SetShaderFloat(string name, float val)
        {
            base.aSetShaderFloat(name, val);
        }

        public void SetTexture(string name, Texture tex)
        {
            base.aSetTexture(name, tex);
        }
    }

}

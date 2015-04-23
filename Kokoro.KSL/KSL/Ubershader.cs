using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.KSL.Lib.General;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib;
using Kokoro.KSL.Lib.Texture;

#if GLSL
using CodeGenerator = Kokoro.KSL.GLSL.GLSLCodeGenerator;
#if PC
using Kokoro.KSL.GLSL.PC;
#endif
#endif

namespace Kokoro.KSL
{
    public class Ubershader
    {
        public enum ShaderMode
        {
            Both = 0, None, Vertex, Fragment,
        }

        public string Name { get; set; }
        List<IKShaderProgram> Shaders;
        List<ShaderMode> ShaderEnabled;
        Dictionary<string, Type> uniforms;

        static bool backEndInited = false;

        public Ubershader(params IKShaderProgram[] shaders)
        {
            if (!backEndInited)
            {
                KSLCompiler.Initialize();
                KSL.KSLCompiler.RegisterPreDefinedUniform<Sampler2D>("ColorMap");
                KSL.KSLCompiler.RegisterPreDefinedUniform<Sampler2D>("LightingMap");
                KSL.KSLCompiler.RegisterPreDefinedUniform<Sampler2D>("NormalMap");

                KSL.KSLCompiler.RegisterPreDefinedUniform<Mat4>("World");
                KSL.KSLCompiler.RegisterPreDefinedUniform<Mat4>("View");
                KSL.KSLCompiler.RegisterPreDefinedUniform<Mat4>("Projection");

                KSL.KSLCompiler.RegisterPreDefinedUniform<KFloat>("ZNear");
                KSL.KSLCompiler.RegisterPreDefinedUniform<KFloat>("ZFar");
                Manager.Init(); //Initialize the manager
                backEndInited = true;
            }

            Shaders = new List<IKShaderProgram>();
            ShaderEnabled = new List<ShaderMode>();
            uniforms = new Dictionary<string, Type>();

            Shaders.AddRange(shaders);
            for (int i = 0; i < shaders.Length; i++)
            {
                ShaderEnabled.Add(ShaderMode.Both);
                shaders[i].Vertex();    //Just execute both so the uniform data becomes available
                shaders[i].Fragment();
                var tmp = shaders[i].GetUniforms();
                if (tmp != null)
                {
                    foreach (KeyValuePair<string, Type> entry in tmp)
                    {
                        //TODO maybe we should do some duplication error checking here
                        uniforms[entry.Key] = entry.Value;
                    }
                }
            }
        }

        public Ubershader()
        {
            Shaders = new List<IKShaderProgram>();
            ShaderEnabled = new List<ShaderMode>();
            uniforms = new Dictionary<string, Type>();
        }

        public IKShaderProgram this[int index]
        {
            get
            {
                return Shaders[index];
            }
            set
            {
                if (value != null)
                {
                    value.Vertex();
                    value.Fragment();
                    var tmp = value.GetUniforms();
                    foreach (KeyValuePair<string, Type> entry in tmp)
                    {
                        //TODO maybe we should do some duplication error checking here
                        uniforms[entry.Key] = entry.Value;
                    }
                    //TODO how do we deal with the fragmentation due to removal? maybe we should make it impossible to null out a shader
                    Shaders[index] = value;
                }
                else
                {
                    throw new InvalidOperationException("A shader may not be set to 'null'. If you wish to disable the shader, please access the 'ShaderEnabled' field");
                }
            }
        }

        public void SetShaderStatus(int index, ShaderMode mode)
        {
            ShaderEnabled[index] = mode;
        }

        public ShaderMode GetShaderStatus(int index) { return ShaderEnabled[index]; }

        public string Compile(KSLCompiler.KShaderType shaderType)
        {
            Manager.Init();
            #region Vertex Shader
            if (shaderType == KSLCompiler.KShaderType.Vertex)
            {
                dynamic Var = Manager.ShaderStart(Name + "_vertex");    //Set the shader name
                Manager.UbershaderMode(true);       //Enable ubershader mode

                uniforms.All((a) => { Manager.DeclareUniformFromType(a.Key, a.Value); return true; });  //Declare all uniforms so the correct uniform buffer structure is generated
                Manager.DeclareUniformFromType("ShaderID", typeof(KInt));  //Define the ShaderID as a uniform as well

                //Iterate over all shaders, compiling the ones which have the vertex shader support marked to be enabled
                for (int i = 0; i < Shaders.Count; i++)
                {
                    if (ShaderEnabled[i] == ShaderMode.Both || ShaderEnabled[i] == ShaderMode.Vertex)
                    {
                        Logic.If(new Comparison(Var.ShaderID, Comparisons.Equal, (KInt)i), (d) =>
                        {
                            Shaders[i].Vertex();
                        });
                        Logic.Else();   //Setup the next statement to be an else if statement as well
                    }
                }
                Logic.UndoElse();   //Remove the last else

                Manager.UbershaderMode(false);  //Disable ubershader mode
                return CodeGenerator.GenerateShader(shaderType);    //Generate the shader

            }
            #endregion
            #region Fragment Shader
            else if (shaderType == KSLCompiler.KShaderType.Fragment)
            {
                dynamic Var = Manager.ShaderStart(Name + "_fragment");  //Set the shader name
                Manager.UbershaderMode(true);       //Enable ubershader mode

                uniforms.All((a) => { Manager.DeclareUniformFromType(a.Key, a.Value); return true; });      //Create all uniforms
                Manager.DeclareUniformFromType("ShaderID", typeof(KInt));   //Define the ShaderID uniform

                //Iterate over all shaders, compiling the ones which have fragment shader support marked as enabled
                for (int i = 0; i < Shaders.Count; i++)
                {
                    if (ShaderEnabled[i] == ShaderMode.Both || ShaderEnabled[i] == ShaderMode.Fragment)
                    {
                        Logic.If(new Comparison(Var.ShaderID, Comparisons.Equal, (KInt)i), (d) =>
                        {
                            Shaders[i].Fragment();
                        });
                        Logic.Else();   //Setup the next statement as an else if
                    }
                }
                Logic.UndoElse();   //Undo the last else

                Manager.UbershaderMode(false);  //Disable ubershader mode
                return CodeGenerator.GenerateShader(shaderType);    //Generate and return the shader
            }
            #endregion

            //Other modes have not been implemented yet
            throw new NotImplementedException("Only Vertex shader and fragment shader support is available for now");

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Kokoro.Engine.Shaders
{
    public class TessellationShader : Shader
    {
        internal TessellationEvalShader eval;
        internal TessellationControlShader control;

        public void SetPatchSize(int num)
        {
            base.aSetPatchSize(num);
        }

        public TessellationShader(string controlShader, string evalShader)
        {
            base.shaderType = ShaderTypes.TessellationControl;
            eval = new TessellationEvalShader(evalShader);
            control = new TessellationControlShader(controlShader);
            SetPatchSize(3);
        }

        public TessellationShader(string name) : this(name, name) { }
    }

    class TessellationControlShader : Shader
    {
        private static Dictionary<string, int> tcsShaderDB = new Dictionary<string, int>();

        public TessellationControlShader(string fshader)
        {
            base.shaderType = ShaderTypes.TessellationControl;
            if (!tcsShaderDB.ContainsKey(fshader + "/tessControl.glsl"))
            {

                fshader += "/tessControl.glsl";
                string file = "#version 430 core \n " + File.ReadAllText(fshader);

                id = base.aCreate(base.shaderType, file);
                base.CheckForErrors(fshader, base.shaderType);
                tcsShaderDB.Add(fshader, base.id);
            }
            else
            {
                fshader += "/tessControl.glsl";
                base.id = tcsShaderDB[fshader];
            }
        }

    }

    class TessellationEvalShader : Shader
    {
        private static Dictionary<string, int> tesShaderDB = new Dictionary<string, int>();

        public TessellationEvalShader(string fshader)
        {
            base.shaderType = ShaderTypes.TessellationEval;
            if (!tesShaderDB.ContainsKey(fshader + "/tessEval.glsl"))
            {
                fshader += "/tessEval.glsl";
                string file = "#version 430 core \n " + File.ReadAllText(fshader);
                id = base.aCreate(base.shaderType, file);

                base.CheckForErrors(fshader, base.shaderType);
                tesShaderDB.Add(fshader, base.id);
            }
            else
            {
                fshader += "/tessEval.glsl";
                base.id = tesShaderDB[fshader];
            }
        }

    }
}

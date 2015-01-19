using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kokoro.Engine.Shaders
{
    public class VertexShader : Shader
    {
        private static Dictionary<string, int> vshaderDB = new Dictionary<string, int>();

        public VertexShader(string fshader)
        {
            base.shaderType = ShaderTypes.Vertex;
            if (!vshaderDB.ContainsKey(fshader + "/vertex.glsl"))
            {
                fshader += "/vertex.glsl";
                string file = "#version 430 core \n " + File.ReadAllText(fshader);

                id = base.aCreate(base.shaderType, file);
                base.CheckForErrors(fshader, base.shaderType);

                vshaderDB.Add(fshader, base.id);
            }
            else
            {
                fshader += "/vertex.glsl";
                base.id = vshaderDB[fshader];
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kokoro.Engine.Shaders
{
    public class GeometryShader : Shader
    {
        private static Dictionary<string, int> gshaderDB = new Dictionary<string, int>();

        public GeometryShader(string fshader)
        {
            base.shaderType = ShaderTypes.Geometry;
            if (!gshaderDB.ContainsKey(fshader + "/geometry.glsl"))
            {
                fshader += "/geometry.glsl";
                string file = "#version 430 core \n " + File.ReadAllText(fshader);
                
                base.aCreate(base.shaderType, file);
                base.CheckForErrors(fshader, base.shaderType);

                gshaderDB.Add(fshader, base.id);
            }
            else
            {
                fshader += "/geometry.glsl";
                base.id = gshaderDB[fshader];
            }
        }

    }
}

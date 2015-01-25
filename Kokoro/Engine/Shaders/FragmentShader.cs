using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kokoro.Engine.Shaders
{
    public class FragmentShader : Shader
    {
        private static Dictionary<string, int> fshaderDB = new Dictionary<string, int>();

        public FragmentShader(string fshader)
        {
            base.shaderType = ShaderTypes.Fragment;
            if (!fshaderDB.ContainsKey(fshader + "/fragment.glsl"))
            {
                fshader += "/fragment.glsl";
                string file = "#version 430 core \n " + File.ReadAllText(fshader);

                id = base.aCreate(base.shaderType, file);
                base.CheckForErrors(fshader, base.shaderType);
                fshaderDB.Add(fshader, base.id);
            }
            else
            {
                fshader += "/fragment.glsl";
                base.id = fshaderDB[fshader];
            }
            Kokoro.Debug.ObjectAllocTracker.NewCreated(this, id, "Fragment Shader");
        }

#if DEBUG
        ~FragmentShader()
        {
            Kokoro.Debug.ObjectAllocTracker.ObjectDestroyed(this, id, "Fragment Shader");
        }
#endif
    }
}

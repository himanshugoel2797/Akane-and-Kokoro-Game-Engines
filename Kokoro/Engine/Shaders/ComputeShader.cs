using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.Shaders
{
    //TODO Compute Shaders don't use the same pipeline as other shaders

    public class ComputeShader : Shader
    {
        public ComputeShader(string fshader) : base(fshader, ShaderTypes.Compute) { }

        public static FragmentShader Load(string dir)
        {
            return new FragmentShader(new StreamReader(VFS.FSReader.OpenFile(dir + "/compute.glsl")).ReadToEnd());
        }
    }
}

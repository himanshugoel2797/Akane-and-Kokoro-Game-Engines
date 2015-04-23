using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL
{
    public abstract class IKShaderProgram
    {
        Dictionary<string, Type> uniforms;
        public Dictionary<string, Type> GetUniforms()
        {
            return uniforms;
        }
        public void RequestUniform<T>(string name) where T : Lib.Obj, new()
        {
            if (uniforms == null) uniforms = new Dictionary<string, Type>();
            uniforms[name] = typeof(T);

            Lib.Manager.RequestUniform<T>(name);
        }
        public abstract void Vertex();
        public abstract void Fragment();
    }
}

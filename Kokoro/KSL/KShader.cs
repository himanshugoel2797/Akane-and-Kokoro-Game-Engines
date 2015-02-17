using Kokoro.Engine.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.KSL
{

    /*
    Kokoro shaders optimize themselves by setting all variables specified before compilation as built in constants
    A KShader consists of a library of shader functions through which a shader can be collected and built
    on compile the referenced functions are all loaded up and a shader is put together, this is then compiled to the target platform's shading language and setup for use
    Any variable who's value isn't initially specified is made a uniform
    Use an expandObject to make it more convenient to specify shader variable values
    */

    public class KShader
    {
        public VertexShader Vertex { get; private set; }
        public FragmentShader Fragment { get; private set; }
        public GeometryShader Geometry { get; private set; }

        public ShaderProgram NativeShader { get; private set; }

        public KShader(string shaderName)
        {

        }

        public void SetShaderParam(string name, object value)
        {

        }

        public void Define(string name)
        {

        }

        public void Compile()
        {

        }

        public void Apply()
        {

        }

        public void Cleanup()
        {

        }

    }
}

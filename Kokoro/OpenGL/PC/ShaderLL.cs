#if OPENGL && PC

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Engine;
using Kokoro.Math;
using Kokoro.Sinus;

using OpenTK.Graphics.OpenGL4;

namespace Kokoro.OpenGL.PC
{
    public class ShaderLL : IDisposable
    {
        public ShaderLL()
        {

        }

        protected Engine.Shaders.ShaderTypes shaderType;
        protected int id;
        protected int pGetID()
        {
            return id;
        }

        protected Engine.Shaders.ShaderTypes pGetShaderType()
        {
            return shaderType;
        }

        protected void aSetPatchSize(int num)
        {
            SinusManager.QueueCommand(() => GL.PatchParameter(PatchParameterInt.PatchVertices, num));
        }

        protected void aCreate(Engine.Shaders.ShaderTypes type, string file)
        {
            SinusManager.QueueCommand(() =>
            {
                id = GL.CreateShader(EnumConverters.EShaderTypes(type));
                GL.ShaderSource(id, file);
                GL.CompileShader(id);
            });
        }

        protected void CheckForErrors(string fshader, Engine.Shaders.ShaderTypes type)
        {
            SinusManager.QueueCommand(() =>
            {
                int result = 0;
                GL.GetShader(id, ShaderParameter.CompileStatus, out result);
                if (result != 1) throw new Exception(GL.GetShaderInfoLog(id));
            });
        }

        public void Dispose()
        {
            SinusManager.QueueCommand(() => GL.DeleteShader(id));
        }

    }
}

#endif
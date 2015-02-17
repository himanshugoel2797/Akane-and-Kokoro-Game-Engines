#if OPENGL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Engine;
using Kokoro.Math;

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
            GL.PatchParameter(PatchParameterInt.PatchVertices, num);
        }

        protected int aCreate(Engine.Shaders.ShaderTypes type, string file)
        {
            int id = GL.CreateShader(EnumConverters.EShaderTypes(type));
            GL.ShaderSource(id, file);
            GL.CompileShader(id);
            return id;
        }

        protected void CheckForErrors(string fshader, Engine.Shaders.ShaderTypes type)
        {
            int result = 0;
            GL.GetShader(id, ShaderParameter.CompileStatus, out result);
            if (result != 1) throw new Exception(GL.GetShaderInfoLog(id));
        }

        public void Dispose()
        {
            GL.DeleteShader(id);
        }

    }
}

#endif
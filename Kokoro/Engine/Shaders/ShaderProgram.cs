﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Kokoro.Math;

#if OPENGL
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine.Shaders
{
    public class ShaderProgram : ShaderProgramLL, IDisposable
    {
        public Action<GraphicsContext, ShaderProgram> PreApply { get; set; }

        public ShaderProgram(params Shader[] shaders) : base(shaders) { }

        public ShaderProgram(string directory) : base(ProcessParams(directory)) { }

        private static Shader[] ProcessParams(string file)
        {
            List<Shader> shaders = new List<Shader>();
            if (File.Exists(Path.Combine(file, "vertex.glsl"))) shaders.Add(new VertexShader(file));
            if (File.Exists(Path.Combine(file, "fragment.glsl"))) shaders.Add(new FragmentShader(file));
            if (File.Exists(Path.Combine(file, "geometry.glsl"))) shaders.Add(new GeometryShader(file));
            return shaders.ToArray();
        }

        public void Apply(GraphicsContext context)
        {
            if(PreApply != null)PreApply(context, this);
            base.sApply(context);
        }

        public void Cleanup(GraphicsContext context)
        {
            base.sCleanup(context);
        }

        public void SetShaderBool(string name, bool val)
        {
            base.aSetShaderBool(name, val);
        }

        public void SetShaderMatrix(string name, Matrix4 val)
        {
            base.aSetShaderMatrix(name, val);
        }

        public void SetShaderMatrix(string name, Matrix2 val)
        {
            base.aSetShaderMatrix(name, val);
        }

        public void SetShaderMatrix(string name, Matrix3 val)
        {
            base.aSetShaderMatrix(name, val);
        }

        public void SetShaderVector(string name, Vector4 val)
        {
            base.aSetShaderVector(name, val);
        }

        public void SetShaderVector(string name, Vector3 val)
        {
            base.aSetShaderVector(name, val);
        }

        public void SetShaderVector(string name, Vector2 val)
        {
            base.aSetShaderVector(name, val);
        }

        public void SetShaderFloat(string name, float val)
        {
            base.aSetShaderFloat(name, val);
        }

        public void SetTexture(string name, Texture tex)
        {
            base.aSetTexture(name, tex);
        }
    }

}

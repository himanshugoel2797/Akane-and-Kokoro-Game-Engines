using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Kokoro.Math;
using Kokoro.Engine.Shaders;

namespace Kokoro.Engine
{
    public class Material
    {
        public string Name { get; set; }
        public Texture ColorMap { get; set; }
        public Texture LightingMap { get; set; }
        public Texture NormalMap { get; set; }
        public ShaderProgram Shader { get; set; }

        public virtual void Apply(GraphicsContext context, Model m)
        {
            Shader["World"] = m.World;
            Shader["View"] = context.View;
            Shader["Projection"] = context.Projection;
            Shader["ZNear"] = context.ZNear;
            Shader["ZFar"] = context.ZFar;

            if (ColorMap != null) Shader["ColorMap"] = ColorMap;
            if (LightingMap != null) Shader["LightingMap"] = LightingMap;
            if (NormalMap != null) Shader["NormalMap"] = NormalMap;

            Shader.Apply(context);
        }

        public virtual void Cleanup(GraphicsContext context, Model m)
        {
            Shader.Cleanup(context);
        }

    }
}

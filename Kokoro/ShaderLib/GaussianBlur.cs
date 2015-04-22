using Kokoro.Engine.Shaders;
using Kokoro.KSL;
using Kokoro.KSL.Lib;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.ShaderLib
{
    public class GaussianBlurShader : IKUbershader
    {
        float[] kern;
        bool vertical;

        public GaussianBlurShader(int taps, bool Vertical)
        {
            vertical = Vertical;
            kern = new float[taps];

            float radius = 1f;
            int width = (int)System.Math.Ceiling(radius);
            float sigma = radius / 3f;
            float norm = 1f / (float)(System.Math.Sqrt(2 * System.Math.PI * sigma * sigma));
            float coeff = 2 * sigma * sigma;
            float total = 0;

            for (int index = 0; index < taps; index++)
            {
                float g = norm * (float)System.Math.Exp(-(System.Math.Pow((index - taps / 2) * ((float)width / (float)taps), 2) / coeff));
                kern[index] = g;
                total += g;
            }

            for (int x = 0; x < taps; x++)
            {
                kern[x] /= total;
            }
        }

        public void Vertex()
        {
            var Vars = Manager.ShaderStart("GaussianBlur" + kern.Length + "_S_Vert");
            Manager.StreamIn<Vec3>("VertexPos", 0);
            Manager.StreamIn<Vec2>("UV0", 2);

            Manager.SharedOut<Vec2>("UV");

            Vars.VertexPosition.Construct(Vars.VertexPos, 1);
            Vars.UV = Vars.UV0;

            Manager.ShaderEnd();
        }

        public void Fragment(int num)
        {
            var Vars = Manager.ShaderStart("GaussianBlur" + kern.Length + "_S_Frag");
            Manager.SharedIn<Vec2>("UV");
            Manager.StreamOut<Vec4>("Color", 0);
            Manager.Uniform<KFloat>("KernelRad");

            Manager.Create<Vec4>("tmpCol");
            Manager.Create<Vec2>("rad");

            if (vertical) Vars.rad.Construct(0, Vars.KernelRad);
            else Vars.rad.Construct(Vars.KernelRad, 0);

            Vars.Color = Texture.Read2D(Vars.ColorMap, Vars.UV) * kern[0];

            for (int i = 0; i < kern.Length; i++)
            {
                Vars.Color += Texture.Read2D(Vars.ColorMap, KMath.Clamp(Vars.UV + ((i - (float)kern.Length / 2f) * Vars.rad), new Vec2(0, 0), new Vec2(1, 1))) * kern[i];
            }
        }

        public static Ubershader Create(int tapCount, bool vertical)
        {
            return new Ubershader(new GaussianBlurShader(tapCount, vertical));
        }

        public static object Create(ShaderTypes t, int tapCount, bool vertical)
        {
            if (t == ShaderTypes.Fragment) return (Action<int>)new GaussianBlurShader(tapCount, vertical).Fragment;
            else if (t == ShaderTypes.Vertex) return (Action)new GaussianBlurShader(tapCount, vertical).Vertex;

            return null;
        }

    }
}

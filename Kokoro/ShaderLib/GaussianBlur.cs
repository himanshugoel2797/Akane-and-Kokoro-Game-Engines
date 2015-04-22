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
            double[,] kernel = new double[taps, taps];

            double sigma = 1;
            double mean = taps / 2;
            double sum = 0.0; // For accumulating the kernel values
            for (int x = 0; x < taps; ++x)
                for (int y = 0; y < taps; ++y)
                {
                    kernel[x, y] = System.Math.Exp(-0.5 * (System.Math.Pow((x - mean) / sigma, 2.0) + System.Math.Pow((y - mean) / sigma, 2.0)))
                                     / (2 * System.Math.PI * sigma * sigma);

                    // Accumulate the kernel values
                    sum += kernel[x, y];
                }

            // Normalize the kernel
            for (int y = 0; y < taps; ++y)
                kern[y] = (float)(kernel[0, y] / sum);
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

            Manager.Uniform<KFloat>("KernRadius");
            Manager.Create<Vec4>("tmpCol");
            Manager.Create<Vec2>("rad");

            if (vertical) Vars.rad.Construct(0, Vars.KernRadius);
            else Vars.rad.Construct(Vars.KernRadius, 0);
            Vars.tmpCol = Texture.Read2D(Vars.ColorMap, Vars.UV);

            for (int i = 0; i < kern.Length; i++)
            {
                Vars.Color += Texture.Read2D(Vars.ColorMap, Vars.UV + (i * Vars.rad)) * kern[i];
                Vars.Color += Texture.Read2D(Vars.ColorMap, Vars.UV - (i * Vars.rad)) * kern[i];
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

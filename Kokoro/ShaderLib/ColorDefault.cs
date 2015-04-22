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
    public class ColorDefaultShader : IKUbershader
    {
        public void Fragment(int num)
        {
            var Vars = Manager.ShaderStart("Color_S_Frag");
            Manager.SharedIn<Vec2>("UV");
            Manager.SharedIn<Vec3>("WorldPos");
            Manager.SharedIn<Vec3>("NormPos");
            //Manager.SharedIn<Vec3>("Tangent");
            //Manager.SharedIn<Vec3>("BiTangent");

            Manager.StreamOut<Vec4>("RGBA0", 0);
            Manager.StreamOut<Vec4>("Depth0", 1);
            Manager.StreamOut<Vec4>("Normal0", 2);

            Manager.Uniform<Vec4>("inColor");

            Vars.RGBA0 = Texture.Read2D(Vars.ColorMap, Vars.UV);
            Vars.Normal0.Construct(KMath.Normalize(0.5f * Vars.NormPos + (Vec3)0.5f), 1);
            Vars.Depth0["xyz"] = Vars.WorldPos;
            Vars.Depth0["w"] = 1f;


            Vars.RGBA0 = Vars.inColor;

            Manager.ShaderEnd();
        }

        public void Vertex()
        {
            var Vars = Manager.ShaderStart("Color_S_Vert");
            Manager.StreamIn<Vec3>("VertexPos", 0);
            Manager.StreamIn<Vec2>("UV0", 2);
            Manager.StreamIn<Vec3>("Normal", 1);
            //Manager.StreamIn<Vec3>("Tan", 3);

            Manager.SharedOut<Vec2>("UV");
            Manager.SharedOut<Vec3>("WorldPos");
            Manager.SharedOut<Vec3>("NormPos");
            //Manager.SharedOut<Vec3>("Tangent");
            //Manager.SharedOut<Vec3>("BiTangent");

            Manager.Create<Mat4>("MVP");
            Manager.Create<Vec4>("tmp");

            Vars.MVP = Vars.Projection * Vars.View * Vars.World;
            Vars.VertexPosition.Construct(Vars.VertexPos, 1);
            Vars.VertexPosition = Vars.MVP * Vars.VertexPosition;

            Vars.tmp.Construct(Vars.Normal, 0);
            Vars.NormPos = (Vars.World * Vars.tmp)["xyz"];

            Vars.tmp.Construct(Vars.VertexPos, 1);
            Vars.WorldPos = (Vars.World * Vars.tmp)["xyz"];

            Vars.WorldPos["z"] = (Vars.VertexPosition["z"] * Vars.VertexPosition["w"] - Vars.ZNear) / (Vars.ZFar - Vars.ZNear);

            //Vars.Tangent = Vars.Tan;
            //Vars.BiTangent = KMath.Cross(Vars.Tangent, Vars.Normal);

            Vars.UV = Vars.UV0;
        }

        public static Ubershader Create()
        {
            return new Ubershader(new ColorDefaultShader());
        }

        public static object Create(ShaderTypes s)
        {
            if (s == ShaderTypes.Vertex) return (Action)new ColorDefaultShader().Vertex;
            else if (s == ShaderTypes.Fragment) return (Action<int>)new ColorDefaultShader().Fragment;
            return null;
        }
    }
}

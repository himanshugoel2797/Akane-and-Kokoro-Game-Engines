using System;
using Kokoro.KSL;
using Kokoro.KSL.Lib;
using Kokoro.KSL.Lib.General;
using Kokoro.KSL.Lib.Math;
using Kokoro.KSL.Lib.Texture;

namespace Akane
{
    public class LayerDrawShader : IKShaderProgram
    {
        public LayerDrawShader()
        {
        }

        #region IKShaderProgram implementation

        public void Fragment()
        {
            dynamic Vars = Manager.ShaderStart("Frag_Layer_Draw");

            //Define Uniform parameters
            Manager.Uniform<KFloat>("maxGid");
            Manager.Uniform<KFloat>("firstGid");
            Manager.Uniform<Vec2>("textureSize");
            Manager.Uniform<Vec2>("mapSize");
            Manager.Uniform<Vec2>("tileSize");
            Manager.Uniform<Vec2>("viewportTileRes");
            Manager.Uniform<Vec2>("viewportOffset");
            Manager.Uniform<KFloat>("layerNum");
            Manager.Uniform<KFloat>("maxLayer");

            //Define Output streams
            Manager.StreamOut<Vec4>("Color", 0);
            Manager.StreamOut<Vec4>("Height", 1);

            //Define input data
            Manager.SharedIn<Vec2>("UV");

            Manager.Create<Vec2>("TileCounts");
            Manager.Create<Vec2>("UVStep");
            Manager.Create<Vec2>("CurPos");
            Manager.Create<Vec2>("TileBaseCoords");
            Manager.Create<Vec2>("NextTilePos");
            Manager.Create<Vec2>("MapInfo");
            Manager.Create<Vec2>("CurPixelPos");
            Manager.Create<KFloat>("TileID");
            Manager.Create<Vec2>("Offset");
            Manager.Create<Vec2>("UVCoords");

            Vars.TileCounts = Vars.textureSize / Vars.tileSize;
            Vars.UVStep = 1 / Vars.viewportTileRes;
            Vars.CurPos = KMath.Mod(Vars.UV, Vars.UVStep);

            Manager.ShaderEnd();
        }

        public void Vertex()
        {
            var Vars = Manager.ShaderStart("Vert_Layer_Draw");

            Manager.ShaderEnd();
        }

        #endregion
    }
}


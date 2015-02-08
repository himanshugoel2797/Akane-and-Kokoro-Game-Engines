using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;
using Kokoro.Engine;
using Kokoro.Debug;
using Kokoro.Engine.SceneGraph;
using Kokoro.Engine.Prefabs;
using Kokoro.Engine.Shaders;
using Kokoro.Engine.HighLevel.Cameras;
using Kokoro.Engine.HighLevel.Rendering;

namespace Kokoro.Game
{
    class TestA : IScene
    {
        Model model, ground;
        Texture tex;
        LightingEngine engine;
        ShaderProgram fur;

        public TestA(GraphicsContext context)
        {
            engine = new LightingEngine(1920, 1080, context);

            context.Camera = new FirstPersonCamera(Vector3.Zero, Vector3.UnitZ);
            context.DepthFunction = (x, y) => x <= y;
            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };


            model = Model.Load("Resources/scene.obj");
            ground = Model.Load("Resources/ground.obj");
            model.Materials[0].Shader = new ShaderProgram("Shaders/Default");
            model.World = Matrix4.CreateTranslation(0, 0, 0) * Matrix4.Scale(1);
            tex = new Texture("Resources/seamless-green-grass-texture.jpg");
            model.Materials[0].ColorMap = tex;

            context.Projection = Matrix4.CreatePerspectiveFieldOfView(0.78539f, 16f / 9f, context.ZNear, context.ZFar);

            fur = new ShaderProgram("Shaders/Fur");
            fur["GrassMap"] = new Texture("Resources/Yubrs.png");
            fur["GrassHeightMap"] = new Texture("Resources/grassHeights.png");

            for (int i = 0; i < model.Materials.Length; i++)
            {
                model.Materials[i].Shader = engine.Shader;
                if (model.Materials[i].ColorMap == null)
                {
                    model.Materials[i].ColorMap = tex;
                    model.Materials[i].NormalMap = new Texture("Resources/woodNormalMap.png");
                }
            }

            //Specify the draw function
            engine.Draw = Draw;
        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Draw(double interval, GraphicsContext context)
        {
            //context.Wireframe = true;
            context.FaceCulling = CullMode.Back;
            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };

            //model.Materials[5].Shader = engine.Shader;
            model.Draw(context);
            model.Draw(context);

            /*ground.World = Matrix4.CreateTranslation(0, 0.1f, 0) * Matrix4.Scale(1, 1f, 1);

            for (int i = 0; i < ground.Materials.Length; i++)
            {
                ground.Materials[i].Shader = fur;
                ground.Materials[i].ColorMap = tex;
                ground.Materials[i].Shader["layer"] = (float)i;
                ground.Materials[i].Shader["density"] = (float)i;
            }
            ground.Draw(context);*/
            //context.FaceCulling = CullMode.Off;
            
            //context.FaceCulling = CullMode.Back;
            //context.Wireframe = false;
        }

        public void Render(double interval, GraphicsContext context)
        {
            engine.Render(interval, context);
        }

        public void Update(double interval, GraphicsContext context)
        {
            context.Camera.Update(interval, context);
        }
    }
}

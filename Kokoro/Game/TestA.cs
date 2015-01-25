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
        Model model, billboard, fsq;
        ShaderProgram normalShader;
        Texture tex;
        DeepGBuffer gbuffer;

        public TestA(GraphicsContext context)
        {
            gbuffer = new DeepGBuffer(1920, 1080, context);
            fsq = new FullScreenQuad();

            context.Camera = new FirstPersonCamera(Vector3.Zero, Vector3.UnitZ);
            context.DepthFunction = (x, y) => x <= y;
            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };
            

            model = Model.Load("Resources/scene.obj");
            model.Materials[0].Shader = new ShaderProgram("Shaders/Default");
            model.World = Matrix4.CreateTranslation(0, 0, 0) * Matrix4.Scale(1);
            tex = new Texture("Resources/seamless-green-grass-texture.jpg");
            model.Materials[0].ColorMap = tex;

            billboard = new Quad(0, 0, 1, 1);
            billboard.Materials[0].Shader = new ShaderProgram("Shaders/Default");
            billboard.Materials[0].ColorMap = new Texture("Resources/grassBillboard.png");
            billboard.World = Matrix4.CreateTranslation(0, 500, -2.5f) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90)) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90));

            Vector3 pos = new Vector3(0, 20, -600);
            //context.View = Matrix4.LookAt((-Vector3.UnitZ + pos), pos, Vector3.UnitY);
            context.Projection = Matrix4.CreatePerspectiveFieldOfView(0.78539f, 16f / 9f, context.ZNear, context.ZFar);

            billboard.Materials[0].Shader = model.Materials[0].Shader = gbuffer.deepGBuffer;
            normalShader = new ShaderProgram("Shaders/FrameBuffer");
            //context.Wireframe = true;
        }

        public IScene Parent
        {
            get;
            set;
        }
        Random rng;
        public void Render(double interval, GraphicsContext context)
        {
            //model.Materials[0].ColorMap = tex;
            //model.Materials[0].Shader = gbuffer.deepGBuffer;
            gbuffer.Bind(context);
            context.Clear(0, 0, 0, 0);
            rng = new Random(0);
            model.Draw(context);
            for (int i = 500; i > 0; i--)
            {
                for(int x = 10; x > 0; x--){
                billboard.Draw(context);
                billboard.World = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(90)) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(90 + rng.Next(90))) * Matrix4.CreateTranslation(i, 1.5f, x + rng.Next(5));
                }
            }
            gbuffer.Unbind();
            fsq.Materials[0].ColorMap = gbuffer.framebuffer["RGBA1"];
            fsq.Materials[0].Shader = normalShader;
            fsq.Draw(context);
        }

        public void Update(double interval, GraphicsContext context)
        {
            context.Camera.Update(interval, context);
            //model.World *= Matrix4.CreateRotationZ((float)(interval * 0.5f)) * Matrix4.CreateRotationX((float)(interval * 0.5f)) * Matrix4.CreateRotationY((float)(interval * 0.5f));
        }
    }
}

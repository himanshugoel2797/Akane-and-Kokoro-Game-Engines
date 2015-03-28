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
using Kokoro.Scripting;
using System.Diagnostics;

namespace Kokoro.Game
{
    class TestA : IScene
    {
        Model room;
        Texture tmp;
        Stopwatch s;

        public TestA(GraphicsContext context)
        {
            context.Camera = new FirstPersonCamera(Vector3.Zero, Vector3.UnitZ);
            context.Projection = Matrix4.CreatePerspectiveFieldOfView(0.78539f, 16f / 9f, context.ZNear, context.ZFar);
            context.DepthFunction = (x, y) => x <= y;
            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };

            tmp = new Texture("Resources/asuna.png");       //Ok so Image loading works as expected, how do we test the actual rendering if we can't make it show anything?
            room = new Sphere(5,100);//Model.Load("room.obj");
            room.Materials[0].Shader = new ShaderProgram(new ShaderLib.DefaultShader());
            room.Materials[0].ColorMap = tmp;
            s = Stopwatch.StartNew();
        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Render(double interval, GraphicsContext context)
        {
            context.Clear(0, 0.5f, 1.0f, 0.0f);

            room.Draw(context);

            context.SwapBuffers();
        }

        public void Update(double interval, GraphicsContext context)
        {
            context.Camera.Update(interval, context);
        }
    }
}

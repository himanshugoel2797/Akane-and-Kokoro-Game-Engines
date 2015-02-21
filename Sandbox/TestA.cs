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

namespace Kokoro.Game
{
    class TestA : IScene
    {
        Model room;

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


            room = Model.Load("room.obj");
        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Render(double interval, GraphicsContext context)
        {
            room.Draw(context);
        }

        public void Update(double interval, GraphicsContext context)
        {
            context.Camera.Update(interval, context);
        }
    }
}

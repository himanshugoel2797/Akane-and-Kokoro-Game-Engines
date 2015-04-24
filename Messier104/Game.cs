using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Messier104;
using Kokoro.Math;
using Kokoro.Engine;
using Kokoro.Debug;
using Kokoro.Engine.SceneGraph;
using Kokoro.Engine.Prefabs;
using Kokoro.Engine.Shaders;
using Kokoro.Engine.HighLevel.Cameras;

namespace Kokoro.Game
{
    public class Game
    {
        GraphicsContext Context;
        SceneManager sceneManager;

        public Game(GraphicsContext context)
        {
            Context = context;

            Context.Initialize += Initialize;

            Context.Start(160000, 160000);
        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Initialize(GraphicsContext context)
        {
            context.Render += Render;
            context.Update += Update;
            context.ResourceManager += LoadResources;

            context.Camera = new Camera();
            context.Projection = Matrix4.CreatePerspectiveFieldOfView(0.78539f, 16f / 9f, context.ZNear, context.ZFar);
            context.DepthFunction = (x, y) => x <= y;
            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };

            sceneManager = new SceneManager();
            sceneManager.Add("MainMenu", new MainMenu(Context));
            sceneManager.Add("InGame", new InGame(Context));
            sceneManager.Activate("InGame");
        }

        public void Render(double interval, GraphicsContext context)
        {
            sceneManager.Render(interval, context);
        }

        public void Update(double interval, GraphicsContext context)
        {
            context.Camera.Update(interval, context);
            sceneManager.Update(interval, context);
        }

        public void LoadResources(GraphicsContext context)
        {
            sceneManager.LoadResources(context);
        }
    }
}

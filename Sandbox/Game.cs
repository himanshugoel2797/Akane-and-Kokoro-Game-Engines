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
            sceneManager = new SceneManager();
            sceneManager.Add("TestA", new TestA(Context));
            sceneManager.Register(context);
            sceneManager.Activate("TestA");
        }

        public void Render(double interval, GraphicsContext context)
        {
            sceneManager.Render(interval, context);
        }

        public void Update(double interval, GraphicsContext context)
        {
            sceneManager.Update(interval, context);
        }

        public void LoadResources(GraphicsContext context)
        {
            sceneManager.LoadResources(context);
        }
    }
}

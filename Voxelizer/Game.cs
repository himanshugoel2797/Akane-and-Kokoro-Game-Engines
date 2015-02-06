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

namespace Voxelizer
{
    public class Game : IScene
    {
        GraphicsContext Context;
        SceneManager sceneManager;

        public Game()
        {
            //request user to supply filename for file to voxelize

            Context = new GraphicsContext(new Vector2(960, 540));
            Context.Render += Render;
            Context.Update += Update;

            sceneManager = new SceneManager();
            sceneManager.Add("ModelVoxelizer", new ModelVoxelizer(Context));

            sceneManager.Activate("ModelVoxelizer");

            Context.Start(0, 0);
        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Render(double interval, GraphicsContext context)
        {
            sceneManager.Render(interval, context);
        }

        public void Update(double interval, GraphicsContext context)
        {
            sceneManager.Update(interval, context);
        }
    }
}

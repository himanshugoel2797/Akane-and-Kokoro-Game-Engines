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

namespace MusicalPlatformer
{
    class MainMenu : IScene
    {
        Model MainMenuFSQ;
        bool loaded = false;

        public MainMenu(GraphicsContext context)
        {

        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Render(double interval, GraphicsContext context)
        {
            if (loaded)
            {
                MainMenuFSQ.Draw(context);
            }

            context.SwapBuffers();
        }

        public void Update(double interval, GraphicsContext context)
        {

        }

        public void LoadResources(GraphicsContext context)
        {
            MainMenuFSQ = new FullScreenQuad();
            (Parent as SceneManager).Activate("InGame");
            loaded = true;
        }
    }
}

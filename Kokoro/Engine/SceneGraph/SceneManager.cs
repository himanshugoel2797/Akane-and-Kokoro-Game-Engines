using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.SceneGraph
{
    public class SceneManager : IScene
    {
        private Dictionary<string, IScene> scenes;
        private IScene curScene;

        public SceneManager()
        {
            Debug.ObjectAllocTracker.NewCreated(this, -1, "SceneManager");

            scenes = new Dictionary<string, IScene>();
        }

        public void Add(string key, IScene scene)
        {
            scene.Parent = this;
            scenes.Add(key, scene);
        }

        public void Remove(string key)
        {
            scenes[key].Parent = null;
            scenes.Remove(key);
        }

        public void Activate(string scene)
        {
            curScene = scenes[scene];
        }

        public void Update(long interval, GraphicsContext context)
        {
            if (curScene != null) curScene.Update(interval, context);
        }

        public void Render(long interval, GraphicsContext context)
        {
            if (curScene != null) curScene.Render(interval, context);
        }

        public IScene Parent
        {
            get;
            set;
        }

    }
}

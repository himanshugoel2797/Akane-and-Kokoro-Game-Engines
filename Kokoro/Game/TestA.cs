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
    class TestA : IScene
    {
        Model model;

        public TestA()
        {
            model = new Sphere(1);
            model.Materials[0].Shader = new ShaderProgram("Shaders/Default");
        }

        public IScene Parent
        {
            get;
            set;
        }

        public void Render(long interval, GraphicsContext context)
        {
            model.Draw(context);
        }

        public void Update(long interval, GraphicsContext context)
        {
            Kokoro.Debug.ErrorLogger.AddMessage(0, "End Render Frame", Kokoro.Debug.DebugType.Other, Kokoro.Debug.Severity.Notification);
            ObjectAllocTracker.MarkGameLoop(interval, context);
        }
    }
}

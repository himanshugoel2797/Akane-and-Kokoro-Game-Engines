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
        public TestA() { }

        public IScene Parent
        {
            get;
            set;
        }

        public void Render(long interval, GraphicsContext context)
        {

        }

        public void Update(long interval, GraphicsContext context)
        {
            Kokoro.Debug.ErrorLogger.AddMessage(0, "End Render Frame", Kokoro.Debug.DebugType.Other, Kokoro.Debug.Severity.Notification);
        }
    }
}

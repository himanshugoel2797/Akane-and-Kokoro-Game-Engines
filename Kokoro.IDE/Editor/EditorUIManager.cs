using Kokoro.Engine.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Engine;

namespace Kokoro.IDE.Editor
{
    public class EditorUIManager : IScene
    {
        public IScene Parent
        {
            get;
            set;
        }

        public void Render(double interval, GraphicsContext context)
        {

        }

        public void Update(double interval, GraphicsContext context)
        {

        }
    }
}

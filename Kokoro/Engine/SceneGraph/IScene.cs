using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.SceneGraph
{
    interface IScene
    {
        IScene Parent
        {
            get;
            set;
        }

        void Render(long interval, GraphicsContext context);
        void Update(long interval, GraphicsContext context);
    }
}

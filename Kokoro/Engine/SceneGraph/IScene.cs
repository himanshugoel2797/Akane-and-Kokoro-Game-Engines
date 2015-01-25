using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kokoro.Engine.SceneGraph
{
    public interface IScene
    {
        IScene Parent
        {
            get;
            set;
        }

        void Render(double interval, GraphicsContext context);
        void Update(double interval, GraphicsContext context);
    }
}

using Kokoro.Engine.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Engine;

namespace Kokoro.Voxelizer
{
    class VoxelRenderer : IScene
    {
        public IScene Parent
        {
            get;
            set;
        }

        public void Initialize(GraphicsContext context)
        {

        }

        public void Render(double interval, GraphicsContext context)
        {
            throw new NotImplementedException();
        }

        public void Update(double interval, GraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }
}

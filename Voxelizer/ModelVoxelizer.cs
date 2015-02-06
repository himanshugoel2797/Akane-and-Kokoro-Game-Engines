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
    public class ModelVoxelizer : IScene
    {
        public string ModelToLoad;
        Model m;

        public IScene Parent
        {
            get;
            set;
        }

        public ModelVoxelizer(GraphicsContext context)
        {
            m = Model.Load(ModelToLoad);

        }

        public void Render(double interval, GraphicsContext context)
        {
            
        }

        private void Voxelize(GraphicsContext context)
        {
            //render the data to a texture, save the texture to disk, repeat

        }

        public void Update(double interval, GraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }
}

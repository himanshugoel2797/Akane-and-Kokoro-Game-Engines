using Kokoro.Engine;
using Kokoro.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akane
{
    public class AkaneManager
    {
        public Action<double, AkaneManager> Update { get; set; }
        public Action<double, AkaneManager> Render { get; set; }

        internal GraphicsContext context;
        public AkaneManager(Vector2 WindowSize)
        {
            context = new GraphicsContext(WindowSize);
            context.Camera = new Kokoro.Engine.HighLevel.Cameras.Camera();
            context.View = Matrix4.Identity;
            context.DepthFunction = (x, y) => true;
            //context.DepthWrite = false;
            //context.View = Matrix4.LookAt(Vector3.Zero - Vector3.UnitZ, Vector3.Zero, Vector3.UnitY);
            //context.Wireframe = true;
            context.Update = _update;
            context.Render = _render;

            context.Blending = new BlendFunc()
            {
                Src = BlendingFactor.SrcAlpha,
                Dst = BlendingFactor.OneMinusSrcAlpha
            };
        }

        public void Start()
        {
            context.Start(60, 60);
        }

        public void Clear(Vector4 col)
        {
            context.Clear(col);
        }

        private void _update(double time, GraphicsContext context)
        {
            Update(time, this);
        }

        private void _render(double time, GraphicsContext context)
        {
            Render(time, this);
        }

    }
}

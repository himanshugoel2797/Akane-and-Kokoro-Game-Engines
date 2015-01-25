using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;

namespace Kokoro.Engine.HighLevel.Cameras
{
    public class Camera
    {
        public Matrix4 View { get; set; }
        public Vector3 Position { get; set; }

        public Camera()
        {
            View = Matrix4.LookAt(new Vector3(0, -1, 0), Vector3.Zero, Vector3.UnitZ);
            Position = -Vector3.UnitY;
        }

        public virtual void Update(double interval, GraphicsContext Context)
        {
            Context.View = View;
        }
    }
}

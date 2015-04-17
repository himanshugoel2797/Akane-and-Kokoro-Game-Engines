using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;

namespace Kokoro.Engine.HighLevel.Cameras
{
    /// <summary>
    /// Represents a Camera in the scene graph
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// The Camera's View Matrix
        /// </summary>
        public Matrix4 View { get; internal set; }

        Vector3 pos;
        /// <summary>
        /// The 3D Position of the Camera
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
            }
        }

        /// <summary>
        /// Create a new Camera object
        /// </summary>
        public Camera()
        {
            View = Matrix4.LookAt(new Vector3(-1, 0, 0), Vector3.Zero, Vector3.UnitY);
            Position = -Vector3.UnitX;
        }

        /// <summary>
        /// Update the camera instance
        /// </summary>
        /// <param name="interval">The time elapsed in ticks since the last update</param>
        /// <param name="Context">The current GraphicsContext</param>
        public virtual void Update(double interval, GraphicsContext Context)
        {
            Context.View = View;
        }
    }
}

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
        public Matrix4 View { get; set; }
        /// <summary>
        /// The 3D Position of the Camera
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Create a new Camera object
        /// </summary>
        public Camera()
        {
            View = Matrix4.LookAt(new Vector3(0, -1, 0), Vector3.Zero, Vector3.UnitZ);
            Position = -Vector3.UnitY;
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

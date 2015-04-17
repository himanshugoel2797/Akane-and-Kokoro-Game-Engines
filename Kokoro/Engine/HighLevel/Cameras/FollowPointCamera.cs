using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;
using Kokoro.Engine.Input;

namespace Kokoro.Engine.HighLevel.Cameras
{
    /// <summary>
    /// Represents a First Person Camera
    /// </summary>
    public class FollowPointCamera : Camera
    {//TODO setup collisions

        public float RotationMargin = 0.15f;
        Vector2 mousePos;

        Vector3 Up = Vector3.UnitY, EyeOffset;

        /// <summary>
        /// Create a new First Person Camera
        /// </summary>
        /// <param name="Position">The Position of the Camera</param>
        /// <param name="Direction">The Direction the Camera initially faces</param>
        public FollowPointCamera()
        {

        }

        /// <summary>
        /// Update the camera instance
        /// </summary>
        /// <param name="interval">The time elapsed in ticks since the last update</param>
        /// <param name="Context">The current GraphicsContext</param>
        public override void Update(double interval, GraphicsContext Context)
        {
            Position -= Vector3.UnitX;
            Vector3 Target = Position + Vector3.UnitX;
            Vector3 Eye = Position + EyeOffset;

            float rotX = RotationMargin * (float)(Mouse.NDMousePos.X - 0.5f);
            float rotY = RotationMargin * (float)((Mouse.NDMousePos.Y) - 0.5f);
            Target += new Vector3(0, rotY, rotX);

            mousePos = Mouse.NDMousePos;

            View = Matrix4.LookAt(Eye, Target, Up);
            base.Update(interval, Context);
        }
    }
}

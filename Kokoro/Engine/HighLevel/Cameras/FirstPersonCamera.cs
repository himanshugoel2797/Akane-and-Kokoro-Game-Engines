using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kokoro.Math;

namespace Kokoro.Engine.HighLevel.Cameras
{
    public class FirstPersonCamera : Camera
    {//TODO setup collisions
        public Vector3 Direction;
        public Vector3 Up;
        float leftrightRot = MathHelper.PiOver2;
        float updownRot = -MathHelper.Pi / 10.0f;
        const float rotationSpeed = 0.02f;
        const float moveSpeed = 5f;
        Vector2 mousePos;
        Vector3 cameraRotatedUpVector;

        public FirstPersonCamera(Vector3 Position, Vector3 Direction)
        {
            this.Position = Position;
            this.Direction = Direction;
            View = Matrix4.LookAt(Position, Position + Direction, Vector3.UnitZ);
            this.Up = Vector3.UnitZ;
        }

        private Matrix4 UpdateViewMatrix()
        {
            Matrix4 cameraRotation = Matrix4.CreateRotationX(updownRot) * Matrix4.CreateRotationY(leftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Direction = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = Position + Direction;

            cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            return Matrix4.LookAt(Position, cameraFinalTarget, cameraRotatedUpVector);
        }

        public override void Update(double interval, GraphicsContext Context)
        {
            if (Context.MouseLeftButtonDown)
            {
                if (System.Math.Abs(mousePos.X - Context.MousePosition.X) > 0) leftrightRot -= (float)MathHelper.DegreesToRadians(rotationSpeed * (mousePos.X - Context.MousePosition.X) * interval);
                if (System.Math.Abs(mousePos.Y - Context.MousePosition.Y) > 0) updownRot -= (float)MathHelper.DegreesToRadians(rotationSpeed * (mousePos.Y - Context.MousePosition.Y) * interval);
            }
            else
            {
                mousePos = Context.MousePosition;
            }
            UpdateViewMatrix();
            Vector3 Right = Vector3.Cross(cameraRotatedUpVector, Direction);

            if (Context.Keys.Contains("Up"))
            {
                Position += Direction * (float)(moveSpeed * interval);
            }
            else if (Context.Keys.Contains("Down"))
            {
                Position -= Direction * (float)(moveSpeed * interval);
            }

            if (Context.Keys.Contains("Left"))
            {
                Position -= Right * (float)(moveSpeed * interval);
            }
            else if (Context.Keys.Contains("Right"))
            {
                Position += Right * (float)(moveSpeed * interval);
            }

            if (Context.Keys.Contains("PageDown"))
            {
                Position -= cameraRotatedUpVector * (float)(moveSpeed * interval);
            }
            else if (Context.Keys.Contains("PageUp"))
            {
                Position += cameraRotatedUpVector * (float)(moveSpeed * interval);
            }

            //View = UpdateViewMatrix();
            View = Matrix4.LookAt(Position, Position + Direction, cameraRotatedUpVector);
            base.Update(interval, Context);
        }
    }
}

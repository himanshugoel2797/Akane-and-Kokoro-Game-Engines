using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kokoro.Engine.Input;
using Kokoro.Math;

namespace Kokoro.Engine.HighLevel.CharacterControllers
{
    public class ThirdPersonController2D : CharacterControllerBase
    {
        public float MoveSpeed = 0.00005f;


        public ThirdPersonController2D(Vector3 InitialPosition)
        {
            Position = InitialPosition;
        }

        public void Update(double interval, GraphicsContext context)
        {
            interval /= 100;

            if (Keyboard.IsKeyPressed(Key.Up))
            {
                Position += Vector3.UnitY * (float)(MoveSpeed * interval / 10000f);
            }
            else if (Keyboard.IsKeyPressed(Key.Down))
            {
                Position -= Vector3.UnitY * (float)(MoveSpeed * interval / 10000f);
            }

            if (Keyboard.IsKeyPressed(Key.Left))
            {
                Position -= Vector3.UnitZ * (float)(MoveSpeed * interval / 10000f);
            }
            else if (Keyboard.IsKeyPressed(Key.Right))
            {
                Position += Vector3.UnitZ * (float)(MoveSpeed * interval / 10000f);
            }
#if DEBUG
            if (Keyboard.IsKeyPressed(Key.PageUp))
            {
                Position += Vector3.UnitX * (float)(MoveSpeed * interval / 10000f);
            }
            else if (Keyboard.IsKeyPressed(Key.PageDown))
            {
                Position -= Vector3.UnitX * (float)(MoveSpeed * interval / 10000f);
            }
#endif
        }
    }
}

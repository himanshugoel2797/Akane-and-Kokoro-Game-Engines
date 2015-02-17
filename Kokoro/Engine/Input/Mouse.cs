using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kokoro.Math;
using System.Threading.Tasks;

#if OPENGL
using Kokoro.OpenGL;
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine.Input
{
    /// <summary>
    /// Stores the states of the Mouse buttons
    /// </summary>
    public struct MouseButtons
    {
        public bool Left;
        public bool Right;
        public bool Middle;
    }

    public class Mouse
    {
        private static Vector2 prevMouse;
        private static Vector2 curMouse;
        private static readonly object locker = new object();

        public static Vector2 MousePos
        {
            get
            {
                return curMouse;
            }
            set
            {
                InputLL.SetMousePos(value);
            }
        }
        public static Vector2 MouseDelta { get; private set; }
        public static MouseButtons ButtonsDown { get; private set; }
        public static Vector2 NDMousePos
        {
            get; private set;
        }
        public static Matrix4 MouseProjection { get; private set; }

        static Mouse()
        {
            MouseProjection = Matrix4.CreateOrthographicOffCenter(0, 1, 1, 0, 0.01f, 1);
        }

        internal static void Update()
        {
            lock (locker)
            {
                prevMouse = curMouse;
                curMouse = InputLL.UpdateMouse();

                MouseDelta = prevMouse - curMouse;

                ButtonsDown = new MouseButtons()
                {
                    Left = InputLL.LeftMouseButtonDown(),
                    Right = InputLL.RightMouseButtonDown(),
                    Middle = InputLL.MiddleMouseButtonDown()
                };

                NDMousePos = InputLL.GetNDMousePos(curMouse);
            }

        }

    }
}

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
    public struct MouseButtons{
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

        internal static void Update()
        {
            lock (locker)
            {
                if (curMouse != null) prevMouse = curMouse;
                curMouse = InputLL.UpdateMouse();

                if (prevMouse != null)
                {
                    MouseDelta = prevMouse - curMouse;
                }

                ButtonsDown = new MouseButtons()
                {
                    Left = InputLL.LeftMouseButtonDown(),
                    Right = InputLL.RightMouseButtonDown(),
                    Middle = InputLL.MiddleMouseButtonDown()
                };
            }

        }

    }
}

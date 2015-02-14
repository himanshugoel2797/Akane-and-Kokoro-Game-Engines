#if OPENGL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Input;
using Kokoro.Math;

namespace Kokoro.OpenGL.PC
{
    public static class InputLL
    {
        static InputLL()
        {
            locker = new object();
            msLocker = new object();
        }

        #region Keyboard
        static KeyboardState kbdState;
        static object locker;
        public static void UpdateKeyboard()
        {
            lock (locker)
            {
                kbdState = Keyboard.GetState();
            }
        }

        public static bool KeyDown(Engine.Input.Key k)
        {
            lock (locker)
            {
                return kbdState[EnumConverters.EKey(k)];
            }
        }
        #endregion

        #region Mouse
        static MouseState msState;
        static object msLocker;
        public static Vector2 UpdateMouse()
        {
            lock (msLocker)
            {
                msState = Mouse.GetCursorState();
                return new Vector2(msState.X, msState.Y);
            }
        }
        public static bool LeftMouseButtonDown() { lock (msLocker) { return msState.IsButtonDown(MouseButton.Left); } }
        public static bool RightMouseButtonDown() { lock (msLocker) { return msState.IsButtonDown(MouseButton.Right); } }
        public static bool MiddleMouseButtonDown() { lock (msLocker) { return msState.IsButtonDown(MouseButton.Middle); } }

        public static bool LeftMouseButtonUp() { lock (msLocker) { return msState.IsButtonUp(MouseButton.Left); } }
        public static bool RightMouseButtonUp() { lock (msLocker) { return msState.IsButtonUp(MouseButton.Right); } }
        public static bool MiddleMouseButtonUp() { lock (msLocker) { return msState.IsButtonUp(MouseButton.Middle); } }

        public static float GetScroll() { lock (msLocker) { return msState.WheelPrecise; } }
        public static void SetMousePos(Vector2 pos) { Mouse.SetPosition(pos.X, pos.Y); }
        #endregion
    }
}

#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if OPENGL
using Kokoro.OpenGL;
#if PC
using Kokoro.OpenGL.PC;
#endif
#endif

namespace Kokoro.Engine.Input
{
    public static class Keyboard
    {
        private static Dictionary<Key, Action> handlers;
        static Keyboard() { handlers = new Dictionary<Key, Action>(); }

        internal static void Update()
        {
            InputLL.UpdateKeyboard();
        }

        public static bool IsKeyPressed(Key k)
        {
            return InputLL.KeyDown(k);
        }

        public static void RegisterKeyHandler(Action handler, Key k)
        {
            if (!handlers.ContainsKey(k)) handlers.Add(k, handler);
            else handlers[k] += handler;
        }

    }
}

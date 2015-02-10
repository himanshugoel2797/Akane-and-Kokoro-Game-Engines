using Kokoro.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akane.World
{
    [Flags]
    public enum InputKeys
    {
        Empty = 0,
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
        Enter = 1 << 4,
        Back = 1 << 5,
        Start = 1 << 6,
        Select = 1 << 7,
        Square = 1 << 8,
        Triangle = 1 << 9
    }

    public static class Input
    {
        public static InputKeys KeysPressed { get; internal set; }

        internal static void Update(GraphicsContext context)
        {
            //Update the Input data
            //TODO Load strings from config
            InputKeys keys = InputKeys.Empty;

            if (context.Keys.Contains("Up")) keys |= InputKeys.Up;
            if (context.Keys.Contains("Down")) keys |= InputKeys.Down;
            if (context.Keys.Contains("Left")) keys |= InputKeys.Left;
            if (context.Keys.Contains("Right")) keys |= InputKeys.Right;
            if (context.Keys.Contains("Right")) keys |= InputKeys.Enter;
            if (context.Keys.Contains("Right")) keys |= InputKeys.Back;
            if (context.Keys.Contains("Right")) keys |= InputKeys.Start;
            if (context.Keys.Contains("Right")) keys |= InputKeys.Select;
            if (context.Keys.Contains("Right")) keys |= InputKeys.Square;
            if (context.Keys.Contains("Right")) keys |= InputKeys.Triangle;

            if (keys.ToString() != InputKeys.Empty.ToString()) keys &= ~InputKeys.Empty;
            KeysPressed = keys;
        }
    }
}
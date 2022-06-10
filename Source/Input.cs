using System;
using GLFW;

namespace PangordsEngine
{
    class Input
    {
        public static bool GetKey(Window window, Keys key, InputState inputState)
        {
            return Glfw.GetKey(window, key) == inputState;
        }
    }
}

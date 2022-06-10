using System;
using GLFW;
using static OpenGL.GL;

namespace PangordsEngine.Core
{
    /// <summary>
    /// Window display mode.
    /// </summary>
    public enum WindowMode
    {
        Windowed,
        Fullscreen,
        MaximizedWindow
    }

    class GameCore
    {
        public static Start start = new Start(Game.Start);
        public static Update update = new Update(Game.Update);
        public static OnWindowDisplay onWindowDisplay = new OnWindowDisplay(Game.OnWindowDisplay);
        
        /// <summary>
        /// Prepares a window context need to create a window.
        /// </summary>
        /// <param name="windowMode">Sets the window display mode.</param>
        public static void PrepareContext(WindowMode windowMode)
        {
            // Set some common hints for the OpenGL profile creation
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Doublebuffer, true);

            switch (windowMode)
            {
                case WindowMode.Windowed:
                    Glfw.WindowHint(Hint.Decorated, true);
                    Glfw.WindowHint(Hint.Maximized, false);
                    break;
                case WindowMode.MaximizedWindow:
                    Glfw.WindowHint(Hint.Decorated, true);
                    Glfw.WindowHint(Hint.Maximized, true);
                    break;
                case WindowMode.Fullscreen:
                    Glfw.WindowHint(Hint.Decorated, false);
                    Glfw.WindowHint(Hint.Maximized, true);
                    break;
            }
        }

        /// <summary>
        /// Creates a window.
        /// </summary>
        /// <param name="width">Width of the window.</param>
        /// <param name="height">Height of the window.</param>
        /// <param name="title">Title of the window.</param>
        /// <returns></returns>
        public static Window CreateWindow(int width, int height, string title)
        {
            var window = Glfw.CreateWindow(width, height, title, Monitor.None, Window.None);

            if (window == Window.None)
            {
                Console.WriteLine("Failed to create GLFW window!");
                Glfw.Terminate();
                return window;
            }

            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);

            onWindowDisplay.Invoke();

            Console.WriteLine("Window successfuly created!");

            glViewport(0, 0, width, height);

            return window;
        }
    }
}

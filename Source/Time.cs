using GLFW;

namespace PangordsEngine
{
    static class Time
    {
        public static float DeltaTime
        {
            get
            {
                return deltaTime;
            }
        }
        static float deltaTime;

        static float lastFrame = 0.0f;

        public static void CalculateDeltaTime()
        {
            float currentFrame = (float)Glfw.Time;
            deltaTime = currentFrame - lastFrame;
            lastFrame = currentFrame;
        }
    }
}
using System;
using System.IO;

namespace PangordsEngine.Shaders
{
    class ShaderUtility
    {
        public static Shader UnlitShader()
        {
            return new Shader(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Shaders\vUnlitShader.glsl", Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Shaders\fUnlitShader.glsl");
        }

        public static Shader LitShader()
        {
            return new Shader(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Shaders\vLitShader.glsl", Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Shaders\fLitShader.glsl");
        }

        public static Shader SkyboxShader()
        {
            return new Shader(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Shaders\vSkyboxShader.glsl", Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Shaders\fSkyboxShader.glsl");
        }

        public static Shader CustomShader(string vertexShaderPath, string fragmentShaderPath)
        {
            return new Shader(vertexShaderPath, fragmentShaderPath);
        }
    }
}

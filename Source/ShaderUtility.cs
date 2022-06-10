namespace PangordsEngine.Shaders
{
    class ShaderUtility
    {
        public static Shader UnlitShader()
        {
            return new Shader(@"D:\Dev\PangordsEngine\Shaders\vUnlitShader.glsl", @"D:\Dev\PangordsEngine\Shaders\fUnlitShader.glsl");
        }

        public static Shader LitShader()
        {
            return new Shader(@"D:\Dev\PangordsEngine\Shaders\vLitShader.glsl", @"D:\Dev\PangordsEngine\Shaders\fLitShader.glsl");
        }

        public static Shader SkyboxShader()
        {
            return new Shader(@"D:\Dev\PangordsEngine\Shaders\vSkyboxShader.glsl", @"D:\Dev\PangordsEngine\Shaders\fSkyboxShader.glsl");
        }

        public static Shader CustomShader(string vertexShaderPath, string fragmentShaderPath)
        {
            return new Shader(vertexShaderPath, fragmentShaderPath);
        }
    }
}

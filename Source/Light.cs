using PangordsEngine.Shaders;
using PangordsEngine.Lighting;

namespace PangordsEngine
{
    class Light : Component
    {
        public enum LightType
        {
            Directional,
            Point,
            Spot
        }

        public string name;
        public LightType type;

        // common attributes
        public Transform transform = new();

        public Vector3 ambient = new Vector3(0.05f, 0.05f, 0.05f);
        public Vector3 diffuse = new Vector3(0.3f, 0.3f, 0.3f);
        public Vector3 specular = new Vector3(1.0f, 1.0f, 1.0f);
        public float constant = 1.0f;
        public float linear = 0.09f;
        public float quadratic = 0.032f;
        public float intensity = 1.0f;

        // spotlight attributes
        public float cutOff = 12.5f;
        public float outerCutOff = 15.0f;

        // directional light attributes
        public Vector3 direction = new Vector3(-0.2f, -1.0f, -0.3f);

        public uint pointLightID = 0;

        public void Process(ref Shader shader)
        {
            switch (type)
            {
                case LightType.Directional:
                    SetDirectionalLightProperties(ref shader);
                    break;
                case LightType.Point:
                    SetPointLightProperties(ref shader);
                    break;
                case LightType.Spot:
                    SetSpotLightProperties(ref shader);
                    break;
            }
        }

        private void SetDirectionalLightProperties(ref Shader shader)
        {
            shader.SetVector3("dirLight.direction", direction);
            shader.SetVector3("dirLight.ambient", ambient);
            shader.SetVector3("dirLight.diffuse", diffuse * intensity);
            shader.SetVector3("dirLight.specular", specular);
            shader.SetInt("hasDirLight", 1);
        }

        private void SetPointLightProperties(ref Shader shader)
        {
            shader.SetVector3($"pointLights[{pointLightID}].position", transform.Position);
            shader.SetVector3($"pointLights[{pointLightID}].ambient", ambient);
            shader.SetVector3($"pointLights[{pointLightID}].diffuse", diffuse * intensity);
            shader.SetVector3($"pointLights[{pointLightID}].specular", specular);
            shader.SetFloat($"pointLights[{pointLightID}].constant", constant);
            shader.SetFloat($"pointLights[{pointLightID}].linear", linear);
            shader.SetFloat($"pointLights[{pointLightID}].quadratic", quadratic);
            shader.SetInt("hasPointLight", 1);
        }

        private void SetSpotLightProperties(ref Shader shader)
        {
            shader.SetVector3("spotLight.position", transform.Position);
            shader.SetVector3("spotLight.direction", direction);
            shader.SetVector3("spotLight.ambient", ambient);
            shader.SetVector3("spotLight.diffuse", diffuse * intensity);
            shader.SetVector3("spotLight.specular", specular);
            shader.SetFloat("spotLight.constant", constant);
            shader.SetFloat("spotLight.linear", linear);
            shader.SetFloat("spotLight.quadratic", quadratic);
            shader.SetFloat("spotLight.cutOff", Mathf.Cos(Mathf.DegreesToRadians(cutOff)));
            shader.SetFloat("spotLight.outerCutOff", Mathf.Cos(Mathf.DegreesToRadians(outerCutOff)));
            shader.SetInt("hasSpotLight", 1);
        }

        public void LoadDefaultProperties()
        {
            ambient = new Vector3(0.05f, 0.05f, 0.05f);
            diffuse = new Vector3(0.3f, 0.3f, 0.3f);
            specular = new Vector3(1.0f, 1.0f, 1.0f);
            constant = 1.0f;
            linear = 0.09f;
            quadratic = 0.032f;

            // spotlight attributes
            cutOff = 12.5f;
            outerCutOff = 15.0f;

            direction = new Vector3(-0.2f, -1.0f, -0.3f);
            intensity = 1.0f;
        }
    }
}

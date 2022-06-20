using GLFW;
using static OpenGL.GL;
using PangordsEngine;
using PangordsEngine.Core;
using PangordsEngine.Shaders;
using PangordsEngine.Geometry;
using PangordsEngine.Lighting;
using System;
using System.IO;

class Program
{
    private const string TITLE = "Pangords Engine";

    // window
    const int width = 1920;
    const int height = 1080;

    // cursor
    static Camera cam = new Camera(new Vector3(0.0f, 0.0f, 3.0f));
    static float lastX = width / 2; // width / 2
    static float lastY = height / 2; // height / 2
    static bool firstMouse = true;

    // lighting
    static Vector3 lightPos = new Vector3(1.2f, 1.0f, 2.0f);

    static Model ourModel;

    static MouseCallback mouse_callback = new MouseCallback(GetMousePosition);
    static WindowContentsScaleCallback windowContentScaleCallback = new WindowContentsScaleCallback(OnWindowScale);

    static unsafe void Main(string[] args)
    {
        // glfw: initialize and configure
        // ------------------------------
        Glfw.Init();
        GameCore.PrepareContext(WindowMode.MaximizedWindow);

        var window = GameCore.CreateWindow(width, height, TITLE);

        Glfw.GetWindowSize(window, out int xscale, out int yscale);
        glViewport(0, 0, xscale, yscale);

        Glfw.MakeContextCurrent(window);
        Glfw.SetCursorPositionCallback(window, mouse_callback);

        // tell GLFW to capture our mouse
        Glfw.SetInputMode(window, InputMode.Cursor, (int)CursorMode.Disabled);

        // configure global opengl state
        // -----------------------------
        glEnable(GL_DEPTH_TEST);
        glEnable(GL_CULL_FACE);
        glCullFace(GL_BACK);
        glEnable(GL_DEPTH_CLAMP);
        glEnable(GL_BLEND);

        Shader lightingShader = ShaderUtility.LitShader();
        Shader lightCubeShader = ShaderUtility.UnlitShader();
        Shader modelShader = ShaderUtility.LitShader();
        Shader skyboxShader = ShaderUtility.SkyboxShader();

        ourModel = new Model(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Assets\sushi_bar\scene.obj");

        float[] vertices = GeometryHelper.CubeVertices;

        // first, configure the cube's VAO (and VBO)
        uint VBO, cubeVAO;
        glGenVertexArrays(1, &cubeVAO);
        glGenBuffers(1, &VBO);

        glBindBuffer(GL_ARRAY_BUFFER, VBO);
        fixed (float* v = vertices)
        {
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * vertices.Length, v, GL_STATIC_DRAW);
        }

        glBindVertexArray(cubeVAO);

        // position attribute
        glVertexAttribPointer(0, 3, GL_FLOAT, false, 8 * sizeof(float), (void*)0);
        glEnableVertexAttribArray(0);
        // normal attribute
        glVertexAttribPointer(1, 3, GL_FLOAT, false, 8 * sizeof(float), (void*)(3 * sizeof(float)));
        glEnableVertexAttribArray(1);
        // texture coord attribute
        glVertexAttribPointer(2, 2, GL_FLOAT, false, 8 * sizeof(float), (void*)(6 * sizeof(float)));
        glEnableVertexAttribArray(2);

        // second, configure the light's VAO (VBO stays the same; the vertices are the same for the light object which is also a 3D cube)
        uint lightCubeVAO;
        glGenVertexArrays(1, &lightCubeVAO);
        glBindVertexArray(lightCubeVAO);

        glVertexAttribPointer(0, 3, GL_FLOAT, false, 8 * sizeof(float), (void*)0);
        glEnableVertexAttribArray(0);

        float[] skyboxVertices = GeometryHelper.SkyboxVertices;

        // skybox
        uint skyboxVAO, skyboxVBO;
        glGenVertexArrays(1, &skyboxVAO);
        glGenBuffers(1, &skyboxVBO);
        glBindVertexArray(skyboxVAO);
        glBindBuffer(GL_ARRAY_BUFFER, skyboxVBO);
        fixed (void* data = skyboxVertices)
        {
            glBufferData(GL_ARRAY_BUFFER, sizeof(float) * skyboxVertices.Length, data, GL_STATIC_DRAW);
        }
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 3, GL_FLOAT, false, 3 * sizeof(float), (void*)0);

        // positions of the point lights
        Vector3[] pointLightPositions = {
        new Vector3( 0.7f,  0.2f,  2.0f),
        new Vector3( 2.3f, -3.3f, -4.0f),
        new Vector3(-4.0f,  2.0f, -12.0f),
        new Vector3( 0.0f,  0.0f, -3.0f)
        };

        // load cubemap
        string[] paths =
        {
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Assets\Skybox\right.jpg",
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Assets\Skybox\left.jpg",
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Assets\Skybox\top.jpg",
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Assets\Skybox\bottom.jpg",
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Assets\Skybox\front.jpg",
            Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"\Assets\Skybox\back.jpg",
        };
        uint cubemap = Texture.LoadCubemap(paths);

        skyboxShader.Use();
        skyboxShader.SetInt("skybox", 0);

        lightCubeShader.Use();
        lightCubeShader.SetVector4("color", 1.0f, 1.0f, 1.0f, 1.0f);

        double counter = 0;
        int frames = 0;

        Entity lightObject = new Entity();
        lightObject.GetComponent<Transform>().Position = new Vector3(1.0f, 1.0f, 1.0f);
        Light pointLight1 = (Light)lightObject.AddComponent<Light>();
        pointLight1.type = Light.LightType.Point;
        pointLight1.LoadDefaultProperties();
        pointLight1.pointLightID = 0;
        pointLight1.transform.Position = pointLightPositions[0];

        Entity pointLightObj1 = new Entity();
        pointLightObj1.AddComponent<Light>();
        Light pointLight2 = pointLightObj1.GetComponent<Light>();
        pointLight2.type = Light.LightType.Point;
        pointLight2.LoadDefaultProperties();
        pointLight2.pointLightID = 1;
        pointLight2.transform.Position = pointLightPositions[1];

        Entity pointLightObj2 = new Entity();
        pointLightObj2.AddComponent<Light>();
        Light pointLight3 = pointLightObj2.GetComponent<Light>();
        pointLight3.type = Light.LightType.Point;
        pointLight3.LoadDefaultProperties();
        pointLight3.pointLightID = 2;
        pointLight3.transform.Position = pointLightPositions[2];

        Entity pointLightObj3 = new Entity();
        pointLightObj3.AddComponent<Light>();
        Light pointLight4 = pointLightObj3.GetComponent<Light>();
        pointLight4.type = Light.LightType.Point;
        pointLight4.LoadDefaultProperties();
        pointLight4.pointLightID = 3;
        pointLight4.transform.Position = pointLightPositions[3];

        Entity directionalLightObj = new Entity();
        directionalLightObj.AddComponent<Light>();
        Light directionalLight = directionalLightObj.GetComponent<Light>();
        directionalLight.type = Light.LightType.Directional;
        directionalLight.ambient = new Vector3(0.05f, 0.05f, 0.05f);
        directionalLight.diffuse = new Vector3(0.4f, 0.4f, 0.4f);
        directionalLight.specular = new Vector3(0.5f, 0.5f, 0.5f);
        directionalLight.direction = new Vector3(-0.2f, -1.0f, -0.3f);
        directionalLight.intensity = 2.0f;

        lightingShader.Use();
        lightingShader.SetFloat("material.shininess", 1.0f);
        lightingShader.SetInt("nrOfPointLights", 4);

        // render loop
        // -----------
        while (!Glfw.WindowShouldClose(window))
        {
            // per-frame time logic
            // --------------------
            Time.CalculateDeltaTime();

            counter++;
            if (Time.DeltaTime >= 1.0f / 30.0f)
            {
                string fps_str = ((int)(1.0f / Time.DeltaTime * counter)).ToString();
                string ms = (Time.DeltaTime / counter * 1000.0f).ToString();
                string newTitle = TITLE + " - " + fps_str + " FPS / " + ms + " ms";
                Glfw.SetWindowTitle(window, newTitle);
                counter = 0;
            }

            // input
            ProcessInput(window);

            // rendering
            glClearColor(0.1f, 0.1f, 0.1f, 1.0f); // sets backgroung color to dark grey
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            lightingShader.Use();
            lightingShader.SetVector3("viewPos", cam.transform.Position);

            // view/projection transformations
            Matrix4x4 projection = Mathf.Perspective(Mathf.DegreesToRadians(cam.FieldOfView), (float)width / (float)height, 0.1f, 100.0f);
            Matrix4x4 view = cam.GetViewMatrix();
            lightingShader.SetMatrix4("projection", projection);
            lightingShader.SetMatrix4("view", view);

            // render the loaded model
            Matrix4x4 model = new Matrix4x4(1.0f);
            model = Mathf.Translate(model, new Vector3(0.0f, 0.0f, 0.0f)); // translate it down so it's at the center of the scene
            model = Mathf.Scale(model, new Vector3(0.5f, 0.5f, 0.5f)); // it's a bit too big for our scene, so scale it down
            lightingShader.SetMatrix4("model", model);

            // applies lighting calculations to shader
            pointLight1.Process(ref lightingShader);
            directionalLight.Process(ref lightingShader);
            pointLight2.Process(ref lightingShader);
            pointLight3.Process(ref lightingShader);
            pointLight4.Process(ref lightingShader);

            ourModel.Draw(lightingShader); // draws model

            // draws lamp cube
            lightCubeShader.Use();
            lightCubeShader.SetMatrix4("projection", projection);
            lightCubeShader.SetMatrix4("view", view);
            model = new Matrix4x4(1.0f);
            model = Mathf.Translate(model, lightPos);
            model = Mathf.Scale(model, new Vector3(0.2f));
            lightCubeShader.SetMatrix4("model", model);

            glBindVertexArray(lightCubeVAO);
            glDrawArrays(GL_TRIANGLES, 0, 36);

            glDepthFunc(GL_LEQUAL);  // change depth function so depth test passes when values are equal to depth buffer's content
            skyboxShader.Use();
            view = cam.GetViewMatrix(); // remove translation from the view matrix
            skyboxShader.SetMatrix4("view", view);
            skyboxShader.SetMatrix4("projection", projection);
            // skybox cube
            glBindVertexArray(skyboxVAO);
            glActiveTexture(GL_TEXTURE0);
            glBindTexture(GL_TEXTURE_CUBE_MAP, cubemap);
            glDrawArrays(GL_TRIANGLES, 0, 36);
            glBindVertexArray(0);
            glDepthFunc(GL_LESS); // set depth function back to default

            // glfw: swap buffers and poll IO events (keys pressed/released, mouse moved etc.)
            Glfw.SwapBuffers(window);
            Glfw.PollEvents();
        }

        // optional: de-allocate all resources once they've outlived their purpose:
        glDeleteVertexArrays(1, &cubeVAO);
        glDeleteBuffers(1, &VBO);
        glDeleteVertexArrays(1, &skyboxVAO);
        glDeleteBuffers(1, &skyboxVBO);

        // glfw: terminate, clearing all previously allocated GLFW resources.
        // ------------------------------------------------------------------
        Glfw.Terminate();
    }

    static void OnWindowScale(Window window, float xScale, float yScale)
    {
        glViewport(0, 0, (int)width, (int)height);
    }

    static void GetMousePosition(Window window, double xpos, double ypos)
    {
        float _xpos = (float)xpos;
        float _ypos = (float)ypos;

        if (firstMouse)
        {
            lastX = _xpos;
            lastY = _ypos;
            firstMouse = false;
        }

        float xoffset = _xpos - lastX;
        float yoffset = lastY - _ypos; // reversed since y-coordinates go from bottom to top

        lastX = _xpos;
        lastY = _ypos;

        cam.ProcessMouseMovement(xoffset, yoffset, true);
    }  

    static void ProcessInput(Window window)
    {
        if (Input.GetKey(window, Keys.Escape, InputState.Press))
        {
            Glfw.SetWindowShouldClose(window, true);
        }

        if (Input.GetKey(window, Keys.W, InputState.Press))
            cam.ProcessKeyboard(CameraMovement.Forward, Time.DeltaTime);
        if (Input.GetKey(window, Keys.S, InputState.Press))
            cam.ProcessKeyboard(CameraMovement.Backward, Time.DeltaTime);
        if (Input.GetKey(window, Keys.A, InputState.Press))
            cam.ProcessKeyboard(CameraMovement.Left, Time.DeltaTime);
        if (Input.GetKey(window, Keys.D, InputState.Press))
            cam.ProcessKeyboard(CameraMovement.Right, Time.DeltaTime);
    }
}
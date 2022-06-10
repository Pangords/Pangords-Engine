using GLFW;
using static OpenGL.GL;
using PangordsEngine;
using PangordsEngine.Core;
using PangordsEngine.Shaders;
using PangordsEngine.Geometry;

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

        ourModel = new Model(@"D:\3DModels\tennis\tenniscout_wii.obj");

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
            @"C:\Users\vasco\Downloads\skybox\right.jpg",
            @"C:\Users\vasco\Downloads\skybox\left.jpg",
            @"C:\Users\vasco\Downloads\skybox\top.jpg",
            @"C:\Users\vasco\Downloads\skybox\bottom.jpg",
            @"C:\Users\vasco\Downloads\skybox\front.jpg",
            @"C:\Users\vasco\Downloads\skybox\back.jpg"
        };
        uint cubemap = Texture.LoadCubemap(paths);

        skyboxShader.Use();
        skyboxShader.SetInt("skybox", 0);

        lightCubeShader.Use();
        lightCubeShader.SetVector4("color", 1.0f, 1.0f, 1.0f, 1.0f);

        double counter = 0;
        int frames = 0;

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
            // -----
            ProcessInput(window);

            // render
            // ------
            glClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            //// be sure to activate shader when setting uniforms/drawing objects
            lightingShader.Use();
            lightingShader.SetVector3("viewPos", cam.transform.position);
            lightingShader.SetFloat("material.shininess", 1.0f);
            lightingShader.SetInt("nrOfPointLights", 4);

            // directional light
            lightingShader.SetVector3("dirLight.direction", -0.2f, -1.0f, -0.3f);
            lightingShader.SetVector3("dirLight.ambient", 0.05f, 0.05f, 0.05f);
            lightingShader.SetVector3("dirLight.diffuse", 0.4f, 0.4f, 0.4f);
            lightingShader.SetVector3("dirLight.specular", 0.5f, 0.5f, 0.5f);
            // point light 1
            lightingShader.SetVector3("pointLights[0].position", pointLightPositions[0]);
            lightingShader.SetVector3("pointLights[0].ambient", 0.05f, 0.05f, 0.05f);
            lightingShader.SetVector3("pointLights[0].diffuse", 0.3f, 0.3f, 0.3f);
            lightingShader.SetVector3("pointLights[0].specular", 1.0f, 1.0f, 1.0f);
            lightingShader.SetFloat("pointLights[0].constant", 1.0f);
            lightingShader.SetFloat("pointLights[0].linear", 0.09f);
            lightingShader.SetFloat("pointLights[0].quadratic", 0.032f);
            // point light 2
            lightingShader.SetVector3("pointLights[1].position", pointLightPositions[1]);
            lightingShader.SetVector3("pointLights[1].ambient", 0.05f, 0.05f, 0.05f);
            lightingShader.SetVector3("pointLights[1].diffuse", 0.3f, 0.3f, 0.3f);
            lightingShader.SetVector3("pointLights[1].specular", 1.0f, 1.0f, 1.0f);
            lightingShader.SetFloat("pointLights[1].constant", 1.0f);
            lightingShader.SetFloat("pointLights[1].linear", 0.09f);
            lightingShader.SetFloat("pointLights[1].quadratic", 0.032f);
            // point light 3
            lightingShader.SetVector3("pointLights[2].position", pointLightPositions[2]);
            lightingShader.SetVector3("pointLights[2].ambient", 0.05f, 0.05f, 0.05f);
            lightingShader.SetVector3("pointLights[2].diffuse", 0.3f, 0.3f, 0.3f);
            lightingShader.SetVector3("pointLights[2].specular", 1.0f, 1.0f, 1.0f);
            lightingShader.SetFloat("pointLights[2].constant", 1.0f);
            lightingShader.SetFloat("pointLights[2].linear", 0.09f);
            lightingShader.SetFloat("pointLights[2].quadratic", 0.032f);
            // point light 4
            lightingShader.SetVector3("pointLights[3].position", pointLightPositions[3]);
            lightingShader.SetVector3("pointLights[3].ambient", 0.05f, 0.05f, 0.05f);
            lightingShader.SetVector3("pointLights[3].diffuse", 0.3f, 0.3f, 0.3f);
            lightingShader.SetVector3("pointLights[3].specular", 1.0f, 1.0f, 1.0f);
            lightingShader.SetFloat("pointLights[3].constant", 1.0f);
            lightingShader.SetFloat("pointLights[3].linear", 0.09f);
            lightingShader.SetFloat("pointLights[3].quadratic", 0.032f);
            // spotLight
            lightingShader.SetVector3("spotLight.position", cam.transform.position);
            lightingShader.SetVector3("spotLight.direction", cam.Front);
            lightingShader.SetVector3("spotLight.ambient", 0.0f, 0.0f, 0.0f);
            lightingShader.SetVector3("spotLight.diffuse", 0.1f, 0.1f, 0.1f);
            lightingShader.SetVector3("spotLight.specular", 0.1f, 0.1f, 0.1f);
            lightingShader.SetFloat("spotLight.constant", 1.0f);
            lightingShader.SetFloat("spotLight.linear", 0.09f);
            lightingShader.SetFloat("spotLight.quadratic", 0.032f);
            lightingShader.SetFloat("spotLight.cutOff", Mathf.Cos(Mathf.DegreesToRadians(12.5f)));
            lightingShader.SetFloat("spotLight.outerCutOff", Mathf.Cos(Mathf.DegreesToRadians(15.0f)));

            // view/projection transformations
            Matrix4x4 projection = Mathf.Perspective(Mathf.DegreesToRadians(cam.FieldOfView), (float)width / (float)height, 0.1f, 100.0f);
            Matrix4x4 view = cam.GetViewMatrix();
            lightingShader.SetMatrix4("projection", projection);
            lightingShader.SetMatrix4("view", view);

            // render the loaded model
            Matrix4x4 model = new Matrix4x4(1.0f);
            model = Mathf.Translate(model, new Vector3(0.0f, 0.0f, 0.0f)); // translate it down so it's at the center of the scene
            model = Mathf.Scale(model, new Vector3(0.5f, 0.5f, 0.5f)); // it's a bit too big for our scene, so scale it down
            //model = Mathf.Rotate(model, 10f, new Vector3(90f, 0, 0));
            lightingShader.SetMatrix4("model", model);
            ourModel.Draw(lightingShader);

            // also draw the lamp object
            lightCubeShader.Use();
            lightCubeShader.SetMatrix4("projection", projection);
            lightCubeShader.SetMatrix4("view", view);
            model = new Matrix4x4(1.0f);
            model = Mathf.Translate(model, lightPos);
            model = Mathf.Scale(model, new Vector3(0.2f)); // a smaller cube
            lightCubeShader.SetMatrix4("model", model);

            glBindVertexArray(lightCubeVAO);
            glDrawArrays(GL_TRIANGLES, 0, 36);

            // ------------------------------------------------------------------------------------
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
            // -------------------------------------------------------------------------------
            Glfw.SwapBuffers(window);
            Glfw.PollEvents();
        }

        // optional: de-allocate all resources once they've outlived their purpose:
        // ------------------------------------------------------------------------
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

        cam.ProcessMouseMovement(xoffset, yoffset);
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
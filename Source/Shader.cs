using System;
using static OpenGL.GL;
using System.IO;

namespace PangordsEngine.Shaders
{
    class Shader
    {
        public uint ID;

        public string vertexCode;
        public string fragmentCode;

        public Shader() { }

        public Shader(string vertexPath, string fragmentPath)
        {
            try
            {
                if (File.Exists(vertexPath))
                {
                    vertexCode = File.ReadAllText(vertexPath);
                }

                if (File.Exists(fragmentPath))
                {
                    fragmentCode = File.ReadAllText(fragmentPath);
                }
            }
            catch(Exception _ex)
            {
                Console.WriteLine("Failed to load shader: " + _ex);
            }

            uint vertex = glCreateShader(GL_VERTEX_SHADER);
            glShaderSource(vertex, vertexCode);
            glCompileShader(vertex);
            CheckCompileErrors(vertex, "VERTEX");

            uint fragment = glCreateShader(GL_FRAGMENT_SHADER);
            glShaderSource(fragment, fragmentCode);
            glCompileShader(fragment);
            CheckCompileErrors(vertex, "FRAGMENT");

            ID = glCreateProgram();
            glAttachShader(ID, vertex);
            glAttachShader(ID, fragment);
            glLinkProgram(ID);
            CheckCompileErrors(ID, "PROGRAM");

            glDeleteShader(vertex);
            glDeleteShader(fragment);
        }

        public void CopyShader(Shader shader)
        {
            vertexCode = shader.vertexCode;
            fragmentCode = shader.fragmentCode;
        }

        public void Use()
        {
            glUseProgram(ID);
        }

        public void SetInt(string name, int value)
        {
            glUniform1i(glGetUniformLocation(ID, name), value);
        }

        public void SetFloat(string name, float value)
        {
            glUniform1f(glGetUniformLocation(ID, name), value);
        }

        public unsafe void SetVector2(string name, float x, float y)
        {
            glUniform2f(glGetUniformLocation(ID, name), x, y);
        }

        public unsafe void SetVector3(string name, float x, float y, float z)
        {
            glUniform3f(glGetUniformLocation(ID, name), x, y, z);
        }

        public unsafe void SetVector4(string name, float x, float y, float z, float w)
        {
            glUniform4f(glGetUniformLocation(ID, name), x, y, z, w);
        }

        public unsafe void SetVector2(string name, Vector2 value)
        {
            glUniform2f(glGetUniformLocation(ID, name), value.x, value.y);
        }

        public unsafe void SetVector3(string name, Vector3 value)
        {
            glUniform3f(glGetUniformLocation(ID, name), value.x, value.y, value.z);
        }

        public unsafe void SetVector4(string name, Vector4 value)
        {
            glUniform4f(glGetUniformLocation(ID, name), value.x, value.y, value.z, value.w);
        }

        public unsafe void SetMatrix4(string name, Matrix4x4 matrix)
        {
            fixed (float* ptr = matrix.to_array())
            {
                glUniformMatrix4fv(glGetUniformLocation(ID, name), 1, false, ptr);
            }
        }

        private unsafe void CheckCompileErrors(uint shader, string type)
        {
            int success;
            string infoLog;
            if (type != "PROGRAM")
            {
                glGetShaderiv(shader, GL_COMPILE_STATUS, &success);
                if (success == 0)
                {
                    infoLog = glGetShaderInfoLog(shader, 1024);
                    Console.WriteLine("ERROR::SHADER_COMPILATION_ERROR of type: " + type + "\n"  + infoLog  + "\n -- --------------------------------------------------- -- ");
                }
            }
            else
            {
                glGetProgramiv(shader, GL_LINK_STATUS, &success);
                if (success == 0)
                {
                    infoLog = glGetProgramInfoLog(shader, 1024);
                    Console.WriteLine("ERROR::PROGRAM_LINKING_ERROR of type: " + type + "\n" + infoLog + "\n -- --------------------------------------------------- -- ");
                }
            }
        }
    }
}

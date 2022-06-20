using GLFW;
using static OpenGL.GL;
using System.Runtime.InteropServices;
using PangordsEngine.Shaders;
using System.Collections.Generic;
using System;

namespace PangordsEngine
{
    //model load
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        const int MAX_BONE_INFLUENCE = 4;

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;
        // tangent
        public Vector3 Tangent;
        // bitangent
        public Vector3 Bitangent;
        //bone indexes which will influence this vertex
        public int[] m_BoneIDs;
        //weights from each bone
        public float[] m_Weights;

        public Vertex(int MAX_BONE_INFLUENCE)
        {
            Position = new Vector3();
            Normal = new Vector3();
            TexCoords = new Vector2();
            Tangent = new Vector3();
            Bitangent = new Vector3();
            m_BoneIDs = new int[MAX_BONE_INFLUENCE];
            m_Weights = new float[MAX_BONE_INFLUENCE];
        }
    };

    class Mesh
    {
        // mesh data
        List<Vertex> vertices = new List<Vertex>();
        List<uint> indices = new List<uint>();
        List<Texture> textures = new List<Texture>();

        uint VAO;

        public Mesh(List<Vertex> vertices, List<uint> indices, List<Texture> textures)
        {
            this.vertices = vertices;
            this.indices = indices;
            this.textures = textures;

            SetupMesh();
        }

        public unsafe void Draw(Shader shader)
        {
            // bind appropriate textures
            uint diffuseNr = 1;
            uint specularNr = 1;
            uint normalNr = 1;
            uint heightNr = 1;
            for (int i = 0; i < textures.Count; i++)
            {
                glActiveTexture(GL_TEXTURE0 + i); // active proper texture unit before binding
                                                  // retrieve texture number (the N in diffuse_textureN)
                string number = "";
                string name = textures[i].type;
                if (name == "texture_diffuse")
                    number = diffuseNr++.ToString();
                else if (name == "texture_specular")
                    number = specularNr++.ToString(); // transfer unsigned int to string
                else if (name == "texture_normal")
                    number = normalNr++.ToString(); // transfer unsigned int to string
                else if (name == "texture_height")
                    number = heightNr++.ToString(); // transfer unsigned int to string

                // now set the sampler to the correct texture unit
                //glUniform1i(glGetUniformLocation(shader.ID, (name + number)), i);
                shader.Use();
                if (textures[i].id == 0)
                    shader.SetVector4("baseColor", textures[i].color);
                else
                    shader.SetInt(name + number, i);
                // and finally bind the texture
                glBindTexture(GL_TEXTURE_2D, textures[i].id);
            }

            // draw mesh
            glBindVertexArray(VAO);
            glDrawElements(GL_TRIANGLES, indices.Count, GL_UNSIGNED_INT, (void*)0);
            glBindVertexArray(0);

            // always good practice to set everything back to defaults once configured.
            glActiveTexture(GL_TEXTURE0);
        }

        //  render data
        private uint VBO, EBO;

        unsafe void SetupMesh()
        {
            fixed (uint* vao = &VAO)
            {
                glGenVertexArrays(1, vao);
            }
            fixed (uint* vbo = &VBO)
            {
                glGenBuffers(1, vbo);
            }
            fixed (uint* ebo = &EBO)
            {
                glGenBuffers(1, ebo);
            }

            glBindVertexArray(VAO);
            glBindBuffer(GL_ARRAY_BUFFER, VBO);

            //converts array of vertices into IntPtr
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(vertices[0]) * vertices.Count);
            long LongPtr = ptr.ToInt64(); // Must work both on x86 and x64
            for (int I = 0; I < vertices.Count; I++)
            {
                IntPtr RectPtr = new IntPtr(LongPtr);
                Marshal.StructureToPtr(vertices[I], RectPtr, false); // You do not need to erase struct in this case
                LongPtr += Marshal.SizeOf(typeof(Vertex));
            }
            glBufferData(GL_ARRAY_BUFFER, vertices.Count * Marshal.SizeOf(typeof(Vertex)), ptr, GL_STATIC_DRAW);

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
            fixed (void* data = indices.ToArray())
            {
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.Count * sizeof(uint), data, GL_STATIC_DRAW);
            }

            // vertex positions
            glEnableVertexAttribArray(0);
            glVertexAttribPointer(0, 3, GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), (void*)0);
            // vertex normals
            glEnableVertexAttribArray(1);
            glVertexAttribPointer(1, 3, GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), (void*)Marshal.OffsetOf(typeof(Vertex), "Normal"));
            // vertex texture coords
            glEnableVertexAttribArray(2);
            glVertexAttribPointer(2, 2, GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), (void*)Marshal.OffsetOf(typeof(Vertex), "TexCoords"));
            // vertex normals
            glEnableVertexAttribArray(3);
            glVertexAttribPointer(3, 3, GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), (void*)Marshal.OffsetOf(typeof(Vertex), "Tangent"));
            // vertex texture coords
            glEnableVertexAttribArray(4);
            glVertexAttribPointer(4, 3, GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), (void*)Marshal.OffsetOf(typeof(Vertex), "Bitangent"));
            // vertex normals
            glEnableVertexAttribArray(5);
            glVertexAttribPointer(5, 4, GL_INT, false, Marshal.SizeOf(typeof(Vertex)), (void*)Marshal.OffsetOf(typeof(Vertex), "m_BoneIDs"));
            // vertex texture coords
            glEnableVertexAttribArray(6);
            glVertexAttribPointer(6, 4, GL_FLOAT, false, Marshal.SizeOf(typeof(Vertex)), (void*)Marshal.OffsetOf(typeof(Vertex), "m_Weights"));

            glBindVertexArray(0);
        }
    }
}
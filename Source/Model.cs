using Assimp;
using PangordsEngine.Shaders;
using System.Collections.Generic;
using System;
using System.IO;

namespace PangordsEngine
{
    class Model
    {
        List<Texture> texturesLoaded = new List<Texture>();
        
        public Model(string path)
        {
            LoadModel(path);
        }
        public void Draw(Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
                meshes[i].Draw(shader);
        }

        // model data
        List<Mesh> meshes = new List<Mesh>();
        string directory = "";

        public void LoadModel(string path)
        {
            AssimpContext import = new AssimpContext();
            Scene scene = import.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace);

            if (scene == null || scene.SceneFlags == SceneFlags.Incomplete || scene.RootNode == null)
            {
                Console.WriteLine("ERROR::ASSIMP::");
                return;
            }

            //directory = path.Remove(path.LastIndexOf(@"\"));
            directory = path.Substring(0, path.LastIndexOf(@"\"));
            Console.WriteLine(directory);

            ProcessNode(scene.RootNode, scene);
        }

        void ProcessNode(Node node, Scene scene)
        {
            // process all the node's meshes (if any)
            for(int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene));
            }
            // then do the same for each of its children
            for(int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }
        Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<uint> indices = new List<uint>();
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex(4);
                Vector3 vector = new Vector3(); // we declare a placeholder vector since assimp uses its own vector class that doesn't directly convert to glm's vec3 class so we transfer the data to this placeholder glm::vec3 first.
                                // positions
                vector.x = mesh.Vertices[i].X;
                vector.y = mesh.Vertices[i].Y;
                vector.z = mesh.Vertices[i].Z;
                vertex.Position = vector;
                // normals
                if (mesh.HasNormals)
                {
                    vector.x = mesh.Normals[i].X;
                    vector.y = mesh.Normals[i].Y;
                    vector.z = mesh.Normals[i].Z;
                    vertex.Normal = vector;
                }
                // texture coordinates
                if (mesh.HasTextureCoords(0)) // does the mesh contain texture coordinates?
                {
                    Vector2 vec;
                    // a vertex can contain up to 8 different texture coordinates. We thus make the assumption that we won't 
                    // use models where a vertex can have multiple texture coordinates so we always take the first set (0).
                    vec.x = mesh.TextureCoordinateChannels[0][i].X;
                    vec.y = mesh.TextureCoordinateChannels[0][i].Y;
                    vertex.TexCoords = vec;
                    // tangent
                    vector.x = mesh.Tangents[i].X;
                    vector.y = mesh.Tangents[i].Y;
                    vector.z = mesh.Tangents[i].Z;
                    vertex.Tangent = vector;
                    // bitangent
                    vector.x = mesh.BiTangents[i].X;
                    vector.y = mesh.BiTangents[i].Y;
                    vector.z = mesh.BiTangents[i].Z;
                    vertex.Bitangent = vector;
                }
                else
                    vertex.TexCoords = new Vector2(0.0f, 0.0f);

                vertices.Add(vertex);
            }
            // now wak through each of the mesh's faces (a face is a mesh its triangle) and retrieve the corresponding vertex indices.
            for(int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                // retrieve all indices of the face and store them in the indices vector
                for(int j = 0; j < face.IndexCount; j++)
                    indices.Add((uint)face.Indices[j]);        
            }
            // process materials
            Material material = scene.Materials[mesh.MaterialIndex];
            // we assume a convention for sampler names in the shaders. Each diffuse texture should be named
            // as 'texture_diffuseN' where N is a sequential number ranging from 1 to MAX_SAMPLER_NUMBER. 
            // Same applies to other texture as the following list summarizes:
            // diffuse: texture_diffuseN
            // specular: texture_specularN
            // normal: texture_normalN

            if (!material.HasTextureDiffuse && !material.HasTextureSpecular && !material.HasTextureNormal && !material.HasTextureHeight)
            {
                Texture texture;

                if (material.HasColorDiffuse)
                {
                    texture = new Texture(new Vector4(material.ColorDiffuse.R,
                                                      material.ColorDiffuse.G,
                                                      material.ColorDiffuse.B,
                                                      material.ColorDiffuse.A));
                }
                else
                {
                    uint id = Texture.LoadTexture(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "/Textures/DefaultTexture.png", true, 0);
                    string type = "texture_diffuse";
                    texture = new Texture(id, type);
                }

                textures.Add(texture);
                return new Mesh(vertices, indices, textures);
            }

            // 1. diffuse maps
            List<Texture> diffuseMaps = LoadMaterialTextures(material, Assimp.TextureType.Diffuse, "texture_diffuse");
            textures.AddRange(diffuseMaps);
            // 2. specular maps
            List<Texture> specularMaps = LoadMaterialTextures(material, Assimp.TextureType.Specular, "texture_specular");
            textures.AddRange(specularMaps);
            // 3. normal maps
            List<Texture> normalMaps = LoadMaterialTextures(material, Assimp.TextureType.Height, "texture_normal");
            textures.AddRange(normalMaps);
            // 4. height maps
            List<Texture> heightMaps = LoadMaterialTextures(material, Assimp.TextureType.Ambient, "texture_height");
            textures.AddRange(heightMaps);
        
            // return a mesh object created from the extracted mesh data
            return new Mesh(vertices, indices, textures);
        }  

        List<Texture> LoadMaterialTextures(Material mat, Assimp.TextureType type, string typeName)
        {
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mat.GetMaterialTextureCount(type); i++)
            {
                TextureSlot slot;
                mat.GetMaterialTexture(type, i, out slot);
                bool skip = false;

                for (int j = 0; j < texturesLoaded.Count; j++)
                {
                    if (texturesLoaded[j].path == slot.FilePath)
                    {
                        textures.Add(texturesLoaded[j]);
                        skip = true;
                        break;
                    }
                }

                if (!skip && slot.FilePath != null)
                {
                    Console.WriteLine(slot.FilePath);
                    string dir = directory + @"\" + slot.FilePath;
                    Console.WriteLine(dir);
                    uint id = Texture.LoadTexture(dir, true, 0);
                    Texture texture = new Texture(id, typeName, slot.FilePath);
                    textures.Add(texture);
                    texturesLoaded.Add(texture);
                }
            }
            return textures;
        }
    }
}
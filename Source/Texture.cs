using System;
using static OpenGL.GL;
using StbImageSharp;
using System.Runtime.InteropServices;
using System.IO;

namespace PangordsEngine
{
    class Texture
    {
        public TextureType textureType;
        public uint id { get; set; }
        public string type;
        public string path;
        public Vector4 color;

        public Texture(Vector4 color)
        {
            this.color = color;
        }

        public Texture(uint id, string type)
        {
            this.id = id;
            this.type = type;
        }

        public Texture(uint id, string type, string path)
        {
            this.id = id;
            this.type = type;
            this.path = path;
        }

        public unsafe static uint LoadTexture(string path, bool transparent, int verticalFlip)
        {
            uint texture;
            glGenTextures(1, &texture);

            if (verticalFlip > 1) verticalFlip = 1;
            StbImage.stbi_set_flip_vertically_on_load(verticalFlip);

            using (var stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, transparent ? ColorComponents.RedGreenBlueAlpha : ColorComponents.RedGreenBlue);

                GCHandle pinnedArray = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();

                glBindTexture(GL_TEXTURE_2D, texture);
                glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, image.Width, image.Height, 0, transparent ? GL_RGBA : GL_RGB, GL_UNSIGNED_BYTE, pointer);
                glGenerateMipmap(GL_TEXTURE_2D);

                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

                pinnedArray.Free();
            }

            return texture;
        }

        public static unsafe uint LoadCubemap(string[] paths)
        {
            uint texture;
            glGenTextures(1, &texture);
            glBindTexture(GL_TEXTURE_CUBE_MAP, texture);

            for (int i = 0; i < paths.Length; i++)
            {
                using (var stream = File.OpenRead(paths[i]))
                {
                    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);

                    GCHandle pinnedArray = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                    IntPtr pointer = pinnedArray.AddrOfPinnedObject();

                    glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, GL_RGB, image.Width, image.Height, 0, GL_RGB, GL_UNSIGNED_BYTE, pointer);

                    pinnedArray.Free();
                }
            }

            glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
            glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);

            return texture;
        }
    }

    enum TextureType
    {
        Default,
        Sprite,
        Cubemap
    }
}
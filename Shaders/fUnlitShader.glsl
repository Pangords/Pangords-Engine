#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D texture_diffuse;
uniform vec4 color;

void main()
{    
    vec4 diffuse = texture(texture_diffuse, TexCoords);
    
    if (length(diffuse) == 0.0) {
        FragColor = color;
    }
    else {
        FragColor = diffuse;
    }
}
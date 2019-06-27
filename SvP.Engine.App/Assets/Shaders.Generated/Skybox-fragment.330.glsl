#version 330 core

struct Shaders_Skybox_VSInput
{
    vec3 Position;
};

struct Shaders_Skybox_FSInput
{
    vec4 Position;
    vec3 TexCoord;
};

layout(std140) uniform Projection
{
    mat4 field_Projection;
};

layout(std140) uniform View
{
    mat4 field_View;
};

uniform samplerCube CubeTexture;

vec4 FS(Shaders_Skybox_FSInput input_)
{
    return texture(CubeTexture, input_.TexCoord);
}


in vec3 fsin_0;
out vec4 _outputColor_;

void main()
{
    Shaders_Skybox_FSInput input_;
    input_.Position = gl_FragCoord;
    input_.TexCoord = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

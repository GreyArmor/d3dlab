#version 330 core

struct Shaders_Grid_VSInput
{
    vec3 Position;
};

struct Shaders_Grid_FSInput
{
    vec4 FragPosition;
    vec3 WorldPosition;
};

layout(std140) uniform Projection
{
    mat4 field_Projection;
};

layout(std140) uniform View
{
    mat4 field_View;
};

uniform sampler2D GridTexture;

vec4 FS(Shaders_Grid_FSInput input_)
{
    return texture(GridTexture, vec2(input_.WorldPosition.x, input_.WorldPosition.z) / 10.0f);
}


in vec3 fsin_0;
out vec4 _outputColor_;

void main()
{
    Shaders_Grid_FSInput input_;
    input_.FragPosition = gl_FragCoord;
    input_.WorldPosition = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

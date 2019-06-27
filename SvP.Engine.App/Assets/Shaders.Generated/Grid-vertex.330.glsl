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

Shaders_Grid_FSInput VS(Shaders_Grid_VSInput input_)
{
    Shaders_Grid_FSInput output_;
    output_.FragPosition =field_Projection * field_View * vec4(input_.Position, 1);
    output_.WorldPosition =input_.Position;
    return output_;
}


in vec3 Position;
out vec3 fsin_0;

void main()
{
    Shaders_Grid_VSInput input_;
    input_.Position = Position;
    Shaders_Grid_FSInput output_ = VS(input_);
    fsin_0 = output_.WorldPosition;
    gl_Position = output_.FragPosition;
        gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;
}

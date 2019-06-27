#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct Shaders_Grid_VSInput
{
    vec3 Position;
};

struct Shaders_Grid_FSInput
{
    vec4 FragPosition;
    vec3 WorldPosition;
};

layout(set = 0, binding = 0) uniform Projection
{
    mat4 field_Projection;
};

layout(set = 0, binding = 1) uniform View
{
    mat4 field_View;
};

layout(set = 0, binding = 2) uniform texture2D GridTexture;
layout(set = 0, binding = 3) uniform sampler GridSampler;
Shaders_Grid_FSInput VS(Shaders_Grid_VSInput input_)
{
    Shaders_Grid_FSInput output_;
    output_.FragPosition =field_Projection * field_View * vec4(input_.Position, 1);
    output_.WorldPosition =input_.Position;
    return output_;
}


layout(location = 0) in vec3 Position;
layout(location = 0) out vec3 fsin_0;

void main()
{
    Shaders_Grid_VSInput input_;
    input_.Position = Position;
    Shaders_Grid_FSInput output_ = VS(input_);
    fsin_0 = output_.WorldPosition;
    gl_Position = output_.FragPosition;
        gl_Position.y = -gl_Position.y; // Correct for Vulkan clip coordinates
}

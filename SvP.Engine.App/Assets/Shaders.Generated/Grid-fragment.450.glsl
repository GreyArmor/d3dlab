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
vec4 FS(Shaders_Grid_FSInput input_)
{
    return texture(sampler2D(GridTexture, GridSampler), vec2(input_.WorldPosition.x, input_.WorldPosition.z) / 10.0f);
}


layout(location = 0) in vec3 fsin_0;
layout(location = 0) out vec4 _outputColor_;

void main()
{
    Shaders_Grid_FSInput input_;
    input_.FragPosition = gl_FragCoord;
    input_.WorldPosition = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

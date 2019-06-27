#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct Shaders_ShadowDepth_VertexInput
{
    vec3 Position;
    vec3 Normal;
    vec2 TexCoord;
};

struct Shaders_ShadowDepth_FragmentInput
{
    vec4 Position;
};

layout(set = 0, binding = 0) uniform ViewProjection
{
    mat4 field_ViewProjection;
};

layout(set = 1, binding = 0) uniform World
{
    mat4 field_World;
};

Shaders_ShadowDepth_FragmentInput VS(Shaders_ShadowDepth_VertexInput input_)
{
    Shaders_ShadowDepth_FragmentInput output_;
    output_.Position =field_ViewProjection * field_World * vec4(input_.Position, 1);
    output_.Position.y +=input_.TexCoord.y * .0001f;
    return output_;
}


layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;

void main()
{
    Shaders_ShadowDepth_VertexInput input_;
    input_.Position = Position;
    input_.Normal = Normal;
    input_.TexCoord = TexCoord;
    Shaders_ShadowDepth_FragmentInput output_ = VS(input_);
    gl_Position = output_.Position;
        gl_Position.y = -gl_Position.y; // Correct for Vulkan clip coordinates
}

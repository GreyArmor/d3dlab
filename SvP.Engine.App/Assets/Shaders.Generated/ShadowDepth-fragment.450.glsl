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

void FS(Shaders_ShadowDepth_FragmentInput input_)
{
}



void main()
{
    Shaders_ShadowDepth_FragmentInput input_;
    input_.Position = gl_FragCoord;
    FS(input_);
}

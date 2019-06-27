#version 330 core

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

layout(std140) uniform ViewProjection
{
    mat4 field_ViewProjection;
};

layout(std140) uniform World
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

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

Shaders_ShadowDepth_FragmentInput VS(Shaders_ShadowDepth_VertexInput input_)
{
    Shaders_ShadowDepth_FragmentInput output_;
    output_.Position =field_ViewProjection * field_World * vec4(input_.Position, 1);
    output_.Position.y +=input_.TexCoord.y * .0001f;
    return output_;
}


in vec3 Position;
in vec3 Normal;
in vec2 TexCoord;

void main()
{
    Shaders_ShadowDepth_VertexInput input_;
    input_.Position = Position;
    input_.Normal = Normal;
    input_.TexCoord = TexCoord;
    Shaders_ShadowDepth_FragmentInput output_ = VS(input_);
    gl_Position = output_.Position;
        gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;
}

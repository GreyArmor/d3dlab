#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct Shaders_FullScreenQuad_VertexInput
{
    vec2 Position;
    vec2 TexCoords;
};

struct Shaders_FullScreenQuad_FragmentInput
{
    vec4 Position;
    vec2 TexCoords;
};

layout(set = 0, binding = 0) uniform texture2D SourceTexture;
layout(set = 0, binding = 1) uniform sampler SourceSampler;
Shaders_FullScreenQuad_FragmentInput VS(Shaders_FullScreenQuad_VertexInput input_)
{
    Shaders_FullScreenQuad_FragmentInput output_;
    output_.Position =vec4(input_.Position.x, input_.Position.y, 0, 1);
    output_.TexCoords =input_.TexCoords;
    return output_;
}


layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 0) out vec2 fsin_0;

void main()
{
    Shaders_FullScreenQuad_VertexInput input_;
    input_.Position = Position;
    input_.TexCoords = TexCoords;
    Shaders_FullScreenQuad_FragmentInput output_ = VS(input_);
    fsin_0 = output_.TexCoords;
    gl_Position = output_.Position;
        gl_Position.y = -gl_Position.y; // Correct for Vulkan clip coordinates
}

#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct Shaders_ScreenDuplicator_VertexInput
{
    vec2 Position;
    vec2 TexCoords;
};

struct Shaders_ScreenDuplicator_FragmentInput
{
    vec4 Position;
    vec2 TexCoords;
};

struct Shaders_ScreenDuplicator_FragmentOutput
{
    vec4 ColorOut0;
    vec4 ColorOut1;
};

layout(set = 0, binding = 0) uniform texture2D SourceTexture;
layout(set = 0, binding = 1) uniform sampler SourceSampler;
Shaders_ScreenDuplicator_FragmentOutput FS(Shaders_ScreenDuplicator_FragmentInput input_)
{
    Shaders_ScreenDuplicator_FragmentOutput output_;
    output_.ColorOut0 =clamp(texture(sampler2D(SourceTexture, SourceSampler), input_.TexCoords), 0, 1);
    output_.ColorOut1 =clamp(texture(sampler2D(SourceTexture, SourceSampler), input_.TexCoords) * vec4(1.0f, 0.7f, 0.7f, 1.f), 0, 1);
    return output_;
}


layout(location = 0) in vec2 fsin_0;
    layout(location = 0) out vec4 _outputColor_0;
    layout(location = 1) out vec4 _outputColor_1;

void main()
{
    Shaders_ScreenDuplicator_FragmentInput input_;
    input_.Position = gl_FragCoord;
    input_.TexCoords = fsin_0;
    Shaders_ScreenDuplicator_FragmentOutput output_ = FS(input_);
    _outputColor_0 = output_.ColorOut0;
    _outputColor_1 = output_.ColorOut1;
}

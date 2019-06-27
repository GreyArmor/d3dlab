#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct Shaders_ShadowmapPreviewShader_VertexIn
{
    vec2 Position;
    vec2 TexCoord;
};

struct Shaders_ShadowmapPreviewShader_FragmentIn
{
    vec4 Position;
    vec2 TexCoord;
};

struct Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo
{
    vec2 Position;
    vec2 Size;
};

layout(set = 0, binding = 0) uniform Projection
{
    mat4 field_Projection;
};

layout(set = 0, binding = 1) uniform SizePos
{
    Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo field_SizePos;
};

layout(set = 0, binding = 2) uniform texture2D Tex;
layout(set = 0, binding = 3) uniform sampler TexSampler;
vec4 FS(Shaders_ShadowmapPreviewShader_FragmentIn input_)
{
    return texture(sampler2D(Tex, TexSampler), input_.TexCoord);
}


layout(location = 0) in vec2 fsin_0;
layout(location = 0) out vec4 _outputColor_;

void main()
{
    Shaders_ShadowmapPreviewShader_FragmentIn input_;
    input_.Position = gl_FragCoord;
    input_.TexCoord = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

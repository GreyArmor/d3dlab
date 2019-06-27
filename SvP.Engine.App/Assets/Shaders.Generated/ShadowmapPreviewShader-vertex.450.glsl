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
Shaders_ShadowmapPreviewShader_FragmentIn VS(Shaders_ShadowmapPreviewShader_VertexIn input_)
{
    Shaders_ShadowmapPreviewShader_FragmentIn output_;
    vec2 scaledInput = (input_.Position * field_SizePos.Size) + field_SizePos.Position;
    output_.Position =field_Projection * vec4(scaledInput, 0, 1);
    output_.TexCoord =input_.TexCoord;
    return output_;
}


layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoord;
layout(location = 0) out vec2 fsin_0;

void main()
{
    Shaders_ShadowmapPreviewShader_VertexIn input_;
    input_.Position = Position;
    input_.TexCoord = TexCoord;
    Shaders_ShadowmapPreviewShader_FragmentIn output_ = VS(input_);
    fsin_0 = output_.TexCoord;
    gl_Position = output_.Position;
        gl_Position.y = -gl_Position.y; // Correct for Vulkan clip coordinates
}

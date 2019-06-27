#version 330 core

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

layout(std140) uniform Projection
{
    mat4 field_Projection;
};

layout(std140) uniform SizePos
{
    Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo field_SizePos;
};

uniform sampler2D Tex;

Shaders_ShadowmapPreviewShader_FragmentIn VS(Shaders_ShadowmapPreviewShader_VertexIn input_)
{
    Shaders_ShadowmapPreviewShader_FragmentIn output_;
    vec2 scaledInput = (input_.Position * field_SizePos.Size) + field_SizePos.Position;
    output_.Position =field_Projection * vec4(scaledInput, 0, 1);
    output_.TexCoord =input_.TexCoord;
    return output_;
}


in vec2 Position;
in vec2 TexCoord;
out vec2 fsin_0;

void main()
{
    Shaders_ShadowmapPreviewShader_VertexIn input_;
    input_.Position = Position;
    input_.TexCoord = TexCoord;
    Shaders_ShadowmapPreviewShader_FragmentIn output_ = VS(input_);
    fsin_0 = output_.TexCoord;
    gl_Position = output_.Position;
        gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;
}

#version 330 core

struct Shaders_Simple2D_VertexIn
{
    vec2 Position;
    vec2 TexCoord;
};

struct Shaders_Simple2D_FragmentIn
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

vec4 FS(Shaders_Simple2D_FragmentIn input_)
{
    return texture(Tex, input_.TexCoord);
}


in vec2 fsin_0;
out vec4 _outputColor_;

void main()
{
    Shaders_Simple2D_FragmentIn input_;
    input_.Position = gl_FragCoord;
    input_.TexCoord = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

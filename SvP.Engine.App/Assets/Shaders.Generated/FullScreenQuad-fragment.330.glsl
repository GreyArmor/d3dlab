#version 330 core

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

uniform sampler2D SourceTexture;

vec4 FS(Shaders_FullScreenQuad_FragmentInput input_)
{
    return texture(SourceTexture, input_.TexCoords);
}


in vec2 fsin_0;
out vec4 _outputColor_;

void main()
{
    Shaders_FullScreenQuad_FragmentInput input_;
    input_.Position = gl_FragCoord;
    input_.TexCoords = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

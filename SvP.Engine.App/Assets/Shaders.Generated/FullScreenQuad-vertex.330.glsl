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

Shaders_FullScreenQuad_FragmentInput VS(Shaders_FullScreenQuad_VertexInput input_)
{
    Shaders_FullScreenQuad_FragmentInput output_;
    output_.Position =vec4(input_.Position.x, input_.Position.y, 0, 1);
    output_.TexCoords =input_.TexCoords;
    return output_;
}


in vec2 Position;
in vec2 TexCoords;
out vec2 fsin_0;

void main()
{
    Shaders_FullScreenQuad_VertexInput input_;
    input_.Position = Position;
    input_.TexCoords = TexCoords;
    Shaders_FullScreenQuad_FragmentInput output_ = VS(input_);
    fsin_0 = output_.TexCoords;
    gl_Position = output_.Position;
        gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;
}

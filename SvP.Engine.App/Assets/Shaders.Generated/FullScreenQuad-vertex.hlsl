struct Shaders_FullScreenQuad_VertexInput
{
    float2 Position : POSITION0;
    float2 TexCoords : TEXCOORD0;
};

struct Shaders_FullScreenQuad_FragmentInput
{
    float4 Position : SV_Position;
    float2 TexCoords : TEXCOORD0;
};

Shaders_FullScreenQuad_FragmentInput VS(Shaders_FullScreenQuad_VertexInput input)
{
    Shaders_FullScreenQuad_FragmentInput output;
    output.Position =float4(input.Position.x, input.Position.y, 0, 1);
    output.TexCoords =input.TexCoords;
    return output;
}



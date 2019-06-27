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

Texture2D SourceTexture : register(t0);

SamplerState SourceSampler : register(s0);

float4 FS(Shaders_FullScreenQuad_FragmentInput input) : SV_Target
{
    return SourceTexture.Sample(SourceSampler, input.TexCoords);
}



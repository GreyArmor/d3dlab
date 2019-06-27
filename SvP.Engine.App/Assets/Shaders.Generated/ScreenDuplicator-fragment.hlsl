struct Shaders_ScreenDuplicator_VertexInput
{
    float2 Position : POSITION0;
    float2 TexCoords : TEXCOORD0;
};

struct Shaders_ScreenDuplicator_FragmentInput
{
    float4 Position : SV_Position;
    float2 TexCoords : TEXCOORD0;
};

struct Shaders_ScreenDuplicator_FragmentOutput
{
    float4 ColorOut0 : SV_Target0;
    float4 ColorOut1 : SV_Target1;
};

Texture2D SourceTexture : register(t0);

SamplerState SourceSampler : register(s0);

Shaders_ScreenDuplicator_FragmentOutput FS(Shaders_ScreenDuplicator_FragmentInput input)
{
    Shaders_ScreenDuplicator_FragmentOutput output;
    output.ColorOut0 =saturate(SourceTexture.Sample(SourceSampler, input.TexCoords));
    output.ColorOut1 =saturate(SourceTexture.Sample(SourceSampler, input.TexCoords) * float4(1.0f, 0.7f, 0.7f, 1.f));
    return output;
}



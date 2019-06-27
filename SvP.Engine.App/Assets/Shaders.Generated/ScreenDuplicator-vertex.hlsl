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

Shaders_ScreenDuplicator_FragmentInput VS(Shaders_ScreenDuplicator_VertexInput input)
{
    Shaders_ScreenDuplicator_FragmentInput output;
    output.Position =float4(input.Position.x, input.Position.y, 0, 1);
    output.TexCoords =input.TexCoords;
    return output;
}



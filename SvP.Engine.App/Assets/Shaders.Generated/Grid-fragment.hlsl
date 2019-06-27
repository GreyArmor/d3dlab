struct Shaders_Grid_VSInput
{
    float3 Position : POSITION0;
};

struct Shaders_Grid_FSInput
{
    float4 FragPosition : SV_Position;
    float3 WorldPosition : POSITION0;
};

Texture2D GridTexture : register(t0);

SamplerState GridSampler : register(s0);

float4 FS(Shaders_Grid_FSInput input) : SV_Target
{
    return GridTexture.Sample(GridSampler, float2(input.WorldPosition.x, input.WorldPosition.z) / 10.0f);
}



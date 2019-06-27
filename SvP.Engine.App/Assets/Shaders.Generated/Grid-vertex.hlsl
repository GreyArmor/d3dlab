struct Shaders_Grid_VSInput
{
    float3 Position : POSITION0;
};

struct Shaders_Grid_FSInput
{
    float4 FragPosition : SV_Position;
    float3 WorldPosition : POSITION0;
};

cbuffer ProjectionBuffer : register(b0)
{
    float4x4 Projection;
}

cbuffer ViewBuffer : register(b1)
{
    float4x4 View;
}

Shaders_Grid_FSInput VS(Shaders_Grid_VSInput input)
{
    Shaders_Grid_FSInput output;
    output.FragPosition =mul(Projection, mul(View, float4(input.Position, 1)));
    output.WorldPosition =input.Position;
    return output;
}



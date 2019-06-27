struct Shaders_Skybox_VSInput
{
    float3 Position : POSITION0;
};

struct Shaders_Skybox_FSInput
{
    float4 Position : SV_Position;
    float3 TexCoord : TEXCOORD0;
};

cbuffer ProjectionBuffer : register(b0)
{
    float4x4 Projection;
}

cbuffer ViewBuffer : register(b1)
{
    float4x4 View;
}

Shaders_Skybox_FSInput VS(Shaders_Skybox_VSInput input)
{
    float4x4 view3x3 = { View[0][0], View[0][1], View[0][2], 0, View[1][0], View[1][1], View[1][2], 0, View[2][0], View[2][1], View[2][2], 0, 0, 0, 0, 1 };
    Shaders_Skybox_FSInput output;
    float4 pos = mul(Projection, mul(view3x3, float4(input.Position, 1.0f)));
    output.Position =float4(pos.x, pos.y, pos.w, pos.w);
    output.TexCoord =input.Position;
    return output;
}



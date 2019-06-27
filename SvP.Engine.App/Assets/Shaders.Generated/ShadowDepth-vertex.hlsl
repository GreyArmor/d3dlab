struct Shaders_ShadowDepth_VertexInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct Shaders_ShadowDepth_FragmentInput
{
    float4 Position : SV_Position;
};

cbuffer ViewProjectionBuffer : register(b0)
{
    float4x4 ViewProjection;
}

cbuffer WorldBuffer : register(b1)
{
    float4x4 World;
}

Shaders_ShadowDepth_FragmentInput VS(Shaders_ShadowDepth_VertexInput input)
{
    Shaders_ShadowDepth_FragmentInput output;
    output.Position =mul(ViewProjection, mul(World, float4(input.Position, 1)));
    output.Position.y +=input.TexCoord.y * .0001f;
    return output;
}



struct TexturedCube_Shaders_Cube_VertexInput
{
    float3 Position : POSITION0;
    float2 TexCoords : TEXCOORD0;
};

struct TexturedCube_Shaders_Cube_FragmentInput
{
    float4 SystemPosition : SV_Position;
    float2 TexCoords : TEXCOORD0;
};

cbuffer ProjectionBuffer : register(b0)
{
    float4x4 Projection;
}

cbuffer ViewBuffer : register(b1)
{
    float4x4 View;
}

cbuffer WorldBuffer : register(b2)
{
    float4x4 World;
}

TexturedCube_Shaders_Cube_FragmentInput VS(TexturedCube_Shaders_Cube_VertexInput input)
{
    TexturedCube_Shaders_Cube_FragmentInput output;
    float4 worldPosition = mul(World, float4(input.Position, 1));
    float4 viewPosition = mul(View, worldPosition);
    float4 clipPosition = mul(Projection, viewPosition);
    output.SystemPosition = clipPosition;
    output.TexCoords = input.TexCoords;
    return output;
}
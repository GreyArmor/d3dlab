struct Shaders_ShadowmapPreviewShader_VertexIn
{
    float2 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct Shaders_ShadowmapPreviewShader_FragmentIn
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};

struct Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo
{
    float2 Position;
    float2 Size;
};

cbuffer ProjectionBuffer : register(b0)
{
    float4x4 Projection;
}

cbuffer SizePosBuffer : register(b1)
{
    Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo SizePos;
}

Shaders_ShadowmapPreviewShader_FragmentIn VS(Shaders_ShadowmapPreviewShader_VertexIn input)
{
    Shaders_ShadowmapPreviewShader_FragmentIn output;
    float2 scaledInput = (input.Position * SizePos.Size) + SizePos.Position;
    output.Position =mul(Projection, float4(scaledInput, 0, 1));
    output.TexCoord =input.TexCoord;
    return output;
}



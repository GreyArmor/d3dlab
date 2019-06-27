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

Texture2D Tex : register(t0);

SamplerState TexSampler : register(s0);

float4 FS(Shaders_ShadowmapPreviewShader_FragmentIn input) : SV_Target
{
    return Tex.Sample(TexSampler, input.TexCoord);
}



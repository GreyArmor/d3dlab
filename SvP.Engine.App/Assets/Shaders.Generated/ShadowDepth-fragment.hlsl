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

void FS(Shaders_ShadowDepth_FragmentInput input)
{
}



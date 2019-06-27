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

Texture2D SurfaceTexture : register(t0);

SamplerState SurfaceSampler : register(s0);

float4 FS(TexturedCube_Shaders_Cube_FragmentInput input) : SV_Target
{
    return SurfaceTexture.Sample(SurfaceSampler, input.TexCoords);
}



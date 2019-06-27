struct Shaders_Skybox_VSInput
{
    float3 Position : POSITION0;
};

struct Shaders_Skybox_FSInput
{
    float4 Position : SV_Position;
    float3 TexCoord : TEXCOORD0;
};

TextureCube CubeTexture : register(t0);

SamplerState CubeSampler : register(s0);

float4 FS(Shaders_Skybox_FSInput input) : SV_Target
{
    return CubeTexture.Sample(CubeSampler, input.TexCoord);
}



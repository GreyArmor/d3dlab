#include <metal_stdlib>
using namespace metal;
struct Shaders_Skybox_VSInput
{
    float3 Position [[ attribute(0) ]];
};

struct Shaders_Skybox_FSInput
{
    float4 Position [[ position ]];
    float3 TexCoord [[ attribute(0) ]];
};

struct ShaderContainer {
thread texturecube<float> CubeTexture;
thread sampler CubeSampler;

ShaderContainer(
thread texturecube<float> CubeTexture_param, thread sampler CubeSampler_param
)
:
CubeTexture(CubeTexture_param), CubeSampler(CubeSampler_param)
{}
float4 FS(Shaders_Skybox_FSInput input)
{
    return CubeTexture.sample(CubeSampler, input.TexCoord);
}


};

fragment float4 FS(Shaders_Skybox_FSInput input [[ stage_in ]], texturecube<float> CubeTexture [[ texture(0) ]], sampler CubeSampler [[ sampler(0) ]])
{
return ShaderContainer(CubeTexture, CubeSampler).FS(input);
}

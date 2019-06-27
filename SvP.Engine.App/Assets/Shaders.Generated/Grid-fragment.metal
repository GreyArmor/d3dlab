#include <metal_stdlib>
using namespace metal;
struct Shaders_Grid_VSInput
{
    float3 Position [[ attribute(0) ]];
};

struct Shaders_Grid_FSInput
{
    float4 FragPosition [[ position ]];
    float3 WorldPosition [[ attribute(0) ]];
};

struct ShaderContainer {
thread texture2d<float> GridTexture;
thread sampler GridSampler;

ShaderContainer(
thread texture2d<float> GridTexture_param, thread sampler GridSampler_param
)
:
GridTexture(GridTexture_param), GridSampler(GridSampler_param)
{}
float4 FS(Shaders_Grid_FSInput input)
{
    return GridTexture.sample(GridSampler, float2(input.WorldPosition[0], input.WorldPosition[2]) / 10.0f);
}


};

fragment float4 FS(Shaders_Grid_FSInput input [[ stage_in ]], texture2d<float> GridTexture [[ texture(0) ]], sampler GridSampler [[ sampler(0) ]])
{
return ShaderContainer(GridTexture, GridSampler).FS(input);
}

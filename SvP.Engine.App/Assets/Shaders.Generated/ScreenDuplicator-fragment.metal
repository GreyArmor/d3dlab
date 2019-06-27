#include <metal_stdlib>
using namespace metal;
struct Shaders_ScreenDuplicator_VertexInput
{
    float2 Position [[ attribute(0) ]];
    float2 TexCoords [[ attribute(1) ]];
};

struct Shaders_ScreenDuplicator_FragmentInput
{
    float4 Position [[ position ]];
    float2 TexCoords [[ attribute(0) ]];
};

struct Shaders_ScreenDuplicator_FragmentOutput
{
    float4 ColorOut0 [[ color(0) ]];
    float4 ColorOut1 [[ color(1) ]];
};

struct ShaderContainer {
thread texture2d<float> SourceTexture;
thread sampler SourceSampler;

ShaderContainer(
thread texture2d<float> SourceTexture_param, thread sampler SourceSampler_param
)
:
SourceTexture(SourceTexture_param), SourceSampler(SourceSampler_param)
{}
Shaders_ScreenDuplicator_FragmentOutput FS(Shaders_ScreenDuplicator_FragmentInput input)
{
    Shaders_ScreenDuplicator_FragmentOutput output;
    output.ColorOut0 =saturate(SourceTexture.sample(SourceSampler, input.TexCoords));
    output.ColorOut1 =saturate(SourceTexture.sample(SourceSampler, input.TexCoords) * float4(1.0f, 0.7f, 0.7f, 1.f));
    return output;
}


};

fragment Shaders_ScreenDuplicator_FragmentOutput FS(Shaders_ScreenDuplicator_FragmentInput input [[ stage_in ]], texture2d<float> SourceTexture [[ texture(0) ]], sampler SourceSampler [[ sampler(0) ]])
{
return ShaderContainer(SourceTexture, SourceSampler).FS(input);
}

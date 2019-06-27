#include <metal_stdlib>
using namespace metal;
struct Shaders_FullScreenQuad_VertexInput
{
    float2 Position [[ attribute(0) ]];
    float2 TexCoords [[ attribute(1) ]];
};

struct Shaders_FullScreenQuad_FragmentInput
{
    float4 Position [[ position ]];
    float2 TexCoords [[ attribute(0) ]];
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
float4 FS(Shaders_FullScreenQuad_FragmentInput input)
{
    return SourceTexture.sample(SourceSampler, input.TexCoords);
}


};

fragment float4 FS(Shaders_FullScreenQuad_FragmentInput input [[ stage_in ]], texture2d<float> SourceTexture [[ texture(0) ]], sampler SourceSampler [[ sampler(0) ]])
{
return ShaderContainer(SourceTexture, SourceSampler).FS(input);
}

#include <metal_stdlib>
using namespace metal;
struct Shaders_Simple2D_VertexIn
{
    float2 Position [[ attribute(0) ]];
    float2 TexCoord [[ attribute(1) ]];
};

struct Shaders_Simple2D_FragmentIn
{
    float4 Position [[ position ]];
    float2 TexCoord [[ attribute(0) ]];
};

struct Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo
{
    packed_float2 Position;
    packed_float2 Size;
};

struct ShaderContainer {
thread texture2d<float> Tex;
thread sampler TexSampler;

ShaderContainer(
thread texture2d<float> Tex_param, thread sampler TexSampler_param
)
:
Tex(Tex_param), TexSampler(TexSampler_param)
{}
float4 FS(Shaders_Simple2D_FragmentIn input)
{
    return Tex.sample(TexSampler, input.TexCoord);
}


};

fragment float4 FS(Shaders_Simple2D_FragmentIn input [[ stage_in ]], texture2d<float> Tex [[ texture(0) ]], sampler TexSampler [[ sampler(0) ]])
{
return ShaderContainer(Tex, TexSampler).FS(input);
}

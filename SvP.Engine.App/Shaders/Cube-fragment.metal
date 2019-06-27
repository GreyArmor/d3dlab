#include <metal_stdlib>
using namespace metal;
struct TexturedCube_Shaders_Cube_VertexInput
{
    float3 Position [[ attribute(0) ]];
    float2 TexCoords [[ attribute(1) ]];
};

struct TexturedCube_Shaders_Cube_FragmentInput
{
    float4 SystemPosition [[ position ]];
    float2 TexCoords [[ attribute(0) ]];
};

struct ShaderContainer {
thread texture2d<float> SurfaceTexture;
thread sampler SurfaceSampler;

ShaderContainer(
thread texture2d<float> SurfaceTexture_param, thread sampler SurfaceSampler_param
)
:
SurfaceTexture(SurfaceTexture_param), SurfaceSampler(SurfaceSampler_param)
{}
float4 FS(TexturedCube_Shaders_Cube_FragmentInput input)
{
    return SurfaceTexture.sample(SurfaceSampler, input.TexCoords);
}


};

fragment float4 FS(TexturedCube_Shaders_Cube_FragmentInput input [[ stage_in ]], texture2d<float> SurfaceTexture [[ texture(0) ]], sampler SurfaceSampler [[ sampler(0) ]])
{
return ShaderContainer(SurfaceTexture, SurfaceSampler).FS(input);
}

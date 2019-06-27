#include <metal_stdlib>
using namespace metal;
struct Shaders_ShadowDepth_VertexInput
{
    float3 Position [[ attribute(0) ]];
    float3 Normal [[ attribute(1) ]];
    float2 TexCoord [[ attribute(2) ]];
};

struct Shaders_ShadowDepth_FragmentInput
{
    float4 Position [[ position ]];
};

struct ShaderContainer {

ShaderContainer(

)
{}
void FS(Shaders_ShadowDepth_FragmentInput input)
{
}


};

fragment void FS(Shaders_ShadowDepth_FragmentInput input [[ stage_in ]])
{
return ShaderContainer().FS(input);
}

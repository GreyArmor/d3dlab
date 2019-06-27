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
constant float4x4& ViewProjection;
constant float4x4& World;

ShaderContainer(
constant float4x4& ViewProjection_param, constant float4x4& World_param
)
:
ViewProjection(ViewProjection_param), World(World_param)
{}
Shaders_ShadowDepth_FragmentInput VS(Shaders_ShadowDepth_VertexInput input)
{
    Shaders_ShadowDepth_FragmentInput output;
    output.Position =ViewProjection * World * float4(input.Position, 1);
    output.Position[1] +=input.TexCoord[1] * .0001f;
    return output;
}


};

vertex Shaders_ShadowDepth_FragmentInput VS(Shaders_ShadowDepth_VertexInput input [[ stage_in ]], constant float4x4 &ViewProjection [[ buffer(1) ]], constant float4x4 &World [[ buffer(2) ]])
{
return ShaderContainer(ViewProjection, World).VS(input);
}

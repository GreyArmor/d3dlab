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
constant Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo& SizePos;
constant float4x4& Projection;

ShaderContainer(
constant Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo& SizePos_param, constant float4x4& Projection_param
)
:
SizePos(SizePos_param), Projection(Projection_param)
{}
Shaders_Simple2D_FragmentIn VS(Shaders_Simple2D_VertexIn input)
{
    Shaders_Simple2D_FragmentIn output;
    float2 scaledInput = (input.Position * SizePos.Size) + SizePos.Position;
    output.Position =Projection * float4(scaledInput, 0, 1);
    output.TexCoord =input.TexCoord;
    return output;
}


};

vertex Shaders_Simple2D_FragmentIn VS(Shaders_Simple2D_VertexIn input [[ stage_in ]], constant float4x4 &Projection [[ buffer(1) ]], constant Veldrid_NeoDemo_Objects_ShadowmapDrawIndexeder_SizeInfo &SizePos [[ buffer(2) ]])
{
return ShaderContainer(SizePos, Projection).VS(input);
}

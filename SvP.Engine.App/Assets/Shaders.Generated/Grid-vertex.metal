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
constant float4x4& Projection;
constant float4x4& View;

ShaderContainer(
constant float4x4& Projection_param, constant float4x4& View_param
)
:
Projection(Projection_param), View(View_param)
{}
Shaders_Grid_FSInput VS(Shaders_Grid_VSInput input)
{
    Shaders_Grid_FSInput output;
    output.FragPosition =Projection * View * float4(input.Position, 1);
    output.WorldPosition =input.Position;
    return output;
}


};

vertex Shaders_Grid_FSInput VS(Shaders_Grid_VSInput input [[ stage_in ]], constant float4x4 &Projection [[ buffer(1) ]], constant float4x4 &View [[ buffer(2) ]])
{
return ShaderContainer(Projection, View).VS(input);
}

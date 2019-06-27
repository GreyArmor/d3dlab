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
constant float4x4& View;
constant float4x4& Projection;

ShaderContainer(
constant float4x4& View_param, constant float4x4& Projection_param
)
:
View(View_param), Projection(Projection_param)
{}
Shaders_Skybox_FSInput VS(Shaders_Skybox_VSInput input)
{
    float4x4 view3x3 = { float4(View[0][0], View[0][1], View[0][2], 0), float4(View[1][0], View[1][1], View[1][2], 0), float4(View[2][0], View[2][1], View[2][2], 0), float4(0, 0, 0, 1) };
    Shaders_Skybox_FSInput output;
    float4 pos = Projection * view3x3 * float4(input.Position, 1.0f);
    output.Position =float4(pos[0], pos[1], pos[3], pos[3]);
    output.TexCoord =input.Position;
    return output;
}


};

vertex Shaders_Skybox_FSInput VS(Shaders_Skybox_VSInput input [[ stage_in ]], constant float4x4 &Projection [[ buffer(1) ]], constant float4x4 &View [[ buffer(2) ]])
{
return ShaderContainer(View, Projection).VS(input);
}

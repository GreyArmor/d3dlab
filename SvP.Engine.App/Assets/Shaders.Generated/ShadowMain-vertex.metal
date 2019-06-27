#include <metal_stdlib>
using namespace metal;
struct Shaders_ShadowMain_VertexInput
{
    float3 Position [[ attribute(0) ]];
    float3 Normal [[ attribute(1) ]];
    float2 TexCoord [[ attribute(2) ]];
};

struct Shaders_ShadowMain_PixelInput
{
    float4 Position [[ position ]];
    float3 Position_WorldSpace [[ attribute(0) ]];
    float4 LightPosition1 [[ attribute(1) ]];
    float4 LightPosition2 [[ attribute(2) ]];
    float4 LightPosition3 [[ attribute(3) ]];
    float3 Normal [[ attribute(4) ]];
    float2 TexCoord [[ attribute(5) ]];
    float FragDepth [[ attribute(6) ]];
    float4 ReflectionPosition [[ attribute(7) ]];
};

struct Veldrid_NeoDemo_DepthCascadeLimits
{
    float NearLimit;
    float MidLimit;
    float FarLimit;
    float _padding;
};

struct Veldrid_NeoDemo_DirectionalLightInfo
{
    packed_float3 Direction;
    float _padding;
    packed_float4 Color;
};

struct Veldrid_NeoDemo_CameraInfo
{
    packed_float3 CameraPosition_WorldSpace;
    float _padding1;
    packed_float3 CameraLookDirection;
    float _padding2;
};

struct Veldrid_NeoDemo_PointLightInfo
{
    packed_float3 Position;
    float _padding0;
    packed_float3 Color;
    float _padding1;
    float Range;
    float _padding2;
    float _padding3;
    float _padding4;
};

struct Veldrid_NeoDemo_PointLightsInfo
{
    Veldrid_NeoDemo_PointLightInfo PointLights[4];
    int NumActiveLights;
    float _padding0;
    float _padding1;
    float _padding2;
};

struct Veldrid_NeoDemo_MaterialProperties
{
    packed_float3 SpecularIntensity;
    float SpecularPower;
    packed_float3 _padding0;
    float Reflectivity;
};

struct Veldrid_NeoDemo_ClipPlaneInfo
{
    packed_float4 ClipPlane;
    int Enabled;
};

struct ShaderContainer {
constant float4x4& World;
constant float4x4& View;
constant float4x4& Projection;
constant float4x4& InverseTransposeWorld;
constant float4x4& LightViewProjection1;
constant float4x4& LightViewProjection2;
constant float4x4& LightViewProjection3;
constant float4x4& ReflectionViewProj;

ShaderContainer(
constant float4x4& World_param, constant float4x4& View_param, constant float4x4& Projection_param, constant float4x4& InverseTransposeWorld_param, constant float4x4& LightViewProjection1_param, constant float4x4& LightViewProjection2_param, constant float4x4& LightViewProjection3_param, constant float4x4& ReflectionViewProj_param
)
:
World(World_param), View(View_param), Projection(Projection_param), InverseTransposeWorld(InverseTransposeWorld_param), LightViewProjection1(LightViewProjection1_param), LightViewProjection2(LightViewProjection2_param), LightViewProjection3(LightViewProjection3_param), ReflectionViewProj(ReflectionViewProj_param)
{}
Shaders_ShadowMain_PixelInput VS(Shaders_ShadowMain_VertexInput input)
{
    Shaders_ShadowMain_PixelInput output;
    float4 worldPosition = World * float4(input.Position, 1);
    float4 viewPosition = View * worldPosition;
    output.Position =Projection * viewPosition;
    output.Position_WorldSpace =float4(worldPosition).xyz;
    float4 outNormal = InverseTransposeWorld * float4(input.Normal, 1);
    output.Normal =normalize(float4(outNormal).xyz);
    output.TexCoord =input.TexCoord;
    output.LightPosition1 =World * float4(input.Position, 1);
    output.LightPosition1 =LightViewProjection1 * output.LightPosition1;
    output.LightPosition2 =World * float4(input.Position, 1);
    output.LightPosition2 =LightViewProjection2 * output.LightPosition2;
    output.LightPosition3 =World * float4(input.Position, 1);
    output.LightPosition3 =LightViewProjection3 * output.LightPosition3;
    output.FragDepth =output.Position[2];
    output.ReflectionPosition =World * float4(input.Position, 1);
    output.ReflectionPosition =ReflectionViewProj * output.ReflectionPosition;
    return output;
}


};

vertex Shaders_ShadowMain_PixelInput VS(Shaders_ShadowMain_VertexInput input [[ stage_in ]], constant float4x4 &Projection [[ buffer(1) ]], constant float4x4 &View [[ buffer(2) ]], constant float4x4 &LightViewProjection1 [[ buffer(3) ]], constant float4x4 &LightViewProjection2 [[ buffer(4) ]], constant float4x4 &LightViewProjection3 [[ buffer(5) ]], constant float4x4 &World [[ buffer(10) ]], constant float4x4 &InverseTransposeWorld [[ buffer(11) ]], constant float4x4 &ReflectionViewProj [[ buffer(13) ]])
{
return ShaderContainer(World, View, Projection, InverseTransposeWorld, LightViewProjection1, LightViewProjection2, LightViewProjection3, ReflectionViewProj).VS(input);
}

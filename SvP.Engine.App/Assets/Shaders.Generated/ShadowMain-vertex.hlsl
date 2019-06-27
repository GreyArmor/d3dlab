struct Shaders_ShadowMain_VertexInput
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct Shaders_ShadowMain_PixelInput
{
    float4 Position : SV_Position;
    float3 Position_WorldSpace : POSITION0;
    float4 LightPosition1 : TEXCOORD0;
    float4 LightPosition2 : TEXCOORD1;
    float4 LightPosition3 : TEXCOORD2;
    float3 Normal : NORMAL0;
    float2 TexCoord : TEXCOORD3;
    float FragDepth : POSITION1;
    float4 ReflectionPosition : TEXCOORD4;
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
    float3 Direction;
    float _padding;
    float4 Color;
};

struct Veldrid_NeoDemo_CameraInfo
{
    float3 CameraPosition_WorldSpace;
    float _padding1;
    float3 CameraLookDirection;
    float _padding2;
};

struct Veldrid_NeoDemo_PointLightInfo
{
    float3 Position;
    float _padding0;
    float3 Color;
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
    float3 SpecularIntensity;
    float SpecularPower;
    float3 _padding0;
    float Reflectivity;
};

struct Veldrid_NeoDemo_ClipPlaneInfo
{
    float4 ClipPlane;
    int Enabled;
};

cbuffer ProjectionBuffer : register(b0)
{
    float4x4 Projection;
}

cbuffer ViewBuffer : register(b1)
{
    float4x4 View;
}

cbuffer LightViewProjection1Buffer : register(b2)
{
    float4x4 LightViewProjection1;
}

cbuffer LightViewProjection2Buffer : register(b3)
{
    float4x4 LightViewProjection2;
}

cbuffer LightViewProjection3Buffer : register(b4)
{
    float4x4 LightViewProjection3;
}

cbuffer WorldBuffer : register(b9)
{
    float4x4 World;
}

cbuffer InverseTransposeWorldBuffer : register(b10)
{
    float4x4 InverseTransposeWorld;
}

cbuffer ReflectionViewProjBuffer : register(b12)
{
    float4x4 ReflectionViewProj;
}

Shaders_ShadowMain_PixelInput VS(Shaders_ShadowMain_VertexInput input)
{
    Shaders_ShadowMain_PixelInput output;
    float4 worldPosition = mul(World, float4(input.Position, 1));
    float4 viewPosition = mul(View, worldPosition);
    output.Position =mul(Projection, viewPosition);
    output.Position_WorldSpace =worldPosition.xyz;
    float4 outNormal = mul(InverseTransposeWorld, float4(input.Normal, 1));
    output.Normal =normalize(outNormal.xyz);
    output.TexCoord =input.TexCoord;
    output.LightPosition1 =mul(World, float4(input.Position, 1));
    output.LightPosition1 =mul(LightViewProjection1, output.LightPosition1);
    output.LightPosition2 =mul(World, float4(input.Position, 1));
    output.LightPosition2 =mul(LightViewProjection2, output.LightPosition2);
    output.LightPosition3 =mul(World, float4(input.Position, 1));
    output.LightPosition3 =mul(LightViewProjection3, output.LightPosition3);
    output.FragDepth =output.Position.z;
    output.ReflectionPosition =mul(World, float4(input.Position, 1));
    output.ReflectionPosition =mul(ReflectionViewProj, output.ReflectionPosition);
    return output;
}



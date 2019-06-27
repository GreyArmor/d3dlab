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
thread texture2d<float> ShadowMapNear;
thread sampler ShadowMapSampler;
thread texture2d<float> ShadowMapMid;
thread texture2d<float> ShadowMapFar;
constant Veldrid_NeoDemo_ClipPlaneInfo& ClipPlaneInfo;
thread texture2d<float> AlphaMap;
thread sampler AlphaMapSampler;
thread texture2d<float> SurfaceTexture;
thread sampler RegularSampler;
constant Veldrid_NeoDemo_MaterialProperties& MaterialProperties;
thread texture2d<float> ReflectionMap;
thread sampler ReflectionSampler;
constant Veldrid_NeoDemo_DirectionalLightInfo& LightInfo;
constant Veldrid_NeoDemo_CameraInfo& CameraInfo;
constant Veldrid_NeoDemo_DepthCascadeLimits& DepthLimits;
constant Veldrid_NeoDemo_PointLightsInfo& PointLights;
bool Shaders_ShadowMain_InRange(float val, float min, float max)
{
    return val >= min && val <= max;
}


float Shaders_ShadowMain_SampleDepthMap(int index, float2 coord)
{
    if (index == 0)
{
    return ShadowMapNear.sample(ShadowMapSampler, coord)[0];
}

else
if (index == 1)
{
    return ShadowMapMid.sample(ShadowMapSampler, coord)[0];
}

else
{
    return ShadowMapFar.sample(ShadowMapSampler, coord)[0];
}





}


float4 Shaders_ShadowMain_WithAlpha(float4 baseColor, float alpha)
{
    return float4(float4(baseColor).xyz, alpha);
}



ShaderContainer(
thread texture2d<float> ShadowMapNear_param, thread sampler ShadowMapSampler_param, thread texture2d<float> ShadowMapMid_param, thread texture2d<float> ShadowMapFar_param, constant Veldrid_NeoDemo_ClipPlaneInfo& ClipPlaneInfo_param, thread texture2d<float> AlphaMap_param, thread sampler AlphaMapSampler_param, thread texture2d<float> SurfaceTexture_param, thread sampler RegularSampler_param, constant Veldrid_NeoDemo_MaterialProperties& MaterialProperties_param, thread texture2d<float> ReflectionMap_param, thread sampler ReflectionSampler_param, constant Veldrid_NeoDemo_DirectionalLightInfo& LightInfo_param, constant Veldrid_NeoDemo_CameraInfo& CameraInfo_param, constant Veldrid_NeoDemo_DepthCascadeLimits& DepthLimits_param, constant Veldrid_NeoDemo_PointLightsInfo& PointLights_param
)
:
ShadowMapNear(ShadowMapNear_param), ShadowMapSampler(ShadowMapSampler_param), ShadowMapMid(ShadowMapMid_param), ShadowMapFar(ShadowMapFar_param), ClipPlaneInfo(ClipPlaneInfo_param), AlphaMap(AlphaMap_param), AlphaMapSampler(AlphaMapSampler_param), SurfaceTexture(SurfaceTexture_param), RegularSampler(RegularSampler_param), MaterialProperties(MaterialProperties_param), ReflectionMap(ReflectionMap_param), ReflectionSampler(ReflectionSampler_param), LightInfo(LightInfo_param), CameraInfo(CameraInfo_param), DepthLimits(DepthLimits_param), PointLights(PointLights_param)
{}
float4 FS(Shaders_ShadowMain_PixelInput input)
{
    if (ClipPlaneInfo.Enabled == 1)
{
    if (dot(ClipPlaneInfo.ClipPlane, float4(input.Position_WorldSpace, 1)) < 0)
{
    discard_fragment();
}



}



    float alphaMapSample = AlphaMap.sample(AlphaMapSampler, input.TexCoord)[0];
    if (alphaMapSample == 0)
{
    discard_fragment();
}



    float4 surfaceColor = SurfaceTexture.sample(RegularSampler, input.TexCoord);
    if (MaterialProperties.Reflectivity > 0)
{
    float2 reflectionTexCoords = float2((input.ReflectionPosition.x / input.ReflectionPosition.w) / 2 + 0.5, (input.ReflectionPosition.y / input.ReflectionPosition.w) / -2 + 0.5);
    float4 reflectionSample = ReflectionMap.sample(ReflectionSampler, reflectionTexCoords);
    surfaceColor =(surfaceColor * (1 - MaterialProperties.Reflectivity)) + (reflectionSample * MaterialProperties.Reflectivity);
}



    float4 ambientLight = float4(0.3f, 0.3f, 0.3f, 1.f);
    float3 lightDir = -LightInfo.Direction;
    float4 directionalColor = ambientLight * surfaceColor;
    float shadowBias = 0.0005f;
    float lightIntensity = 0.f;
    float4 directionalSpecColor = float4(0, 0, 0, 0);
    float3 vertexToEye = normalize(input.Position_WorldSpace - CameraInfo.CameraPosition_WorldSpace);
    float3 lightReflect = normalize(reflect(LightInfo.Direction, input.Normal));
    float specularFactor = dot(vertexToEye, lightReflect);
    if (specularFactor > 0)
{
    specularFactor =pow(abs(specularFactor), (float)MaterialProperties.SpecularPower);
    directionalSpecColor =float4(float4(LightInfo.Color).xyz * MaterialProperties.SpecularIntensity * specularFactor, 1.0f);
}



    float depthTest = input.FragDepth;
    float2 shadowCoords_0 = float2((input.LightPosition1.x / input.LightPosition1.w) / 2 + 0.5, (input.LightPosition1.y / input.LightPosition1.w) / -2 + 0.5);
    float2 shadowCoords_1 = float2((input.LightPosition2.x / input.LightPosition2.w) / 2 + 0.5, (input.LightPosition2.y / input.LightPosition2.w) / -2 + 0.5);
    float2 shadowCoords_2 = float2((input.LightPosition3.x / input.LightPosition3.w) / 2 + 0.5, (input.LightPosition3.y / input.LightPosition3.w) / -2 + 0.5);
    float lightDepthValues_0 = input.LightPosition1[2] / input.LightPosition1[3];
    float lightDepthValues_1 = input.LightPosition2[2] / input.LightPosition2[3];
    float lightDepthValues_2 = input.LightPosition3[2] / input.LightPosition3[3];
    int shadowIndex = 3;
    float2 shadowCoords = float2(0, 0);
    float lightDepthValue = 0;
    if ((depthTest < DepthLimits.NearLimit) && Shaders_ShadowMain_InRange(shadowCoords_0[0], 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_0[1], 0, 1))
{
    shadowIndex =0;
    shadowCoords =shadowCoords_0;
    lightDepthValue =lightDepthValues_0;
}

else
if ((depthTest < DepthLimits.MidLimit) && Shaders_ShadowMain_InRange(shadowCoords_1[0], 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_1[1], 0, 1))
{
    shadowIndex =1;
    shadowCoords =shadowCoords_1;
    lightDepthValue =lightDepthValues_1;
}

else
if (depthTest < DepthLimits.FarLimit && Shaders_ShadowMain_InRange(shadowCoords_2[0], 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_2[1], 0, 1))
{
    shadowIndex =2;
    shadowCoords =shadowCoords_2;
    lightDepthValue =lightDepthValues_2;
}







    if (shadowIndex != 3)
{
    float shadowMapDepth = Shaders_ShadowMain_SampleDepthMap(shadowIndex, shadowCoords);
    float biasedDistToLight = (lightDepthValue - shadowBias);
    if (biasedDistToLight < shadowMapDepth)
{
    lightIntensity =saturate(dot(input.Normal, lightDir));
    if (lightIntensity > 0.0f)
{
    directionalColor =surfaceColor * (lightIntensity * LightInfo.Color);
}



}

else
{
    directionalColor =ambientLight * surfaceColor;
    directionalSpecColor =float4(0, 0, 0, 0);
}



}

else
{
    lightIntensity =saturate(dot(input.Normal, lightDir));
    if (lightIntensity > 0.0f)
{
    directionalColor =surfaceColor * lightIntensity * LightInfo.Color;
}



}



    float4 pointDiffuse = float4(0, 0, 0, 1);
    float4 pointSpec = float4(0, 0, 0, 1);
    for (int i = 0; i < PointLights.NumActiveLights; i++)
{
    Veldrid_NeoDemo_PointLightInfo pli = PointLights.PointLights[i];
    float3 ptLightDir = normalize(pli.Position - input.Position_WorldSpace);
    float intensity = saturate(dot(input.Normal, ptLightDir));
    float lightDistance = distance(pli.Position, input.Position_WorldSpace);
    intensity =saturate(intensity * (1 - (lightDistance / pli.Range)));
    pointDiffuse +=intensity * float4(pli.Color, 1) * surfaceColor;
    float3 lightReflect0 = normalize(reflect(ptLightDir, input.Normal));
    float specularFactor0 = dot(vertexToEye, lightReflect0);
    if (specularFactor0 > 0 && pli.Range > lightDistance)
{
    specularFactor0 =pow(abs(specularFactor0), (float)MaterialProperties.SpecularPower);
    pointSpec +=(1 - (lightDistance / pli.Range)) * (float4(pli.Color * MaterialProperties.SpecularIntensity * specularFactor0, 1.0f));
}



}


    return Shaders_ShadowMain_WithAlpha(saturate(directionalSpecColor + directionalColor + pointSpec + pointDiffuse), alphaMapSample);
}


};

fragment float4 FS(Shaders_ShadowMain_PixelInput input [[ stage_in ]], constant Veldrid_NeoDemo_DepthCascadeLimits &DepthLimits [[ buffer(5) ]], constant Veldrid_NeoDemo_DirectionalLightInfo &LightInfo [[ buffer(6) ]], constant Veldrid_NeoDemo_CameraInfo &CameraInfo [[ buffer(7) ]], constant Veldrid_NeoDemo_PointLightsInfo &PointLights [[ buffer(8) ]], constant Veldrid_NeoDemo_MaterialProperties &MaterialProperties [[ buffer(11) ]], texture2d<float> SurfaceTexture [[ texture(0) ]], sampler RegularSampler [[ sampler(0) ]], texture2d<float> AlphaMap [[ texture(1) ]], sampler AlphaMapSampler [[ sampler(1) ]], texture2d<float> ShadowMapNear [[ texture(2) ]], texture2d<float> ShadowMapMid [[ texture(3) ]], texture2d<float> ShadowMapFar [[ texture(4) ]], sampler ShadowMapSampler [[ sampler(2) ]], texture2d<float> ReflectionMap [[ texture(5) ]], sampler ReflectionSampler [[ sampler(3) ]], constant Veldrid_NeoDemo_ClipPlaneInfo &ClipPlaneInfo [[ buffer(13) ]])
{
return ShaderContainer(ShadowMapNear, ShadowMapSampler, ShadowMapMid, ShadowMapFar, ClipPlaneInfo, AlphaMap, AlphaMapSampler, SurfaceTexture, RegularSampler, MaterialProperties, ReflectionMap, ReflectionSampler, LightInfo, CameraInfo, DepthLimits, PointLights).FS(input);
}

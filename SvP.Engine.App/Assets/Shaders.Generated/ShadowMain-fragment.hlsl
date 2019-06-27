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

cbuffer DepthLimitsBuffer : register(b5)
{
    Veldrid_NeoDemo_DepthCascadeLimits DepthLimits;
}

cbuffer LightInfoBuffer : register(b6)
{
    Veldrid_NeoDemo_DirectionalLightInfo LightInfo;
}

cbuffer CameraInfoBuffer : register(b7)
{
    Veldrid_NeoDemo_CameraInfo CameraInfo;
}

cbuffer PointLightsBuffer : register(b8)
{
    Veldrid_NeoDemo_PointLightsInfo PointLights;
}

cbuffer MaterialPropertiesBuffer : register(b11)
{
    Veldrid_NeoDemo_MaterialProperties MaterialProperties;
}

Texture2D SurfaceTexture : register(t0);

SamplerState RegularSampler : register(s0);

Texture2D AlphaMap : register(t1);

SamplerState AlphaMapSampler : register(s1);

Texture2D ShadowMapNear : register(t2);

Texture2D ShadowMapMid : register(t3);

Texture2D ShadowMapFar : register(t4);

SamplerState ShadowMapSampler : register(s2);

Texture2D ReflectionMap : register(t5);

SamplerState ReflectionSampler : register(s3);

cbuffer ClipPlaneInfoBuffer : register(b13)
{
    Veldrid_NeoDemo_ClipPlaneInfo ClipPlaneInfo;
}

bool Shaders_ShadowMain_InRange(float val, float min, float max)
{
    return val >= min && val <= max;
}


float Shaders_ShadowMain_SampleDepthMap(int index, float2 coord)
{
    if (index == 0)
{
    return ShadowMapNear.Sample(ShadowMapSampler, coord).x;
}

else
if (index == 1)
{
    return ShadowMapMid.Sample(ShadowMapSampler, coord).x;
}

else
{
    return ShadowMapFar.Sample(ShadowMapSampler, coord).x;
}





}


float4 Shaders_ShadowMain_WithAlpha(float4 baseColor, float alpha)
{
    return float4(baseColor.xyz, alpha);
}


float4 FS(Shaders_ShadowMain_PixelInput input) : SV_Target
{
    if (ClipPlaneInfo.Enabled == 1)
{
    if (dot(ClipPlaneInfo.ClipPlane, float4(input.Position_WorldSpace, 1)) < 0)
{
    discard;
}



}



    float alphaMapSample = AlphaMap.Sample(AlphaMapSampler, input.TexCoord).x;
    if (alphaMapSample == 0)
{
    discard;
}



    float4 surfaceColor = SurfaceTexture.Sample(RegularSampler, input.TexCoord);
    if (MaterialProperties.Reflectivity > 0)
{
    float2 reflectionTexCoords = float2((input.ReflectionPosition.x / input.ReflectionPosition.w) / 2 + 0.5, (input.ReflectionPosition.y / input.ReflectionPosition.w) / -2 + 0.5);
    float4 reflectionSample = ReflectionMap.Sample(ReflectionSampler, reflectionTexCoords);
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
    specularFactor =pow(abs(specularFactor), MaterialProperties.SpecularPower);
    directionalSpecColor =float4(LightInfo.Color.xyz * MaterialProperties.SpecularIntensity * specularFactor, 1.0f);
}



    float depthTest = input.FragDepth;
    float2 shadowCoords_0 = float2((input.LightPosition1.x / input.LightPosition1.w) / 2 + 0.5, (input.LightPosition1.y / input.LightPosition1.w) / -2 + 0.5);
    float2 shadowCoords_1 = float2((input.LightPosition2.x / input.LightPosition2.w) / 2 + 0.5, (input.LightPosition2.y / input.LightPosition2.w) / -2 + 0.5);
    float2 shadowCoords_2 = float2((input.LightPosition3.x / input.LightPosition3.w) / 2 + 0.5, (input.LightPosition3.y / input.LightPosition3.w) / -2 + 0.5);
    float lightDepthValues_0 = input.LightPosition1.z / input.LightPosition1.w;
    float lightDepthValues_1 = input.LightPosition2.z / input.LightPosition2.w;
    float lightDepthValues_2 = input.LightPosition3.z / input.LightPosition3.w;
    int shadowIndex = 3;
    float2 shadowCoords = float2(0, 0);
    float lightDepthValue = 0;
    if ((depthTest < DepthLimits.NearLimit) && Shaders_ShadowMain_InRange(shadowCoords_0.x, 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_0.y, 0, 1))
{
    shadowIndex =0;
    shadowCoords =shadowCoords_0;
    lightDepthValue =lightDepthValues_0;
}

else
if ((depthTest < DepthLimits.MidLimit) && Shaders_ShadowMain_InRange(shadowCoords_1.x, 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_1.y, 0, 1))
{
    shadowIndex =1;
    shadowCoords =shadowCoords_1;
    lightDepthValue =lightDepthValues_1;
}

else
if (depthTest < DepthLimits.FarLimit && Shaders_ShadowMain_InRange(shadowCoords_2.x, 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_2.y, 0, 1))
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
    specularFactor0 =pow(abs(specularFactor0), MaterialProperties.SpecularPower);
    pointSpec +=(1 - (lightDistance / pli.Range)) * (float4(pli.Color * MaterialProperties.SpecularIntensity * specularFactor0, 1.0f));
}



}


    return Shaders_ShadowMain_WithAlpha(saturate(directionalSpecColor + directionalColor + pointSpec + pointDiffuse), alphaMapSample);
}



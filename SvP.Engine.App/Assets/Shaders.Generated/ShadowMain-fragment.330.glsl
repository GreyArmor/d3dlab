#version 330 core

struct Shaders_ShadowMain_VertexInput
{
    vec3 Position;
    vec3 Normal;
    vec2 TexCoord;
};

struct Shaders_ShadowMain_PixelInput
{
    vec4 Position;
    vec3 Position_WorldSpace;
    vec4 LightPosition1;
    vec4 LightPosition2;
    vec4 LightPosition3;
    vec3 Normal;
    vec2 TexCoord;
    float FragDepth;
    vec4 ReflectionPosition;
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
    vec3 Direction;
    float _padding;
    vec4 Color;
};

struct Veldrid_NeoDemo_CameraInfo
{
    vec3 CameraPosition_WorldSpace;
    float _padding1;
    vec3 CameraLookDirection;
    float _padding2;
};

struct Veldrid_NeoDemo_PointLightInfo
{
    vec3 Position;
    float _padding0;
    vec3 Color;
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
    vec3 SpecularIntensity;
    float SpecularPower;
    vec3 _padding0;
    float Reflectivity;
};

struct Veldrid_NeoDemo_ClipPlaneInfo
{
    vec4 ClipPlane;
    int Enabled;
};

layout(std140) uniform Projection
{
    mat4 field_Projection;
};

layout(std140) uniform View
{
    mat4 field_View;
};

layout(std140) uniform LightViewProjection1
{
    mat4 field_LightViewProjection1;
};

layout(std140) uniform LightViewProjection2
{
    mat4 field_LightViewProjection2;
};

layout(std140) uniform LightViewProjection3
{
    mat4 field_LightViewProjection3;
};

layout(std140) uniform DepthLimits
{
    Veldrid_NeoDemo_DepthCascadeLimits field_DepthLimits;
};

layout(std140) uniform LightInfo
{
    Veldrid_NeoDemo_DirectionalLightInfo field_LightInfo;
};

layout(std140) uniform CameraInfo
{
    Veldrid_NeoDemo_CameraInfo field_CameraInfo;
};

layout(std140) uniform PointLights
{
    Veldrid_NeoDemo_PointLightsInfo field_PointLights;
};

layout(std140) uniform World
{
    mat4 field_World;
};

layout(std140) uniform InverseTransposeWorld
{
    mat4 field_InverseTransposeWorld;
};

layout(std140) uniform MaterialProperties
{
    Veldrid_NeoDemo_MaterialProperties field_MaterialProperties;
};

uniform sampler2D SurfaceTexture;

uniform sampler2D AlphaMap;

uniform sampler2D ShadowMapNear;

uniform sampler2D ShadowMapMid;

uniform sampler2D ShadowMapFar;

uniform sampler2D ReflectionMap;

layout(std140) uniform ReflectionViewProj
{
    mat4 field_ReflectionViewProj;
};

layout(std140) uniform ClipPlaneInfo
{
    Veldrid_NeoDemo_ClipPlaneInfo field_ClipPlaneInfo;
};

bool Shaders_ShadowMain_InRange(float val, float min, float max)
{
    return val >= min && val <= max;
}


float Shaders_ShadowMain_SampleDepthMap(int index, vec2 coord)
{
    if (index == 0)
{
    return texture(ShadowMapNear, coord).x;
}

else
if (index == 1)
{
    return texture(ShadowMapMid, coord).x;
}

else
{
    return texture(ShadowMapFar, coord).x;
}





}


vec4 Shaders_ShadowMain_WithAlpha(vec4 baseColor, float alpha)
{
    return vec4(baseColor.xyz, alpha);
}


vec4 FS(Shaders_ShadowMain_PixelInput input_)
{
    if (field_ClipPlaneInfo.Enabled == 1)
{
    if (dot(field_ClipPlaneInfo.ClipPlane, vec4(input_.Position_WorldSpace, 1)) < 0)
{
    discard;
}



}



    float alphaMapSample = texture(AlphaMap, input_.TexCoord).x;
    if (alphaMapSample == 0)
{
    discard;
}



    vec4 surfaceColor = texture(SurfaceTexture, input_.TexCoord);
    if (field_MaterialProperties.Reflectivity > 0)
{
    vec2 reflectionTexCoords = vec2((input_.ReflectionPosition.x / input_.ReflectionPosition.w) / 2 + 0.5, (input_.ReflectionPosition.y / input_.ReflectionPosition.w) / 2 + 0.5);
    vec4 reflectionSample = texture(ReflectionMap, reflectionTexCoords);
    surfaceColor =(surfaceColor * (1 - field_MaterialProperties.Reflectivity)) + (reflectionSample * field_MaterialProperties.Reflectivity);
}



    vec4 ambientLight = vec4(0.3f, 0.3f, 0.3f, 1.f);
    vec3 lightDir = -field_LightInfo.Direction;
    vec4 directionalColor = ambientLight * surfaceColor;
    float shadowBias = 0.0005f;
    float lightIntensity = 0.f;
    vec4 directionalSpecColor = vec4(0, 0, 0, 0);
    vec3 vertexToEye = normalize(input_.Position_WorldSpace - field_CameraInfo.CameraPosition_WorldSpace);
    vec3 lightReflect = normalize(reflect(field_LightInfo.Direction, input_.Normal));
    float specularFactor = dot(vertexToEye, lightReflect);
    if (specularFactor > 0)
{
    specularFactor =pow(abs(specularFactor), field_MaterialProperties.SpecularPower);
    directionalSpecColor =vec4(field_LightInfo.Color.xyz * field_MaterialProperties.SpecularIntensity * specularFactor, 1.0f);
}



    float depthTest = input_.FragDepth;
    vec2 shadowCoords_0 = vec2((input_.LightPosition1.x / input_.LightPosition1.w) / 2 + 0.5, (input_.LightPosition1.y / input_.LightPosition1.w) / 2 + 0.5);
    vec2 shadowCoords_1 = vec2((input_.LightPosition2.x / input_.LightPosition2.w) / 2 + 0.5, (input_.LightPosition2.y / input_.LightPosition2.w) / 2 + 0.5);
    vec2 shadowCoords_2 = vec2((input_.LightPosition3.x / input_.LightPosition3.w) / 2 + 0.5, (input_.LightPosition3.y / input_.LightPosition3.w) / 2 + 0.5);
    float lightDepthValues_0 = input_.LightPosition1.z / input_.LightPosition1.w;
    float lightDepthValues_1 = input_.LightPosition2.z / input_.LightPosition2.w;
    float lightDepthValues_2 = input_.LightPosition3.z / input_.LightPosition3.w;
    int shadowIndex = 3;
    vec2 shadowCoords = vec2(0, 0);
    float lightDepthValue = 0;
    if ((depthTest < field_DepthLimits.NearLimit) && Shaders_ShadowMain_InRange(shadowCoords_0.x, 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_0.y, 0, 1))
{
    shadowIndex =0;
    shadowCoords =shadowCoords_0;
    lightDepthValue =lightDepthValues_0;
}

else
if ((depthTest < field_DepthLimits.MidLimit) && Shaders_ShadowMain_InRange(shadowCoords_1.x, 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_1.y, 0, 1))
{
    shadowIndex =1;
    shadowCoords =shadowCoords_1;
    lightDepthValue =lightDepthValues_1;
}

else
if (depthTest < field_DepthLimits.FarLimit && Shaders_ShadowMain_InRange(shadowCoords_2.x, 0, 1) && Shaders_ShadowMain_InRange(shadowCoords_2.y, 0, 1))
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
    lightIntensity =clamp(dot(input_.Normal, lightDir), 0, 1);
    if (lightIntensity > 0.0f)
{
    directionalColor =surfaceColor * (lightIntensity * field_LightInfo.Color);
}



}

else
{
    directionalColor =ambientLight * surfaceColor;
    directionalSpecColor =vec4(0, 0, 0, 0);
}



}

else
{
    lightIntensity =clamp(dot(input_.Normal, lightDir), 0, 1);
    if (lightIntensity > 0.0f)
{
    directionalColor =surfaceColor * lightIntensity * field_LightInfo.Color;
}



}



    vec4 pointDiffuse = vec4(0, 0, 0, 1);
    vec4 pointSpec = vec4(0, 0, 0, 1);
    for (int i = 0; i < field_PointLights.NumActiveLights; i++)
{
    Veldrid_NeoDemo_PointLightInfo pli = field_PointLights.PointLights[i];
    vec3 ptLightDir = normalize(pli.Position - input_.Position_WorldSpace);
    float intensity = clamp(dot(input_.Normal, ptLightDir), 0, 1);
    float lightDistance = distance(pli.Position, input_.Position_WorldSpace);
    intensity =clamp(intensity * (1 - (lightDistance / pli.Range)), 0, 1);
    pointDiffuse +=intensity * vec4(pli.Color, 1) * surfaceColor;
    vec3 lightReflect0 = normalize(reflect(ptLightDir, input_.Normal));
    float specularFactor0 = dot(vertexToEye, lightReflect0);
    if (specularFactor0 > 0 && pli.Range > lightDistance)
{
    specularFactor0 =pow(abs(specularFactor0), field_MaterialProperties.SpecularPower);
    pointSpec +=(1 - (lightDistance / pli.Range)) * (vec4(pli.Color * field_MaterialProperties.SpecularIntensity * specularFactor0, 1.0f));
}



}


    return Shaders_ShadowMain_WithAlpha(clamp(directionalSpecColor + directionalColor + pointSpec + pointDiffuse, 0, 1), alphaMapSample);
}


in vec3 fsin_0;
in vec4 fsin_1;
in vec4 fsin_2;
in vec4 fsin_3;
in vec3 fsin_4;
in vec2 fsin_5;
in float fsin_6;
in vec4 fsin_7;
out vec4 _outputColor_;

void main()
{
    Shaders_ShadowMain_PixelInput input_;
    input_.Position = gl_FragCoord;
    input_.Position_WorldSpace = fsin_0;
    input_.LightPosition1 = fsin_1;
    input_.LightPosition2 = fsin_2;
    input_.LightPosition3 = fsin_3;
    input_.Normal = fsin_4;
    input_.TexCoord = fsin_5;
    input_.FragDepth = fsin_6;
    input_.ReflectionPosition = fsin_7;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

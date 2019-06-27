#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
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

layout(set = 0, binding = 0) uniform Projection
{
    mat4 field_Projection;
};

layout(set = 0, binding = 1) uniform View
{
    mat4 field_View;
};

layout(set = 1, binding = 0) uniform LightViewProjection1
{
    mat4 field_LightViewProjection1;
};

layout(set = 1, binding = 1) uniform LightViewProjection2
{
    mat4 field_LightViewProjection2;
};

layout(set = 1, binding = 2) uniform LightViewProjection3
{
    mat4 field_LightViewProjection3;
};

layout(set = 1, binding = 3) uniform DepthLimits
{
    Veldrid_NeoDemo_DepthCascadeLimits field_DepthLimits;
};

layout(set = 1, binding = 4) uniform LightInfo
{
    Veldrid_NeoDemo_DirectionalLightInfo field_LightInfo;
};

layout(set = 1, binding = 5) uniform CameraInfo
{
    Veldrid_NeoDemo_CameraInfo field_CameraInfo;
};

layout(set = 1, binding = 6) uniform PointLights
{
    Veldrid_NeoDemo_PointLightsInfo field_PointLights;
};

layout(set = 2, binding = 0) uniform World
{
    mat4 field_World;
};

layout(set = 2, binding = 1) uniform InverseTransposeWorld
{
    mat4 field_InverseTransposeWorld;
};

layout(set = 2, binding = 2) uniform MaterialProperties
{
    Veldrid_NeoDemo_MaterialProperties field_MaterialProperties;
};

layout(set = 2, binding = 3) uniform texture2D SurfaceTexture;
layout(set = 2, binding = 4) uniform sampler RegularSampler;
layout(set = 2, binding = 5) uniform texture2D AlphaMap;
layout(set = 2, binding = 6) uniform sampler AlphaMapSampler;
layout(set = 2, binding = 7) uniform texture2D ShadowMapNear;
layout(set = 2, binding = 8) uniform texture2D ShadowMapMid;
layout(set = 2, binding = 9) uniform texture2D ShadowMapFar;
layout(set = 2, binding = 10) uniform sampler ShadowMapSampler;
layout(set = 3, binding = 0) uniform texture2D ReflectionMap;
layout(set = 3, binding = 1) uniform sampler ReflectionSampler;
layout(set = 3, binding = 2) uniform ReflectionViewProj
{
    mat4 field_ReflectionViewProj;
};

layout(set = 3, binding = 3) uniform ClipPlaneInfo
{
    Veldrid_NeoDemo_ClipPlaneInfo field_ClipPlaneInfo;
};

Shaders_ShadowMain_PixelInput VS(Shaders_ShadowMain_VertexInput input_)
{
    Shaders_ShadowMain_PixelInput output_;
    vec4 worldPosition = field_World * vec4(input_.Position, 1);
    vec4 viewPosition = field_View * worldPosition;
    output_.Position =field_Projection * viewPosition;
    output_.Position_WorldSpace =worldPosition.xyz;
    vec4 outNormal = field_InverseTransposeWorld * vec4(input_.Normal, 1);
    output_.Normal =normalize(outNormal.xyz);
    output_.TexCoord =input_.TexCoord;
    output_.LightPosition1 =field_World * vec4(input_.Position, 1);
    output_.LightPosition1 =field_LightViewProjection1 * output_.LightPosition1;
    output_.LightPosition2 =field_World * vec4(input_.Position, 1);
    output_.LightPosition2 =field_LightViewProjection2 * output_.LightPosition2;
    output_.LightPosition3 =field_World * vec4(input_.Position, 1);
    output_.LightPosition3 =field_LightViewProjection3 * output_.LightPosition3;
    output_.FragDepth =output_.Position.z;
    output_.ReflectionPosition =field_World * vec4(input_.Position, 1);
    output_.ReflectionPosition =field_ReflectionViewProj * output_.ReflectionPosition;
    return output_;
}


layout(location = 0) in vec3 Position;
layout(location = 1) in vec3 Normal;
layout(location = 2) in vec2 TexCoord;
layout(location = 0) out vec3 fsin_0;
layout(location = 1) out vec4 fsin_1;
layout(location = 2) out vec4 fsin_2;
layout(location = 3) out vec4 fsin_3;
layout(location = 4) out vec3 fsin_4;
layout(location = 5) out vec2 fsin_5;
layout(location = 6) out float fsin_6;
layout(location = 7) out vec4 fsin_7;

void main()
{
    Shaders_ShadowMain_VertexInput input_;
    input_.Position = Position;
    input_.Normal = Normal;
    input_.TexCoord = TexCoord;
    Shaders_ShadowMain_PixelInput output_ = VS(input_);
    fsin_0 = output_.Position_WorldSpace;
    fsin_1 = output_.LightPosition1;
    fsin_2 = output_.LightPosition2;
    fsin_3 = output_.LightPosition3;
    fsin_4 = output_.Normal;
    fsin_5 = output_.TexCoord;
    fsin_6 = output_.FragDepth;
    fsin_7 = output_.ReflectionPosition;
    gl_Position = output_.Position;
        gl_Position.y = -gl_Position.y; // Correct for Vulkan clip coordinates
}

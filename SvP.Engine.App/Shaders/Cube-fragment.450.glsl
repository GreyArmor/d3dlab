#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct TexturedCube_Shaders_Cube_VertexInput
{
    vec3 Position;
    vec2 TexCoords;
};

struct TexturedCube_Shaders_Cube_FragmentInput
{
    vec4 SystemPosition;
    vec2 TexCoords;
};

layout(set = 0, binding = 0) uniform Projection
{
    mat4 field_Projection;
};

layout(set = 0, binding = 1) uniform View
{
    mat4 field_View;
};

layout(set = 1, binding = 0) uniform World
{
    mat4 field_World;
};

layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
vec4 FS(TexturedCube_Shaders_Cube_FragmentInput input_)
{
    return texture(sampler2D(SurfaceTexture, SurfaceSampler), input_.TexCoords);
}


layout(location = 0) in vec2 fsin_0;
layout(location = 0) out vec4 _outputColor_;

void main()
{
    TexturedCube_Shaders_Cube_FragmentInput input_;
    input_.SystemPosition = gl_FragCoord;
    input_.TexCoords = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

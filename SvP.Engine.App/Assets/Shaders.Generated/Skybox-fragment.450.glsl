#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable
struct Shaders_Skybox_VSInput
{
    vec3 Position;
};

struct Shaders_Skybox_FSInput
{
    vec4 Position;
    vec3 TexCoord;
};

layout(set = 0, binding = 0) uniform Projection
{
    mat4 field_Projection;
};

layout(set = 0, binding = 1) uniform View
{
    mat4 field_View;
};

layout(set = 0, binding = 2) uniform textureCube CubeTexture;
layout(set = 0, binding = 3) uniform sampler CubeSampler;
vec4 FS(Shaders_Skybox_FSInput input_)
{
    return texture(samplerCube(CubeTexture, CubeSampler), input_.TexCoord);
}


layout(location = 0) in vec3 fsin_0;
layout(location = 0) out vec4 _outputColor_;

void main()
{
    Shaders_Skybox_FSInput input_;
    input_.Position = gl_FragCoord;
    input_.TexCoord = fsin_0;
    vec4 output_ = FS(input_);
    _outputColor_ = output_;
}

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
Shaders_Skybox_FSInput VS(Shaders_Skybox_VSInput input_)
{
    mat4 view3x3 = mat4(field_View[0][0], field_View[0][1], field_View[0][2], 0, field_View[1][0], field_View[1][1], field_View[1][2], 0, field_View[2][0], field_View[2][1], field_View[2][2], 0, 0, 0, 0, 1);
    Shaders_Skybox_FSInput output_;
    vec4 pos = field_Projection * view3x3 * vec4(input_.Position, 1.0f);
    output_.Position =vec4(pos.x, pos.y, pos.w, pos.w);
    output_.TexCoord =input_.Position;
    return output_;
}


layout(location = 0) in vec3 Position;
layout(location = 0) out vec3 fsin_0;

void main()
{
    Shaders_Skybox_VSInput input_;
    input_.Position = Position;
    Shaders_Skybox_FSInput output_ = VS(input_);
    fsin_0 = output_.TexCoord;
    gl_Position = output_.Position;
        gl_Position.y = -gl_Position.y; // Correct for Vulkan clip coordinates
}

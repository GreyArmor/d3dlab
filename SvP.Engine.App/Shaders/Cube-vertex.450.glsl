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
TexturedCube_Shaders_Cube_FragmentInput VS(TexturedCube_Shaders_Cube_VertexInput input_)
{
    TexturedCube_Shaders_Cube_FragmentInput output_;
    vec4 worldPosition = field_World * vec4(input_.Position, 1);
    vec4 viewPosition = field_View * worldPosition;
    vec4 clipPosition = field_Projection * viewPosition;
    output_.SystemPosition =clipPosition;
    output_.TexCoords =input_.TexCoords;
    return output_;
}


layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 0) out vec2 fsin_0;

void main()
{
    TexturedCube_Shaders_Cube_VertexInput input_;
    input_.Position = Position;
    input_.TexCoords = TexCoords;
    TexturedCube_Shaders_Cube_FragmentInput output_ = VS(input_);
    fsin_0 = output_.TexCoords;
    gl_Position = output_.SystemPosition;
        gl_Position.y = -gl_Position.y; // Correct for Vulkan clip coordinates
}

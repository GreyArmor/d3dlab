#version 330 core

struct Shaders_Skybox_VSInput
{
    vec3 Position;
};

struct Shaders_Skybox_FSInput
{
    vec4 Position;
    vec3 TexCoord;
};

layout(std140) uniform Projection
{
    mat4 field_Projection;
};

layout(std140) uniform View
{
    mat4 field_View;
};

uniform samplerCube CubeTexture;

Shaders_Skybox_FSInput VS(Shaders_Skybox_VSInput input_)
{
    mat4 view3x3 = mat4(field_View[0][0], field_View[0][1], field_View[0][2], 0, field_View[1][0], field_View[1][1], field_View[1][2], 0, field_View[2][0], field_View[2][1], field_View[2][2], 0, 0, 0, 0, 1);
    Shaders_Skybox_FSInput output_;
    vec4 pos = field_Projection * view3x3 * vec4(input_.Position, 1.0f);
    output_.Position =vec4(pos.x, pos.y, pos.w, pos.w);
    output_.TexCoord =input_.Position;
    return output_;
}


in vec3 Position;
out vec3 fsin_0;

void main()
{
    Shaders_Skybox_VSInput input_;
    input_.Position = Position;
    Shaders_Skybox_FSInput output_ = VS(input_);
    fsin_0 = output_.TexCoord;
    gl_Position = output_.Position;
        gl_Position.z = gl_Position.z * 2.0 - gl_Position.w;
}

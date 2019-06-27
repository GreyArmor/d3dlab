#include <metal_stdlib>
using namespace metal;
struct Shaders_FullScreenQuad_VertexInput
{
    float2 Position [[ attribute(0) ]];
    float2 TexCoords [[ attribute(1) ]];
};

struct Shaders_FullScreenQuad_FragmentInput
{
    float4 Position [[ position ]];
    float2 TexCoords [[ attribute(0) ]];
};

struct ShaderContainer {

ShaderContainer(

)
{}
Shaders_FullScreenQuad_FragmentInput VS(Shaders_FullScreenQuad_VertexInput input)
{
    Shaders_FullScreenQuad_FragmentInput output;
    output.Position =float4(input.Position[0], input.Position[1], 0, 1);
    output.TexCoords =input.TexCoords;
    return output;
}


};

vertex Shaders_FullScreenQuad_FragmentInput VS(Shaders_FullScreenQuad_VertexInput input [[ stage_in ]])
{
return ShaderContainer().VS(input);
}

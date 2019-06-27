#include <metal_stdlib>
using namespace metal;
struct Shaders_ScreenDuplicator_VertexInput
{
    float2 Position [[ attribute(0) ]];
    float2 TexCoords [[ attribute(1) ]];
};

struct Shaders_ScreenDuplicator_FragmentInput
{
    float4 Position [[ position ]];
    float2 TexCoords [[ attribute(0) ]];
};

struct Shaders_ScreenDuplicator_FragmentOutput
{
    float4 ColorOut0 [[ color(0) ]];
    float4 ColorOut1 [[ color(1) ]];
};

struct ShaderContainer {

ShaderContainer(

)
{}
Shaders_ScreenDuplicator_FragmentInput VS(Shaders_ScreenDuplicator_VertexInput input)
{
    Shaders_ScreenDuplicator_FragmentInput output;
    output.Position =float4(input.Position[0], input.Position[1], 0, 1);
    output.TexCoords =input.TexCoords;
    return output;
}


};

vertex Shaders_ScreenDuplicator_FragmentInput VS(Shaders_ScreenDuplicator_VertexInput input [[ stage_in ]])
{
return ShaderContainer().VS(input);
}

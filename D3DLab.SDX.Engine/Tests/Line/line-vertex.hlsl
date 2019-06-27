struct VSInputLS
{
	float4 p	: POSITION0;
	float4 c	: COLOR0;

	//float4 mr0	: TEXCOORD1;
	//float4 mr1	: TEXCOORD2;
	//float4 mr2	: TEXCOORD3;
	//float4 mr3	: TEXCOORD4;
};

struct GSInputLS
{
	float4 p	: SV_Position;
	//float4 wp   : POSITION0;
	float4 c	: COLOR0;
};

cbuffer ProjectionBuffer : register(b0)
{
	float4x4 Projection;
}
cbuffer ViewBuffer : register(b1)
{
	float4x4 View;
}

GSInputLS VShaderLines(VSInputLS input)
{
	GSInputLS output;// = (GSInputLS)0;
	
	float4 inputp = input.p;

	//output.p = mul(World, inputp);
	//output.wp = output.p;
	output.p = mul(View, output.p);
	output.p = mul(Projection, output.p);
	
	output.c = input.c;

	return output;
}
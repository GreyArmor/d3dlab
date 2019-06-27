@vertex@

#include "Game"
//#include "Light"

struct VSOut
{
	float4 position : SV_POSITION;
	float4 positionOrig : TEXCOORD0;
	//float4 color : COLOR;
	//float2 tex : TEXCOORD0;
	//float3 normal : NORMAL;
};
VSOut main(float4 position : POSITION) {//, float3 normal : NORMAL, float4 color : COLOR, float2 tex : TEXCOORD
	VSOut output = (VSOut)0;

	position.w = 1.0f;
	output.positionOrig = position;
	output.position = toWVP(position);
	
	//normal = mul(World, normal);
	//normal = normalize(normal);

	//output.color = color * computeLight(output.position.xyz, normal, -LookDirection.xyz, 1000);
	//output.normal = normal;

	return output;
}

@fragment@

cbuffer GradientBuffer : register(b0) {
	float4 apexColor;
	float4 centerColor;
};

struct PSIn
{
	float4 position : SV_POSITION;
	float4 positionOrig : TEXCOORD0;
};

float4 main(PSIn input) : SV_TARGET{
	float height;
	float4 outputColor;

	// Determine the position on the sky dome where this pixel is located.
	height = input.positionOrig.y;

	// The value ranges from -1.0f to +1.0f so change it to only positive values.
	if (height < 0.0)
	{
		height = 0.0f;
	}

	// Determine the gradient color by interpolating between the apex and center based on the height of the pixel in the sky dome.
	outputColor = lerp(centerColor, apexColor, height);

	return outputColor;
}

@geometry@
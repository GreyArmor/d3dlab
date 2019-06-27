@vertex@

#include "Game"
//#include "Light"

struct VSOut
{
	float4 position : SV_POSITION;
	float2 tex : TEXCOORD0;
};
VSOut main(float4 position : POSITION, float2 tex : TEXCOORD) {//, float3 normal : NORMAL, float4 color : COLOR, 
	VSOut output = (VSOut)0;

	output.position = toWVP(position);
	
	output.tex = tex;

	return output;
}

@fragment@

Texture2D cloudTexture1 : register(t0);
Texture2D cloudTexture2 : register(t1);
SamplerState SampleType;

cbuffer SkyBuffer : register(b0)
{
	float2 firstTranslation;
	float2 secondTranslation;
	float brightness;
	float3 padding;
};

struct PSIn
{
	float4 position : SV_POSITION;
	float2 tex : TEXCOORD0;
};

float4 main(PSIn input) : SV_TARGET{

	float2 sampleLocation;
	float4 textureColor1;
	float4 textureColor2;
	float4 finalColor;


	// Translate the position where we sample the pixel from using the first texture translation values.
	sampleLocation.x = input.tex.x + firstTranslation.x;
	sampleLocation.y = input.tex.y + firstTranslation.y;

	// Sample the pixel color from the first cloud texture using the sampler at this texture coordinate location.
	textureColor1 = cloudTexture1.Sample(SampleType, sampleLocation);

	// Translate the position where we sample the pixel from using the second texture translation values.
	sampleLocation.x = input.tex.x + secondTranslation.x;
	sampleLocation.y = input.tex.y + secondTranslation.y;

	// Sample the pixel color from the second cloud texture using the sampler at this texture coordinate location.
	textureColor2 = cloudTexture2.Sample(SampleType, sampleLocation);

	// Combine the two cloud textures evenly.
	finalColor = lerp(textureColor1, textureColor2, 0.5f);

	// Reduce brightness of the combined cloud textures by the input brightness value.
	finalColor = finalColor * brightness;

	return finalColor;
}

@geometry@
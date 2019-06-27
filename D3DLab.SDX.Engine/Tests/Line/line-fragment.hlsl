struct PSInputLS
{
	float4 p	: SV_Position;
	//float4 wp   : POSITION0;
	//noperspective
	//	float3 t	: TEXCOORD;
	float4 c	: COLOR;
};

float4 vLineParams = float4(4, 1, 0, 0);

float4 PShaderLinesFade(PSInputLS input) : SV_Target
{
	return float4(1,0,0,1);//input.c;//
/*
	// Compute distance of the fragment to the edges    
	//float dist = min(abs(input.t[0]), abs(input.t[1]));	
	float dist = abs(input.t.y);
	// Cull fragments too far from the edge.
	//if (dist > 0.5*vLineParams.x+1) discard;

	// Map the computed distance to the [0,2] range on the border of the line.
	//dist = clamp((dist - (0.5*vLineParams.x - 1)), 0, 2);

	// Alpha is computed from the function exp2(-2(x)^2).
	float sigma = 2.0f / (vLineParams.y + 1e-6);
	dist *= dist;
	float alpha = exp2(-2 * dist / sigma);

	//if(alpha<0.1) discard;

	// Standard wire color
	float4 color = input.c;

	//color = texDiffuseMap.Sample(SSLinearSamplerWrap, input.t.xy);	
	color.a = alpha;
	
	return color;*/
}
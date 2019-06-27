cbuffer Game : register(b0) {
	float4 LookDirection;
	float4 CameraPosF4;
	
	float4x4 View;
	float4x4 Projection;

	
};

cbuffer Transformation : register(b2) {
	float4x4 World;
	float4x4 WorldInverse;
}


float4 toScreen(float3 v) {
	float4 p = float4(v, 1);
	p = mul(View, p);
	p = mul(Projection, p);
	return p;
}

float4 toWVP(float4 position) {
	// Change the position vector to be 4 units for proper matrix calculations.
	position.w = 1.0f;

	position = mul(position, World);
	position = mul(position, View);
	position = mul(position, Projection);

	return position;
}
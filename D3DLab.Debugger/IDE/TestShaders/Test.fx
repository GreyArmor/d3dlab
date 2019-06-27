//--------------------------------------------------------------------------------------
// Date: 11/11/12
// References & Sources: 
// parts of the code based on: http://takinginitiative.net/2010/08/30/directx-10-tutorial-8-lighting-theory-and-hlsl/
//--------------------------------------------------------------------------------------
#include "./Shaders/Common.fx"

bool boolDeclaration;
float2 float2Declaration;
float3 float3Declaration;
float4 float4Declaration;
float floatDeclaration;
int intDeclaration;

/*
	multiline comments 
*/

struct VS_Input
{
	float4 p			: POSITION;
	float4 c			: COLOR;
	float2 t			: TEXCOORD;
	float3 n			: NORMAL; // normal
	float3 t1			: TANGENT;
	float3 t2			: BINORMAL;

	float4 mr_0			: TEXCOORD1;
	float4 mr1_			: TEXCOORD2;
	float4 mr2			: TEXCOORD3;
	float4 mr3			: TEXCOORD4; 
};    
struct GSInputLS{ };
struct VSInputLS { };
struct PSInputLS { 

};

cbuffer ProjectionBuffer : register(b0)
{
	float4x4 Projection;
}

/*
	initializations
*/

float floatDefinition = 9.1234567890;
float2 float2Definition = float2(4, 1);
float3 float3Definition = float3(4.1, 1, 0.1);
float4 float4Definition = float4(4, 1, 0, 0);
bool boolDefinition = false;
int intDefinition = 101;


void TestFunction(float4 any, VS_Input custom, inout TriangleStream<VS_Input> generic, triangle VS_Input inputArray[3])
{
	float4 pow_sat_dot = pow(saturate(dot(p, p)));
	custom.t = float2(dx / abs(dx), abs(dy) / dy);
	custom.foo(0);
	custom.p.x = 0;
	any.y = 0.1;
}

[maxvertexcount(18)]
void GS_Add_Point(float4 p, VS_Input input, inout TriangleStream<VS_Input> outStream, triangle VS_Input inputArray[3])
{
	VS_Input output;
	output.p = p;
	
	//TriangleStream
	outStream.Append(output);
	outStream.RestartStrip();

	//declataton/definition
	float4 sp;
	bool bDisableBlandDif = true;
	float thikness = float4Definition.x;
	float4 lineCorners[4];
	output.t = float2(dx / abs(dx), dy / abs(dy));
	/* 
	//not supported yet 
	matrix mInstance =
	{
		input.mr0.x, input.mr1.x, input.mr2.x, input.mr3.x, // row 1
		input.mr0.y, input.mr1.y, input.mr2.y, input.mr3.y, // row 2
		input.mr0.z, input.mr1.z, input.mr2.z, input.mr3.z, // row 3
		input.mr0.w, input.mr1.w, input.mr2.w, input.mr3.w, // row 4
	};
	*/
	GSAddPoint(lineCorners[0], input[i0], outStream);
	float2 p0 = (float3)projToWindow(input.p);
	
	//not supported yet
	//float3 _2DWorldPoint = { 0, 0, 0 };


	//sys methods
	output.p = mul(input.p, Projection);
	float _abs = abs(dx);
	float4 pow_sat_dot = pow(saturate(dot(p, p)));
	float4 _normalize = normalize(input.n);
	float3 r = reflect(v, input.n);
	float3 d = distance();

	//operations
	float w = float4Definition.x * 0.5;
	float a = l < k1 ? 1 : l > k2 ? 0 : (1 - (l - k1) * (1 / (k2 - k1)));
	float step = pi / 4;
	float centerX = (input.x + input.x) / 2;

	//Sample
	//float4 bumpMap = texNormalMap.Sample(NormalSampler, input.t);
	//texShadowMap.SampleCmpLevelZero

	//not supported yet
	/*
	for (int i = 0; i < 3; i++)
	{

	}
	if (bDisableBlandDif)
		thikness = thikness;
	else
		thikness *= thikness;
	if (sp.x < -1.0f || sp.x > 1.0f || sp.y < -1.0f || sp.y > 1.0f || sp.z < 0.0f || sp.z > 1.0f)
	{
		discard;
	}
	*/
}
float4 PShaderLinesFade(PSInputLS input) : SV_Target
{
	return input.c; 
}
GSInputLS VShaderLines(VSInputLS input)
{
	GSInputLS output;
	return output;
}